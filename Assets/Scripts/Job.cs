using System.IO;
using System;
using UnityEngine;
using OpenCVForUnity;
using System.Collections;
using System.Collections.Generic;

using OpenCVForUnityExample;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Job : ThreadedJob
{

    public Mat image;
    public AdaptiveThreshold athreshold;
    public Utilities utilities;
    public RawImage rimgmodel;
    public Texture2D texEdges;

    private Color32[] colorsBuffer;


    protected override void ThreadFunction(Handler handle)
    {

        Mat edges = athreshold.adapTiveThreshold(image);

        Mat tempMat = new Mat();
        Core.bitwise_not(edges, tempMat);

        Mat redMat = new Mat();

        List<Mat> listMat = new List<Mat>();
        Mat zeroMat = Mat.zeros(tempMat.size(), CvType.CV_8U);
        listMat.Add(tempMat);
        listMat.Add(zeroMat);
        listMat.Add(zeroMat);
        listMat.Add(tempMat);
        Core.merge(listMat, redMat);

        utilities.set(edges);
        colorsBuffer = new Color32[edges.width() * edges.height()];

        handle(redMat);
        
        //redMat.Dispose();
    }
}
