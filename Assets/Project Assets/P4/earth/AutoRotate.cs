using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {

    public void Update() {

        transform.Rotate(Vector3.up * Time.deltaTime * 3.0f, Space.Self);
    }
}
