using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenDecorateScene : MonoBehaviour {

    public Button btnBack;
    public Button btnHome;

    void Awake()
    {
        LeanTween.scale(btnBack.GetComponent<RectTransform>(), new Vector3(1f, 1, 1f), GVs.DURATION_TWEEN_UNIFY).setFrom(new Vector3(0f, 0f, 0f)).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.moveY(btnHome.GetComponent<RectTransform>(), 132f, GVs.DURATION_TWEEN_UNIFY).setFrom(-62f).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.resumeAll();
    }
	
}
