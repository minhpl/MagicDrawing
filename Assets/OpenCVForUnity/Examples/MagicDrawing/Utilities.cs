using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class Utilities
    {
        public void CovertGrayMatToRedTransparentMat(Mat inputMat,Mat outputMat,bool inverse = false)
        {
            if(inverse)
            {
                Core.bitwise_not(inputMat, inputMat);
            }

            List<Mat> listMat = new List<Mat>();
            Mat zeroMat = Mat.zeros(inputMat.size(), CvType.CV_8U);
            Mat maxValueMat = new Mat(inputMat.size(), CvType.CV_8U,new Scalar(255));
            Mat red = Mat.zeros(inputMat.size(), CvType.CV_8U);
            inputMat.copyTo(red);
            listMat.Add(red);
            listMat.Add(zeroMat);
            listMat.Add(zeroMat);
            listMat.Add(red);
            Core.merge(listMat, outputMat);
        }

        public void OverlayOnRGBMat(Mat frontMat, Mat backgroudMat, Mat outputMat)
        {
            Core.bitwise_not(frontMat, frontMat );
            //Imgproc.cvtColor(frontMat, frontMat, Imgproc.COLOR_GRAY2RGB);
            List<Mat> splittedMat = new List<Mat>();
            Core.split(backgroudMat, splittedMat);

            //Debug.LogFormat("channel number is {0}", frontMat.channels());
            //Core.bitwise_not(frontMat, frontMat);
            frontMat.copyTo(splittedMat[0], frontMat);

            //splittedMat[0].setTo(frontMat,frontMat);
            splittedMat[1].setTo(new Scalar(0),frontMat);
            splittedMat[2].setTo(new Scalar(0),frontMat);
            Core.merge(splittedMat, outputMat);
            //List<Mat> listMat = new List<Mat>();
            //Mat zeroMat = Mat.zeros(inputMat.size(), CvType.CV_8U);
            //Mat maxValueMat = new Mat(inputMat.size(), CvType.CV_8U, new Scalar(255));
            //Mat red = Mat.zeros(inputMat.size(), CvType.CV_8U);
            //inputMat.copyTo(red);
            //listMat.Add(red);
            //listMat.Add(zeroMat);
            //listMat.Add(zeroMat);
            //listMat.Add(red);
            //Core.merge(listMat, outputMat);



            //List<Mat> listMat = new List<Mat>();
            //Core.split(m, listMat);           
            //listMat[1].setTo(new Scalar(0));
            //listMat[2].setTo(new Scalar(0));
            //Core.merge(listMat, m);
        }
    }
}
