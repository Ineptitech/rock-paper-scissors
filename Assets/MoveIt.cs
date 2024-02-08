using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIt : MonoBehaviour
{
    [SerializeField]
    public Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        body.AddForce(Vector3.forward * 100f, ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
