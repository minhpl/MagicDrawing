using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackHomeScripts : MonoBehaviour {	
	void Start () {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            GVs.TRACE_SCENE.Clear();
            GVs.SCENE_MANAGER.loadHomeScene();
        });
	}	
}
