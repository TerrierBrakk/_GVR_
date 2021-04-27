using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewCloser : MonoBehaviour {

    public Button closeButton;

    void Update() {

        if(Input.GetKeyDown(KeyCode.Escape)) 
            closeButton.onClick.Invoke();
    }
}
