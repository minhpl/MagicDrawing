using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Utilities
{
    Mat zeroMat;
    Mat tempMat;
    Mat tempMat2;
    List<Mat> splittedMat;
    List<Mat> listMat;

    public Utilities()
    {
        listMat = new List<Mat>(new Mat[] { null, null, null, null });
    }
    public void Dispose()
    {
        zeroMat.Dispose();
        tempMat.Dispose();
        monoAlphaMat.Dispose();
        splittedMat = null;
    }


    public void OverlayTransparentOnRGBMat(Mat frontMat, Mat backgroudMat, Mat outputMat, float redWeight = 0.0f)
    {
        if (tempMat == null)
            tempMat = new Mat();
        Core.bitwise_not(frontMat, tempMat);
        if (splittedMat == null)
            splittedMat = new List<Mat>();
        Core.split(backgroudMat, splittedMat);
        splittedMat[1].setTo(new Scalar(0), tempMat);
        splittedMat[2].setTo(new Scalar(0), tempMat);
        Core.merge(splittedMat, outputMat);
    }

    Mat monoAlphaMat;
    public enum MonoColor {RED, GREEN, BLUE};
    public Mat makeMonoAlphaMat(Mat inputMat, MonoColor color,int alpha = 100, bool invertColor = false)
    {
        if (tempMat == null)
        {
            tempMat = new Mat();            
        }
        if (invertColor) Core.bitwise_not(inputMat, tempMat);
        else inputMat.copyTo(tempMat);
        if (zeroMat == null || zeroMat.width() != inputMat.width() || zeroMat.height() != inputMat.height())
        {
            zeroMat = Mat.zeros(inputMat.size(), CvType.CV_8U);
            tempMat2 = Mat.zeros(inputMat.size(), CvType.CV_8U);
        }
        if (color == MonoColor.RED)
        {
            listMat[0] = tempMat;
            listMat[1] = zeroMat;
            listMat[2] = zeroMat;
        }
        else if(color==MonoColor.GREEN)
        {
            listMat[0] = zeroMat;
            listMat[1] = tempMat;            
            listMat[2] = zeroMat;
        }
        else if(color==MonoColor.BLUE)
        {
            listMat[0] = zeroMat;
            listMat[1] = zeroMat;
            listMat[2] = tempMat;
        }
        //tempMat2.setTo(new Scalar(alpha), tempMat);
        //listMat[3] = tempMat2;
        listMat[3] = tempMat;
        if (monoAlphaMat == null)
            monoAlphaMat = new Mat();
        Core.merge(listMat, monoAlphaMat);
        return monoAlphaMat;
    }

    public static void Log(string msgFormat, params object[] args)
    {
        Debug.LogFormat("mlogcat " + msgFormat, args);
    }

    public static string customFmts = "yyyyMMd_HHmmss";


    public static TimeSpan Time(Action action)
    {
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
    
}
