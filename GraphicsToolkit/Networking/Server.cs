using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;

namespace GraphicsToolkit.Networking
{
    public delegate void ServerHandlePacketData(byte[] data, int bytesRead, TcpClient client);

    /// <summary>
    /// Implements a simple TCP server which uses one thread per client
    /// </summary>
    public class Server
    {
        public event ServerHandlePacketData OnDataReceived;

        private TcpListener listener;
        private ConcurrentDictionary<TcpClient, NetworkBuffer> clientBuffers;
        private List<TcpClient> clients;
        private int sendBufferSize = 1024;
        private int readBufferSize = 1024;
        private int port;
        private bool started = false;

        /// <summary>
        /// The list of currently connected clients
        /// </summary>
        public List<TcpClient> Clients
        {
            get
            {
                return clients;
            }
        }

        /// <summary>
        /// The number of clients currently connected
        /// </summary>
        public int NumClients
        {
            get
            {
                return clients.Count;
            }
        }

        /// <summary>
        /// Constructs a new TCP server which will listen on a given port
        /// </summary>
        /// <param name="port"></param>
        public Server(int port)
        {
            this.port = port;
            clientBuffers = new ConcurrentDictionary<TcpClient, NetworkBuffer>();
            clients = new List<TcpClient>();
        }

        /// <summary>
        /// Begins listening on the port provided to the constructor
        /// </summary>
        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            Console.WriteLine("Started server on port " + port);

            Thread thread = new Thread(new ThreadStart(ListenForClients));
            thread.Start();
            started = true;
        }

        /// <summary>
        /// Runs in its own thread. Responsible for accepting new clients and kicking them off into their own thread
        /// </summary>
        private void ListenForClients()
        {
            listener.Start();

            while (started)
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(WorkWithClient));
                Console.WriteLine("New client connected");

                NetworkBuffer newBuff = new NetworkBuffer();
                newBuff.WriteBuffer = new byte[sendBufferSize];
                newBuff.ReadBuffer = new byte[readBufferSize];
                newBuff.CurrentWriteByteCount = 0;
                clientBuffers.GetOrAdd(client, newBuff);
                clients.Add(client);

                clientThread.Start(client);
                Thread.Sleep(15);
            }
        }

        /// <summary>
        /// Stops the server from accepting new clients
        /// </summary>
        public void Stop()
        {
            if (!listener.Pending())
            {
                listener.Stop();
                started = false;
            }
        }

        /// <summary>
        /// This method lives on a thread, one per client. Responsible for reading data from the client
        /// and pushing the data off to classes listening to the server.
        /// </summary>
        /// <param name="client"></param>
        private void WorkWithClient(object client)
        {
            TcpClient tcpClient = client as TcpClient;
            if (tcpClient == null)
            {
                Console.WriteLine("TCP client is null, stopping processing for this client");
                DisconnectClient(tcpClient);
                return;
            }

            NetworkStream clientStream = tcpClient.GetStream();
            int bytesRead;

            while (started)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(clientBuffers[tcpClient].ReadBuffer, 0, readBufferSize);
                }
                catch
                {
                    //a socket error has occurred
                    Console.WriteLine("A socket error has occurred with client: " + tcpClient.ToString());
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                if (OnDataReceived != null)
                {
                    //Send off the data for other classes to handle
                    OnDataReceived(clientBuffers[tcpClient].ReadBuffer, bytesRead, tcpClient);
                }

                Thread.Sleep(15);
            }

            DisconnectClient(tcpClient);
        }

        /// <summary>
        /// Removes a given client from our list of clients
        /// </summary>
        /// <param name="client"></param>
        private void DisconnectClient(TcpClient client)
        {
            if (client == null)
            {
                return;
            }

            Console.WriteLine("Disconnected client: " + client.ToString());

            client.Close();

            clients.Remove(client);
            NetworkBuffer buffer;
            clientBuffers.TryRemove(client, out buffer);
        }

        /// <summary>
        /// Adds data to the packet to be sent out, but does not send it across the network
        /// </summary>
        /// <param name="data">The data to be sent</param>
        /// <param name="client">The client to send the data to</param>
        public void AddToPacket(byte[] data, TcpClient client)
        {
            if (clientBuffers[client].CurrentWriteByteCount + data.Length > clientBuffers[client].WriteBuffer.Length)
            {
                FlushData(client);
            }

            Array.ConstrainedCopy(data, 0, clientBuffers[client].WriteBuffer, clientBuffers[client].CurrentWriteByteCount, data.Length);

            clientBuffers[client].CurrentWriteByteCount += data.Length;
        }

        /// <summary>
        /// Adds data to the packet to be sent out, but does not send it across the network. This
        /// data gets sent to every connected client
        /// </summary>
        /// <param name="data">The data to be sent</param>
        public void AddToPacketToAll(byte[] data)
        {
            lock (clients)
            {
                foreach (TcpClient client in clients)
                {
                    if (clientBuffers[client].CurrentWriteByteCount + data.Length > clientBuffers[client].WriteBuffer.Length)
                    {
                        FlushData(client);
                    }

                    Array.ConstrainedCopy(data, 0, clientBuffers[client].WriteBuffer, clientBuffers[client].CurrentWriteByteCount, data.Length);

                    clientBuffers[client].CurrentWriteByteCount += data.Length;
                }
            }
        }

        /// <summary>
        /// Flushes all outgoing data to the specified client
        /// </summary>
        /// <param name="client"></param>
        private void FlushData(TcpClient client)
        {
            client.GetStream().Write(clientBuffers[client].WriteBuffer, 0, clientBuffers[client].CurrentWriteByteCount);
            client.GetStream().Flush();
            clientBuffers[client].CurrentWriteByteCount = 0;
        }

        /// <summary>
        /// Flushes all outgoing data to every client
        /// </summary>
        private void FlushDataToAll()
        {
            lock (clients)
            {
                foreach (TcpClient client in clients)
                {
                    client.GetStream().Write(clientBuffers[client].WriteBuffer, 0, clientBuffers[client].CurrentWriteByteCount);
                    client.GetStream().Flush();
                    clientBuffers[client].CurrentWriteByteCount = 0;
                }
            }
        }

        /// <summary>
        /// Sends the byte array data immediately to the specified client
        /// </summary>
        /// <param name="data">The data to be sent</param>
        /// <param name="client">The client to send the data to</param>
        public void SendImmediate(byte[] data, TcpClient client)
        {
            AddToPacket(data, client);
            FlushData(client);
        }

        /// <summary>
        /// Sends the byte array data immediately to all clients
        /// </summary>
        /// <param name="data">The data to be sent</param>
        public void SendImmediateToAll(byte[] data)
        {
            lock (clients)
            {
                foreach (TcpClient client in clients)
                {
                    AddToPacket(data, client);
                    FlushData(client);
                }
            }
        }
    }
}
