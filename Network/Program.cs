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
        static void Server(int port)
        {
            Socket listenerSocket;
            List<Socket> client = new List<Socket>();

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Blocking = false;
            listenerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            Console.WriteLine("Waiting for incoming connection...");

            byte[] receivingBuffer = new byte[150];
            string message = "";

            while (true)
            {
                listenerSocket.Listen(10);

                //--------------------- LISTINING FOR INCOMING CONNECTIONS --------------------------
                try
                {
                    Socket acceptSocket = listenerSocket.Accept();
                    Console.WriteLine("Connection established...");
                    client.Add(acceptSocket);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.WouldBlock)
                        Console.WriteLine(e);
                }
                //--------------------- LISTINING FOR INCOMING CONNECTIONS --------------------------

                for (int i = 0; i < client.Count; i++)
                {
                    Array.Clear(receivingBuffer, 0, receivingBuffer.Length);
                    try
                    {
                        //--------------------- RECEIVING DATA --------------------------
                        client[i].Receive(receivingBuffer);
                        message = Encoding.ASCII.GetString(receivingBuffer);
                        //--------------------- RECEIVING DATA --------------------------

                        //------------ SENDING DATA ------------------
                        for (int j = 0; j < client.Count; j++)
                        {
                            try
                            {
                                byte[] sendingBuffer = Encoding.ASCII.GetBytes(message);
                                client[j].Send(sendingBuffer);
                            }
                            catch (SocketException e)
                            {
                                if (e.SocketErrorCode != SocketError.WouldBlock)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                        }
                        //------------ SENDING DATA ------------------

                        Console.WriteLine(message);
                    }
                    catch (SocketException e)
                    {
                        if (e.SocketErrorCode != SocketError.WouldBlock)
                        {
                            if (e.SocketErrorCode == SocketError.ConnectionAborted || e.SocketErrorCode == SocketError.ConnectionReset)
                            {
                                Console.WriteLine("Client Disconnected...");
                                client[i].Close();
                                client.RemoveAt(i);
                            }
                            else
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                }
            }

            //listenerSocket.Close();
        }

        static void Client(string ipOrHostName, int port, string nickname)
        {
            Socket mainSocket;
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSocket.Blocking = false;
            Console.WriteLine("Attempting to connect to server...");

            IPAddress ip;
            bool isIpValid = IPAddress.TryParse(ipOrHostName, out ip);

            byte[] receivingBuffer = new byte[150];
            string sendingMessage = "";
            string receiveMessage = "";

            bool connectedToServer = false;
            int cursorIndex = 0;

            while (true)
            {
                if (!connectedToServer)
                {
                    try
                    {
                        connectedToServer = true;
                        if (isIpValid)
                            mainSocket.Connect(new IPEndPoint(ip, port));
                        else
                            mainSocket.Connect(ipOrHostName, port);

                        Console.WriteLine("Connection established...");
                        Console.WriteLine("Please type the message you want to send");
                    }
                    catch (SocketException e)
                    {
                        if (e.SocketErrorCode != SocketError.WouldBlock)
                        {
                            if (e.SocketErrorCode == SocketError.ConnectionAborted || e.SocketErrorCode == SocketError.ConnectionReset)
                            {
                                Console.WriteLine("Server Disconnected...");
                                mainSocket.Close();
                                return;
                            }
                            else
                            {
                                Console.WriteLine(e);
                            }
                        }
                    }
                }

                //--------------------- SENDING DATA --------------------------
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(false);

                    if (key.Key == ConsoleKey.Backspace)
                    {
                        if (cursorIndex > 0)
                        {
                            sendingMessage = sendingMessage.Remove(sendingMessage.Length - 1);
                            Console.Write(' ');
                            Console.Write(key.KeyChar);
                            cursorIndex--;
                        }
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        string finalMessage = nickname + ": " + sendingMessage;

                        try
                        {
                            byte[] sendingBuffer = Encoding.ASCII.GetBytes(finalMessage);
                            mainSocket.Send(sendingBuffer);
                            sendingMessage = "";
                            cursorIndex = 0;
                        }
                        catch (SocketException e)
                        {
                            if (e.SocketErrorCode != SocketError.WouldBlock)
                            {
                                if (e.SocketErrorCode == SocketError.ConnectionAborted || e.SocketErrorCode == SocketError.ConnectionReset)
                                {
                                    Console.WriteLine("Server Disconnected...");
                                    mainSocket.Close();
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine(e);
                                }
                            }
                        }
                    }
                    else
                    {
                        sendingMessage += key.KeyChar;
                        cursorIndex++;
                    }
                }
                //--------------------- SENDING DATA --------------------------

                //--------------------- RECEIVING DATA --------------------------
                try
                {
                    Array.Clear(receivingBuffer, 0, receivingBuffer.Length);
                    mainSocket.Receive(receivingBuffer);
                    receiveMessage = Encoding.ASCII.GetString(receivingBuffer);

                    ClearLine();
                    Console.WriteLine(receiveMessage);
                    Console.Write(sendingMessage);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.WouldBlock)
                    {
                        if (e.SocketErrorCode == SocketError.ConnectionAborted || e.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            Console.WriteLine("Server Disconnected...");
                            mainSocket.Close();
                            return;
                        }
                        else
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            //--------------------- RECEIVING DATA --------------------------

            //mainSocket.Close();
        }

        static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-server":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Please provide the server port number");
                            return;
                        }
                        Server(int.Parse(args[1]));
                        break;
                    case "-client":
                        if (args.Length < 4)
                        {
                            Console.WriteLine("Please provide the server IP address, port number and nickname");
                            return;
                        }
                        Client(args[1], int.Parse(args[2]), args[3]);
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