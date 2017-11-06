using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTweenAvata : MonoBehaviour
{
    public UIPlayTween uiPlayTween;
    void Start()
    {
        uiPlayTween = GetComponent<UIPlayTween>();
    }
    public void Play()
    {
        PlayTween(() =>
        {

        });
    }

    public bool loop = false;

    public void NextTween()
    {
        uiPlayTween.tweenGroup++;
        if (uiPlayTween.tweenGroup > 1)
        {
            uiPlayTween.tweenGroup = 0;
            if (!loop) return;
        }
        uiPlayTween.Play(true);
        uiPlayTween.resetOnPlay = true;
    }

    public void PlayTween(Handler handler, bool loop = false)
    {
        this.loop = loop;
        uiPlayTween.tweenGroup = -1;
        this.handler = handler;
        NextTween();
    }

    public delegate void Handler();
    private Handler handler = null;
    public void SetHandler(Handler handler)
    {
        this.handler = handler;
    }

    public void SetDelay(float delay)
    {
        TweenScale[] tss = GetComponents<TweenScale>();
        tss[0].delay = delay;
        TweenRotation[] trs = GetComponents<TweenRotation>();
        trs[0].delay = delay;
    }
    public void FinishTween()
    {
        if (handler != null) handler();
        if (loop) NextTween();
        Reset();
    }

    public void SetPositionEnd(Vector3 ve)
    {
        TweenPosition[] tps = GetComponents<TweenPosition>();
        tps[1].to = ve;
    }

    public void Reset()
    {
        try
        {
            if (uiPlayTween != null)
            uiPlayTween.tweenGroup = -1;
            TweenScale[] tss = GetComponents<TweenScale>();
            tss[1].enabled = false;
            transform.localScale = Vector3.zero;
        }
        catch (System.Exception)
        {
        }
    }
}
