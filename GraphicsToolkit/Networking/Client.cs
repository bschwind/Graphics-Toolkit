using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GraphicsToolkit.Networking
{
    public delegate void ClientHandlePacketData(byte[] data, int bytesRead);

    public class Client
    {
        public event ClientHandlePacketData OnDataReceived;

        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private NetworkBuffer buffer;
        private int writeBufferSize = 1024;
        private int readBufferSize = 1024;
        private int port;

        public Client(int port)
        {
            this.port = port;

            buffer = new NetworkBuffer();
            buffer.WriteBuffer = new byte[writeBufferSize];
            buffer.ReadBuffer = new byte[readBufferSize];
            buffer.CurrentWriteByteCount = 0;
        }

        public void ConnectToServer(string ipAddress)
        {
            tcpClient = new TcpClient(ipAddress, port);
            clientStream = tcpClient.GetStream();
            Console.WriteLine("Connected to server, listening for packets");

            Thread t = new Thread(new ThreadStart(ListenForPackets));
            t.Start();
        }

        private void ListenForPackets()
        {
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(buffer.ReadBuffer, 0, readBufferSize);
                }
                catch
                {
                    //a socket error has occurred
                    Console.WriteLine("A socket error has occurred with the client socket " + tcpClient.ToString());
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
                    OnDataReceived(buffer.ReadBuffer, bytesRead);
                }
            }

            DisconnectClient();
        }

        private void DisconnectClient()
        {
            if (tcpClient == null)
            {
                return;
            }

            Console.WriteLine("Disconnected from server");

            tcpClient.Close();
        }

        public void AddToPacket(byte[] data)
        {
            if (buffer.CurrentWriteByteCount + data.Length > buffer.WriteBuffer.Length)
            {
                FlushData();
            }

            Array.ConstrainedCopy(data, 0, buffer.WriteBuffer, buffer.CurrentWriteByteCount, data.Length);

            buffer.CurrentWriteByteCount += data.Length;
        }

        public void FlushData()
        {
            clientStream.Write(buffer.WriteBuffer, 0, buffer.CurrentWriteByteCount);
            clientStream.Flush();
            buffer.CurrentWriteByteCount = 0;
        }

        public void SendImmediate(byte[] data)
        {
            AddToPacket(data);
            FlushData();
        }

        public bool IsConnected()
        {
            return tcpClient.Connected;
        }

        public void Disconnect()
        {
            DisconnectClient();
        }
    }
}
