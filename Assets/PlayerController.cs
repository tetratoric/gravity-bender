using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewtonianObject))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public bool useNewtonianPhysics = false;

    float camX, camY;

    public float movementForce = 10f;
    public float maxSpeed = 20f;
    public float jumpForce = 10f;
    public float mouseSensitivity = 100f;

    private float movementMultipler = 10f;
    private float jumpMultiplier = 10f;

    Rigidbody rb;
    Camera playerCamera;
    NewtonianObject playerGravity;
    
    bool grounded;

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
        HandleMovement();
        HandleJumping();
    }

    void OnCollisionStay(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment")) {
            grounded = true;
        }
    }

    void OnCollisionExit(Collision other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment")) {
            grounded = false;
        }
    }

    void HandleMovement() {
        float currentSpeed = rb.velocity.magnitude;
        if (currentSpeed > maxSpeed) {
            Debug.Log("Too fast boiii");
            return;
        }
        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 movementDir = new Vector3(x, 0.0f, y).normalized;
        
        float airMult = 1;
        if (!grounded) {
            airMult = 0.5f;
        }

        rb.AddRelativeForce(movementDir * movementForce * movementMultipler * airMult * Time.deltaTime);
    }

    void HandleJumping() {
        if (grounded && Input.GetButtonDown("Jump")) {
            rb.AddForce(transform.up * jumpForce * jumpMultiplier, ForceMode.Impulse);
        }
    }

    void Look() {
        camX -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;
        camX = Mathf.Clamp(camX, -90f, 90f);
        camY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;

        playerCamera.transform.localRotation = Quaternion.Euler(camX, 0, 0);

        transform.Rotate(0, camY, 0);
    }
}
