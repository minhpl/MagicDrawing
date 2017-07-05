using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class SobelEdgeDetector : MonoBehaviour
    {
        public InputField inputField;
        public InputField inputField1;
        public InputField inputField2;
        public InputField inputField3;

        public Slider slider;

        private void Start()
        {
            inputField.onValueChange.AddListener(delegate { ValueChangeCheck(); });
            inputField1.onValueChange.AddListener(delegate { ValueChangeCheck1(); });
            inputField2.onValueChange.AddListener(delegate { ValueChangeCheck2(); });
            inputField3.onValueChange.AddListener(delegate { ValueChangeCheck3(); });

            slider.onValueChanged.AddListener(delegate { slider_onValueChanged(); });
            setParameter(slider.value);

            edgeDetected = new Mat();
            tempMat = new Mat();
            grad_x = new Mat();
            grad_y = new Mat();
            abs_grad_x = new Mat();
            abs_grad_y = new Mat();
        }

        int KSizeSobel1 = 0, KSizeSobel2 = 0, KSizeSobel3 = 0;
        double ScaleSobel1 = 4, ScaleSobel2 = 4, ScaleSobel3 = 4;
        int BlockSizeAdaptive1 = 2, BlockSizeAdaptive2 = 3, BlockSizeAdaptive3 = 4;
        double c_adaptive_threshold1 = -10, c_adaptive_threshold2 = -5, c_adaptive_threshold3 = -1;


        void slider_onValueChanged()
        {
            //float sliderValue = slider.value;
            //setParameter(sliderValue);
        }

        public void setParameter(float sliderValue)
        {
            if (sliderValue < 50)
            {
                KSizeSobel = (int)Math.Round(KSizeSobel1 + (KSizeSobel2 - KSizeSobel1) * (sliderValue / 50f));
                scaleSobel = (ScaleSobel1 + (ScaleSobel2 - ScaleSobel1) * (sliderValue / 50f));
                blockSizeAdaptive = (int)Math.Round(BlockSizeAdaptive1 + (BlockSizeAdaptive2 - BlockSizeAdaptive1) * (sliderValue / 50f));
                c_adaptiveThreshold = (c_adaptive_threshold1 + (c_adaptive_threshold2 - c_adaptive_threshold1) * (sliderValue / 50f));
            }
            else
            {
                sliderValue = sliderValue - 50;         
                KSizeSobel = (int)Math.Round(KSizeSobel2 + (KSizeSobel3 - KSizeSobel2) * (sliderValue / 50f));
                scaleSobel = (ScaleSobel2 + (ScaleSobel3 - ScaleSobel2) * (sliderValue / 50f));
                blockSizeAdaptive = (int)Math.Round(BlockSizeAdaptive2 + (BlockSizeAdaptive3 - BlockSizeAdaptive2) * (sliderValue / 50f));
                c_adaptiveThreshold = (c_adaptive_threshold2 + (c_adaptive_threshold3 - c_adaptive_threshold2) * (sliderValue / 50f));
            }
        }


        public void ValueChangeCheck()
        {
            KSizeSobel = int.Parse(inputField.text);
            Debug.LogFormat("Value Changed {0}", KSizeSobel);
        }

        public void ValueChangeCheck1()
        {
            scaleSobel = double.Parse(inputField1.text);
        }

        public void ValueChangeCheck2()
        {
            blockSizeAdaptive = int.Parse(inputField2.text);
        }

        public void ValueChangeCheck3()
        {
            c_adaptiveThreshold = double.Parse(inputField3.text);
        }

        void LockInput(InputField input)
        {
            if (input.text.Length > 0)
            {
                Debug.Log("Text has been entered");
            }
            else if (input.text.Length == 0)
            {
                Debug.Log("Main Input Empty");
            }
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

        public Mat sobelEdgeDetect2(Mat inputMat)
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
