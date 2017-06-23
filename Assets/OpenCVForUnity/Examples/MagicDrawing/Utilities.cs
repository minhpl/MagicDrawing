using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenCVForUnity.Examples.MagicDrawing
{
    class Utilities
    {
        public void ConvertGrayMatToRedMat(Mat inputMat,Mat outputMat)
        {
            int channels = inputMat.channels();
            if (channels > 1)
                Debug.LogFormat("This is not the grayImage");
            List<Mat> listMat = new List<Mat>();
            Core.split(outputMat, listMat);
            Debug.LogFormat("{0}",listMat[0] == null);
            Debug.LogFormat("output mat channels = {0}", outputMat.channels());
            inputMat.copyTo(outputMat);
            Debug.LogFormat("output mat channels = {0}", outputMat.channels());
            //Debug.LogFormat("number of mat is {0}", listMat.Count());
        }



    }
}
