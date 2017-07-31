using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestCallSO : MonoBehaviour
{

    void Start()
    {
        try
        {
            int width = 1;
            int height = 2;

            var result = ndktestop.SayHello(ref width, ref height);

            Utilities.Log("Width is {0}, height is {1}, and the result is {0}", width, height, result);
        }
        catch (Exception e)
        {
            Utilities.Log("Error is {0}", e.ToString());
            Utilities.Log("Trace is {0}", e.StackTrace.ToString());
        }
    }


    internal static class ndktestop
    {
        [DllImport("ndktest")]
        internal static extern int SayHello(ref int outCameraWidth, ref int outCameraHeight);
    }
}