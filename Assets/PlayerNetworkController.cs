using UnityEngine;
using System.Net;
using System.Text;
using System;
using RPS;

public class PlayerNetworkController : MonoBehaviour
{
    byte[] packet;


    // Update is called once per frame
    void Update()
    {
        // the float position values need to be adapted to work in short format
        packet = new RPS.MPlayerUpdate {
            x = (short)(transform.position.x), 
            y = (short)(transform.position.y),
            z = (short)(transform.position.z),
            r = (short)(transform.rotation.eulerAngles.y) 
        };
        Debug.Log($"Sending {MPlayerUpdate.Decode(packet)}");

        RPS.Network.NetSock.Send(packet, packet.Length, IPAddress.Broadcast.ToString(), RPS.Network.PORT);
    }
}