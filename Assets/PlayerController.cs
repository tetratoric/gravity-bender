using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewtonianObject))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public bool useNewtonianPhysics = false;

    float camX, camY;

    public float speed = 10f;
    public float mouseSensitivity = 100f;

    Rigidbody rb;
    Camera playerCamera;
    NewtonianObject playerGravity;
    
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        playerGravity = GetComponent<NewtonianObject>();
    }
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerGravity.active = useNewtonianPhysics;
        rb.useGravity = !useNewtonianPhysics;
    }

    void Update()
    {
        Look();
    }

    void FixedUpdate() {
        Movement();
    }

    void Movement() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(x, 0.0f, y).normalized;
        
        rb.AddRelativeForce(movement * speed * Time.deltaTime);
    }

    void Look() {
        camX -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;
        camX = Mathf.Clamp(camX, -90f, 90f);
        camY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;

        playerCamera.transform.localRotation = Quaternion.Euler(camX, 0, 0);

        transform.Rotate(0, camY, 0);
    }
}
