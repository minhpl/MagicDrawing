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
        Mat tempMat;
        Size kSizeGaussSize;
        int ddepth = CvType.CV_16U;
        int adaptiveMethod = Imgproc.ADAPTIVE_THRESH_MEAN_C;
        int thresholdType = Imgproc.THRESH_BINARY;
        Mat matResult;
        public int blockSize = 3;
        public double c = 1;
        public double c2 = 1;
        public int KSiszeGauss = 1;
        public int kSize = 1;
        public double scale = 3;
        public double delta = 0;
        public int reduceValue = 1;
        public int threshold1 = 1;
        public int threshold2 = 1;
        private void Start()
        {
            tempMat = new Mat();
            kSizeGaussSize = new Size(KSiszeGauss, KSiszeGauss);
            matResult = new Mat();
        }


        public int morph_elem = 0;
        public int morph_size = 0;
        public int morph_operator = 0;
        const int max_operator = 4;
        const int max_elem = 2;
        const int max_kernel_size = 21;


        public Mat laplaceEdgeDetect(Mat inputMat)
        {
            //int operation = morph_operator + 2;
            //Mat element = Imgproc.getStructuringElement(morph_elem, new Size(2 * morph_size + 1, 2 * morph_size + 1), new Point(morph_size, morph_size));
            //Imgproc.morphologyEx(inputMat, matResult, operation, element);

            Imgproc.GaussianBlur(inputMat, tempMat, new Size(KSiszeGauss, KSiszeGauss), 5, 5, Core.BORDER_DEFAULT);
            ////inputMat.copyTo(tempMat);
            Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_BGR2GRAY);
            ////Core.subtract(tempMat, new Scalar(reduceValue), tempMat);
            ////Imgproc.Laplacian(tempMat, matResult, ddepth);
            Imgproc.Laplacian(tempMat, matResult, ddepth, kSize, scale, delta, Core.BORDER_DEFAULT);
            ////Core.subtract(matResult, new Scalar(reduceValue), matResult);
            Core.convertScaleAbs(matResult, matResult);
            //////Mat a = new Mat();
            ////Imgproc.threshold(tempMat, matResult,   0, 255, Imgproc.THRESH_TRIANGLE);
            //Imgproc.threshold(matResult, matResult, threshold1, 255, CvType.CV_16U);
            Imgproc.adaptiveThreshold(matResult, matResult, 255, adaptiveMethod, thresholdType, blockSize, c);
            //Imgproc.Canny(matResult, matResult, threshold1, threshold1 * 3, 3, true);
            //Imgproc.adaptiveThreshold(matResult, matResult, 255, adaptiveMethod, thresholdType, blockSize, c2);
            //Core.convertScaleAbs(matResult, matResult);
            return matResult;
        }
    }
}
