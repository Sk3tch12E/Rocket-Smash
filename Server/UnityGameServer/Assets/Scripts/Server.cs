using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>(); //maybe not the best type to use?

    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    public static TcpListener tcpListener;//Initializes a new instance of the System.Net.Sockets.TcpListener class that listens on the specified port
    public static UdpClient udpListener; //Initializes a new instance of the System.Net.Sockets.UdpClient class and binds it to the local port number provided

    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }

    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        Debug.Log("Starting server...");
        InitServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();        
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);//Begin runs async
                                                                                      
        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on {Port}.");
        //Debug.Log($"the server is set to {}");
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }
        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full");
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpListener.EndReceive(_result, ref clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (data.Length < 4)
            {
                return;
            }

            using (Packet packet = new Packet(data))
            {
                int clientId = packet.ReadInt();

                if (clientId == 0)
                {
                    return;
                }

                if (clients[clientId].udp.endPoint == null)//is this a new connection? if so the packet is the one used to open client port
                {
                    clients[clientId].udp.Connect(clientEndPoint);
                    return;//skip the handle data
                }

                if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())//check that end points match to stop people from sending fake id
                {//tostring used otherwise always false for some reason
                    clients[clientId].udp.HandleData(packet);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error receiving UDP data: {ex}");
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
        catch (Exception ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {ex}");
        }
    }

    private static void InitServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
            {(int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
            {(int)ClientPackets.playerShoot, ServerHandle.PlayerShoot}
        };

        Debug.Log("Initialized packets.");
    }
}
