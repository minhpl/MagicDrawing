using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Utilities
{
    List<Mat> listMat;
    Mat zeroMat;
    Mat MonoChormeMatOneChannel;
    Mat MonoChormeMatThreeChannel;
    Mat tempMat;
    List<Mat> splittedMat;
    Mat maxValueMat;
    //public const int transparent = 200;

    public Utilities()
    {
        listMat = new List<Mat>();
        tempMat = new Mat();
        splittedMat = new List<Mat>();
    }


    public void set(Mat inputMat)
    {
        maxValueMat = new Mat(inputMat.size(), CvType.CV_8U, new Scalar(255));
        zeroMat = Mat.zeros(inputMat.size(), CvType.CV_8U);
        MonoChormeMatOneChannel = Mat.zeros(inputMat.size(), CvType.CV_8U);
        MonoChormeMatThreeChannel = Mat.zeros(inputMat.size(), CvType.CV_8UC3);
    }

    public void CovertGrayMatToRedTransparentMat(Mat inputMat, Mat outputMat, bool inverse = false)
    {
        if (inverse)
        {
            Core.bitwise_not(inputMat, inputMat);
        }
        inputMat.copyTo(MonoChormeMatOneChannel);
        listMat.Add(MonoChormeMatOneChannel);
        listMat.Add(zeroMat);
        listMat.Add(zeroMat);
        listMat.Add(MonoChormeMatOneChannel);
        Core.merge(listMat, outputMat);
    }

    public void OverlayOnRGBMat(Mat frontMat, Mat backgroudMat, Mat outputMat)
    {
        Core.bitwise_not(frontMat, tempMat);
        Core.split(backgroudMat, splittedMat);
        tempMat.copyTo(splittedMat[0], tempMat);
        splittedMat[1].setTo(new Scalar(0), tempMat);
        splittedMat[2].setTo(new Scalar(0), tempMat);
        Core.merge(splittedMat, outputMat);
    }

    public void OverlayTransparentOnRGBMat(Mat frontMat, Mat backgroudMat, Mat outputMat, float redWeight = 0.0f)
    {
        Core.bitwise_not(frontMat, tempMat);
        Core.split(backgroudMat, splittedMat);
        //float alpha = 1 - redWeight;
        //Core.add(splittedMat[0], new Scalar(10), splittedMat[0], tempMat);
        //Core.add(splittedMat[0] * alpha, tempMat * redWeight, splittedMat[0], tempMat);
        //Core.addWeighted(splittedMat[0], alpha, tempMat, redWeight, 0, splittedMat[0]);
        //tempMat.copyTo(splittedMat[0], tempMat);
        splittedMat[1].setTo(new Scalar(0), tempMat);
        splittedMat[2].setTo(new Scalar(0), tempMat);
        Core.merge(splittedMat, outputMat);
    }


    public void test()
    {

        Mat a = new Mat(3, 3, CvType.CV_8UC3);
        a.setTo(new Scalar(300, 10, 10));

        for (int i = 0; i < a.cols(); i++)
            for (int j = 0; j < a.rows(); j++)
            {
                var val = a.get(i, j)[0];
                Debug.LogFormat("{0} ", val);
            }
    }

    //public void OverlayTransparentOnRGBMat2(Mat frontMat, Mat backgroudMat, Mat outputMat, float redWeight = 0.05f)
    //{
    //    Core.bitwise_not(frontMat, tempMat);
    //    //Core.split(backgroudMat, splittedMat);
    //    float alpha = 1 - redWeight;
    //    MonoChormeMatThreeChannel = Mat.zeros(frontMat.size(), CvType.CV_8UC4);
    //    MonoChormeMatThreeChannel.setTo(new Scalar(255, 0, 0,255), tempMat);

    //    Mat a = new Mat(frontMat.size(), CvType.CV_8UC3);

    //    Core.addWeighted(backgroudMat, alpha, MonoChormeMatThreeChannel, redWeight, 0, a);

    //    a.copyTo(outputMat);

    //    //tempMat.copyTo(splittedMat[0], tempMat);
    //    //splittedMat[1].setTo(new Scalar(0), tempMat);
    //    //splittedMat[2].setTo(new Scalar(0), tempMat);
    //    //Core.merge(splittedMat, outputMat);
    //}

    public static void Log(string msgFormat, params object[] args)
    {
        Debug.LogFormat("mlogcat " + msgFormat, args);
    }

}
