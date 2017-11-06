using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoHomeSceneScripts : MonoBehaviour {
    public void OnHomeBtnClicked()
    {        
        GVs.TRACE_SCENE.Clear();
        GVs.SCENE_MANAGER.loadHomeScene();
    }
}
