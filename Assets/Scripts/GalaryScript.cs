using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaryScript : MonoBehaviour {

    public GameObject imageItem;
    public UIScrollView uiScrollView;
    public int imageCount = 20;
	// Use this for initialization
	void Start () {
        Vector3  v3 = imageItem.transform.localPosition;
        v3.y += 340;
        float x = v3.x;
        for (int i = 0; i < imageCount; i++)
        {
            if (i % 3 == 0)
            {
                v3.y -= 340;
                v3.x = x;
            }
            if (i % 3 == 1) v3.x = 0;
            if (i % 3 == 2) v3.x = -x;

            GameObject go = Instantiate(imageItem) as GameObject;
            go.transform.parent = imageItem.transform.parent.transform;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = v3;
            go.GetComponent<UIButton>().tweenTarget = null;
            go.SetActive(true);
            uiScrollView.Scroll(20);
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
