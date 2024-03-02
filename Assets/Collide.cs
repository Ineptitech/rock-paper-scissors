using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPS;

public class Collide : MonoBehaviour
{
    [SerializeReference]
    MeshFilter filter;

    [SerializeField]
    public Team currentTeam;

    public Mesh[] meshes;

    // Start is called before the first frame update
    void Start()
    {
        SetTeam(currentTeam);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision with {collision.gameObject}");

        var comp = collision.collider.GetComponent<Collide>();
        if (comp == null) return;

        Team colliderTeam = comp.currentTeam;

        if ((int) colliderTeam == ((int) currentTeam + 1) % 3) {
            SetTeam(colliderTeam);
        }
    }

    public void SetTeam(Team team)
    {
        currentTeam = team;
        filter.mesh = meshes[(int)currentTeam];

        RPS.Network.NetSock.SendToAll((byte[])new RPS.MPlayerChange(team));
    }
}
