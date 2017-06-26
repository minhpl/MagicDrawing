using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class ScharrEdgeDetector : MonoBehaviour
    {
        private void Start()
        {
            edgeDetected = new Mat();
            tempMat = new Mat();
            grad_x = new Mat();
            grad_y = new Mat();
            abs_grad_x = new Mat();
            abs_grad_y = new Mat();
        }
        Mat edgeDetected;
        Mat tempMat;
        public void Dispose()
        {
            edgeDetected.Dispose();
            tempMat.Dispose();
        }
        int depth = CvType.CV_16S;
        public int KSizeGaussBlur = 0;
        public double sigmaX = 0;
        public double sigmaY = 0;        
        public double scaleScharr = 1;
        public double deltaScharr = 0;
        public int dxX = 1;
        public int dyX = 0;
        public int dxY = 0;
        public int dyY = 1;
        Mat grad_x;
        Mat grad_y;
        Mat abs_grad_x, abs_grad_y;
        int borderType = Core.BORDER_REPLICATE;
        public Mat scharrEdgeDetect(Mat inputMat)
        {
            inputMat.copyTo(tempMat);
            Imgproc.GaussianBlur(tempMat, tempMat, new Size(KSizeGaussBlur * 2 + 1, KSizeGaussBlur * 2 + 1), sigmaX, sigmaY, Core.BORDER_DEFAULT);
            if (inputMat.channels() < 2)
            {
                //Debug.LogFormat("The input Image is the gray mat");
            }
            else
            {
                Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_BGR2GRAY);
            }
            Imgproc.Scharr(tempMat, grad_x, depth, dxX, dyX, scaleScharr, deltaScharr, borderType);
            //Imgproc.Sobel(tempMat, grad_x, depth, 1, 0, KSizeSobel, scaleSobel, deltaSobel, borderType);
            Core.convertScaleAbs(grad_x, abs_grad_x);
            Imgproc.Scharr(tempMat, grad_y, depth, dxY, dyY, scaleScharr, deltaScharr, borderType);
            //Imgproc.Sobel(tempMat, grad_y, depth, 0, 1, KSizeSobel, scaleSobel, deltaSobel, borderType);
            Core.convertScaleAbs(grad_y, abs_grad_y);
            Core.addWeighted(abs_grad_x, 0.5, abs_grad_y, 0.5, 0, edgeDetected);
            return edgeDetected;
        }
        int adaptiveMethod = Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C;
        int thresholdType = Imgproc.THRESH_BINARY_INV;
        public int blockSizeAdaptive = 1;
        public double c_adaptiveThreshold = 0.99;
        public void adapTiveThreshold(Mat inputMat, Mat outputMat)
        {
            Imgproc.adaptiveThreshold(inputMat, outputMat, 255, adaptiveMethod, thresholdType, 2 * blockSizeAdaptive + 1, c_adaptiveThreshold);
        }
    }
}
