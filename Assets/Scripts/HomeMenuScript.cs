using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HomeMenuScript : MonoBehaviour
{


    public GameObject goNavPanel;
    public GameObject goBG;


    public GameObject goHomeMenu;
    public GameObject goUser;
    public GameObject goUserInfo;
    public GameObject goArchievement;
    public GameObject goCodePanel;
    // Use this for initialization

    public UIPlayTween[] panelNavPlayTweens;
    void Awake()
    {
        goNavPanel.transform.Find("container").GetComponent<UITexture>().height = (int)(1080*(Screen.height* 1.0f) / Screen.width);
        // TweenPosition.Begin(goNavPanel, 0, new Vector3(-600, 0, 0));
        // TweenAlpha.Begin(goBG, 0, 0);
        goNavPanel.transform.Find("container").transform.Find("svApp").gameObject.SetActive(true);
        goNavPanel.GetComponent<UIPanel>().depth = 3;
        UIEventListener.Get(goBG).onClick += CloseMenuOnClick;
    }

    void Start()
    {

    }

    private bool menuIsShow = false;
    public void MenuOnClick()
    {
        if (menuIsShow)
        {

            GFs.PlayTweens(panelNavPlayTweens, false, () =>
            {
            });
            // TweenScale.Begin(gameObject, 0.2f, new Vector3(1f, 1f, 1));
            // TweenPosition.Begin(gameObject, 0.2f, Vector3.zero);
            // TweenPosition.Begin(goNavPanel, 0.2f, new Vector3(-600, 0, 0));
            // TweenAlpha.Begin(goBG, 0.2f, 0);
        }
        else
        {

            GFs.PlayTweens(panelNavPlayTweens);
            // TweenScale.Begin(gameObject, 0.2f, new Vector3(0.9f, 0.9f, 1));
            // TweenPosition.Begin(gameObject, 0.2f, new Vector3(400, 0, 0));
            // TweenPosition.Begin(goNavPanel, 0.2f, Vector3.zero);
            // TweenAlpha.Begin(goBG, 0.2f, 0.5f);
        }
        menuIsShow = !menuIsShow;
    }

    public void CloseMenuOnClick()
    {
        if (menuIsShow)
        {
            GFs.PlayTweens(panelNavPlayTweens, false, () =>
            {
            });
            // TweenScale.Begin(gameObject, 0.2f, new Vector3(1f, 1f, 1));
            // TweenPosition.Begin(gameObject, 0.2f, Vector3.zero);
            // TweenPosition.Begin(goNavPanel, 0.2f, new Vector3(-600, 0, 0));
            // TweenAlpha.Begin(goBG, 0.2f, 0);
            menuIsShow = false;
        }
    }
    public void CloseMenuOnClick(GameObject go)
    {
        CloseMenuOnClick();
    }

    public void CloseUserOnClick()
    {
        goHomeMenu.SetActive(true);
        TweenAlpha.Begin(goHomeMenu, 0.3f, 1);
        TweenAlpha.Begin(goUser, 0.3f, 0);
        MenuOnClick();
    }
    public void CloseArchievementOnClick()
    {
        goHomeMenu.SetActive(true);
        TweenAlpha.Begin(goHomeMenu, 0.3f, 1);
        TweenAlpha.Begin(goArchievement, 0.3f, 0);
        MenuOnClick();
    }
    public void CloseUserInforOnClick()
    {
        TweenAlpha.Begin(goUserInfo, 0.3f, 0);
        goUser.SetActive(true);
        TweenAlpha.Begin(goUser, 0.3f, 1);
    }

    public void OpenUserOnCLick()
    {
        CloseMenuOnClick();
        TweenAlpha.Begin(goHomeMenu, 0.3f, 0);
        TweenAlpha.Begin(goUser, 0f, 0);
        goUser.SetActive(true);
        TweenAlpha.Begin(goUser, 0.3f, 1);
    }

    public void OpenArchievementOnCLick()
    {
        CloseMenuOnClick();
        TweenAlpha.Begin(goHomeMenu, 0.3f, 0);
        TweenAlpha.Begin(goArchievement, 0f, 0);
        goArchievement.SetActive(true);
        TweenAlpha.Begin(goArchievement, 0.3f, 1);
    }


    public void OpenUserInFoOnClick()
    {

    }

    public void OpenCodePanelOnClick()
    {
        TweenAlpha.Begin(goCodePanel, 0, 0);
        goCodePanel.SetActive(true);
        TweenAlpha.Begin(goCodePanel, 0.3f, 1);
    }

    public void CloseCodePanelOnClick()
    {
        TweenAlpha.Begin(goCodePanel, 0.3f, 0);
    }

    public UILabel[] lblPoints;
}

