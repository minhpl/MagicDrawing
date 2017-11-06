using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenDrawingScene : MonoBehaviour {

    public Button backBtn;
    public GameObject panel_tool;

    void Awake()
    {
        LeanTween.scale(backBtn.GetComponent<RectTransform>(), new Vector3(1f, 1, 1f), GVs.DURATION_TWEEN_UNIFY).setFrom(new Vector3(0f, 0f, 0f)).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();
        LeanTween.moveY(panel_tool.GetComponent<RectTransform>(), -7f, GVs.DURATION_TWEEN_UNIFY).setFrom(-252f).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.resumeAll();
    }

}
