using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLook : MonoBehaviour
{
    public Transform slender = null;
    public bool isLooking;
    public static float timeNotLooking;


    void Start() {

        SceneManager.sceneLoaded += OnSceneChange;
    }

    public void OnSceneChange(Scene scene, LoadSceneMode mode) {

        EnemyBehaviour tmp = FindObjectOfType<EnemyBehaviour>();
        if(tmp)
            slender = tmp.transform;
    }

    void Update()
    {
        if (!slender) return;

        float distance = Vector3.Distance(slender.position, transform.position);
        float angle = Vector3.Angle(transform.forward, slender.position - transform.position);

        if (angle < 60 && distance < 15)
        {
            timeNotLooking = 0;
        }
        else
        {
            isLooking = false;
            timeNotLooking += Time.deltaTime;
        }

    }

}
