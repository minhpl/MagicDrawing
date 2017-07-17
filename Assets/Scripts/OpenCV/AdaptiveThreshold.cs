using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class AdaptiveThreshold : MonoBehaviour
{
    public int KSizeGaussBlur = 0;
    public int blockSizeAdaptive = 0;
    public double c_adaptiveThreshold = 0;
    public double sigmaX = 0;
    public double sigmaY = 0;
    public int adaptiveMethod = Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C;    
    public int thresholdType = Imgproc.THRESH_BINARY;

    public Mat outputMat;
    public Mat tempMat;

    public void Start()
    {
        outputMat = new Mat();
        tempMat = new Mat();
    }

    int blockSizeAdaptive1 = 3, blockSizeAdaptive2 = 3, blockSizeAdaptive3 = 5;
    float c_adaptive_threshold1 = 15f, c_adaptive_threshold2 = 3f, c_adaptive_threshold3 = 1f;

    public void setParameter(float sliderValue)
    {
        int head = 50;
        int maxValue = 100;
        if (sliderValue < head)
        {
            blockSizeAdaptive = (int)Math.Round(blockSizeAdaptive1 + (blockSizeAdaptive2 - blockSizeAdaptive1) * (sliderValue / (float)head));            
            c_adaptiveThreshold = (c_adaptive_threshold1 + (c_adaptive_threshold2 - c_adaptive_threshold1) * (sliderValue / (float)head));
        }
        else
        {
            sliderValue = sliderValue - head;
            int tail = maxValue - head;
            blockSizeAdaptive = (int)Math.Round(blockSizeAdaptive2 + (blockSizeAdaptive3 - blockSizeAdaptive2) * (sliderValue / (float)tail));
            c_adaptiveThreshold = (c_adaptive_threshold2 + (c_adaptive_threshold3 - c_adaptive_threshold2) * (sliderValue / (float)tail));
        }
    }

    public Mat adapTiveThreshold(Mat inputMat)
    {
        inputMat.copyTo(tempMat);
        Imgproc.GaussianBlur(tempMat, tempMat, new Size(KSizeGaussBlur * 2 + 1, KSizeGaussBlur * 2 + 1), sigmaX, sigmaY, Core.BORDER_DEFAULT);
        Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_RGBA2GRAY);
        Imgproc.adaptiveThreshold(tempMat, outputMat, 255, adaptiveMethod % 2, thresholdType % 8, 2 * blockSizeAdaptive + 3, c_adaptiveThreshold);     
        return outputMat;
    }

    private void OnValidate()
    {
        var a = GetComponent<DrawingScripts>();
        if (a != null)
            a.ValueChangeCheck(null);
    }
}

