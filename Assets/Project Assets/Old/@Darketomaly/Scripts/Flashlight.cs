using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {

    public Transform pivot;
    public Transform cam;

    void Update() {

        transform.position = pivot.position;

        transform.localRotation =
            Quaternion.Lerp(transform.localRotation, cam.localRotation, 2.5f * Time.deltaTime);
    }
}
