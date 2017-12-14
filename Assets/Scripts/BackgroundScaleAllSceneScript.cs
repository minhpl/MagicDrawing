using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaleAllSceneScript : MonoBehaviour
{
    void Awake()
    {
        float ratio = ((Screen.height * 1.0f) / (Screen.width * 1.0f)) * (1080.0f / 1920f) * 2.0f;
        Vector3 scale = new Vector3(ratio, ratio, ratio);
        transform.localScale = scale;
        TweenPosition tw = GetComponent<TweenPosition>();
        tw.from.x = -2029 * (ratio / 2.0f);
        tw.to.x = 2029 * (ratio / 2.0f);
    }
}
