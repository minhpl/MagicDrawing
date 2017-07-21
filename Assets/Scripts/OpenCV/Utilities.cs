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
    List<Mat> splittedMat;

    public Utilities()
    {
      
    }
    public void Dispose()
    {
        zeroMat.Dispose();
        tempMat.Dispose();
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

    public void makeMonoAlphaMat(Mat inputMat, Mat outPutMat, bool invertColor = false)
    {
        List<Mat> listMat = new List<Mat> ( new Mat[] { null, null, null, null } );
        if (tempMat == null) tempMat = new Mat();
        if (invertColor)
            Core.bitwise_not(inputMat, tempMat);
        else
            inputMat.copyTo(tempMat);        
        if (zeroMat == null || zeroMat.width() != inputMat.width() || zeroMat.height() != inputMat.height())
            zeroMat = Mat.zeros(inputMat.size(), CvType.CV_8U);
        listMat[0] = tempMat;
        listMat[1] = zeroMat;
        listMat[2] = zeroMat;
        listMat[3] = tempMat;
        Core.merge(listMat, outPutMat);
    }

    public static void Log(string msgFormat, params object[] args)
    {
        Debug.LogFormat("mlogcat " + msgFormat, args);
    }

    public static TimeSpan Time(Action action)
    {
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
    
}
