using UnityEngine;
using System.Net;
using System.Text;
using System;
using RPS;

public class PlayerNetworkController : MonoBehaviour
{
    byte[] packet;

    // Move is called once per frame
    void Update()
    {
        // the float position values need to be adapted to work in short format
        packet = (byte[]) new RPS.MPlayerUpdate(
            transform.position.x, 
            transform.position.y,
            transform.position.z,
            transform.rotation.eulerAngles.y
        );

        RPS.Network.NetSock.SendToAll(packet);
    }
}