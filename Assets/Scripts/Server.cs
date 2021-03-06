﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
public class Server
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
    public static UdpClient udpListener;
    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;

        Port = _port;

        Debug.Log($"Server starting.");
        InitializeServerData();
        // Creating the socket
        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);
        // Starts a loop of accepting tcp clients or players
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        Debug.Log($"Server started on {Port}.");

    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        Debug.Log(_client);

        // Starts async function again in case we get a new one
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");


        // Gets the next available client/playerwhere stream havent been started yet and assigns that to the new connection
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed  to connect: Server full");
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }
                if (clients[_clientId].udp.endPoint == null)
                {
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {

            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {

            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }

    // Creates all players of the clients and store them for easy referencing, stream has not started yet
    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }
        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                {(int)ClientPackets.moveRoom, ServerHandle.MoveRoom },
            };
        Debug.Log("Initialized packets");
    }

    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }
}
