using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    class Server
    {
        Socket listenerSocket;
        List<Socket> client = new List<Socket>();
        byte[] receivingBuffer = new byte[150];
        string message = "";

        public Server(int port)
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Blocking = false;
            listenerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            Console.WriteLine("Waiting for incoming connection...");
        }

        public void Start()
        {
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
    }
}