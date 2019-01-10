using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : Iplugin
{
    private SpinCube spinCube;
    private bool doSpin = false;
    

    void Start()
    {
        Debug.Log("start IpluginImpl");
        spinCube = transform.parent.GetComponentInChildren<SpinCube>();
    }

    public override void GazeClick()
    {
        Debug.Log("OnMouseClick");
        if (spinCube == null) return;
        doSpin = !doSpin;
        spinCube.SetSpinStatus(doSpin);
        SendMsg(doSpin.ToString());
    }

    public override void GazeOn()
    {
        Debug.Log("OnMouseEnter");
    }

    public override void GazeOff()
    {
        Debug.Log("OnMouseExit");
    }

    public override void OnReceiveMsg(string msg)
    {
        bool spinFlag = bool.Parse(msg);
        doSpin = spinFlag;
        spinCube.SetSpinStatus(doSpin);
    }

    public override bool supportPRS()
    {
        return false;
    }
}
