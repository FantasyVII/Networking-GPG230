using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace Network
{
    class Program
    {
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

                        Server server = new Server(int.Parse(args[1]));
                        server.Start();

                        break;
                    case "-client":
                        if (args.Length < 4)
                        {
                            Console.WriteLine("Please provide the server IP address, port number and nickname");
                            return;
                        }
                        Client client = new Client(args[1], int.Parse(args[2]), args[3], Util.ConvertStringToColor(args[4]));
                        client.Start();
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