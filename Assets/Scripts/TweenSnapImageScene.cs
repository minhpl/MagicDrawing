using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenSnapImageScene : MonoBehaviour {

    public Button BackButton;
    public GameObject panel_tool;

	void Awake () {
        LeanTween.scale(BackButton.GetComponent<RectTransform>(), new Vector3(1f, 1, 1f), GVs.DURATION_TWEEN_UNIFY).setFrom(new Vector3(0f, 0f, 0f)).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();
        LeanTween.moveY(panel_tool.GetComponent<RectTransform>(), 0f, GVs.DURATION_TWEEN_UNIFY).setFrom(-354f).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad() ;

        LeanTween.resumeAll();
    }

}
