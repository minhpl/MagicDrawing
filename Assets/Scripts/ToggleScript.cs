using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour {
    public Button BtnSlider;
    public Button BtnSlider_active;
    public Button btnContract;
    public GameObject panel_line;
    public GameObject panel_constract;
    public GameObject line;
    public GameObject contrast;    
    public Button Recording;
    public Button BtnPush;
    public Button BtnPushActive;       
    public GameObject panel_tool;
    public GameObject backBtn;
    public TapGesture doubleTapGesture;
    public PressGesture pressGesture;
    public Button BtnTick;
    public Button BtnCancel;
    public Text textComfirm;
    void Start() {

        doubleTapGesture.Tapped += tappedHandler;
        pressGesture.Pressed += (object sender, System.EventArgs e) =>
        {
            panel_line.gameObject.SetActive(false);
            panel_constract.gameObject.SetActive(false);
            btnContract.gameObject.SetActive(false);
            BtnSlider_active.gameObject.SetActive(false);
            BtnSlider.gameObject.SetActive(true);
        };
        
        BtnTick.onClick.AddListener(() =>
        {
            if(!BtnCancel.gameObject.activeSelf)
            {
                BtnCancel.gameObject.SetActive(true);
                textComfirm.gameObject.SetActive(true);
            }
        });

        BtnCancel.onClick.AddListener(() =>
        {
            BtnCancel.gameObject.SetActive(false);
            textComfirm.gameObject.SetActive(false);
        });

        BtnSlider.onClick.AddListener(() =>
        {
            BtnSlider.gameObject.SetActive(false);
            BtnSlider_active.gameObject.SetActive(true);
            btnContract.gameObject.SetActive(true);
            btnContract.gameObject.transform.Find("contrast").gameObject.SetActive(false);
            btnContract.gameObject.transform.Find("line").gameObject.SetActive(true);
            line.SetActive(true);
            contrast.SetActive(false);
            panel_line.SetActive(true);
            BtnPushActive.gameObject.SetActive(false);
            BtnPush.gameObject.SetActive(true);
        });

        BtnSlider_active.onClick.AddListener(() =>
        {
            BtnSlider_active.gameObject.SetActive(false);
            BtnSlider.gameObject.SetActive(true);
            btnContract.gameObject.SetActive(false);
            panel_line.SetActive(false);
            panel_constract.SetActive(false);
        });

        btnContract.onClick.AddListener(() =>
        {
            bool stateLine = panel_line.activeSelf;            
            line.SetActive(!stateLine);
            contrast.SetActive(stateLine);
            panel_line.SetActive(!stateLine);
            panel_constract.SetActive(stateLine);
        });

        BtnPush.onClick.AddListener(() =>
        {
            BtnPush.gameObject.SetActive(false);
            BtnPushActive.gameObject.SetActive(true);
            BtnSlider_active.gameObject.SetActive(false);
            BtnSlider.gameObject.SetActive(true);
            panel_line.SetActive(false);
            panel_constract.SetActive(false);
            btnContract.gameObject.SetActive(false);
        });

        BtnPushActive.onClick.AddListener(() =>
        {
            BtnPushActive.gameObject.SetActive(false);
            BtnPush.gameObject.SetActive(true);
        });
    }


    private void tappedHandler(object sender, System.EventArgs e)
    {
        Debug.Log("xin chao tapped handler");
        if (panel_tool)
            panel_tool.SetActive(!panel_tool.activeSelf);
        if (backBtn)
            backBtn.SetActive(!backBtn.activeSelf);
    }
}
