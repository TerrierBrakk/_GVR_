using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFn : MonoBehaviour {

    public static EventFn instance;
    bool move = false;

    void Awake() {

        instance = this;
    }

    public static void LoadFunctions(string _function, string[] _data) {

        switch (_function) {

            case "Sumar": {

                    Debug.Log($"Sumar :: {int.Parse(_data[0]) + int.Parse(_data[1])}");
                }
                break;

            case "Click": {

                    Debug.Log("Click");
                }
                break;
            default:
                break;
        }
    }
}
