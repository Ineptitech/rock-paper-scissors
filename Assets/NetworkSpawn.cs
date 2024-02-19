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

    async void Update()
    {
        await Task.Run(async () =>
        {
            var res = RPS.Network.NetSock.Receive(ref mpep);
            Console.WriteLine(BitConverter.ToString(res));
            if (res[0] == (byte)PacketHeader.Start)
            {
                GameObject clone = Instantiate(MPlayerPrefab);
                clone.GetComponent<MPlayerNetworkController>().mPlayerEP = mpep;
            }
        });
    }
}
