using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BundleTest : MonoBehaviour, IPointerClickHandler {

    public BundleActionHandler sss;
    public GameObject obj;
    public void OnPointerClick(PointerEventData eventData) {
        sss.OnBundleAction(eventData);
    }

    // Use this for initialization
    void Start () {
        //sss = obj.GetComponent<BundleActionHandler>();


        Type type = System.Type.GetType("SetActive_False");
        gameObject.AddComponent(type);
    }
    [ContextMenu("go")]
    void go() {
        Type type = System.Type.GetType("SetActive_False");
        gameObject.AddComponent(type);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
