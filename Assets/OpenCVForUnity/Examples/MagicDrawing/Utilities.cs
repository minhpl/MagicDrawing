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
        List<Mat> listMat;
        Mat zeroMat;
        Mat red;
        Mat tempMat;
        List<Mat> splittedMat;
        Mat maxValueMat;

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
            red = Mat.zeros(inputMat.size(), CvType.CV_8U);
        }

        public void CovertGrayMatToRedTransparentMat(Mat inputMat, Mat outputMat, bool inverse = false)
        {
            if (inverse)
            {
                Core.bitwise_not(inputMat, inputMat);
            }                        
            inputMat.copyTo(red);
            listMat.Add(red);
            listMat.Add(zeroMat);
            listMat.Add(zeroMat);
            listMat.Add(red);
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
    }
}
