using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

    public void onBackButtonClicked()
    {
        if(GVs.TRACE_SCENE.Count > 1)
        {
            GVs.TRACE_SCENE.Pop();
            int i = GVs.TRACE_SCENE.Pop();
            Debug.LogFormat("index scene back is {0}", i);
            SceneManager.LoadScene(i);
        }                 
    }
}
