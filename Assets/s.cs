using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s : MonoBehaviour {

	public Camera camera;
	// Use this for initialization
	void Start () {
		float width = (camera.ScreenToViewportPoint (new Vector3(Screen.width, 0, 0)).x - camera.ScreenToViewportPoint(new Vector3(0, 0, 0)).x);
		float height = width * 16.0f / 9.0f;
		Debug.Log (width + " - " + height);
//		transform.localScale = Vector3.Scale(transform.localScale, new Vector3( Screen.width /1080.0f, (Screen.width*16.0f/9.0f)/1920.0f,0.0f));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
