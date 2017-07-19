using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onBackButtonClicked()
    {        
        if(GVs.TRACE_SCENE.Count > 1)
        {
            GVs.TRACE_SCENE.Pop();
            SceneManager.LoadScene(GVs.TRACE_SCENE.Pop());            
        }
        


        //if (GVs.PREV_SCENE.Count > 0)
        //{
        //    int temp = GVs.PREV_SCENE.Count - 1;
        //    int SceneIndex = GVs.PREV_SCENE[temp];
        //    Debug.LogFormat("Previous Scene index is {0}", SceneIndex);
        //    GVs.PREV_SCENE.RemoveAt(temp);
        //    SceneManager.LoadScene(SceneIndex);
        //}            
    }
}
