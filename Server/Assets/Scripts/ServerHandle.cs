using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int clientIDCheck = _packet.ReadInt();
        string username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != clientIDCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: { _fromClient}) has assumed the wrong client ID ({clientIDCheck})!");
        }
        Server.clients[_fromClient].SendInToGame(username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = _packet.ReadBool();
        }
        Quaternion rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInputs(inputs, rotation);
    }


    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Quaternion shootDir = _packet.ReadQuaternion();
        Server.clients[_fromClient].player.Shoot(shootDir);
    }

}
