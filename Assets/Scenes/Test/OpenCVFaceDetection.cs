using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class OpenCVFaceDetection : MonoBehaviour
{
    public static List<Vector2> NormalizedFacePositions { get; private set; }
    public static Vector2 CameraResolution;

    /// <summary>
    /// Downscale factor to speed up detection.
    /// </summary>
    private const int DetectionDownScale = 1;

    private bool _ready;
    private int _maxFaceDetectCount = 5;
    private CvCircle[] _faces;

    void Start()
    {
        try
        {
            int camWidth = 0, camHeight = 0;
            string currentDirectory = "";
            int result = OpenCVInterop.Init(ref camWidth, ref camHeight);

            Utilities.Log("Camera width is {0}", camWidth);
            if (result < 0)
            {
                if (result == -1)
                {
                    Debug.LogWarningFormat("[{0}] Failed to find cascades definition.", GetType());
                }
                else if (result == -2)
                {
                    Debug.LogWarningFormat("[{0}] Failed to open camera stream.", GetType());
                }

                return;
            }

            CameraResolution = new Vector2(camWidth, camHeight);
            _faces = new CvCircle[_maxFaceDetectCount];
            NormalizedFacePositions = new List<Vector2>();
            OpenCVInterop.SetScale(DetectionDownScale);
            _ready = true;
        }
        catch (Exception e)
        {
            Utilities.Log("Error: {0}", e.ToString());
            Utilities.Log("Trace: {0}", e.ToString());
        }
        
    }

    void OnApplicationQuit()
    {
        if (_ready)
        {
            OpenCVInterop.Close();
        }
    }

    void Update()
    {
        if (!_ready)
            return;

        int detectedFaceCount = 0;
        unsafe
        {
            fixed (CvCircle* outFaces = _faces)
            {
                OpenCVInterop.Detect(outFaces, _maxFaceDetectCount, ref detectedFaceCount);
            }
        }

        //NormalizedFacePositions.Clear();
        //for (int i = 0; i < detectedFaceCount; i++)
        //{
        //    NormalizedFacePositions.Add(new Vector2((_faces[i].X * DetectionDownScale) / CameraResolution.x, 1f - ((_faces[i].Y * DetectionDownScale) / CameraResolution.y)));
        //}
    }
}



// Define the functions which can be called from the .dll.
internal static class OpenCVInterop
{
    [DllImport("UnityOpenCVSample")]
    internal static extern int Init(ref int outCameraWidth, ref int outCameraHeight);

    [DllImport("UnityOpenCVSample")]
    internal static extern int Close();

    [DllImport("UnityOpenCVSample")]
    internal static extern int SetScale(int downscale);

    [DllImport("UnityOpenCVSample")]
    internal unsafe static extern void Detect(CvCircle* outFaces, int maxOutFacesCount, ref int outDetectedFacesCount);
}

[StructLayout(LayoutKind.Sequential, Size = 12)]
public struct CvCircle
{
    public int X, Y, Radius;
}