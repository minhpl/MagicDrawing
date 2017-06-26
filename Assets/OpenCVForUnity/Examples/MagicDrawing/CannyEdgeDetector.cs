using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class CannyEdgeDetector : MonoBehaviour
    {
        private void Start()
        {
            edgeDetected = new Mat();
            tempMat = new Mat();
        }
        Mat edgeDetected;
        Mat tempMat;
        public int KSizeGaussBlur = 1;
        public double sigmaX = 0;
        public double sigmaY = 0;
        public double lowThreshold = 1;
        public int ratioHighThreshold = 3;
        public int KSizeCanny = 3;
        public Mat edgeDetect(Mat inputMat)
        {
            Imgproc.GaussianBlur(inputMat, tempMat, new Size(KSizeGaussBlur, KSizeGaussBlur), sigmaX, sigmaY, Core.BORDER_DEFAULT);
            if (inputMat.channels() < 2)
            {
                //Debug.LogFormat("The input Image is the gray mat");
            }
            else
            {
                Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_BGR2GRAY);
            }
            Imgproc.Canny(tempMat, edgeDetected, lowThreshold, lowThreshold * ratioHighThreshold, KSizeCanny,false);
            Core.bitwise_not(edgeDetected, edgeDetected);
            return edgeDetected;
        }
        int adaptiveMethod = Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C;
        int thresholdType = Imgproc.THRESH_BINARY;
        public int blockSizeAdaptive = 1;
        public double c_adaptiveThreshold = 0.99;
        public void adapTiveThreshold(Mat inputMat, Mat outputMat)
        {
            Imgproc.adaptiveThreshold(inputMat, outputMat, 255, adaptiveMethod, thresholdType, 2 * blockSizeAdaptive + 1, c_adaptiveThreshold);
        }
    }
}
