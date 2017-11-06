﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenMasterpieceCreationNGUIScene : MonoBehaviour {

    public Button BackButton;
    public Text textTile;
    private float duration = GVs.DURATION_TWEEN_UNIFY;
    private float delay = GVs.DELAY_TWEEN_UNIFY;

    void Awake () {
        List<LTDescr> listLT = new List<LTDescr>();
        listLT.Add(LeanTween.scale(BackButton.GetComponent<RectTransform>(), new Vector3(1f, 1, 1f), duration).setFrom(new Vector3(0f, 0f, 0f)).pause().setOnStart(() =>
        {
            BackButton.gameObject.SetActive(false);
        }));
        listLT.Add(LeanTween.moveY(textTile.GetComponent<RectTransform>(), -102.7f, duration).setFrom(45f).pause());

        foreach (var lt in listLT)
        {
            lt.setEase(LeanTweenType.easeInOutQuart).setDelay(delay).resume();
        }
    }
	
}
