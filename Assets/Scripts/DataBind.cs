using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBind : MonoBehaviour {
    public DrawingTemplateModel drawingTemplateModel = null; 
    public string imagePath = null;
    public string videoPath = null;

    private void Awake()
    {
        drawingTemplateModel = null;
        videoPath = null;
        imagePath = null;
    }
}
