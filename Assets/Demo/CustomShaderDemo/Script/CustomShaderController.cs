using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShaderController : Iplugin {

    private Material material;
    private int colorCode = 1;

    private Color GetColorByCode(int code)
    {
        if (code == 1) return Color.red;
        else if (code == 2) return Color.yellow ;
        else  return Color.green ;
    }
    public override void GazeClick()
    {
        colorCode = (colorCode+1)%3;
        Color c = GetColorByCode(colorCode);
        material.SetColor("_BackSurfaceColor", c*0.1f);
        material.SetColor("_BackRimColor", c);
        material.SetColor("_FrontSurfaceColor", c*0.3f);
        material.SetColor("_FrontRimColor", c);
        SendMsg(colorCode.ToString());
    }

    public override void GazeOff()
    {

    }

    public override void GazeOn()
    {

    }

    public override void OnReceiveMsg(string msg)
    {
        colorCode = int.Parse(msg);
        Color c = GetColorByCode(colorCode);
        material.SetColor("_BackSurfaceColor", c * 0.1f);
        material.SetColor("_BackRimColor", c);
        material.SetColor("_FrontSurfaceColor", c * 0.3f);
        material.SetColor("_FrontRimColor", c);
    }

    public override bool supportPRS()
    {
        return true;
    }

    // Use this for initialization
    void Start () {
        material = transform.GetComponent<MeshRenderer>().material;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
