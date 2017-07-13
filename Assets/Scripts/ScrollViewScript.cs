using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewScript : MonoBehaviour {
    public GameObject imageItem;
    public int imageCount = 9;

    // Use this for initialization
    void Start()
    {
        Vector3 v3 = imageItem.transform.localPosition;
        float x = v3.x;
        //Debug.Log(v3.y);
        for (int i = 0; i < imageCount; i++)
        {
            GameObject go = Instantiate(imageItem) as GameObject;            
            go.transform.parent = imageItem.transform.parent.transform;
            go.transform.localScale = imageItem.transform.localScale;           
            go.SetActive(true);            
            go.transform.localPosition = v3;

        }




    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
