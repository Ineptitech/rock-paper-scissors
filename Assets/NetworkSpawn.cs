using OpenCover.Framework.Model;
using RPS;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class NetworkSpawn : MonoBehaviour
{
    [SerializeField]
    public Collide playerCollide;

    public GameObject MPlayerPrefab;
    public ConcurrentQueue<PendingPacket> pendingPackets = new();
    public struct PendingPacket {
        public IPEndPoint endPoint;
        public PacketHeader packetType;
        public object packet;
    }

    public Dictionary<IPAddress, MPlayerNetworkController> MPlayers = new();

    void Start()
    {
        // Broadcast start
        RPS.Network.NetSock.SendToAll((byte[]) new RPS.MPlayerStart(playerCollide.currentTeam));

        new Thread(() =>
        {
            IPEndPoint mpep = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var dgram = RPS.Network.NetSock.Receive(ref mpep);
                object decodedPacket = null;

                if (dgram[0] == (byte)PacketHeader.Start)
                    decodedPacket = MPlayerStart.Decode(dgram);

                if (dgram[0] == (byte)PacketHeader.AckStart)
                    decodedPacket = MPlayerAck.Decode(dgram);

                if (dgram[0] == (byte)PacketHeader.Move)
                    decodedPacket = MPlayerUpdate.Decode(dgram);

                if (dgram[0] == (byte)PacketHeader.Change)
                    decodedPacket = MPlayerChange.Decode(dgram);

                pendingPackets.Enqueue(
                    new PendingPacket { 
                        endPoint = mpep, 
                        packetType = (PacketHeader) dgram[0],
                        packet = decodedPacket 
                    });
            }
        }).Start();
    }
    void Update()
    {
        while (pendingPackets.TryDequeue(out PendingPacket pendingFrame))
        {
            IPAddress ip = pendingFrame.endPoint.Address;

            //Debug.Log($"Packet from {ip.ToString()}");
            switch (pendingFrame.packetType)
            {
                // In the case of Start or AckStart, we want essentially the same behavior and result -
                // Spawn in the player as indicated if they haven't already been spawned in
                // Packets just need to be differentiated to prevent an infinite acknowledgement loop.
                case PacketHeader.Start:
                    {
                        if (!MPlayers.ContainsKey(ip))
                        {
                            GameObject clone = Instantiate(MPlayerPrefab);
                            MPlayers.Add(pendingFrame.endPoint.Address, clone.GetComponent<MPlayerNetworkController>());
                        }

                        var ply = MPlayers[ip];

                        ply.ep = pendingFrame.endPoint;
                        ply.SetTeam((Team)pendingFrame.packet);

                        // Transmit Start ACK
                        byte[] netmsg = (byte[])new MPlayerAck(playerCollide.currentTeam);
                        RPS.Network.NetSock.SendToAll(netmsg);

                        break;
                    }
                case PacketHeader.AckStart:
                    {
                        if (!MPlayers.ContainsKey(ip))
                        {
                            GameObject clone = Instantiate(MPlayerPrefab);
                            MPlayers.Add(pendingFrame.endPoint.Address, clone.GetComponent<MPlayerNetworkController>());
                        }

                        var ply = MPlayers[ip];
                        ply.ep = pendingFrame.endPoint;
                        ply.SetTeam((Team)pendingFrame.packet);

                        break;
                    }
                case PacketHeader.Move:
                    {
                        if (!MPlayers.ContainsKey(ip)) break; // Dee Daws Pro Tech Shun
                        if (pendingFrame.packet.GetType() != typeof(Vector4)) break;

                        Vector4 update = (Vector4) pendingFrame.packet;
                        MPlayers[ip].Move(update);

                        break;
                    }
                case PacketHeader.Change:
                    {
                        if (!MPlayers.ContainsKey(ip)) break; // Dee Daws Pro Tech Shun

                        MPlayers[ip].SetTeam((Team)pendingFrame.packet);
                        break;
                    }
            }
        }
    }
}
