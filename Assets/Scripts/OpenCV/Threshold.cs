using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Threshold : MonoBehaviour
{

    Mat resultImage;
    Mat tempMat;

    public int KSizeGaussBlur = 0;
    public int blockSizeAdaptive = 9;
    public double c_adaptiveThreshold = 13;
    public double sigmaX = 0;
    public double sigmaY = 0;

    void Start()
    {
        resultImage = new Mat();
        tempMat = new Mat();
    }

    public double thresholdValue = 40.53;
    public int thresholdType = Imgproc.THRESH_BINARY;
    public int max_BINARY_value = 255;

    public Mat threshold(Mat inputMat)
    {
        inputMat.copyTo(tempMat);
        Imgproc.GaussianBlur(tempMat, tempMat, new Size(KSizeGaussBlur * 2 + 1, KSizeGaussBlur * 2 + 1), sigmaX, sigmaY, Core.BORDER_DEFAULT);
        Imgproc.cvtColor(inputMat, tempMat, Imgproc.COLOR_BGR2GRAY);
        Imgproc.threshold(tempMat, resultImage, thresholdValue, max_BINARY_value, thresholdType % 8);
        return resultImage;
    }
}
