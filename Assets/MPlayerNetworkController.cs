using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using RPS;

public class MPlayerNetworkController : MonoBehaviour
{
    public IPEndPoint mPlayerEP; // Assigned when player is spawned

    void Start()
    {
        // mPlayerEP = new IPEndPoint(255255255255, RPS.Network.PORT);
    }

    void FixedUpdate()
    {
        //Update MPlayer's position from net buffer
        var recvBuffer = RPS.Network.NetSock.Receive(ref mPlayerEP);
        if (recvBuffer == null)
        {
            Stop();
            return;
        }
        Vector4 pr = RPS.Network.DecodePacket(recvBuffer);
        
        // The values are scaled up x100 to make use of the short format. Convert back to float 
        GetComponent<Rigidbody>()
            .Move(
                new Vector3(pr.x / 100, pr.y / 100, pr.z / 100),
                Quaternion.Euler(0, pr.w / 100, 0)
            );

        // Check that we're still receiving data from this player. If not, call stop
    }

    void Stop() { 
        // Discard network socket
        
    }
}
