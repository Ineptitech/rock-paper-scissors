using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPS;

public class Collide : MonoBehaviour
{

    [SerializeField]
    public Team currentTeam = Team.paper;

    [SerializeField]
    MeshFilter filter;

    public Mesh[] meshes;

    // Start is called before the first frame update
    void Start()
    {
        SetTeam(currentTeam);
    }

    // Move is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision with {collision.gameObject}");

        Team colliderTeam = collision.collider.GetComponent<Collide>().currentTeam;

        if ((int) colliderTeam == ((int) currentTeam + 1) % 3) {
            SetTeam(colliderTeam);
        }
    }

    public void SetTeam(Team team)
    {
        this.currentTeam = team;
        filter.mesh = meshes[(int)currentTeam];
    }
}
