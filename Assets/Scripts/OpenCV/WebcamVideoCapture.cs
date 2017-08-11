using OpenCVForUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

class WebcamVideoCapture
{
    private int codec;
    private double fps;
    public VideoWriter writer;        
    static public string filename = null;

    public WebcamVideoCapture(Size size, bool createNewVideo = true)
    {
        if(createNewVideo)
        {
            string name = String.Format("video_{0}.avi", DateTime.Now.ToString(Utilities.customFmts));
            //#if UNITY_IPHONE
            //                    Debug.Log("file name is 3" + filename);
            //                                filename = iphoneDir + name;
            //#endif
            //#if UNITY_ANDROID
            //filename = androidDir + name;
            //        if (!Directory.Exists(androidDir))
            //        {
            //            Directory.CreateDirectory(androidDir);
            //        }
            //        Utilities.Log("filename is 2{0}", "hello");
            //#else
            //                //PC here                
            //                filename = pcDir + name;
            //#endif            
            if (Application.platform == RuntimePlatform.Android)
            {
                filename = GVs.androidDir + name;
                if (!Directory.Exists(GVs.androidDir))
                {
                    Directory.CreateDirectory(GVs.androidDir);
                }
                Utilities.Log("filename is {0}", filename);
            }
            else
            {
                filename = GVs.pcDir + name;
                if (!Directory.Exists(GVs.pcDir))
                {
                    Directory.CreateDirectory(GVs.pcDir);
                }
                Utilities.Log("filename is {0}", filename);
            }
        }        
        codec = VideoWriter.fourcc('M', 'J', 'P', 'G');
        fps = 60;
        writer = new VideoWriter(filename, codec, fps, size);
    }
    public void write(Mat img)
    {
        writer.write(img);
    }
}
