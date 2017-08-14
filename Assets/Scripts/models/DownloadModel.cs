using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadModel
{
    //ảnh đại diện cho category
    public static int DOWNLOAD_CATEGORY_AVATA = 1;
    //ảnh trong mỗi category
    public static int DOWNLOAD_CATEGORY = 2;
    //tát cả ảnh avata
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
