using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainToolsScripts : MonoBehaviour {

    public Button btnLibrary;
    public Button btnCam;
    public Button btnGallary;
    public Button btnHistory;
   

	// Use this for initialization
	void Start () {
        btnLibrary.onClick.AddListener(() =>
        {
            GVs.SCENE_MANAGER.loadLibraryScene();
        });

        btnCam.onClick.AddListener(()=>{
            GVs.SCENE_MANAGER.loadSnapImageScene();
        });

        btnGallary.onClick.AddListener(() =>
        {
            GVs.SCENE_MANAGER.loadGalleryScene();
        });

        btnHistory.onClick.AddListener(() =>
        {
            GVs.SCENE_MANAGER.loadHistoryScene();
        });
	}
}
