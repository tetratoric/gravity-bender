using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NewtonianObject : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    float G;

    public bool attractObjects = false;
    [HideInInspector]
    public bool active = true;

    public static List<NewtonianObject> newtonianObjects;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        if (attractObjects) {
            gameObject.tag = "Attractor";
        }
    }

    void OnEnable() {
        if (newtonianObjects == null) {
            newtonianObjects = new List<NewtonianObject>();
        }
        newtonianObjects.Add(this);
    }

    void Start() {
        G = GameManager.G;
    }

    void OnDisable() {
        newtonianObjects.Remove(this);
    }

    void FixedUpdate() {
        if (active) AttractToObjects();
    }

    void AttractToObjects() {
        foreach (var attractor in newtonianObjects) {
            
            Vector3 displacement = attractor.transform.position - transform.position;
            
            if (attractor != this && displacement != Vector3.zero && attractor.tag == "Attractor") {
                
                Vector3 direction = displacement.normalized;
                float distance = displacement.magnitude;

                float forceMagnitude = G * (rb.mass * attractor.rb.mass) / distance;
                Vector3 forceVector = forceMagnitude * direction;

                rb.AddForce(forceVector);
            }
        }
    }
}
