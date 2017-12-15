using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSatanAnim : MonoBehaviour {

    UISprite uiSprite;

    void Start () {
        uiSprite = GetComponent<UISprite>();
        StartCoroutine(Play());
    }

    int i = 0;
    IEnumerator Play()
    {
        yield return new WaitForSeconds(0.04f);        
        i %= 26;
        i++;
        uiSprite.spriteName = "satan_THUMB_" + (i);
        StartCoroutine(Play());
    }
}
