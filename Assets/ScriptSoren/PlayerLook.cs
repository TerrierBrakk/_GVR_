using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform slender;

    public bool isLooking;
    public static float timeNotLooking;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
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
