using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class TestMPlay : MonoBehaviour
{
    // Start is called before the first frame update
    IPEndPoint mPlayerEP = new IPEndPoint(1921681196, RPS.Network.PORT);
    CharacterController cc;
    void Start()
    {
        cc = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Task.Run(async () =>
        {
            var res = await RPS.Network.NetSock.ReceiveAsync();
            if (res.Buffer[0] == (byte) RPS.PacketHeader.Update)
            {
                Vector4 update = RPS.MPlayerUpdate.Decode(res.Buffer);
                cc.Move(new(update.x, update.y + 10, update.z));
                gameObject.transform.Rotate(0, update.w, 0);
            }
        });
    }
}
