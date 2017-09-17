using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TestCoroutine : MonoBehaviour {

    // Use this for initialization
    void Start() {
        var cancel = Observable.FromCoroutine(AsyncA)
         .SelectMany(AsyncB)
         .Subscribe((_) => { }, () => { Debug.Log("heheheh"); });
    }

    IEnumerator AsyncA()
    {
        Debug.Log("a start");
        yield return new WaitForSeconds(1);
        Debug.Log("a end");
    }

    IEnumerator AsyncB()
    {
        Debug.Log("b start");
        yield return new WaitForEndOfFrame();
        Debug.Log("b end");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
