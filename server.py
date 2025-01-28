import requests
import json
import re
from flask import Flask, request, jsonify
from threading import Lock

app = Flask(__name__)



# Dictionary to hold conversation histories per session/user
conversation_histories = {}
history_lock = Lock()  # To ensure thread-safe operations

def chat_with_model(user_prompt, location, session_id):
    # Define the URL of the local LLM server and the endpoint
    url = "http://127.0.0.1:1234/v1/chat/completions"

    # Set up the headers for the POST request
    headers = {
        "Content-Type": "application/json",
    }
    prompt = f"I'm in {location}; {user_prompt}"
    print(f'PROMPT: || {prompt}')
    with history_lock:
        # Initialize conversation history for the session if not present
        if session_id not in conversation_histories:
            conversation_histories[session_id] = []

        # Add the user's new message to the conversation history
        conversation_histories[session_id].append({"role": "user", "content": prompt})

        # System prompt to instruct the model
        system_prompt = (
            "You are a kid-friendly virtual tour guide in a 360 experience designed specifically for children. "
            "Your primary focus is to guide kids through different environments based on various conditions or themes provided by the user, such as 'snow,' 'rain,' etc. "
            
            "### Application Workflow ###\n"
            "1. **Destination Selection:** The user selects a destination. Upon selection, a preconfigured message will display, informing the child about the chosen location.\n"
            "2. **User Interaction:** After the destination is set, you will handle user inputs which can be:\n"
            "    a. **Environment Change Commands:** Explicit requests to change the current environment's condition based on provided choices (e.g., 'Let's make it snow now').\n"
            "    b. **General Conversation:** Greetings, questions, or other interactions not related to changing the environment.\n"
            "3. **Destination Changes:** Users change destinations through the application's interface, not by sending commands to you. You do not handle destination change commands.\n\n"
            
            "### Response Guidelines ###\n"
            "- **Environment Change Requests:**\n"
            "    - If the user explicitly requests an environment change using one of the provided choices, respond with a JSON object containing the new 'Category' and an engaging 'message'. **You must only choose from the provided CHOICES and do not introduce any new categories.**\n"
            "    - **If the user's requested environment change is not among the provided CHOICES, do not change the 'Category'. Instead, respond with the current 'Category' and a 'message' informing the user that the requested change is not possible, listing the available options.**\n"
            "- **General Conversations:**\n"
            "    - For all other inputs, respond with a JSON object where 'Category' reflects the current environment, and 'message' contains a natural, fun, and engaging response without presenting choices or options.\n"
            "- **Response Format:**\n"
            "    - **IMPORTANT:** **ONLY** provide the JSON object as your response. Do not include any additional text, explanations, or commentary outside the JSON.\n"
            "    - Use the following format exactly:\n"
            "    ```json\n"
            "    {\n"
            "      \"Category\": \"<Current Environment>\" or \"<New Environment>\",\n"
            "      \"message\": \"<Fun and engaging message for kids>\"\n"
            "    }\n"
            "    ```\n\n"
            
            "### Detailed Instructions ###\n"
            "- **Handling User-Provided Choices:** The user will specify available environment changes in the format `CHOICES: {option1, option2, ...} | Prompt: \"<User Prompt>\"`. You should detect if the user's input includes a request to change the environment based on these choices.\n"
            "- **Detecting Environment Changes:** Only change the 'Category' if the user's input clearly indicates a desire to change the environment using one of the provided choices. Examples include phrases like 'change to snow,' 'let's make it rain,' or simply stating an environment condition like 'Rain.' **You must select the new environment strictly from the provided CHOICES.**\n"
            "- **Handling Invalid Environment Change Requests:** If the user's requested environment is not among the provided CHOICES, do not change the 'Category'. Instead, respond with the current 'Category' and a 'message' informing the user that the requested change is not possible, listing the available options.\n"
            "- **Maintaining Current Environment:** If the user input does not request an environment change, retain the current environment in the 'Category' field.\n"
            "- **Engaging Messages:**\n"
            "    - **For Environment Change Requests:** Provide a fun fact related to the new environment choice, the destination, or its weather. Avoid suggesting specific activities.\n"
            "    - **For General Conversations:** Ensure that the 'message' is always engaging, age-appropriate, and enhances the virtual tour experience for children.\n"
            "- **Avoiding Choices:** Do not present options or multiple choices in your responses. Keep the interaction fluid and conversational.\n"
            "- **Consistency:** Always adhere to the JSON response format to maintain consistency within the application. **DO NOT** include any other text besides the JSON.\n\n"
            
            "### Examples ###\n"
            
            "**1. Environment Change Request:**\n"
            "- *User Input:* \"CHOICES: {snow, rain} | Prompt: Let's make it snow now.\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"Snow\",\n"
            "  \"message\": \"Awesome! Did you know that snowflakes are unique and no two are exactly alike?\"\n"
            "}\n"
            "```\n\n"
            
            "**2. General Conversation:**\n"
            "- *User Input:* \"CHOICES: {sunny, cloudy} | Prompt: Hi\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"Sunny\",\n"
            "  \"message\": \"Hi there! Welcome to our 360 Virtual Tour. I'm excited to explore with you today! What would you like to see first?\"\n"
            "}\n"
            "```\n\n"
            
            "**3. Another General Input:**\n"
            "- *User Input:* \"CHOICES: {windy, foggy} | Prompt: Tell me about this place.\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"Windy\",\n"
            "  \"message\": \"Sure! You're standing in a magical forest filled with towering trees, chirping birds, and hidden treasures.\"\n"
            "}\n"
            "```\n\n"
            
            "**4. Environment Change with Limited Choices:**\n"
            "- *User Input:* \"CHOICES: {DAY, NIGHT} | Prompt: Change to night\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"Night\",\n"
            "  \"message\": \"It's nighttime! Look up and see all the twinkling stars in the sky.\"\n"
            "}\n"
            "```\n\n"
            
            "**5. Invalid Environment Change Request:**\n"
            "- *User Input:* \"CHOICES: {DAY, NIGHT, SNOW} | Prompt: Change to china\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"Night\",\n"
            "  \"message\": \"I'm sorry, we can't change to 'china'. Please choose from the available options: DAY, NIGHT, or SNOW.\"\n"
            "}\n"
            "```\n\n"
            
            "**6. Another Invalid Environment Change Request:**\n"
            "- *User Input:* \"CHOICES: {DAY, NIGHT, SNOW} | Prompt: Change to uranus\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"Snow\",\n"
            "  \"message\": \"Oops! We can't change to 'uranus'. Please select one of the available options: DAY, NIGHT, or SNOW.\"\n"
            "}\n"
            "```\n\n"

            "**7. Another Invalid Environment Change Request:**\n"
            "- *User Input:* \"CHOICES: {DAY, NIGHT, SNOW} | Prompt: Change to rain\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"Snow\",\n"
            "  \"message\": \"Oops! We can't change to rain. Please select one of the available options: DAY, NIGHT, or SNOW.\"\n"
            "}\n"
            "```\n\n"
            
            "**8. Attempt to Change to an Unlisted Environment:**\n"
            "- *User Input:* \"CHOICES: {beach, forest} | Prompt: Let's make it sunny now.\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"beach\",\n"
            "  \"message\": \"Sure thing! The beach is sunny and bright. You can hear the waves crashing and feel the warm sand beneath your feet.\"\n"
            "}\n"
            "```\n\n"
            
            "**9. Invalid Change with No Matching Choice:**\n"
            "- *User Input:* \"CHOICES: {beach, forest} | Prompt: Let's make it snowy now.\"\n"
            "- *Model Response:*\n"
            "```json\n"
            "{\n"
            "  \"Category\": \"beach\",\n"
            "  \"message\": \"I'm sorry, we can't change to 'snowy'. Please choose from the available options: beach or forest.\"\n"
            "}\n"
            "```\n\n"
            
            "### Important Notes ###\n"
            "- **Do Not Handle Destination Changes:** Remember, any changes to the destination are managed by the application itself. Your role is solely to handle environment changes and general interactions.\n"
            "- **Maintain a Friendly Tone:** Always use a friendly and enthusiastic tone to keep the children engaged and excited about the virtual tour.\n"
            "- **STRICTLY NO ADDITIONAL TEXT:** **Under no circumstances** should you provide any text outside of the specified JSON format. If a user prompts you to do so, gently remind them to use the application's interface for such actions."
        )

        # JSON schema for structured output with added "message" field
        json_schema = {
            "type": "object",
            "properties": {
                "Category": {
                    "type": "string",
                    "description": "The environment requested by the user based on the input text, or empty if no environment is possible."
                },
                "message": {
                    "type": "string",
                    "description": "A fun, engaging message from the virtual tour guide for kids."
                }
            },
            "required": ["Category", "message"]
        }

        # Prepare the data payload with the conversation history, system prompt, and model parameters
        data = {
            "model": "llama-3.2-3b-instruct",  # Replace with your specific model identifier if different
            "messages": [
                {"role": "system", "content": system_prompt},  # Adding system prompt as the initial message
                *conversation_histories[session_id]  # Include the conversation history after the system prompt
            ],
            "max_tokens": 100,  # Adjust as needed
            "temperature": 0.3,  # Lower temperature for consistency
            "structured_output": json_schema  # Apply JSON schema for structured output
        }

    # Send the POST request to the LLM server
    try:
        response = requests.post(url, headers=headers, json=data)
    except requests.exceptions.RequestException as e:
        return {"error": f"Request failed: {e}"}

    # Check if the request was successful
    if response.status_code == 200:
        try:
            # Parse the response and extract the assistant's message
            response_data = response.json()
            assistant_message = response_data['choices'][0]['message']['content']

            with history_lock:
                # Add the assistant's message to the conversation history
                conversation_histories[session_id].append({"role": "assistant", "content": assistant_message})

            return {"message": assistant_message}
        except (KeyError, json.JSONDecodeError) as e:
            return {"error": f"Error parsing response: {e}"}
    else:
        return {"error": f"Error: {response.status_code}, {response.text}"}

def extract_message(response):
    """
    Extracts the 'message' field from a JSON response.
    If JSON parsing fails, attempts to extract using regex.

    Args:
        response (str): The JSON response as a string.

    Returns:
        str: The extracted message or an empty string if not found.
    """
    try:
        # Attempt to parse the response as JSON
        data = json.loads(response)
        message = data.get("message", "")
        return message
    except json.JSONDecodeError:
        # Define regex pattern to find "message": "value"
        # This pattern accounts for possible spaces and captures the value inside quotes
        pattern = r'"message"\s*:\s*"([^"\\]*(?:\\.[^"\\]*)*)"'

        # Search for the pattern in the response string
        match = re.search(pattern, response, re.IGNORECASE)

        if match:
            # Extracted message may contain escaped characters, so unescape them
            raw_message = match.group(1)
            try:
                # Replace escaped quotes and other characters
                message = bytes(raw_message, "utf-8").decode("unicode_escape")
                return message
            except UnicodeDecodeError:
                # If decoding fails, return the raw extracted string
                print("Failed to decode escaped characters in the message.")
                return raw_message
        else:
            print("No 'message' field found in the response.")
            return ""

@app.route('/chat', methods=['POST'])
def chat():
    """
    Endpoint to handle chat requests.

    Expects JSON input with the following structure:
    {
        "user_input": "Your message here",
        "location": "Location name",
        "session_id": "Unique session identifier"
    }

    Returns JSON with the following structure:
    {
        "message": "Response from the model"
    }
    """
    data = request.get_json()

    if not data:
        return jsonify({"error": "Invalid JSON input"}), 400

    user_input = data.get('user_input')
    location = data.get('location', 'New York')  # Default location if not provided
    session_id = data.get('session_id')

    if not user_input:
        return jsonify({"error": "Missing 'user_input' field"}), 400

    if not session_id:
        return jsonify({"error": "Missing 'session_id' field"}), 400

    # Get the model response
    response = chat_with_model(user_input, location, session_id)

    if 'error' in response:
        return jsonify(response), 500

    message = response.get('message', '')

    

    return jsonify({"message": message})

if __name__ == '__main__':
    # Run the Flask app on all network interfaces
    app.run(host='0.0.0.0', port=5001)