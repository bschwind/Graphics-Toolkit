using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GraphicsToolkit.Networking
{
    public class Server
    {
        TcpListener listener;

        public Server(int port)
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
                clientThread.Start(client);
            }
        }

        private void WorkWithClient(object client)
        {
            TcpClient tcpClient = client as TcpClient;
            if (tcpClient == null)
            {
                return;
            }

            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occurred
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                Console.WriteLine(encoder.GetString(message, 0, bytesRead));
            }

            tcpClient.Close();
        }
    }
}
