using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineScaleCotraller : Iplugin {

    private bool state = false;

    void Start() {
        

    }
    public override void GazeClick() {
        Debug.Log("GazeClick");
        state = !state;
        if (state)
        {
            transform.localScale = new Vector3(1.5f,1.5f,1.5f);
        }else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void GazeOff() {
        Debug.Log("GazeOff");
    }

    public override void GazeOn() {
        Debug.Log("GazeOn");
    }

    public override void OnReceiveMsg(string msg) {
     
    }

    public override bool supportPRS() {
        return false;
    }
}
