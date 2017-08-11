using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScene : MonoBehaviour {
	// Use this for initialization
	void Start () {      
        GVs.TRACE_SCENE.Push(this.gameObject.scene.buildIndex);
        Utilities.Log("stack trace scene have {0} element", GVs.TRACE_SCENE.Count);
    }
}
