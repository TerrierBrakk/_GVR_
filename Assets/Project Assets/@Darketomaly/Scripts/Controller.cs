using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform child;

    void Update() {

        //probably lame code optimization but ¯\_(ツ)_/¯

        if (Input.GetAxisRaw("Horizontal") != 0 ||
            Input.GetAxisRaw("Vertical") != 0) 

            agent.Move(child.rotation * new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f,Input.GetAxisRaw("Vertical")) * 3.50f * Time.deltaTime);
        
        /*
        if(Input.GetKey(KeyCode.W))
            agent.Move(child.rotation * Vector3.forward * 0.025f);
        if(Input.GetKey(KeyCode.S))
            agent.Move(child.rotation * Vector3.back * 0.025f);
        if(Input.GetKey(KeyCode.D))
            agent.Move(child.rotation * Vector3.right * 0.025f);
        if (Input.GetKey(KeyCode.A))
            agent.Move(child.rotation * Vector3.left * 0.025f);
        */
    }
}
