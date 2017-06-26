using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class SobelEdgeDetector : MonoBehaviour
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
        public int KSizeGaussBlur = 1;
        public double sigmaX = 0;
        public double sigmaY = 0;       
        public int KSizeSobel = 1;
        public double scaleSobel = 1;
        public double deltaSobel = 0;
        Mat grad_x;
        Mat grad_y;
        Mat abs_grad_x, abs_grad_y;
        int borderType = Core.BORDER_DEFAULT;
        public Mat sobelEdgeDetect(Mat inputMat)
        {
            Imgproc.GaussianBlur(inputMat, tempMat, new Size(KSizeGaussBlur, KSizeGaussBlur), sigmaX, sigmaY, Core.BORDER_DEFAULT);
            if (inputMat.channels()<2)
            {
                //Debug.LogFormat("The input Image is the gray mat");
            }
            else
            {
                Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_BGR2GRAY);
            }
            Imgproc.Sobel(tempMat, grad_x, depth, 1, 0, KSizeSobel, scaleSobel, deltaSobel, borderType);
            Core.convertScaleAbs(grad_x, abs_grad_x);
            Imgproc.Sobel(tempMat, grad_y, depth, 0, 1, KSizeSobel, scaleSobel, deltaSobel, borderType);
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
            Imgproc.adaptiveThreshold(inputMat, outputMat, 255, adaptiveMethod, thresholdType, 2* blockSizeAdaptive+1, c_adaptiveThreshold);
        }
    }
}
