using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string msg = _packet.ReadString();
        int myId = _packet.ReadInt();

        Debug.Log($"Message from server: {msg}");
        Client.instance.myId = myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

    }
#region Player
    public static void SpawnPlayer(Packet _packet)
    {
        int id = _packet.ReadInt();
        string username = _packet.ReadString();
        Vector3 position = _packet.ReadVector3();
        Quaternion rotation = _packet.ReadQuaternion();
        GameManager.instance.SpawnPlayer(id, username, position, rotation);
        GameManager.instance.UpdateScore();
    }

    public static void PlayerPosition(Packet _packet)
    {
        int id = _packet.ReadInt();
        Vector3 posistion = _packet.ReadVector3();
        
        GameManager.players[id].transform.position = posistion;
    }

    public static void PlayerRotation(Packet _packet)
    {
        int id = _packet.ReadInt();
        Quaternion rotation = _packet.ReadQuaternion();

        GameManager.players[id].transform.rotation = rotation;
    }

    public static void PlayerDisconnected(Packet _packet) //remove the players rockets
    {
        int id = _packet.ReadInt();

        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
        GameManager.instance.UpdateScore();
    }

    public static void playerMultiply(Packet _packet)
    {
        int id = _packet.ReadInt();
        int mul = _packet.ReadInt();
        GameManager.players[id].setMultiplier(mul);
    }
    public static void playerRespawned(Packet _packet)
    {
        int id = _packet.ReadInt();
        GameManager.players[id].Respawn();
        
    }
    public static void playerDead(Packet _packet)
    {
        int id = _packet.ReadInt();
        GameManager.players[id].Die();
        GameManager.instance.UpdateScore();
    }
    #endregion
    public static void SetScore(Packet _packet)
    {
        int id = _packet.ReadInt();
        int score = _packet.ReadInt();
        GameManager.instance.SetScore(id, score);
    }

    #region Rocket
    public static void RocketSpawn(Packet _packet)
    {
        Debug.Log("Recieved spawn rocket");
        int pid = _packet.ReadInt();
        int rid = _packet.ReadInt();
        Vector3 position = _packet.ReadVector3();
        Quaternion rotation = _packet.ReadQuaternion();
        GameManager.instance.SpawnRocket(pid, rid, position, rotation);
    }

    public static void RocketExplode(Packet _packet)
    {
        int pid = _packet.ReadInt();
        int rid = _packet.ReadInt();
        GameManager.instance.RocketExplode(pid, rid);
    }
    #endregion
}
