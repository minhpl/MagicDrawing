using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Test2 : MonoBehaviour {

    public GameObject button;
    public GameObject nguiButton;

    void Start() {
        nguiButton.transform.localPosition = new Vector3(0, 0, 0);
        Debug.Log(nguiButton.transform.localPosition);
        Debug.Log(button.transform.position.ToString());
        Debug.Log(button.transform.localPosition.ToString());

        int id = LeanTween.moveLocalX(button, -367f, 1f).id;
         LTDescr d = LeanTween.descr(id).setFrom(12).setOnUpdate((float i)=> { }).setEase(LeanTweenType.easeInOutQuart).setLoopPingPong();
        LeanTween.alpha(button.GetComponent<RectTransform>(), 0.1f, 1f).setEase(LeanTweenType.easeInOutQuart).setLoopPingPong().setDelay(2f);

        Debug.Log("here");
        if (d != null)
        { 
          d.setEase(LeanTweenType.easeInOutQuart);
        }

        LeanTween.value(nguiButton, updateValueExampleCallback, 180f, 500f, 1f).setEase(LeanTweenType.easeInOutQuart).setLoopPingPong();
        
    }


    void updateValueExampleCallback(float val)
    {
        var pos = nguiButton.transform.position;
        nguiButton.transform.localPosition = new Vector3(pos.x - val, pos.y, pos.z);
        //Debug.Log(val);
    }

}
