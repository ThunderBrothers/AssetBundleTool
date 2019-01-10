using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//
//
// Require these components when using this script
[RequireComponent(typeof(Animator))]

public class IdleChanger :  Iplugin
{

	private Animator anim;						// Animatorへの参照
	private AnimatorStateInfo currentState;		// 現在のステート状態を保存する参照
	private AnimatorStateInfo previousState;	// ひとつ前のステート状態を保存する参照
    //private IpluginImpl plugin;

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();
		currentState = anim.GetCurrentAnimatorStateInfo (0);
		previousState = currentState;
        Debug.Log("start in IdleChanger");
        Debug.LogError("start in IdleChanger");
    }

    bool run = false;
    int action = 0;
    public void OnMsg(string msg)
    {
        action = int.Parse(msg);
        action--;
        PlayNextAction();
    }

    public void PlayNextAction()
    {
        if (run)
        {
            run = false;
            anim.SetBool("Run", false);
            return;
        }
        //int action = Random.Range(0, 9);
        action++;
        if ((action % 8) == 1)
            anim.SetBool("Jab", true);
        else if ((action % 8) == 2)
            anim.SetBool("Hikick", true);
        else if ((action % 8) == 3)
            anim.SetBool("Spinkick", true);
        else if ((action % 8) == 4)
            anim.SetBool("Rising_P", true);
        else if ((action % 8) == 5)
            anim.SetBool("Headspring", true);
        else if ((action % 8) == 6)
            anim.SetBool("Land", true);
        else if ((action % 8) == 7)
            anim.SetBool("SAMK", true);
        else if ((action % 8) == 0)
        {
            run = true;
            anim.SetBool("Run", run);
        }
    }

    public override void OnReceiveMsg(string msg)
    {
        throw new System.NotImplementedException();
    }

    public override void GazeOn()
    {
    }

    public override void GazeClick()
    {
        Debug.Log("OnPointerClick");
        PlayNextAction(); 
    }

    public override void GazeOff()
    {
    }

    public override bool supportPRS()
    {
        return false;
    }
}
