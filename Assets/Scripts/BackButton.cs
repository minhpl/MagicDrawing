using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {

    private void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            onBackButtonClicked();
        });
    }

    public void onBackButtonClicked()
    {    
        Utilities.Log("BackbuttonScripts: stack trace scene have {0} element", GVs.TRACE_SCENE.Count);
        if (GVs.TRACE_SCENE.Count > 1)
        {
            GVs.TRACE_SCENE.Pop();
            int i = GVs.TRACE_SCENE.Pop();
            Utilities.Log("index scene back is {0}", i);
            SceneManager.LoadScene(i);
        }                 
    }
}
