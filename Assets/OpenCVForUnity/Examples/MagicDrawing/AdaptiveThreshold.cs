using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class AdaptiveThreshold : MonoBehaviour 
    {

        public int KSizeGaussBlur = 0;
        public int blockSizeAdaptive = 0;
        public double c_adaptiveThreshold = 0;
        public double sigmaX = 0;
        public double sigmaY = 0;

        int adaptiveMethod = Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C;
        int thresholdType = Imgproc.THRESH_BINARY;

        Mat outputMat;
        Mat tempMat;

        public void Start()
        {
            outputMat = new Mat();
            tempMat = new Mat();
        }

        public Mat adapTiveThreshold(Mat inputMat)
        {
            inputMat.copyTo(tempMat);
            Imgproc.GaussianBlur(tempMat, tempMat, new Size(KSizeGaussBlur * 2 + 1, KSizeGaussBlur * 2 + 1), sigmaX, sigmaY, Core.BORDER_DEFAULT);
            Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_BGR2GRAY);
            Imgproc.adaptiveThreshold(tempMat, tempMat, 255, adaptiveMethod, thresholdType, 2 * blockSizeAdaptive + 3, c_adaptiveThreshold);
            Debug.LogFormat("width is {0}", outputMat.size().width);
            return tempMat;
        }

    }
}
