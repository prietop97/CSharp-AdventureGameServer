

using System.Collections.Generic;
using UnityEngine;

class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int room, Packet _packet)
    {
        _packet.WriteLength();
        foreach (KeyValuePair<int,Client> client in Server.clients)
        {
            if(client.Value.player.room == room)
            {
                client.Value.tcp.SendData(_packet);
            }
        }

    }
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }

    }
    #region Packets
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            SendTCPData(_toClient, _packet);
        }
    }
    public static void AnimatorWalk(int _id, Vector3 _change)
    {
        using (Packet _packet = new Packet((int)ServerPackets.animatorWalk))
        {
            _packet.Write(_id);
            _packet.Write(_change);
            SendUDPDataToAll(_packet);
        }
    }

    public static void AnimatorIsWalking(int _id, bool _isWalking)
    {
        using (Packet _packet = new Packet((int)ServerPackets.animatorIsWalking))
        {
            _packet.Write(_id);
            _packet.Write(_isWalking);
            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            SendUDPDataToAll(_packet);
        }
    }
    #endregion
}
