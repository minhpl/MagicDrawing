using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVideo : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Handheld.PlayFullScreenMovie("E:\\WorkspaceMinh\\MagicDrawing\\x64\\Release\\20170812_202917.avi", Color.black, FullScreenMovieControlMode.CancelOnInput);
        Debug.Log("herer");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
