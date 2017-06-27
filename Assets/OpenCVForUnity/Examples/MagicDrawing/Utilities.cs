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

        public Utilities()
        {
            listMat = new List<Mat>();
        }

        public void CovertGrayMatToRedTransparentMat(Mat inputMat, Mat outputMat, bool inverse = false)
        {
            if (inverse)
            {
                Core.bitwise_not(inputMat, inputMat);
            }
            zeroMat = Mat.zeros(inputMat.size(), CvType.CV_8U);
            Mat maxValueMat = new Mat(inputMat.size(), CvType.CV_8U, new Scalar(255));
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
            List<Mat> splittedMat = new List<Mat>();
            Core.split(backgroudMat, splittedMat);
            frontMat.copyTo(splittedMat[0], frontMat);           
            splittedMat[1].setTo(new Scalar(0),frontMat);
            splittedMat[2].setTo(new Scalar(0),frontMat);
            Core.merge(splittedMat, outputMat);
        }
    }
}
