using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class OpenCVFaceDetection : MonoBehaviour
{
    public static List<Vector2> NormalizedFacePositions { get; private set; }
    public static Vector2 CameraResolution;

    // Downscale factor to spped up detection.

    private const int DetectionDownScale = 1;

    private bool _ready;
    private int _maxFaceDetectCount = 5;
    private CvsCircle[] _faces;

    void Start()
    {
        int camWidth = 0, camHeight = 0;
        int result = OpenCVInterop.Init(ref camWidth, ref camHeight);
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
        _faces = new CvsCircle[_maxFaceDetectCount];
        for(int i=0;i< _maxFaceDetectCount; i++)
        {
            _faces[i].X = (i+1) * 10;
            _faces[i].Y = (i + 1) * 10;
            _faces[i].Radius = (i + 1) * 10;
        }

        NormalizedFacePositions = new List<Vector2>();
        OpenCVInterop.SetScale(DetectionDownScale);
        _ready = true;
    }


    void OnApplicationQuit()
    {
        if(_ready)
        {
            OpenCVInterop.Close();
        }
    }

    void Update()
    {
        if(!_ready)
            return;
        int detectedFaceCount = 0;
        //unsafe
        {
            //fixed(CvsCircle* outFaces = _faces)
            {

                var circlesHandle = GCHandle.Alloc(_faces, GCHandleType.Pinned);
                var ptr = circlesHandle.AddrOfPinnedObject();

                OpenCVInterop.Detect(ptr, _maxFaceDetectCount, ref detectedFaceCount);                
                //OpenCVInterop.Detect(outFaces, _maxFaceDetectCount, ref detectedFaceCount);
            }
        }

        Debug.Log(detectedFaceCount);

        return;
        NormalizedFacePositions.Clear();
        for (int i = 0; i < detectedFaceCount; i++)
        {
            NormalizedFacePositions.Add(new Vector2((_faces[i].X * DetectionDownScale) / CameraResolution.x,
                1f - ((_faces[i].Y * DetectionDownScale) / CameraResolution.y)));
        }
    }
}


// Define the functions which can be called from the .dll
internal static class OpenCVInterop
{
    [DllImport("UnityOpenCVSample")]
    internal static extern int Init(ref int outCameraWidth, ref int outCameraHeight);

    [DllImport("UnityOpenCVSample")]
    internal static extern int Close();

    [DllImport("UnityOpenCVSample")]
    internal static extern int SetScale(int downscale);

    [DllImport("UnityOpenCVSample")]
    internal unsafe static extern void Detect(IntPtr outFaces, int maxOutFacesCount, ref int outDetectedFacesCount);
}

// Define the structure to be sequential and width the correct byte size (3 ints = 4bytes*3 = 12 bytes)
//[StructLayout(LayoutKind.Sequential, Size = 12)]
public struct CvsCircle
{
    public int X, Y, Radius;
}