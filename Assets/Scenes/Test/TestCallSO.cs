using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class TestCallSO : MonoBehaviour
{
    public RawImage rimg;

    void Start()
    {
        Utilities.Log("hello {0}", "");
        try
        {
            Texture texture = rimg.mainTexture;
            int ptr= (int)texture.GetNativeTexturePtr();
            int width = texture.width;
            int height = texture.height;
            var result = ndktestop.test(ref width, ref height,ref ptr);
            Utilities.Log("Width is {0}, height is {1}, and the result is {0}", width, height, result);
        }
        catch (Exception e)
        {
            Utilities.Log("Error is {0}", e.ToString());
            Utilities.Log("Trace is {0}", e.StackTrace.ToString());
        }
    } 
}

internal static class ndktestop
{
    [DllImport("ndktest")]
    internal static extern int test(ref int outCameraWidth, ref int outCameraHeight, ref int z);
}