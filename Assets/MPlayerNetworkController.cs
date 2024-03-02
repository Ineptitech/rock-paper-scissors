using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using RPS;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

public class MPlayerNetworkController : MonoBehaviour
{
    public IPEndPoint ep; // Assigned when player is spawned

    [SerializeReference]
    CharacterController cc;

    [SerializeReference]
    Rigidbody rb;

    void Start()
    {
    }

    void FixedUpdate()
    {        
        // The values are scaled up x100 to make use of the short format. Convert back to float 
        //GetComponent<Rigidbody>()
        //    .Move(
        //        new Vector3(pr.x / 100, pr.y / 100, pr.z / 100),
        //        Quaternion.Euler(0, pr.w / 100, 0)
        //    );

        // Check that we're still receiving data from this player. If not, call stop
    }

    void Stop() { 
        // Discard network socket
        
    }

    public void Move(Vector4 movement)
    {
        //cc.attachedRigidbody.Move(new (movement.x, movement.y, movement.z), Quaternion.Euler(0, movement.w, 0));
        rb.Move(new(movement.x, movement.y, movement.z), Quaternion.Euler(0, movement.w, 0));
    }
    public void Move(float x, float y, float z)
    {
    }
}
