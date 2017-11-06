using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : MonoBehaviour {
	public GameObject spinObject1;
	public GameObject spinObject2;
	int check = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClickSkip (){
		if (check == 0) {
			spinObject1.SetActive (false);
			spinObject2.SetActive (true);
			check = 1;
		} else if (check == 1) {
			GVs.SCENE_MANAGER.StartPreloadScene ();
		}
	}
}
