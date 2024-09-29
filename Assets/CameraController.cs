using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraPivot;

    public void SyncAttributes() {
        transform.position = cameraPivot.position;
        transform.rotation = cameraPivot.rotation;
    }
}
