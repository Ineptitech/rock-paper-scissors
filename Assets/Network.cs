using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace RPS
{
    public enum Team
    {
        rock = 0,
        paper = 1,
        scissors = 2
    }
    public enum PacketHeader
    {
        Start = 0,
        Move = 1,
        AckStart = 2
    }

    #region Packet Structs
    public static class SocketExtensions
    {
        public static void SendToAll(this UdpClient client, byte[] dgram)
        {
            client.Send(dgram, dgram.Length, Network.DEST.ToString(), Network.PORT);
        }
    }
    public class MPlayerStart
    {
        //public string MPlayerName { get; set; }
        public RPS.Team team;
        public MPlayerStart(RPS.Team team = RPS.Team.paper)
        {
            // MPlayerName = name;
            this.team = team;
        }
        public static explicit operator byte[] (MPlayerStart p) => new byte[] { (byte) RPS.PacketHeader.Start, (byte) p.team };

        public static Team Decode(byte[] netdata)
        {
            return (Team) netdata[1];
        }
    }
    // This class should store floats internally, only casting them to a short as necessary
    public class MPlayerUpdate
    {
        public const int SCALE = 100;
        public float x, y, z, r;
        public MPlayerUpdate(float x, float y, float z, float r)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
        }
        public MPlayerUpdate(short sx, short sy, short sz, short sr)
        {
            x = (float) (sx / SCALE);
            y = (float) (sy / SCALE);
            z = (float) (sz / SCALE);
            r = (float) (sr / SCALE);
        }
        public static explicit operator byte[](MPlayerUpdate p)
        {
            List<byte> l = new List<byte> { (byte)RPS.PacketHeader.Move };

            // Cast to format
            short[] sCoords = {
                (short)(p.x * SCALE), 
                (short)(p.y * SCALE), 
                (short)(p.z * SCALE),
                (short)(p.r * SCALE) 
            };

            //Combine raw bytes
            foreach (short v in sCoords)
                l.AddRange(BitConverter.GetBytes(v)); // Scale up float to fit it into short

            return l.ToArray();
        }
        public static Vector4 Decode(byte[] ba)
        {
            return new Vector4(
                (float) (Network.ShortAt(ba, 1) / MPlayerUpdate.SCALE),
                (float) (Network.ShortAt(ba, 3) / MPlayerUpdate.SCALE),
                (float) (Network.ShortAt(ba, 5) / MPlayerUpdate.SCALE),
                Network.ShortAt(ba, 7)
            );
        }
    }
    public class MPlayerAck
    {
        private Team team;
        public MPlayerAck(Team team)
        {   
            this.team = team;
        }
        public static Team Decode(byte[] netdata)
        {
            return (Team) netdata[1];
        }
        public static explicit operator byte[](MPlayerAck p) => new byte[] { (byte)RPS.PacketHeader.Start, (byte) p.team };
    }
    //public struct MPlayerChange
    //{
    //    RPS.Team NewCaste;
    //    IPAddress Tagger;
    //    public static implicit operator byte[](MPlayerChange p)
    //    {
    //        List<byte> l = new List<byte> { (byte)RPS.PacketHeader.Change, (byte)p.NewCaste };
    //        l.AddRange(p.Tagger.GetAddressBytes());
    //        return l.ToArray();
    //    }
    //}

    #endregion

    public static class Network
    {
        // TODO Allow user to select network to bind to. For most people the default will be fine
        // Needs to be public so MPlayer objects can access the buffer.
        public const short PORT = 27000;
        public static readonly IPAddress DEST = IPAddress.Parse("100.117.198.109"); // IPAddress.Broadcast
        public static UdpClient NetSock = new(new IPEndPoint(IPAddress.Any, PORT));

        public static short ShortAt(byte[] ba, int i)
        {
            byte b1 = ba[i];
            byte b2 = ba[i + 1];

            return (short) (b1 | (b2 << 8));
        }
    }
}