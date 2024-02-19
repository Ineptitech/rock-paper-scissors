using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIt : MonoBehaviour
{
    private const float PLAYER_SPEED = 2.5f;
    private const float THIRD_PERSON_RANGE = 3f;

    [SerializeReference]
    protected GameObject cam;

    protected CharacterController controller;

    private float scrolled = 0f;


    protected Vector2 lookRotate = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        // Looking
        //Rotate the player horizontally, but the camera vertically
        // Move this stuff into a separate function that can be overridden by children?
        lookRotate.x = Input.GetAxis("Mouse X");
        lookRotate.y -= Input.GetAxis("Mouse Y");
        lookRotate.y = Mathf.Clamp(lookRotate.y, -70, 80);

        cam.transform.localEulerAngles = new Vector3(lookRotate.y, 0.0f, 0.0f);

        transform.Rotate(Vector3.up, lookRotate.x);

        // Walking
        Vector3 move = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = transform.TransformDirection(move);
        controller.Move(PLAYER_SPEED * Time.deltaTime * move);

        // Third person camera movement
        scrolled -= Input.GetAxis("Mouse ScrollWheel");
        scrolled = Mathf.Clamp01(scrolled);
        cam.transform.localPosition = new Vector3(0, 0, -Mathf.SmoothStep(0, THIRD_PERSON_RANGE, scrolled));
    }
}
