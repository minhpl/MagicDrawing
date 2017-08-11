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
    private string androidDir = "/storage/emulated/0/DCIM/MagicDrawing/newfolder/";
    private string pcDir = "E:\\WorkspaceMinh\\MagicDrawing\\x64\\Release\\";
    private string iphoneDir = "\\";
    private string customFmts = "yyyyMMd_HHmmss";
    static public string filename = "";

    public WebcamVideoCapture(Size size, bool createNewVideo = true)
    {
        if(createNewVideo)
        {
            string name = String.Format("video_{0}.avi", DateTime.Now.ToString(customFmts));
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
            Utilities.Log("xin chao noi day {0}", name);
            if (Application.platform == RuntimePlatform.Android)
            {
                filename = androidDir + name;
                if (!Directory.Exists(androidDir))
                {
                    Directory.CreateDirectory(androidDir);
                }
                Utilities.Log("filename is {0}", filename);
            }
            else
            {
                filename = pcDir + name;
                if (!Directory.Exists(pcDir))
                {
                    Directory.CreateDirectory(pcDir);
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
