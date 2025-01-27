using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;
using UnityEngine.Networking; // For UnityWebRequest & GetAudioClip

public class TCPServer : MonoBehaviour
{
    public static TCPServer Instance { get; private set; }

    public int port = 65432;
    private TcpListener server;
    private Thread serverThread;
    private bool isRunning = false;

    private ConcurrentBag<ClientHandler> clients = new ConcurrentBag<ClientHandler>();
    public ManagingEnvironments managingEnvironments;

    // Queue for incoming messages
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        StartServer();
    }

    void StartServer()
    {
        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;
            Debug.Log($"TCP Server started on port {port}.");

            serverThread = new Thread(AcceptClients);
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Server exception: {e.Message}");
        }
    }

    void AcceptClients()
    {
        try
        {
            while (isRunning)
            {
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Client connected.");

                ClientHandler handler = new ClientHandler(client, messageQueue);
                clients.Add(handler);
                handler.Start();
            }
        }
        catch (SocketException se)
        {
            if (isRunning)
            {
                Debug.LogError($"Socket exception: {se.Message}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"AcceptClients exception: {e.Message}");
        }
    }

    /// <summary>
    /// Main Unity update loop: we dequeue any messages from Python,
    /// parse them as JSON (if possible), or handle them as plain strings.
    /// </summary>
    void Update()
    {
        while (messageQueue.TryDequeue(out string message))
        {
            Debug.Log($"[Update] Received from Python: {message}");

            // Attempt to parse JSON with UnityEngine.JsonUtility
            try
            {
                MyAudioMessage parsed = JsonUtility.FromJson<MyAudioMessage>(message);

                // If we successfully decoded a Category and AudioData, handle it.
                if (!string.IsNullOrEmpty(parsed.Category) && 
                    !string.IsNullOrEmpty(parsed.AudioData))
                {
                    Debug.Log($"[TCPServer] Category: {parsed.Category}");
                    Debug.Log($"[TCPServer] AudioData length: {parsed.AudioData.Length}");

                    // 1) Switch environment
                    switch (parsed.Category.ToUpper())
                    {
                        case "DAY":
                            managingEnvironments.SwitchToDay();
                            break;
                        case "NIGHT":
                            managingEnvironments.SwitchToNight();
                            break;
                        case "SNOW":
                            managingEnvironments.SwitchToSnow();
                            break;
                        case "RAIN":
                            managingEnvironments.SwitchToRain();
                            break;
                        default:
                            Debug.Log($"[TCPServer] Unrecognized category: {parsed.Category}");
                            break;
                    }

                    // 2) Decode audio and play
                    byte[] mp3Bytes = Convert.FromBase64String(parsed.AudioData);
                    string filePath = Path.Combine(Application.persistentDataPath, "tempAudio.mp3");
                    File.WriteAllBytes(filePath, mp3Bytes);
                    StartCoroutine(PlayAudioClip(filePath));
                }
                else
                {
                    // Probably not a valid MyAudioMessage, so treat as plain text
                    HandlePlainString(message);
                }
            }
            catch (Exception)
            {
                // If it's not valid JSON, handle as a plain text environment command
                HandlePlainString(message);
            }
        }
    }

    /// <summary>
    /// Handles plain text commands like "DAY", "NIGHT", etc.
    /// </summary>
    void HandlePlainString(string message)
    {
        string msg = message.ToUpper().Trim();
        Debug.Log($"[TCPServer] Handling plain string: {msg}");

        if (msg == "DAY")       managingEnvironments.SwitchToDay();
        else if (msg == "NIGHT") managingEnvironments.SwitchToNight();
        else if (msg == "SNOW")  managingEnvironments.SwitchToSnow();
        else if (msg == "RAIN")  managingEnvironments.SwitchToRain();
        else
        {
            Debug.Log($"[TCPServer] Plain message not recognized as environment command: {msg}");
        }
    }

    /// <summary>
    /// Coroutine to load and play an MP3 from a local file path using UnityWebRequest.
    /// </summary>
    IEnumerator PlayAudioClip(string path)
    {
        // For a local file, prefix with file://
        string url = "file://" + path;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"[TCPServer] Error loading audio clip: {www.error}");
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip == null)
            {
                Debug.LogError("[TCPServer] Failed to decode audio clip");
                yield break;
            }

            // Immediately play the clip at the origin
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (server != null)
        {
            server.Stop();
            Debug.Log("Server stopped.");
        }

        foreach (var client in clients)
        {
            client.Stop();
        }

        if (serverThread != null && serverThread.IsAlive)
        {
            serverThread.Join();
            Debug.Log("Server thread terminated.");
        }
    }

    public void SendMessageToAll(string message)
    {
        foreach (var client in clients)
        {
            if (client.IsConnected)
            {
                client.EnqueueMessage(message);
            }
        }
    }

    public void SendMessageToClient(ClientHandler client, string message)
    {
        if (client != null && client.IsConnected)
        {
            client.EnqueueMessage(message);
        }
    }

    // ---- CLIENT HANDLER CLASS ----
    public class ClientHandler
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private Thread sendThread;
        private bool isConnected = false;

        private ConcurrentQueue<string> sendQueue = new ConcurrentQueue<string>();
        private AutoResetEvent sendEvent = new AutoResetEvent(false);
        private ConcurrentQueue<string> serverMessageQueue;

        public bool IsConnected { get { return isConnected; } }

        public ClientHandler(TcpClient client, ConcurrentQueue<string> messageQueue)
        {
            this.client = client;
            this.stream = client.GetStream();
            this.serverMessageQueue = messageQueue;
        }

        public void Start()
        {
            isConnected = true;
            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            sendThread = new Thread(SendLoop);
            sendThread.IsBackground = true;
            sendThread.Start();
        }

        private void ReceiveLoop()
        {
            try
            {
                byte[] buffer = new byte[1024];
                // Accumulate partial data here
                StringBuilder partialData = new StringBuilder();

                while (isConnected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Debug.Log("Client disconnected.");
                        Stop();
                        break;
                    }

                    // Convert to string
                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Append to the partial buffer
                    partialData.Append(chunk);

                    // Check if we have one or more '\n' in the buffer
                    while (true)
                    {
                        // Look for newline
                        int newlineIndex = partialData.ToString().IndexOf('\n');
                        if (newlineIndex == -1)
                        {
                            // No full message yet
                            break;
                        }

                        // Extract everything up to the newline
                        string fullMessage = partialData.ToString(0, newlineIndex);
                        // Remove that line + the newline from the StringBuilder
                        partialData.Remove(0, newlineIndex + 1);

                        // Now we have a complete message
                        Debug.Log($"Received a full message: {fullMessage}");
                        serverMessageQueue.Enqueue(fullMessage);
                    }
                }
            }
            catch (Exception e)
            {
                if (isConnected)
                {
                    Debug.LogError($"ReceiveLoop exception: {e.Message}");
                    Stop();
                }
            }
        }

        private void SendLoop()
        {
            try
            {
                while (isConnected)
                {
                    sendEvent.WaitOne();
                    while (sendQueue.TryDequeue(out string message))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        Debug.Log($"Sent to client: {message}");
                    }
                }
            }
            catch (Exception e)
            {
                if (isConnected)
                {
                    Debug.LogError($"SendLoop exception: {e.Message}");
                    Stop();
                }
            }
        }

        public void EnqueueMessage(string message)
        {
            sendQueue.Enqueue(message);
            sendEvent.Set();
        }

        public void Stop()
        {
            if (!isConnected) return;
            isConnected = false;

            try { stream?.Close(); } catch {}
            try { client?.Close(); } catch {}

            sendEvent.Set();

            if (receiveThread != null && receiveThread.IsAlive)
                receiveThread.Join();

            if (sendThread != null && sendThread.IsAlive)
                sendThread.Join();

            Debug.Log("Client handler stopped.");
        }
    }
}

[Serializable]
public struct MyAudioMessage
{
    public string Category;
    public string AudioData;
}