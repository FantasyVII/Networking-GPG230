using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket listenerSocket;
            Socket acceptSocket;

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(new IPEndPoint(IPAddress.Loopback, 27016));
            Console.WriteLine("Waiting for incoming connection...");
            listenerSocket.Listen(5);

            acceptSocket = listenerSocket.Accept();
            Console.WriteLine("Connection established...");
        }
    }
}