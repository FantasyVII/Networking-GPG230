using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Network
{
    class Program
    {
        static void Server()
        {
            Socket listenerSocket;
            Socket acceptSocket;

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(new IPEndPoint(IPAddress.Loopback, 27016));
            Console.WriteLine("Waiting for incoming connection...");
            listenerSocket.Listen(5);

            acceptSocket = listenerSocket.Accept();
            Console.WriteLine("Connection established...");
            Console.WriteLine("Receiving data...");

            byte[] receivingBuffer = new byte[100];
            string message = "";

            while (true)
            {
                //--------------------- RECEIVING DATA --------------------------
                acceptSocket.Receive(receivingBuffer);
                message = Encoding.ASCII.GetString(receivingBuffer);
                Console.WriteLine(message);
                //--------------------- RECEIVING DATA --------------------------

                //--------------------- SENDING DATA --------------------------
                message = Console.ReadLine();
                byte[] sendingBuffer = Encoding.ASCII.GetBytes(message);
                acceptSocket.Send(sendingBuffer);
                //--------------------- SENDING DATA --------------------------
            }

            acceptSocket.Close();
            listenerSocket.Close();
        }

        static void Client()
        {
            Socket mainSocket;
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Attempting to connect to server...");

            mainSocket.Connect(new IPEndPoint(IPAddress.Loopback, 27016));
            Console.WriteLine("Connection established...");

            Console.WriteLine("Please type the message you want to send");

            byte[] receivingBuffer = new byte[100];
            string message = "";

            while (true)
            {
                //--------------------- SENDING DATA --------------------------
                message = Console.ReadLine();
                byte[] sendingBuffer = Encoding.ASCII.GetBytes(message);
                mainSocket.Send(sendingBuffer);
                //--------------------- SENDING DATA --------------------------

                //--------------------- RECEIVING DATA --------------------------
                mainSocket.Receive(receivingBuffer);
                message = Encoding.ASCII.GetString(receivingBuffer);
                Console.WriteLine(message);
                //--------------------- RECEIVING DATA --------------------------
            }

            mainSocket.Close();
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-server":
                        Server();
                        break;
                    case "-client":
                        Client();
                        break;
                    default:
                        Console.WriteLine("Cannot run program with " + args[0] + " as an argument");
                        break;
                }
            }
            else
                Console.WriteLine("Cannot run program with no arugments. Please run the program with -server or -client as arugments");
        }
    }
}