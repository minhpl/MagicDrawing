using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallComponentOfOtherObject : MonoBehaviour {

    public GameObject go;
	void Start () {
        var btn = GetComponent<UIButton>();
        btn.onClick.Add(new EventDelegate(() =>
        {
            Debug.Log("xin chao the gioi");
            var audioSource = go.GetComponent<AudioSource>();
            Debug.Log(audioSource == null);
            audioSource.Play();
        }));        
	}
	
}
