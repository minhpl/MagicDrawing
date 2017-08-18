using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBoxScript : MonoBehaviour
{

    public UILabel text;
    public GameObject bg;
    public GameObject[] items;
    void Start()
    {
        TweenAlpha.Begin(bg, 0, 0);
        for (int i = 0; i < items.Length; i++)
        {
            UIEventListener.Get(items[i]).onClick += SelectOnClick;
        }
        if (GVs.CURRENT_LEVEL > 0)
        {
            text.text = items[GVs.CURRENT_LEVEL - 1].GetComponent<UILabel>().text;
        }
    }

    public bool isPopupShow = false;

    public void ShowHideOnClick()
    {
        if (isPopupShow) CloseOnClick();
        else OpenOnCLick();
    }
    public void OpenOnCLick()
    {
        bg.SetActive(true);
        TweenAlpha.Begin(bg, 0.3f, 1);
        isPopupShow = true;
    }
    public void CloseOnClick()
    {
        TweenAlpha.Begin(bg, 0.3f, 0);
        isPopupShow = false;
        // DeActiveObject(bg, 0.3f);
    }
    public void SelectOnClick(GameObject sender)
    {
        text.text = sender.GetComponent<UILabel>().text;
        CloseOnClick();
    }

    IEnumerator DeActiveObject(GameObject go, float time)
    {
        if (!isPopupShow)
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
