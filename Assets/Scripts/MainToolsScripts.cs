using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainToolsScripts : MonoBehaviour {

    public Button btn_cam;
    public Button btn_cam_active;
    public Button btn_history;
    public Button btn_history_active;

	// Use this for initialization
	void Start () {
        btn_cam.onClick.AddListener(()=>{
            //btn_cam.gameObject.SetActive(false);
            //btn_cam_active.gameObject.SetActive(true);
        });
	}

}
