import socket
import sys
import requests
import json
import re
import threading
from queue import Queue, Empty
import os
from gtts import gTTS
import base64  # <-- new import for base64 encoding

# ========================
# AUDIO + STT IMPORTS
# ========================
import ssl
ssl._create_default_https_context = ssl._create_unverified_context  # TEMP: Disables SSL checks for Whisper
import whisper

import sounddevice as sd
from scipy.io.wavfile import write
import numpy as np

# ========================
# AUDIO + STT SETUP
# ========================

stt_model = whisper.load_model("tiny")  # Replace "tiny" with "base" if desired

def transcribe(filename):
    try:
        result = stt_model.transcribe(filename)
        return result
    except Exception as ex:
        return ex

is_recording = False
recording_thread = None
fs = 44100  # Sample rate
recording_data = []
device_info = sd.query_devices(kind='input')
channels = min(device_info['max_input_channels'], 2)  # Use up to 2 channels if available

def record_audio():
    global recording_data
    recording_data = []
    with sd.InputStream(samplerate=fs, channels=channels) as stream:
        while is_recording:
            data, _ = stream.read(1024)
            recording_data.append(data)

def toggle_recording():
    global is_recording, recording_thread
    if is_recording:
        is_recording = False
        recording_thread.join()
        save_audio()
        print("[AUDIO] Recording stopped and saved.")
    else:
        is_recording = True
        recording_thread = threading.Thread(target=record_audio)
        recording_thread.start()
        print("[AUDIO] Recording started.")

def save_audio():
    audio_data = np.concatenate(recording_data, axis=0)
    write("C:\\Users\\julia\\OneDrive\\Desktop\\Ergo2\\Ergo-G2\\recorded_audio.wav", fs, audio_data)
    print("[AUDIO] Audio saved as 'recorded_audio.wav'")

# ========================
# ORIGINAL SCRIPT
# ========================

def send_chat_request(url, user_input, location, session_id):
    headers = {"Content-Type": "application/json"}
    payload = {
        "user_input": user_input,
        "location": location,
        "session_id": session_id
    }

    try:
        response = requests.post(url, headers=headers, json=payload)
        response.raise_for_status()
        return response.json()
    except requests.exceptions.HTTPError as http_err:
        print(f"HTTP error occurred: {http_err}")
    except requests.exceptions.ConnectionError as conn_err:
        print(f"Connection error occurred: {conn_err}")
    except requests.exceptions.Timeout as timeout_err:
        print(f"Timeout error occurred: {timeout_err}")
    except requests.exceptions.RequestException as req_err:
        print(f"An error occurred: {req_err}")
    except json.JSONDecodeError:
        print("Failed to parse response as JSON.")
    return None

def connect_to_unity(host, port):
    try:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.connect((host, port))
        sock.settimeout(1.0)  # Set timeout for non-blocking recv
        print(f"[INFO] Connected to Unity server at {host}:{port}")
        return sock
    except ConnectionRefusedError:
        print("[ERROR] Connection refused. Is the Unity server running and accessible?")
        sys.exit(1)
    except Exception as e:
        print(f"[ERROR] Failed to connect to Unity server: {e}")
        sys.exit(1)

def send_message_to_unity(sock, message):
    try:
        # Append a newline to clearly delimit the end of the JSON
        message_with_newline = message + "\n"
        sock.sendall(message_with_newline.encode('utf-8'))
        print(f"[SEND] Sent to Unity: {message_with_newline[0:40]}......")
    except Exception as e:
        print(f"[ERROR] Failed to send message to Unity: {e}")

def receive_message_from_unity(sock, message_queue, stop_event):
    while not stop_event.is_set():
        try:
            data = sock.recv(1024)
            if data:
                received = data.decode('utf-8')
                print(f"[RECEIVE] Received from Unity: {received}")
                message_queue.put(received)
            else:
                print("[INFO] Unity server closed the connection.")
                stop_event.set()
        except socket.timeout:
            continue
        except Exception as e:
            print(f"[ERROR] Error receiving data from Unity: {e}")
            stop_event.set()

def sender_thread_function(sock, send_queue, stop_event):
    while not stop_event.is_set():
        try:
            message = send_queue.get(timeout=0.5)
            send_message_to_unity(sock, message)
        except Empty:
            continue
        except Exception as e:
            print(f"[ERROR] Sender thread encountered an error: {e}")
            stop_event.set()

def extract_category(message_str):
    try:
        message_json = json.loads(message_str)
        category = message_json.get("Category")
        if category:
            return category
        else:
            print("[WARNING] JSON works, but 'Category' field not found.")
            return ""
    except json.JSONDecodeError:
        print("[WARNING] JSON parsing failed. Attempting to extract 'Category' using regex.")
        pattern = r'"Category"\s*:\s*"(.*?)"|\'Category\'\s*:\s*\'(.*?)\''
        match = re.search(pattern, message_str, re.IGNORECASE)
        if match:
            category = match.group(1) if match.group(1) else match.group(2)
            print(f"[INFO] Extracted 'Category' using regex: {category}")
            return category
        else:
            print("[ERROR] 'Category' field not found using regex.")
            return "Category Not Found"

def extract_message(message_str):
    try:
        message_json = json.loads(message_str)
        msg = message_json.get("Message")
        if msg:
            return msg
        else:
            print("[WARNING] JSON works, but 'Message' field not found.")
            return ""
    except json.JSONDecodeError:
        print("[WARNING] JSON parsing failed. Attempting to extract 'Message' using regex.")
        pattern = r'"Message"\s*:\s*"(.*?)"|\'Message\'\s*:\s*\'(.*?)\''
        match = re.search(pattern, message_str, re.IGNORECASE)
        if match:
            msg = match.group(1) if match.group(1) else match.group(2)
            print(f"[INFO] Extracted 'Message' using regex: {msg}")
            return msg
        else:
            print("[ERROR] 'Message' field not found using regex.")
            return "Message Not Found"

ENVIRONMENT_DICT = {
    "Egypt, Pyramids": ["DAY", "NIGHT"],
    "India, Taj Mahal": ["DAY", "NIGHT"],
    "Mount Everest": ["DAY", "NIGHT"],
    "Japan, Temple": ["DAY", "NIGHT", "SNOW"],
    "China, The Great Wall of China": ["DAY", "NIGHT", "SNOW"],
    "Paris, Eiffel Tower": ["DAY", "NIGHT", "SNOW"],
    "Ocean": ["DAY"],
    "Finland": ["DAY", "NIGHT"],
    "Brazil, Christ The Redeemer": ["DAY", "NIGHT"],
    "San Fransisco, Golden Gate Bridge": ["DAY", "NIGHT", "RAIN"]
}

def main():
    SERVER_URL = "http://127.0.0.1:5001/chat"  # Replace with your LLM server URL
    UNITY_HOST = '127.0.0.1'
    UNITY_PORT = 65432
    LOCATION = "Space"
    SESSION_ID = "unique_session_123"

    sock = connect_to_unity(UNITY_HOST, UNITY_PORT)

    message_queue = Queue()
    send_queue = Queue()
    stop_event = threading.Event()

    receiver_thread = threading.Thread(
        target=receive_message_from_unity,
        args=(sock, message_queue, stop_event),
        daemon=True
    )
    receiver_thread.start()
    print("[INFO] Started receiver thread.")

    sender_thread = threading.Thread(
        target=sender_thread_function,
        args=(sock, send_queue, stop_event),
        daemon=True
    )
    sender_thread.start()
    print("[INFO] Started sender thread.")

    print('[INFO] Connecting to LLM...')
    send_chat_request(SERVER_URL, 'test', LOCATION, SESSION_ID)

    print("[INFO] Press Enter (no text) to START/STOP recording. Type 'q' + Enter to quit.")
    print("[INFO] Or type your text directly and press Enter to send it without voice recording.")

    try:
        while not stop_event.is_set():
            user_input = input().strip()

            # Check for quit command
            if user_input.lower() == 'q':
                print("[INFO] Quit command received. Exiting...")
                break

            recognized_text = None

            if user_input == '':
                # Toggle voice recording
                toggle_recording()

                # If we just STOPPED recording, do STT
                if not is_recording:
                    stt_result = transcribe("C:/Users/julia/OneDrive/Desktop/Ergo2/Ergo-G2/recorded_audio.wav")
                    if isinstance(stt_result, Exception):
                        print("[STT ERROR]", stt_result)
                        continue
                    recognized_text = stt_result["text"]
                    print(f"[STT] Recognized text: {recognized_text}")
            else:
                recognized_text = user_input
                print(f"[INFO] Using typed input: {recognized_text}")

            if recognized_text:
                # Check if there's a new location from Unity in the queue
                try:
                    while True:
                        received_from_unity = message_queue.get_nowait()
                        LOCATION = received_from_unity
                        print(f"[LOCATION] {LOCATION}")
                except Empty:
                    pass

                if LOCATION == "Space":
                    print("[INFO] You are in 'Space' - you cannot change environment here. Move to a valid location first.")
                    continue
                elif LOCATION not in ENVIRONMENT_DICT:
                    print(f"[ERROR] Location '{LOCATION}' not recognized.\nAvailable: {list(ENVIRONMENT_DICT.keys())}")
                    continue

                # Build the prompt for LLM
                choices = ", ".join(ENVIRONMENT_DICT[LOCATION])
                user_input_formatted = f"CHOICES: {choices} | Prompt: {recognized_text}"
                print(f"[CHAT] Sending to LLM server: {user_input_formatted}")

                # Send request to LLM
                response = send_chat_request(SERVER_URL, user_input_formatted, LOCATION, SESSION_ID)

                llm_message = ""
                category = ""
                if response:
                    if "message" in response:
                        llm_message = response["message"]
                        print(f"[CHAT] Received from LLM: {llm_message}")

                        category = extract_category(llm_message)
                        message = extract_message(llm_message)
                        print(f"[CHAT] Extracted Category: {category}")
                        print(f"[CHAT] Extracted Message: {message}")

                        # Generate TTS if we have a message
                        if message:
                            tts = gTTS(text=message, lang='en')
                            tts.save('message.mp3')

                            # Read MP3 as binary and encode base64
                            with open('message.mp3', 'rb') as f:
                                audio_bytes = f.read()
                            audio_base64 = base64.b64encode(audio_bytes).decode('utf-8')

                            # Create a single JSON string containing Category and AudioData
                            json_for_unity = json.dumps({
                                "Category": category,
                                "AudioData": audio_base64
                            })

                            # Send it to Unity
                            send_queue.put(json_for_unity)
                    elif "error" in response:
                        llm_message = f"Error: {response['error']}"
                        print(f"[CHAT ERROR] {llm_message}")
                        category = ""
                    else:
                        llm_message = f"Unexpected response format: {response}"
                        print(f"[CHAT WARNING] {llm_message}")
                        category = ""
                else:
                    llm_message = "No valid response received from LLM server."
                    print(f"[CHAT WARNING] {llm_message}")
                    category = ""

                # In case you still want to send just the category alone (optional):
                # send_queue.put(category)

    except KeyboardInterrupt:
        print("\n[INFO] Interrupted by user. Closing connections...")

    except Exception as e:
        print(f"[ERROR] An unexpected error occurred: {e}")

    finally:
        stop_event.set()
        receiver_thread.join(timeout=2)
        sender_thread.join(timeout=2)
        print("[INFO] Receiver and sender threads terminated.")

        try:
            sock.close()
            print("[INFO] Socket to Unity closed.")
        except Exception as e:
            print(f"[ERROR] Error closing socket: {e}")

if __name__ == "__main__":
    main()