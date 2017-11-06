using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarChangeScript : MonoBehaviour
{
    public UILabel label;
    UISlider slider;
    void Start()
    {
        slider = GetComponent<UISlider>();
        
    }
    public void OnValueChange()
    {
        label.text = ((int)(slider.value * 100)) + "%";
    }
}
