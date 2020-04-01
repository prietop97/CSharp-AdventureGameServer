using System;
using UnityEngine;

class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }
    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        Vector3 _change = _packet.ReadVector3();
        bool _isWalking = _packet.ReadBool();
        Server.clients[_fromClient].player.change = _change;
        Server.clients[_fromClient].player.attackInput = _isWalking;
    }
}
