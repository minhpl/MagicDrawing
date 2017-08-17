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
    static public string filenameWithoutExt = null;
    public string filePath = null;
    public WebcamVideoCapture(Size size, bool createNewVideo = true)
    {
        if(createNewVideo)
        {            
            filenameWithoutExt = String.Format("{0}", DateTime.Now.ToString(Utilities.customFmts));
            filename = filenameWithoutExt + ".avi";

            filePath = GFs.getMasterpieceDirPath() + filename;
            if (!Directory.Exists(GFs.getMasterpieceDirPath()))
            {
                Directory.CreateDirectory(GFs.getMasterpieceDirPath());
            }
            
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
        }
        codec = VideoWriter.fourcc('M', 'J', 'P', 'G');
        fps = 60;
        writer = new VideoWriter(filePath, codec, fps, size);
    }
    public void write(Mat img)
    {
        writer.write(img);
    }
}
