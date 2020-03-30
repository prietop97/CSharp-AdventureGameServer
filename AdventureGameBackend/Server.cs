using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AdventureGameBackend
{
    class Server
    {
        // Max Player Allowed in the server
        public static int MaxPlayers { get; private set; }

        // Port for the server to listen on
        public static int Port { get; private set; }

        // Dictionary of all the clients connections
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;


        // Socket Connection (From System.Net.Sockets)
        public static TcpListener tcpListener;
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;

            Port = _port;

            Console.WriteLine($"Server starting.");
            InitializeServerData();
            // Creating the socket
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();

            // Starts a loop of accepting tcp clients or players
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on {Port}.");

        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            Console.WriteLine(_client);
            
            // Starts async function again in case we get a new one
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");


            // Gets the next available client/playerwhere stream havent been started yet and assigns that to the new connection
            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed  to connect: Server full");
        }

        // Creates all players of the clients and store them for easy referencing, stream has not started yet
        private static void InitializeServerData()
        {
            for(int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i,new Client(i));
            }
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived }
            };
            Console.WriteLine("Initialized packets");
        }
    }
}
