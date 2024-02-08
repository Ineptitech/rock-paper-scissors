using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RPS
{
    public class Network
    {
        // Data structure

        // UID - 8 bytes. first 8 bytes from SHA256(MAC address)

        // Pos: x, y, z 2 bytes each, quantized
        // short quantizedX = (short)(pos.X * 100);
        // short quantizedY = (short)(pos.Y * 100);
        // short quantizedZ = (short)(pos.Z * 100);

        // Rot: only Y (vertical axis)
        // short quantizedYrot = (short)(rot.X * 100);


        // TODO Allow user to select network to bind to. For most people the default will be fine
        public static UdpClient netSock;
        public const short PORT = 27000;

        IPEndPoint[] playerEPs;


        public Network()
        {
            netSock = new UdpClient(new IPEndPoint(IPAddress.Any, PORT));
            // Create netsock to listen and broadcast.
            // Filtering and validating is not our problem. Let firewalls do that
            // Needs to be public so MPlayer objects can access the buffer.
        }

        // TODO make a packet struct or class

        public static byte[] EncodePacket(Vector3 pos, Vector3 rot)
        {
            short
                x = (short)(pos.x * 100),
                y = (short)(pos.y * 100),
                z = (short)(pos.z * 100),
                w = (short) rot.y;

            List<byte> l = new List<byte>();

            // Take out all the bytes and add them
            foreach (short v in new short[] {x, y, z, w})
                l.AddRange(BitConverter.GetBytes(v));


            return l.ToArray();
        }
        private static short ShortAt(byte[] ba, int i)
        {
            byte b1 = ba[i * 2];
            byte b2 = ba[i * 2 + 1];

            return (short)(b1 | (b2 << 8));
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
        // Need to monitor for start command from new players
        // add their IP to IPEndPoint[] playerIPs
        // Spawn them in
        // Assign the MPlayerNetworkController script component
        // Set their IP address in the field
    }
}