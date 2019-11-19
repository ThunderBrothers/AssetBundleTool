using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class BundleTest : MonoBehaviour {


    public GameObject obj;
    public BoxCollider Collider;


    // Use this for initialization
    void Start() {
        Collider = obj.GetComponent<BoxCollider>();
    }

}
