using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScript : MonoBehaviour {
    public Button BtnSlider;
    public Button BtnSlider_active;
    public Button btnContract;
    public GameObject panel_line;
    public GameObject panel_constract;
    // Use this for initialization
    public Button Record;
    public Button Recording;
    void Start () {
        
        BtnSlider.onClick.AddListener(() =>
        {
            BtnSlider.gameObject.SetActive(false);
            BtnSlider_active.gameObject.SetActive(true);
            btnContract.gameObject.SetActive(true);
            btnContract.gameObject.transform.Find("contrast").gameObject.SetActive(false);
            btnContract.gameObject.transform.Find("line").gameObject.SetActive(true);
            panel_line.SetActive(true);
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
            btnContract.gameObject.transform.Find("line").gameObject.SetActive(!stateLine);
            btnContract.gameObject.transform.Find("contrast").gameObject.SetActive(stateLine);
            panel_line.SetActive(!stateLine);
            panel_constract.SetActive(stateLine);
        });


        Record.onClick.AddListener(() =>
        {
            Record.gameObject.SetActive(false);
            Recording.gameObject.SetActive(true);
            GetComponent<DrawingScripts>().StartRecordVideo();
        });

        Recording.onClick.AddListener(() =>
        {
            Recording.gameObject.SetActive(false);
            Record.gameObject.SetActive(true);
            GetComponent<DrawingScripts>().StopVideoRecording();
        });

    }
}
