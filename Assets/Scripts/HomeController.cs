using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeController : MonoBehaviour {

    public GameObject go;
	// Use this for initialization
	void Start () {
        go.tag = "id";
        UIEventListener.Get(go).onClick += LoadOtherScreen2;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadOtherScreen()
    {
        //
    }
    public void LoadOtherScreen2(GameObject sender)
    {
        //
    }
}
