using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour {

    public string countryName;

    public Transform planet;
    public float speed;

    public Vector3 rotation;


    private Color rayColor;

    void Update() {

        transform.RotateAround(planet.position, rotation, speed * Time.deltaTime);

        RaycastHit hit;

        Vector3 dir = transform.position - Camera.main.transform.position;

        Debug.DrawRay(Camera.main.transform.position, dir, rayColor);

        if (Physics.Raycast(Camera.main.transform.position, dir, out hit)) {

            if(hit.transform == transform) {

                rayColor = Color.green;

            } else {

                rayColor = Color.red;

            }
        }
    }
}
