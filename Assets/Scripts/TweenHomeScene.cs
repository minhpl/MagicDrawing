using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenHomeScene : MonoBehaviour
{

    public AnimationCurve curve;
    public GameObject topBackground;
    public GameObject bottomBackgound;
    public GameObject single;
    public GameObject multi;
    public GameObject nav;
    // Use this for initialization
    void Awake()
    {
        single.transform.localScale = Vector3.zero;
        multi.transform.localScale = Vector3.zero;
 

        LeanTween.scale(topBackground, new Vector3(1f, 1, 1f), GVs.DURATION_TWEEN_UNIFY).setFrom(new Vector3(0f, 1f, 0f))
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();

        LeanTween.scale(bottomBackgound, new Vector3(1, 1, 1), GVs.DURATION_TWEEN_UNIFY)
            .setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad().setOnComplete(() =>
            {
                LeanTween.scale(single, new Vector3(1, 1, 1), 0.5f).setEaseInOutQuad().setOnComplete(() =>
                {
                    UIButtonScale uiButtonScale = single.AddComponent<UIButtonScale>();
                    uiButtonScale.duration = 0.1f;
                    uiButtonScale.hover = new Vector3(0.95f, 0.95f, 0.95f);
                    uiButtonScale.pressed = new Vector3(0.95f, 0.95f, 0.95f);
                });
                LeanTween.scale(multi, new Vector3(1, 1, 1), 0.5f).setEaseInOutQuad().setDelay(0.1f).setOnComplete(() =>
                {
                    UIButtonScale uiButtonScale = multi.AddComponent<UIButtonScale>();
                    uiButtonScale.duration = 0.1f;
                    uiButtonScale.hover = new Vector3(0.95f, 0.95f, 0.95f);
                    uiButtonScale.pressed = new Vector3(0.95f, 0.95f, 0.95f);
                });

                //LeanTween.scale(specialMode, new Vector3(1, 1, 1), 0.5f).setEaseInOutQuad().setDelay(0.1f).setOnComplete(() =>
                //{
                //    UIButtonScale uiButtonScale = specialMode.AddComponent<UIButtonScale>();
                //    uiButtonScale.duration = 0.1f;
                //    uiButtonScale.hover = new Vector3(0.95f, 0.95f, 0.95f);
                //    uiButtonScale.pressed = new Vector3(0.95f, 0.95f, 0.95f);
                //});
            });
        LeanTween.moveLocalX(nav, 0, GVs.DURATION_TWEEN_UNIFY).setDelay(GVs.DELAY_TWEEN_UNIFY).setEaseInOutQuad();
    }
}
