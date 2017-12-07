using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoHomeSceneScripts : MonoBehaviour {
    public void OnHomeBtnClicked()
    {
        Debug.Log("Fuck all why deo vao day");

        GVs.TRACE_SCENE.Clear();
        GVs.SCENE_MANAGER.loadHomeScene();
    }
}
