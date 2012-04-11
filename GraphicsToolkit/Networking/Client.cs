using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GraphicsToolkit.Networking
{
    public delegate void ClientHandlePacketData(byte[] data);

    public class Client
    {
        private TcpClient clientSocket;
        private NetworkStream clientStream;
        public event ClientHandlePacketData OnDataReceived;
        private NetworkBuffer buffer;

        public Client(int port)
        {
            buffer = new NetworkBuffer();
            buffer.WriteBuffer = new byte[4096];
            buffer.CurrentWriteByteCount = 0;

            clientSocket = new TcpClient("localhost", port);
            clientStream = clientSocket.GetStream();
            Console.WriteLine("Connected to server");
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

        private void FlushData()
        {
            clientStream.Write(buffer.WriteBuffer, 0, buffer.CurrentWriteByteCount);
            clientStream.Flush();
            buffer.CurrentWriteByteCount = 0;
        }

        public void SendData(byte[] data)
        {
            AddToPacket(data);
            FlushData();
        }

        public bool IsConnected()
        {
            return clientSocket.Connected;
        }

        public void Disconnect()
        {
            clientSocket.Close();
        }
    }
}
