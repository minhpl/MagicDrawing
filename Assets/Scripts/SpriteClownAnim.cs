using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteClownAnim : MonoBehaviour {

    UISprite uiSprite;
    void Start () {
        uiSprite = GetComponent<UISprite>();
        StartCoroutine(Play());
    }

    int i = 0;
    IEnumerator Play()
    {
        yield return new WaitForSeconds(0.04f);
        i %= 34;
        i++;
        uiSprite.spriteName = "clown_THUMB_" + (i);
        StartCoroutine(Play());
    }
}
