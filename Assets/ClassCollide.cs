using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPS
{

    public enum Caste
    {
        paper = 0,
        scissors = 1,
        rock = 2
    }
}
public class Collide : MonoBehaviour
{

    [SerializeField]
    RPS.Caste currentClass = RPS.Caste.paper;

    [SerializeField]
    MeshFilter filter;

    public Mesh[] meshes;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision with {collision.gameObject}");

        RPS.Caste colliderClass = collision.collider.GetComponent<Collide>().currentClass;

        if ((int)colliderClass == ((int)currentClass + 1) % 3)
        {
            currentClass = colliderClass;
            filter.mesh = meshes[(int)currentClass];
        }


    }
}