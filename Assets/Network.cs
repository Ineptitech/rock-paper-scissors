using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using RPS;
using System.Xml.Linq;

namespace RPS
{
    #region Packet Structs

    enum PacketHeader { 
        Start = 0,
        Update = 1,
        Change = 2
    }
    public struct MPlayerStart
    {
        //public string MPlayerName { get; set; }
        public RPS.Caste MPlayerCaste { get; set; }
        public MPlayerStart(RPS.Caste caste = RPS.Caste.paper)
        {
            // MPlayerName = name;
            MPlayerCaste = caste;
        }
        public static implicit operator byte[] (MPlayerStart p) => new byte[] { (byte) RPS.PacketHeader.Start, (byte) p.MPlayerCaste };
    }
    public class MPlayerUpdate
    {
        public const int SCALE = 100;
        private short x, y, z, r;
        public MPlayerUpdate(float fx, float fy, float fz, float fr)
        {
            x = (short) (fx * SCALE);
            y = (short) (fy * SCALE);
            z = (short) (fz * SCALE);
            r = (short) (fr * SCALE);
        }

        public static implicit operator byte[](MPlayerUpdate p)
        {
            List<byte> l = new List<byte> { (byte)RPS.PacketHeader.Update };
            //Combine raw bytes
            foreach (short v in new short[] { p.x, p.y, p.z, p.r })
                l.AddRange(BitConverter.GetBytes(v)); // Scale up float to fit it into short

            return l.ToArray();
        }
        public static Vector4 Decode(byte[] ba)
        {
            return new Vector4(
                x: BitConverter.ToInt16(new byte[] { ba[1], ba[2] }) / SCALE,
                y: BitConverter.ToInt16(new byte[] { ba[3], ba[4] }) / SCALE,
                z: BitConverter.ToInt16(new byte[] { ba[5], ba[6] }) / SCALE,
                w: BitConverter.ToInt16(new byte[] { ba[7], ba[8] })
            );
        }
    }
    public struct MPlayerChange
    {
        RPS.Caste NewCaste;
        IPAddress Tagger;
        public static implicit operator byte[](MPlayerChange p)
        {
            List<byte> l = new List<byte> { (byte)RPS.PacketHeader.Change, (byte)p.NewCaste };
            l.AddRange(p.Tagger.GetAddressBytes());
            return l.ToArray();
        }
    }

    #endregion

    public static class Network
    {
        // TODO need a secondary thread that monitors for network messages and updates local game accordingly 

        // TODO Allow user to select network to bind to. For most people the default will be fine
        // Needs to be public so MPlayer objects can access the buffer.
        public const short PORT = 27000;
        public static UdpClient NetSock = new UdpClient(new IPEndPoint(IPAddress.Any, PORT));


        // TODO make a packet struct or class

        //public static byte[] EncodePacket(Vector3 pos, Vector3 rot)
        //{
        //    short
        //        x = (short)(pos.x * 100),
        //        y = (short)(pos.y * 100),
        //        z = (short)(pos.z * 100),
        //        w = (short) rot.y;

        //    List<byte> l = new List<byte>();

        //    // Take out all the bytes and add them
        //    foreach (short v in new short[] {x, y, z, w})
        //        l.AddRange(BitConverter.GetBytes(v));


        //    return l.ToArray();
        //}
        //public static byte[] EncodePacket(float posX, float posY, float posZ, float rot)
        //{
        //    short
        //        x = (short)(posX * 100),
        //        y = (short)(posY * 100),
        //        z = (short)(posZ * 100),
        //        w = (short) rot;

        //    List<byte> l = new List<byte>();

        //    // Take out all the bytes and add them
        //    foreach (short v in new short[] { x, y, z, w })
        //        l.AddRange(BitConverter.GetBytes(v));


        //    return l.ToArray();
        //}
        //public static byte[] EncodePacket(Vector4 pos)
        //{
        //    short
        //        x = (short)(pos.x * 100),
        //        y = (short)(pos.y * 100),
        //        z = (short)(pos.z * 100),
        //        w = (short) pos.w;

        //    List<byte> l = new List<byte>();

        //    // Take out all the bytes and add them
        //    foreach (short v in new short[] { x, y, z, w })
        //        l.AddRange(BitConverter.GetBytes(v));


        //    return l.ToArray();
        //}
        private static short ShortAt(byte[] ba, int i)
        {
            byte b1 = ba[i * 2];
            byte b2 = ba[i * 2 + 1];

            return (short)(b1 | (b2 << 8));
            //return BitConverter.ToInt16(new byte[] { b1, b2 });
        }
        public static Vector4 DecodePacket(byte[] packet)
        {
            short
                x = ShortAt(packet, 0), // gonna give it to ya
                y = ShortAt(packet, 1),
                z = ShortAt(packet, 2),
                w = ShortAt(packet, 3);

            // Decode packet
            return new Vector4(x, y, z, w);
        }
    }
}