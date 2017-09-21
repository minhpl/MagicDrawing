using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButtonGoLibraryScene : MonoBehaviour {
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GVs.SCENE_MANAGER.loadLibraryNGUIScene();            
        });
    }
}
