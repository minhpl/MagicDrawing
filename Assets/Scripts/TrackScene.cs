using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GVs.TRACE_SCENE.Push(this.gameObject.scene.buildIndex);
        Debug.LogFormat("Xin chao, day la Scene {0}", this.gameObject.scene.name);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
