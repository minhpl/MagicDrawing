using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayTweenScript : MonoBehaviour
{
    void Awake()
    {
        TweenAlpha ta = GetComponent<TweenAlpha>();
        if (ta != null)
        {
            ta.delay += GVs.DELAY_TWEEN;
        }
        TweenScale ts = GetComponent<TweenScale>();
        if (ts != null)
        {
            ts.delay += GVs.DELAY_TWEEN;
        }
        TweenPosition tp = GetComponent<TweenPosition>();
        if (tp != null)
        {
            tp.delay += GVs.DELAY_TWEEN;
        }
    }

}
