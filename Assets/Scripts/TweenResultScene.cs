using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenResultScene : MonoBehaviour {

    public Button btnBack;
    public Button btnHome;
    public GameObject rawImageTitle;
    public Text title;
    public GameObject pnlTools;
    public GameObject btnDelete;

	void Awake () {
        LeanTween.scale(btnBack.GetComponent<RectTransform>(), new Vector3(1f, 1, 1f), GVs.DURATION_TWEEN_UNIFY).setFrom(new Vector3(0f, 0f, 0f)).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.moveY(btnHome.GetComponent<RectTransform>(), 132f, GVs.DURATION_TWEEN_UNIFY).setFrom(-62f).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.moveY(btnDelete.GetComponent<RectTransform>(),-49,GVs.DURATION_TWEEN_UNIFY).setFrom(130).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.moveY(title.GetComponent<RectTransform>(),749,GVs.DURATION_TWEEN_UNIFY).setFrom(1002).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.moveY(rawImageTitle.GetComponent<RectTransform>(), -153, GVs.DURATION_TWEEN_UNIFY).setFrom(107).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.moveY(pnlTools.GetComponent<RectTransform>(), 0, GVs.DURATION_TWEEN_UNIFY).setFrom(-421).pause()
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.resumeAll();
    }
	
}
