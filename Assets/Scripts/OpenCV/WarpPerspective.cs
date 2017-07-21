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
    Mat transformationMatrix;
    public float widthScale = 1;
    public float heightScale = 1;
    public int heightRedundancy = 100;
    Size sizeOriginal;
    Size sizeNew;
    OpenCVForUnity.Rect myROI;

    Mat transformation_x;
    Mat transformation_y;

    public void setParam(float heightScale, float widthScale = 1)
    {
        this.heightScale = heightScale;
        this.widthScale = widthScale;
    }


    public float scaleX = 0;
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

        Mat src_corner = new Mat(4, 2, CvType.CV_32FC1);
        Mat dst_corner = new Mat(4, 2, CvType.CV_32FC1);

        src_corner.put(0, 0,
                0, 0,
                w, 0,
                w, h,
                0, h);
        dst_corner.put(0, 0,
                needWidthInDe, 0,
                w - needWidthInDe, 0,
                w, h,
                0, h);
        transformationMatrix = Imgproc.getPerspectiveTransform(src_corner, dst_corner);
        //src_corner.Dispose();
        //dst_corner.Dispose();
       
        Mat map_x = new Mat(), map_y = new Mat(), srcTM = new Mat();
        Core.invert(transformationMatrix, srcTM);
        
        map_x.create(sourceImage.size(), CvType.CV_32FC1);
        map_y.create(sourceImage.size(), CvType.CV_32FC1);

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

        transformation_x = new Mat(sourceImage.size(),CvType.CV_16SC2);
        transformation_y = new Mat(sourceImage.size(), CvType.CV_16UC1);

        Imgproc.convertMaps(map_x, map_y, transformation_x, transformation_y, CvType.CV_16SC2, true);

        //map_x.Dispose();
        //map_y.Dispose();

        //Debug.LogFormat("Perspective Transform is {0}", perspectiveTransform.size().ToString());

        sizeOriginal = sourceImage.size();
        sizeNew = new Size(sizeOriginal.width, newHeight);
        myROI = new OpenCVForUnity.Rect((int)needWidthInDe, heightRedundancy, (int)(topWidthAfter), newHeight - heightRedundancy);

        scaleX = (float)w / topWidthAfter;

    }

    Mat tempMat;
    Mat tempMat2;
    Mat result;

    private void Awake()
    {
        tempMat = new Mat();
        tempMat2 = new Mat();
        result = new Mat();
    }

    public Mat warpPerspective(Mat inputMat)
    {
        if (heightScale == 1)
        {
            Imgproc.remap(inputMat, tempMat, transformation_x, transformation_y, Imgproc.INTER_NEAREST);


            //Imgproc.warpPerspective(inputMat, tempMat, transformationMatrix, sizeOriginal,Imgproc.INTER_NEAREST);
            //Mat roi = tempMat.submat(myROI);
            //Imgproc.resize(roi, result, sizeOriginal);
        }
        else
        {
            Imgproc.remap(inputMat, tempMat, transformation_x, transformation_y, Imgproc.INTER_LINEAR);
            //Imgproc.warpPerspective(inputMat, tempMat, transformationMatrix, sizeOriginal, Imgproc.INTER_NEAREST);
            //Imgproc.resize(tempMat, tempMat2, sizeNew);
            //Mat roi = tempMat2.submat(myROI);
            //Imgproc.resize(roi, result, sizeOriginal);
        }
        return tempMat;
    }

    private void OnValidate()
    {
        
    }

}
