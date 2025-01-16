using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    // Singleton instance
    public static TCPServer Instance { get; private set; }

    // Server configuration
    public int port = 65432;
    private TcpListener server;
    private Thread serverThread;
    private bool isRunning = false;

    // To keep track of connected clients
    private ConcurrentBag<ClientHandler> clients = new ConcurrentBag<ClientHandler>();
    public ManagingEnvironments managingEnvironments;

    // Queue to store incoming messages from clients
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Optional: Persist across scenes
    }

    void Start()
    {
        StartServer();
    }

    void StartServer()
    {
        try
        {
            // Initialize the server to listen on the specified port
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;
            Debug.Log($"TCP Server started on port {port}.");

            // Start the server thread to accept clients
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
                // Accept a pending client connection
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Client connected.");

                // Create a new handler for the connected client, passing the message queue
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

    void Update()
    {
        // Process incoming messages from the queue
        while (messageQueue.TryDequeue(out string message))
        {
            // Here you can parse and handle the message as needed
            Debug.Log($"[Update] Updating Scene: {message}");
            if (message.ToUpper() == "DAY")
            {
                managingEnvironments.SwitchToDay();
            }
            else if (message.ToUpper() == "NIGHT")
            {
                managingEnvironments.SwitchToNight();
            }
            else if (message.ToUpper() == "SNOW")
            {
                managingEnvironments.SwitchToSnow();
            }
            else if (message.ToUpper() == "RAIN")
            {
                managingEnvironments.SwitchToRain();
            }
        }
    }

    void OnApplicationQuit()
    {
        // Stop the server and all client handlers
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

    /// <summary>
    /// Sends a message to all connected clients.
    /// </summary>
    /// <param name="message">The message to send.</param>
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

    /// <summary>
    /// Sends a message to a specific client.
    /// </summary>
    /// <param name="client">The client to send the message to.</param>
    /// <param name="message">The message to send.</param>
    public void SendMessageToClient(ClientHandler client, string message)
    {
        if (client != null && client.IsConnected)
        {
            client.EnqueueMessage(message);
        }
    }

    /// <summary>
    /// Optionally, provide a method to get all clients or a specific client based on some criteria.
    /// </summary>
    /// <returns>Enumerable of clients.</returns>
    public ConcurrentBag<ClientHandler> GetAllClients()
    {
        return clients;
    }

    /// <summary>
    /// Handles communication with a connected client.
    /// </summary>
    public class ClientHandler
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private Thread sendThread;
        private bool isConnected = false;

        // Thread-safe queue for outgoing messages
        private ConcurrentQueue<string> sendQueue = new ConcurrentQueue<string>();
        private AutoResetEvent sendEvent = new AutoResetEvent(false);

        // Reference to the main server's message queue
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

            // Start the receive and send threads
            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            sendThread = new Thread(SendLoop);
            sendThread.IsBackground = true;
            sendThread.Start();
        }

        /// <summary>
        /// Continuously receives data from the client.
        /// </summary>
        private void ReceiveLoop()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (isConnected)
                {
                    // Read data from the client
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Debug.Log("Client disconnected.");
                        Stop();
                        break;
                    }

                    string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log($"Received from client: {received}");

                    // Enqueue the received message to the server's message queue
                    serverMessageQueue.Enqueue(received);
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

        /// <summary>
        /// Continuously sends data to the client.
        /// </summary>
        private void SendLoop()
        {
            try
            {
                while (isConnected)
                {
                    // Wait for a message to send
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

        /// <summary>
        /// Adds a message to the send queue.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void EnqueueMessage(string message)
        {
            sendQueue.Enqueue(message);
            sendEvent.Set();
        }

        /// <summary>
        /// Stops the client handler and closes connections.
        /// </summary>
        public void Stop()
        {
            if (!isConnected)
                return;

            isConnected = false;

            // Close the network stream and client
            try
            {
                if (stream != null)
                    stream.Close();
            }
            catch { }

            try
            {
                if (client != null)
                    client.Close();
            }
            catch { }

            // Signal the send thread to exit
            sendEvent.Set();

            // Wait for threads to finish
            if (receiveThread != null && receiveThread.IsAlive)
                receiveThread.Join();

            if (sendThread != null && sendThread.IsAlive)
                sendThread.Join();

            Debug.Log("Client handler stopped.");
        }
    }
}