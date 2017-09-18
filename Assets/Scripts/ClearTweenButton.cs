using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTweenButton : MonoBehaviour
{
    TweenScale tweenScale;
    void Start()
    {
        try
        {
            GetComponent<UIPlayTween>().Play(true);
            tweenScale = GetComponent<TweenScale>();
            tweenScale.SetOnFinished(FinishTween);
        }
        catch (System.Exception)
        {
        }
    }

    public void FinishTween()
    {
        if (GetComponent<TweenScale>() != null)
            Destroy(GetComponent<TweenScale>());
        UIButtonScale uiButtonScale = gameObject.AddComponent<UIButtonScale>();
        uiButtonScale.duration = 0.1f;
        uiButtonScale.hover = new Vector3(0.95f, 0.95f, 0.95f);
        uiButtonScale.pressed = new Vector3(0.95f, 0.95f, 0.95f);
    }
}
