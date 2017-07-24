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
        //Core.bitwise_not(edges, tempMat);

        Mat redMat = new Mat();

        List<Mat> listMat = new List<Mat>();
        Mat zeroMat = Mat.zeros(edges.size(), CvType.CV_8U);
        listMat.Add(edges);
        listMat.Add(zeroMat);
        listMat.Add(zeroMat);
        listMat.Add(edges);
        Core.merge(listMat, redMat);        
        
        colorsBuffer = new Color32[edges.width() * edges.height()];

        handle(redMat);        
        //redMat.Dispose();
    }
}
