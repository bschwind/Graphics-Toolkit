using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GraphicsToolkit.Networking
{
    public delegate void ServerHandlePacketData(byte[] data, int dataLength, TcpClient client);

    public class Server
    {
        TcpListener listener;
        public event ServerHandlePacketData OnDataReceived;
        private Dictionary<TcpClient, NetworkBuffer> clientBuffers;
        int sendBufferSize = 1024;
        int readBufferSize = 1024;

        public Server(int port)
        {
            clientBuffers = new Dictionary<TcpClient, NetworkBuffer>();

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
                clientBuffers.Add(client, newBuff);

                clientThread.Start(client);
            }
        }

        public void Disconnect()
        {
            listener.Stop();
        }

        private void WorkWithClient(object client)
        {
            TcpClient tcpClient = client as TcpClient;
            if (tcpClient == null)
            {
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
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    Console.WriteLine("Client has disconnected");
                    break;
                }

                if (OnDataReceived != null)
                {
                    OnDataReceived(clientBuffers[tcpClient].ReadBuffer, bytesRead, tcpClient);
                }
            }

            tcpClient.Close();
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
