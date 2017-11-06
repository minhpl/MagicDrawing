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
    public const int FPS = 24;
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
        }
        codec = VideoWriter.fourcc('M', 'J', 'P', 'G');
        Debug.LogFormat("file path is {0}",filePath);
        writer = new VideoWriter(filePath, codec, FPS, size);
        Debug.LogFormat("is writer open ? {0}", writer.isOpened());
    }
    public void write(Mat img)
    {
        writer.write(img);
    }
}
