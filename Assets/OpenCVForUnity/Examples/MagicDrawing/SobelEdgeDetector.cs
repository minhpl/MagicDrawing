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
        public int KSizeGaussBlur = 0;
        //public int min_KSizeGaussBlur = 0, max_KSizeGaussBlur = 4;


        public double sigmaX = 0;
        public double sigmaY = 0;       

        public int KSizeSobel = 0;
        //public int Max_KSizeSobel = 1, Min_KSizeSobel = 0;      

        public double scaleSobel = 1;
        //public double max_scaleSobel = 5, min_scaleSobel = 0.5;        

        public double deltaSobel = 0;

        public int blockSizeAdaptive = 1;
        //public int maxblockSizeAdaptive = 16, minblockSizeAdaptive = 0;

        public double c_adaptiveThreshold = -1;
        //public int max_c_adaptiveThreshold = -1, min_c_adaptiveThreshold = -35;        

        Mat grad_x;
        Mat grad_y;
        Mat abs_grad_x, abs_grad_y;
        int borderType = Core.BORDER_DEFAULT;

        public Mat sobelEdgeDetect(Mat inputMat)
        {
            inputMat.copyTo(tempMat);
            Imgproc.GaussianBlur(tempMat, tempMat, new Size(KSizeGaussBlur*2+1, KSizeGaussBlur*2+1), sigmaX, sigmaY, Core.BORDER_DEFAULT);
            if (inputMat.channels()<2)
            {
                //Debug.LogFormat("The input Image is the gray mat");
            }
            else
            {
                Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_BGR2GRAY);
            }
            Imgproc.Sobel(tempMat, grad_x, depth, 1, 0, 2 * KSizeSobel + 1, scaleSobel, deltaSobel, borderType);
            Core.convertScaleAbs(grad_x, abs_grad_x);
            Imgproc.Sobel(tempMat, grad_y, depth, 0, 1, 2 * KSizeSobel + 1, scaleSobel, deltaSobel, borderType);
            Core.convertScaleAbs(grad_y, abs_grad_y);
            Core.addWeighted(abs_grad_x, 0.5, abs_grad_y, 0.5, 0, edgeDetected);           
            return edgeDetected;
        }

        int adaptiveMethod = Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C;
        int thresholdType = Imgproc.THRESH_BINARY_INV;

        public void adapTiveThreshold(Mat inputMat, Mat outputMat)
        {
            Imgproc.adaptiveThreshold(inputMat, outputMat, 255, adaptiveMethod, thresholdType, 2* blockSizeAdaptive+1, c_adaptiveThreshold);
        }
    }
}
