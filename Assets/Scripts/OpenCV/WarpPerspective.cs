using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class WarpPerspective : MonoBehaviour
{
    public Slider slider;

    Mat sourceImage;
    Mat src_mat;
    Mat dst_mat;
    Mat perspectiveTransform;
    public float widthScale = 1;
    public float heightScale = 1;
    public int heightRedundancy = 100;
    Size sizeOriginal;
    Size sizeNew;
    OpenCVForUnity.Rect myROI;

    void warpPerspectiveTunner(Mat image)
    {
        int w = image.cols();
        int h = image.rows();
        int topWidth = 400;
        int botWidth = 272;
        topWidth = (int)(topWidth / widthScale);
        int differWidth = topWidth - botWidth;
        int halfWidth = differWidth / 2;
        int needWidthInDe = (int)(((float)halfWidth / (float)topWidth) * w);
        int topWidthAfter = w - needWidthInDe * 2;
        src_mat.put(0, 0,
                0, 0,
                w, 0,
                w, h,
                0, h);
        dst_mat.put(0, 0,
                needWidthInDe, 0,
                w - needWidthInDe, 0,
                w, h,
                0, h);
        perspectiveTransform = Imgproc.getPerspectiveTransform(src_mat, dst_mat);
    }

    private void OnValidate()
    {
        //Debug.LogFormat("here1 = {0}", widthScale);
        //if (sourceImage != null)
        //    Init(sourceImage);
    }

    public void setParam(float heightScale, float widthScale = 1)
    {
        this.heightScale = heightScale;
        this.widthScale = widthScale;
    }

    public void Init(Mat image)
    {
        sourceImage = image;
        int w = image.cols();
        int h = image.rows();
        int topWidth = 400;
        int botWidth = 272;
        topWidth = (int)(topWidth / widthScale);
        int differWidth = topWidth - botWidth;
        int halfWidth = differWidth / 2;
        int needWidthInDe = (int)(((float)halfWidth / (float)topWidth) * w);
        int topWidthAfter = w - needWidthInDe * 2;
        int newHeight = (int)(h / heightScale);
        src_mat.put(0, 0,
                0, 0,
                w, 0,
                w, h,
                0, h);
        dst_mat.put(0, 0,
                needWidthInDe, 0,
                w - needWidthInDe, 0,
                w, h,
                0, h);
        perspectiveTransform = Imgproc.getPerspectiveTransform(src_mat, dst_mat);
        sizeOriginal = image.size();
        sizeNew = new Size(sizeOriginal.width, newHeight);
        myROI = new OpenCVForUnity.Rect((int)needWidthInDe, heightRedundancy, (int)(topWidthAfter), newHeight - heightRedundancy);
    }

    Mat tempMat;
    Mat tempMat2;
    Mat result;

    private void Awake()
    {
        src_mat = new Mat(4, 2, CvType.CV_32FC1);
        dst_mat = new Mat(4, 2, CvType.CV_32FC1);
        tempMat = new Mat();
        tempMat2 = new Mat();
        result = new Mat();
    }

    private void Start()
    {
        
    }

    public Mat warpPerspective(Mat inputMat)
    {
        if (heightScale == 1)
        {
            Imgproc.warpPerspective(inputMat, tempMat, perspectiveTransform, sizeOriginal);
            Mat roi = tempMat.submat(myROI);
            Imgproc.resize(roi, result, sizeOriginal);
        }
        else
        {
            Imgproc.warpPerspective(inputMat, tempMat, perspectiveTransform, sizeOriginal);
            Imgproc.resize(tempMat, tempMat2, sizeNew);
            Mat roi = tempMat2.submat(myROI);
            Imgproc.resize(roi, result, sizeOriginal);
        }
        return result;
    }
}
