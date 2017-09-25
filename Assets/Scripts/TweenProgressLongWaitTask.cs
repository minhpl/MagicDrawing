using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenProgressLongWaitTask : MonoBehaviour {
    LTDescr ltdesc;
    private void OnEnable()
    {     
        ltdesc =  LeanTween.rotateAround(this.gameObject,Vector3.forward, 360, 1).setRepeat(-1);
    }

    private void OnDisable()
    {        
        if (ltdesc != null)
            LeanTween.cancel(ltdesc.id);
        this.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);        
    }
}
