using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour
{
    public Text txtProgress;
    void Awake()
    {
        var slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener((float v) =>
        {
            Debug.Log("here");
            int percent = (int)(v * 100);
            txtProgress.text = string.Format("Đang tải lên facebook {0}%...", percent);
        });
    }
}
