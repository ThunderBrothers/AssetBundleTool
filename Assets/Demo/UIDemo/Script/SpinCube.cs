using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCube : MonoBehaviour {

    private bool doSpin = false;
	// Update is called once per frame
	void Update () {
        if(doSpin) transform.Rotate(new Vector3(0f,1f,0f),Time.deltaTime*30f,Space.World);
	}

    public void SetSpinStatus(bool status)
    {
        doSpin = status;
    }
}
