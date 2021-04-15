using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAbehaviour : MonoBehaviour
{

    public Transform playerLoc;
    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(playerLoc);

    }

    // Update is called once per frame
    void Update()   
    {
        
    }
}
