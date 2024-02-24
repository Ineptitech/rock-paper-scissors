using RPS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkSpawn : MonoBehaviour
{
    public GameObject MPlayerPrefab;
    private IPEndPoint mpep = new IPEndPoint(IPAddress.Any, 0);

    void Update()
    {
        Task.Run(async () =>
        {
            var res = await RPS.Network.NetSock.ReceiveAsync();
            Console.WriteLine(BitConverter.ToString(res.Buffer));
            if (res.Buffer[0] == (byte)PacketHeader.Start)
            {
                GameObject clone = Instantiate(MPlayerPrefab);
                clone.GetComponent<MPlayerNetworkController>().mPlayerEP = mpep;
            }
        });
    }
}
