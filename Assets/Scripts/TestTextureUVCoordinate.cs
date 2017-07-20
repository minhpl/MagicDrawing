using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTextureUVCoordinate : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Image a = GetComponent<Image>();
        var name = a.gameObject.name;
        var uv = a.sprite.uv;
        Debug.LogFormat("texture name is {0} have length uv is {1}", name, a.sprite.rect.ToString());
        for (int i = 0; i < uv.Length; i++)
            Debug.LogFormat(uv[i].ToString());
    }
	// Update is called once per frame
	void Update () {
		
	}

}
