using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    class Client
    {
        Socket mainSocket;
        IPAddress ip;

        string ipOrHostName = "";
        int port = 0;
        string nickname = "";

        byte[] receivingBuffer = new byte[150];
        string sendingMessage = "";
        string receiveMessage = "";

        bool connectedToServer = false;
        int cursorIndex = 0;
        bool isIpValid = false;

        public Client(string ipOrHostName, int port, string nickname)
        {
            this.ipOrHostName = ipOrHostName;
            this.port = port;
            this.nickname = nickname;

            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSocket.Blocking = false;
            Console.WriteLine("Attempting to connect to server...");

            isIpValid = IPAddress.TryParse(ipOrHostName, out ip);
        }

        void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        public void Start()
        {
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
    }
}