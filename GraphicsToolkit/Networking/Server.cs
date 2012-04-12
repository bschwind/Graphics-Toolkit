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

    public class Server
    {
        public event ServerHandlePacketData OnDataReceived;

        private TcpListener listener;
        private ConcurrentDictionary<TcpClient, NetworkBuffer> clientBuffers;
        private List<TcpClient> clients;
        private int sendBufferSize = 1024;
        private int readBufferSize = 1024;
        private int port;

        public List<TcpClient> Clients
        {
            get
            {
                return clients;
            }
        }

        public int NumClients
        {
            get
            {
                return clients.Count;
            }
        }

        public Server(int port)
        {
            this.port = port;
            clientBuffers = new ConcurrentDictionary<TcpClient, NetworkBuffer>();
            clients = new List<TcpClient>();
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, port);
            Console.WriteLine("Started server on port " + port);

            Thread thread = new Thread(new ThreadStart(ListenForClients));
            thread.Start();
        }

        private void ListenForClients()
        {
            listener.Start();

            while (true)
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
                Thread.Sleep(100);
            }
        }

        public void Disconnect()
        {
            if (!listener.Pending())
            {
                listener.Stop();
            }
        }

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

            while (true)
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
            }

            DisconnectClient(tcpClient);
        }

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

        public void AddToPacket(byte[] data, TcpClient client)
        {
            if (clientBuffers[client].CurrentWriteByteCount + data.Length > clientBuffers[client].WriteBuffer.Length)
            {
                FlushData(client);
            }

            Array.ConstrainedCopy(data, 0, clientBuffers[client].WriteBuffer, clientBuffers[client].CurrentWriteByteCount, data.Length);

            clientBuffers[client].CurrentWriteByteCount += data.Length;
        }

        private void FlushData(TcpClient client)
        {
            client.GetStream().Write(clientBuffers[client].WriteBuffer, 0, clientBuffers[client].CurrentWriteByteCount);
            client.GetStream().Flush();
            clientBuffers[client].CurrentWriteByteCount = 0;
        }

        public void SendData(byte[] data, TcpClient client)
        {
            AddToPacket(data, client);
            FlushData(client);
        }
    }
}
