using System.Collections;
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

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
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
        using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(_player.id);
            packet.Write(_player.username);
            packet.Write(_player.transform.position);
            packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, packet);//using tcp as it is esential that they get the packet to spawn
        }

    }

    public static void PlayerRotation(Player _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotate))
        {
            packet.Write(_player.id);
            packet.Write(_player.transform.rotation);
            SendUDPDataToAll(_player.id, packet);
        }
    }

    public static void PlayerPosition(Player _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(_player.id);
            packet.Write(_player.transform.position);
            SendUDPDataToAll(packet);
        }
    }

    public static void PlayerDisconnected(int _playerid)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            packet.Write(_playerid);

            SendTCPDataToAll(packet);
        }
    }

    //playerMultiply,
    //playerRespawned,//why do i need this? why not just spawn
    //playerDead
    public static void playerMultiply(Player _p)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            packet.Write(_p.id);
            packet.Write(_p.multiplyer);

            SendTCPDataToAll(packet);
        }
    }
    public static void playerRespawned(Player _p)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRespawned))
        {
            packet.Write(_p.id);

            SendTCPDataToAll(packet);
        }
    }
    public static void playerDead(Player _p)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDead))
        {
            packet.Write(_p.id);

            SendTCPDataToAll(packet);
        }
    }

    public static void RocketSpawn(GameObject _r)
    {
        Rocket r = _r.GetComponent<Rocket>();
        using (Packet packet = new Packet((int)ServerPackets.rocketSpawn))
        {
            packet.Write(r.playerid);
            packet.Write(r.id);
            packet.Write(_r.transform.position);
            packet.Write(_r.transform.rotation);

            SendTCPDataToAll(packet);
        }
        Debug.Log($"Sent Rocket player id: {r.playerid}");
    }

    public static void RocketExplode(int _pid, int _rid)
    {//using udp so that it happens as close to the real pos as posible since rocket pos isnt sent
        using (Packet packet = new Packet((int)ServerPackets.rocketExplode))
        {
            packet.Write(_pid);
            packet.Write(_rid);

            SendUDPDataToAll(packet);
        }
        Debug.Log($"Sent Rocket Explode. Player id: {_pid} Rocketid: {_rid}");
    }
    public static void SetScore(int _id, int _score)
    {
        using (Packet packet = new Packet((int)ServerPackets.scoreSet))
        {
            packet.Write(_id);
            packet.Write(_score);

            SendTCPDataToAll(packet);
        }
        Debug.Log($"player id {_id} score changed to {_score}");
    }
    #endregion

}
