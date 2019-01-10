using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSwitch : Iplugin {
    private bool flag = false;
    private ParticleSystem particle;

    void Start()
    {
        particle = transform.GetComponent<ParticleSystem>();

    }
    public override void GazeClick()
    {
        Debug.Log("GazeClick");
        flag = !flag;
        if(flag) particle.Play();
        else  particle.Pause();
        SendMsg(flag.ToString());
    }

    public override void GazeOff()
    {
        Debug.Log("GazeOff");
    }

    public override void GazeOn()
    {
        Debug.Log("GazeOn");
    }

    public override void OnReceiveMsg(string msg)
    {
        flag = bool.Parse(msg);
        if (flag) particle.Play();
        else particle.Pause();
    }

    public override bool supportPRS()
    {
        return false;
    }
    
	
	// Update is called once per frame
	void Update () {
		
	}



}
