using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class Threshold : MonoBehaviour
    {

        Mat resultImage;
        Mat tempMat;

        void Start()
        {
            resultImage = new Mat();
            tempMat = new Mat();
        }

        public double thresholdValue = 40.53;
        public int thresholdType = 1;
        public int max_BINARY_value = 255;        

        public Mat threshold(Mat inputMat)
        {
            if (inputMat.channels() < 2)
            {                
                inputMat.copyTo(tempMat);
            }
            else
            {
                Imgproc.cvtColor(inputMat, tempMat, Imgproc.COLOR_BGR2GRAY);
            }

            Imgproc.threshold(tempMat, resultImage, thresholdValue, max_BINARY_value, thresholdType % 8);
            return resultImage;
        }
    }
}
