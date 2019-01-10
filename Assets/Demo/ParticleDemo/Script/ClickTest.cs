using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickTest : MonoBehaviour,IPointerClickHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPointerClick (PointerEventData eventData){
		transform.position += new Vector3 (0.1f, 0f, 0f);
	}

}
