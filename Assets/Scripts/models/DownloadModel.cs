using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DownloadModel
{
    public static int DOWNLOAD_CATEGORY_AVATA = 1;
    public static int DOWNLOAD_CATEGORY = 2;
    public static int DOWNLOAD_AVATAS = 3;
    public static int DOWNLOAD_OTHER_APP_ICON = 10;

    public int downloadType;
    public string _id;
    
    public string filename;
    public string dir;
    public bool compress;
    public DownloadHistory dh;
    public DownloadModel(int downloadType = 0)
    {
        dh = GVs.DOWNLOAD_HISTORY_STORE.GetDownloadHistory(downloadType);
        this.downloadType = downloadType;
    }
    public DownloadModel(int downloadType, string _id)
    {
        dh = GVs.DOWNLOAD_HISTORY_STORE.GetDownloadHistory(downloadType, _id);
        this.downloadType = downloadType;
        this._id = _id;
    }
}

[Serializable]
public class DownloadHistory
{
    public int downloadType;
    public string _id;
    public long mTime;
    public DownloadHistory(int _downloadType = 0, string id = "", long _mTime = 0)
    {
        this.downloadType = _downloadType;
        this._id = id;
        this.mTime = _mTime;
    }
}

[Serializable]
public class DownloadHistoryStore
{
    public List<DownloadHistory> data = new List<DownloadHistory>();
    public DownloadHistory GetDownloadHistory(int _downloadType, string id = "")
    {
        if (data == null) return null;
        foreach (DownloadHistory item in data)
        {
            if (item.downloadType == _downloadType && item._id.Equals(id)) return item;
        }
        return null;
    }

    public void Update(DownloadHistory dh)
    {
        if (GVs.DEBUG) Debug.Log(JsonUtility.ToJson(this));
        if (dh == null) return;
        if (dh.downloadType == 0 || dh.mTime == 0) return;
        if (data == null) data = new List<DownloadHistory>();
        bool find = false;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].downloadType == dh.downloadType && data[i]._id.Equals(dh._id))
            {
                find = true;
                data[i] = dh;
            }
        }
        if (!find) data.Add(dh);
        GFs.SaveDownloadHistoryStore();
    }
}