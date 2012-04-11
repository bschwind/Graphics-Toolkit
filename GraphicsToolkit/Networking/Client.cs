using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GraphicsToolkit.Networking
{
    public delegate void HandlePacketData(byte[] data);

    public class Client
    {
        private TcpClient clientSocket;
        private NetworkStream clientStream;
        public event HandlePacketData OnDataReceived;

        public Client(int port)
        {
            clientSocket = new TcpClient("localhost", port);
            clientStream = clientSocket.GetStream();
            Console.WriteLine("Connected to server");

            if (OnDataReceived != null)
            {
                OnDataReceived(null);
            }
        }

        public void SendData(byte[] data)
        {
            clientStream.Write(data, 0, data.Length);
            clientStream.Flush();
        }

        public bool IsConnected()
        {
            if (OnDataReceived != null)
            {
                OnDataReceived(null);
            }
            return clientSocket.Connected;
        }

        public void Disconnect()
        {
            clientSocket.Close();
        }
    }
}
