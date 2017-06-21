using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetector : MonoBehaviour {
    Mat src, src_gray;
    Mat dst;
    public Mat detected_edges;
    public int kernel_blur_size = 3;
    const int MAX_KERNEL_BLUR_SIZE = 100;

    public int lowThreashold;
    const int MAX_LOW_THRESHOLD = 100;

    public int kernel_size = 3;
    const int MAX_KERNEL_SIZE = 100;

    Size blurSize;
    int ratio = 3;
   
    public UISlider slider_kernel_blur;
    public UISlider slider_kernel;
    public UISlider slider_lowThreshold;

    private void Start()
    {
        kernel_blur_size = 3;
        lowThreashold = 16;
        kernel_size = 3;

        slider_kernel_blur.value = (float)kernel_blur_size / (float)MAX_KERNEL_BLUR_SIZE;
        slider_kernel.value = (float)kernel_size / (float)MAX_KERNEL_SIZE;
        slider_lowThreshold.value = (float)lowThreashold / (float)MAX_LOW_THRESHOLD;

        src = new Mat();
        src_gray = new Mat();
        dst = new Mat();
        detected_edges = new Mat();
        blurSize = new Size(kernel_blur_size, kernel_blur_size);

        EventDelegate.Add(slider_lowThreshold.onChange, slider_lowThreshold_onChange);
        EventDelegate.Add(slider_kernel.onChange, slider_kernel_onChange);
        EventDelegate.Add(slider_kernel_blur.onChange, slider_kernel_blur_onChange);
    }
    
    void slider_kernel_blur_onChange()
    {
        kernel_blur_size = (int)(slider_kernel_blur.value * MAX_KERNEL_BLUR_SIZE + 0.5);
    }

    void slider_kernel_onChange()
    {
        kernel_size = (int)(slider_kernel.value * MAX_KERNEL_SIZE + 0.5);
    }

    void slider_lowThreshold_onChange()
    {
        lowThreashold = (int)(slider_lowThreshold.value * MAX_LOW_THRESHOLD + 0.5);
    }

    public void Dispose()
    {
        src.Dispose();
        src_gray.Dispose();
        dst.Dispose();
        detected_edges.Dispose();
    }

    void cannyThreshold()
    {
        src_gray.copyTo(detected_edges);
        Imgproc.blur(src_gray, detected_edges, blurSize);
        Imgproc.Canny(detected_edges, detected_edges, lowThreashold, lowThreashold * ratio, kernel_size, true);
        //Debug.LogFormat("Xin chao, so channel la {0}", detected_edges.channels());      
        Core.bitwise_not(detected_edges, detected_edges);
    }

    public Mat Canny(Mat m)
    {
        Imgproc.cvtColor(m, src_gray, Imgproc.COLOR_BGR2GRAY);
        cannyThreshold();
        return detected_edges;
    }

    Mat sobelDetectedEdge;
    Mat tempMat;
    Size gausionBlurSize;
    Mat grad_x;
    Mat grad_y;

    public Mat SobelEdgeDetector(Mat inputMat)
    {
        Imgproc.GaussianBlur(inputMat, tempMat,gausionBlurSize, 0, 0, Core.BORDER_DEFAULT);
        Imgproc.cvtColor(tempMat, tempMat, Imgproc.COLOR_BGR2GRAY);

        //Imgproc.Sobel(tempMat,grad_x)


        return null;
    }


}
