using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            try
            {
                _packet.Write(UIManager.instance.usernameField.text);
            }
            catch
            {
                _packet.Write("Username");
            }

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        //Debug.Log($"My id: {Client.instance.myId}");
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(_inputs.Length);
            foreach (bool input in _inputs)
            {
                packet.Write(input);
            }
            packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(packet);
        }
    }

    public static void PlayerShoot(Quaternion _lookDir)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(_lookDir);

            SendTCPData(packet);
        }
    }
    #endregion
}
