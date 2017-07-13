using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadModel
{
    public static int DOWNLOAD_TEMPLATE= 1;
    public static int DOWNLOAD_AVATAS = 3;

    public int downloadType;
    public string _id;

    public string filename;
    public string dir;
    public bool compress;
    public DownloadModel(int downloadType = 0)
    {
        this.downloadType = downloadType;
    }
    public DownloadModel(int downloadType, string _id)
    {
        this.downloadType = downloadType;
        this._id = _id;
    }
}
