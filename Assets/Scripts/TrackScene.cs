using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.LogFormat("TrackScene: Xin chao Scene Name {0}, Scene Index {1}", this.gameObject.scene.name, this.gameObject.scene.buildIndex);
        GVs.TRACE_SCENE.Push(this.gameObject.scene.buildIndex);        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
