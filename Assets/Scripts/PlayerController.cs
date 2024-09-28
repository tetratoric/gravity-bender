using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementMode { Normal, Planetry}

[RequireComponent(typeof(OrbitingObject))]
[RequireComponent(typeof(NewtonianObject))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public MovementMode movementMode;
    public bool useGravitationalPhysics = false;

    float camX, camY;

    public float movementForce = 4500f;
    public float maxSpeed = 20f;
    public float counterMovementMagnitude = 10f;
    public float jumpForce = 10f;
    public float jumpCooldown = 0.25f;
    public float groundingRaycastDistance = 1.55f;
    public LayerMask groundLayer;

    public float mouseSensitivity = 100f;

    private float movementMultipler = 10f;
    private float jumpMultiplier = 10f;

    Rigidbody rb;
    Camera playerCamera;
    NewtonianObject playerGravity;
    OrbitingObject playerOrbital;


    bool grounded, readyToJump = true;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        playerGravity = GetComponent<NewtonianObject>();
        playerOrbital = GetComponent<OrbitingObject>();

        playerCamera = GetComponentInChildren<Camera>();
    }
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerGravity.active = useGravitationalPhysics;
        rb.useGravity = !useGravitationalPhysics;
    }

    void Update()
    {
        Look();
        CheckIfGrounded();
        Debug.Log("Grounded: " + grounded);
    }

    void FixedUpdate() {
        HandleMovement();
        HandleCounterMovement();
        
        HandleJumping();
    }


    void CheckIfGrounded() {
        RaycastHit hit;

        Vector3 rayDirection;
        if (movementMode == MovementMode.Planetry) {
            rayDirection = -(transform.position - playerOrbital.primaryBody.transform.position).normalized;
        } else {
            rayDirection = Vector3.down;
        }

        if (Physics.Raycast(transform.position, rayDirection, out hit, groundingRaycastDistance, groundLayer)) {
            grounded = true;
        } else {
            grounded = false;
        }

        Debug.DrawRay(transform.position, rayDirection * groundingRaycastDistance, Color.red);
    }

    // void OnCollisionStay(Collision other) {
    //     if (other.gameObject.layer == LayerMask.NameToLayer("Environment")) {
    //         grounded = true;
    //     }
    // }

    // void OnCollisionExit(Collision other) {
    //     if (other.gameObject.layer == LayerMask.NameToLayer("Environment")) {
    //         grounded = false;
    //     }
    // }

    void HandleMovement() {
        float currentSpeed = rb.velocity.magnitude;
        if (currentSpeed > maxSpeed && movementMode == MovementMode.Normal) {
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

    void HandleCounterMovement() {
        if (!grounded) return;

        switch (movementMode) {
            case MovementMode.Normal:
                Vector3 normLocalVelocity = transform.InverseTransformDirection(rb.velocity);
                
                if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f) {
                    rb.AddRelativeForce(new Vector3(-normLocalVelocity.x * counterMovementMagnitude, 0, 0) * Time.deltaTime, ForceMode.VelocityChange);
                }
                if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f) {
                    rb.AddRelativeForce(new Vector3(0, 0, -normLocalVelocity.z * counterMovementMagnitude) * Time.deltaTime, ForceMode.VelocityChange);
                }
                
                
                break;
            
            case MovementMode.Planetry:
                Vector3 velocityReference = playerOrbital.primaryBody.GetComponent<Rigidbody>().velocity;
                Vector3 relativeVelocity = rb.velocity - velocityReference;
                Vector3 planetLocalVelocity = transform.InverseTransformDirection(relativeVelocity);

                if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f) {
                    rb.AddRelativeForce(new Vector3(-planetLocalVelocity.x * counterMovementMagnitude, 0, 0) * Time.deltaTime, ForceMode.VelocityChange);
                }
                if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f) {
                    rb.AddRelativeForce(new Vector3(0, 0, -planetLocalVelocity.z * counterMovementMagnitude) * Time.deltaTime, ForceMode.VelocityChange);
                }
                
                
                break;
        }
    }

    void HandleJumping() {
        if (grounded && readyToJump && Input.GetButtonDown("Jump")) {
            readyToJump = false;
            rb.AddForce(transform.up * jumpForce * jumpMultiplier, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump() {
        readyToJump = true;
    }

    void Look() {
        camX -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;
        camX = Mathf.Clamp(camX, -90f, 90f);
        camY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;

        playerCamera.transform.localRotation = Quaternion.Euler(camX, 0, 0);

        transform.Rotate(0, camY, 0);
    }
}
