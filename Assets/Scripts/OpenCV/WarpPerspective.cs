using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class WarpPerspective : MonoBehaviour
{
    Mat sourceImage;
    Mat transformationMatrix;
    public float widthScale = 1;
    public float heightScale = 1;
    public int heightRedundancy = 100;
    Size sizeOriginal;
    Size sizeNew;
    OpenCVForUnity.Rect myROI;
    private  Mat src_corner;
    private Mat dst_corner;
    private Mat srcTM;
    private Mat map_x;
    private Mat map_y;
    Mat transformation_x;
    Mat transformation_y;

    Mat tempMat;
    Mat tempMat2; 
    Mat result;

    private void Awake()
    {
        tempMat = new Mat();
        tempMat2 = new Mat();
        result = new Mat();        
    }

    public void setParam(float heightScale, float widthScale = 1)
    {
        this.heightScale = heightScale;
        this.widthScale = widthScale;
    }

    public float scaleX = 0;
    public void Init(Mat image)
    {
        sourceImage = image;
        src_corner = new Mat(4, 2, CvType.CV_32FC1);
        dst_corner = new Mat(4, 2, CvType.CV_32FC1);

        map_x = new Mat();
        map_y = new Mat();
        srcTM = new Mat();
        map_x.create(sourceImage.size(), CvType.CV_32FC1);
        map_y.create(sourceImage.size(), CvType.CV_32FC1);
        transformation_x = new Mat(sourceImage.size(), CvType.CV_16SC2);
        transformation_y = new Mat(sourceImage.size(), CvType.CV_16UC1);
        PreCalculateTransformMatrix();
    }

    public void PreCalculateTransformMatrix()
    {
        int w = sourceImage.width();
        int h = sourceImage.height();
        int topWidth = 800;
        int botWidth = 300;
        topWidth = (int)(topWidth / widthScale);
        int differWidth = topWidth - botWidth;
        int halfWidth = differWidth / 2;
        int needWidthInDe = (int)(((float)halfWidth / (float)topWidth) * w);
        int topWidthAfter = w - needWidthInDe * 2;
        int newHeight = (int)(h / heightScale);

        float delta = 0.41f;         //tham số bóp đầu lại, càng to càng bóp
        float deltaHT = 0.08f;
        float deltaHB = 0.35f;

        const float SCREEN_RATIO1 = 0.75f;   // SAMSUNG
        const float SCREEN_RATIO2 = 0.658f;  // HUAWEI
        const float SCREEN_RATIO3 = 0.5625;  //VIVO 

        Utilities.Log("device model is {0}, device name is {1}, device type is {2}", SystemInfo.deviceModel, SystemInfo.deviceName, SystemInfo.deviceType);
        float uniformScale = 1.1f;
        Utilities.Log("device width is {0}, device height is {1}", Screen.width, Screen.height);

        float scaleW = 0.325f*uniformScale;  //scale width param
        float scaleH = 0.41f* uniformScale;  //scale height param

        float addW = w * scaleW;   
        float addH = h * scaleH;
        float topH = 0.14f;        //


        src_corner.put(0, 0,
                0, 0,
                w, 0,
                w, h,
                0, h);
        //dst_corner.put(0, 0,
        //        needWidthInDe-addW, 0-topH*addH,
        //        w - needWidthInDe+addW, 0-topH*addH,
        //        w+addW, h+(2-topH)*addH,
        //        0-addW, h+(2-topH)*addH);

        dst_corner.put(0, 0,
            w * delta - addW, -h * deltaHT - topH * addH,
            w - w * delta + addW, -h * deltaHT - topH * addH,
            w + w * delta + addW, h + h * deltaHB + (2 - topH) * addH,
            -w * delta - addW, h + h * deltaHB + (2 - topH) * addH);


        transformationMatrix = Imgproc.getPerspectiveTransform(src_corner, dst_corner);
        Core.invert(transformationMatrix, srcTM);

        double M11, M12, M13, M21, M22, M23, M31, M32, M33;
        M11 = srcTM.get(0, 0)[0];
        M12 = srcTM.get(0, 1)[0];
        M13 = srcTM.get(0, 2)[0];
        M21 = srcTM.get(1, 0)[0];
        M22 = srcTM.get(1, 1)[0];
        M23 = srcTM.get(1, 2)[0];
        M31 = srcTM.get(2, 0)[0];
        M32 = srcTM.get(2, 1)[0];
        M33 = srcTM.get(2, 2)[0];

        srcTM.Dispose();

        for (int y = 0; y < sourceImage.rows(); y++)
        {
            double fy = (double)y;
            for (int x = 0; x < sourceImage.cols(); x++)
            {
                double fx = (double)x;
                double _w = ((M31 * fx) + (M32 * fy) + M33);
                _w = _w != 0.0f ? 1.0f / _w : 0.0f;
                float new_x = (float)(((M11 * fx) + (M12 * fy) + M13) * _w);
                float new_y = (float)(((M21 * fx) + (M22 * fy) + M23) * _w);
                map_x.put(y, x,new_x);
                map_y.put(y, x, new_y);
            }
        }
        // This creates a fixed-point representation of the mapping resulting in ~4% CPU savings
        

        Imgproc.convertMaps(map_x, map_y, transformation_x, transformation_y, CvType.CV_16SC2, false);

        //map_x.Dispose();
        //map_y.Dispose();

        //Debug.LogFormat("Perspective Transform is {0}", perspectiveTransform.size().ToString());

        sizeOriginal = sourceImage.size();
        sizeNew = new Size(sizeOriginal.width, newHeight);
        myROI = new OpenCVForUnity.Rect((int)needWidthInDe, heightRedundancy, (int)(topWidthAfter), newHeight - heightRedundancy);

        scaleX = (float)w / topWidthAfter;
    }

    public Mat warpPerspective(Mat inputMat)
    {
        Imgproc.remap(inputMat, tempMat, transformation_x, transformation_y, Imgproc.INTER_LINEAR);
        if (heightScale == 1)
        {
            //Imgproc.remap(inputMat, tempMat, transformation_x, transformation_y, Imgproc.INTER_LINEAR);   
            //Imgproc.warpPerspective(inputMat, tempMat, transformationMatrix, sizeOriginal,Imgproc.INTER_NEAREST);
            //Mat roi = tempMat.submat(myROI);
            //Imgproc.resize(roi, result, sizeOriginal);
        }
        else
        {
            //Imgproc.remap(inputMat, tempMat, transformation_x, transformation_y, Imgproc.INTER_LINEAR);
            //Imgproc.warpPerspective(inputMat, tempMat, transformationMatrix, sizeOriginal, Imgproc.INTER_NEAREST);
            //Imgproc.resize(tempMat, tempMat2, sizeNew);
            //Mat roi = tempMat2.submat(myROI);
            //Imgproc.resize(roi, result, sizeOriginal);
        }
        return tempMat;
    }
}
