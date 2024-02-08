using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using RPS;

public class PlayerNetworkController : MonoBehaviour
{
    GameObject go;
    Transform goT;
    byte[] packet;
    // Start is called before the first frame update
    void Start()
    {
        goT = go.transform;
    }

    // Update is called once per frame
    void Update()
    {
        packet = RPS.Network.EncodePacket(new Vector4(goT.position.x, goT.position.y, goT.position.z, goT.rotation.y));
        RPS.Network.netSock.Send(packet, packet.Length, IPAddress.Broadcast, RPS.Network.PORT);
    }
}
