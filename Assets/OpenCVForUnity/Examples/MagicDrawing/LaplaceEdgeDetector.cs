using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class LaplaceEdgeDetector : MonoBehaviour
    {
        private void Start()
        {
            edgeDetected = new Mat();
            tempMat = new Mat();
        }
        Mat edgeDetected;
        Mat tempMat;
        int depth = CvType.CV_16S;
        public int KSizeGaussBlur = 0;
        public double sigmaX = 0;
        public double sigmaY = 0;
        public int kSizeLaplcae = 0;
        public double scale = 3;
        public double delta = 0;
        public Mat laplaceEdgeDetect(Mat inputMat)
        {
            //int operation = morph_operator + 2;
            //Mat element = Imgproc.getStructuringElement(morph_elem, new Size(2 * morph_size + 1, 2 * morph_size + 1), new Point(morph_size, morph_size));
            //Imgproc.morphologyEx(inputMat, matResult, operation, element);

            inputMat.copyTo(tempMat);
            Imgproc.GaussianBlur(tempMat, tempMat, new Size(KSizeGaussBlur * 2 + 1, KSizeGaussBlur * 2 + 1), sigmaX, sigmaX, Core.BORDER_DEFAULT);
            ////inputMat.copyTo(tempMat);
            if (inputMat.channels() < 2)
            {
                //Debug.LogFormat("The input Image is the gray mat");
            }
            else
            {
                Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_BGR2GRAY);
            }
            ////Core.subtract(tempMat, new Scalar(reduceValue), tempMat);
            ////Imgproc.Laplacian(tempMat, matResult, ddepth);
            Imgproc.Laplacian(tempMat, edgeDetected, depth, kSizeLaplcae * 2 + 1, scale, delta, Core.BORDER_ISOLATED);
            ////Core.subtract(matResult, new Scalar(reduceValue), matResult);
            Core.convertScaleAbs(edgeDetected, edgeDetected);
            //////Mat a = new Mat();
            ////Imgproc.threshold(tempMat, matResult,   0, 255, Imgproc.THRESH_TRIANGLE);
            //Imgproc.threshold(matResult, matResult, threshold1, 255, CvType.CV_16U);
            //Imgproc.adaptiveThreshold(matResult, matResult, 255, adaptiveMethod, thresholdType, blockSize, c);
            //Imgproc.Canny(matResult, matResult, threshold1, threshold1 * 3, 3, true);
            //Imgproc.adaptiveThreshold(matResult, matResult, 255, adaptiveMethod, thresholdType, blockSize, c2);
            //Core.convertScaleAbs(matResult, matResult);
            return edgeDetected;
        }
        int adaptiveMethod = Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C;
        int thresholdType = Imgproc.THRESH_BINARY_INV;
        public int blockSizeAdaptive = 1;
        public double c_adaptiveThreshold = -7.5;
        public void adapTiveThreshold(Mat inputMat, Mat outputMat)
        {
            Imgproc.adaptiveThreshold(inputMat, outputMat, 255, adaptiveMethod, thresholdType, 2 * blockSizeAdaptive + 1, c_adaptiveThreshold);
        }
    }
}
