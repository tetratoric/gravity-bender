using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NewtonianObject))]
public class OrbitingObject : MonoBehaviour
{
    [HideInInspector]
    public GameObject primaryBody;
    public float rotationDamp = 0.3f;
    
    private void FixedUpdate() {
        if (primaryBody != null) {
            Rotate();
        }
    }

    void Rotate() {
        Vector3 targetDirection = (transform.position - primaryBody.transform.position).normalized;
        Quaternion target = Quaternion.FromToRotation(transform.up, targetDirection) * transform.rotation;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, rotationDamp/10);
    }
}
