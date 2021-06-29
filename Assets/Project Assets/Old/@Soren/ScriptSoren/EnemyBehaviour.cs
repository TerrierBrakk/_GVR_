using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    Camera mainCamera;
    public NavMeshAgent agent;

    void Start() {
        mainCamera = Camera.main;
    }

    void Update() {

        //tiempo para saber cuando cambiara de posicion si no lo vemos
        if (PlayerLook.timeNotLooking > 8) {

            PlayerLook.timeNotLooking = 0;
            spawn();
        }

        transform.LookAt(new Vector3(mainCamera.transform.position.x, transform.position.y, transform.position.z));
    }

    void spawn() {

        RaycastHit hit;
        float randomDistance = Random.Range(10, 20);
        float randomAngle = Random.Range(0, 360);

        Vector3 raySpawnPosition = mainCamera.transform.position
            + new Vector3(randomDistance * Mathf.Cos(randomAngle), 50, randomDistance * Mathf.Sin(randomAngle));

        Ray ray = new Ray(raySpawnPosition, Vector3.down);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            if (hit.collider != null)
                agent.Warp(hit.point);
        
    }

}
