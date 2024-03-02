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
    Rigidbody rb;

    [SerializeReference]
    MeshFilter filter;

    [SerializeField]
    public Team currentTeam;

    public Mesh[] meshes;

    public void Move(Vector4 movement)
    {
        rb.Move(new (movement.x, movement.y, movement.z), Quaternion.Euler(0, movement.w, 0));
    }

    public void SetTeam(Team team)
    {
        this.currentTeam = team;
        filter.mesh = meshes[(int)currentTeam];
    }
}
