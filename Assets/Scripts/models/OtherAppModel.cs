using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class OtherAppModel
{
    public string _id;
    public string packageName;
    public string name;
    public string image;
    public string appStore;
    public string googlePlayStore;
    public int _v;
}

[Serializable]
public class OtherAppListModel
{
    public List<OtherAppModel> data = new List<OtherAppModel>();
    public string dir;
    public int Count()
    {
        if (data == null) return 0;
        return data.Count;
    }

    public OtherAppModel Get(int i)
    {
        if (data == null) return null;
        if (i >= 0 && i < data.Count) return data[i];
        return null;
    }

    public OtherAppModel Get(string _id)
    {
        if (data == null) return null;
        foreach (OtherAppModel item in data)
        {
            if (item._id.Equals(_id)) return item;
        }
        return null;
    }

    public void Clean()
    {
        List<OtherAppModel> data2 = new List<OtherAppModel>();
        foreach (OtherAppModel item in data)
        {
            if (item._v < 0) continue;
            if (item.packageName.Equals(Application.identifier)) continue;
            data2.Add(item);
        }
        data = data2;

    }

    public OtherAppModel GetMyApp()
    {
        if (data == null) return null;
        foreach (OtherAppModel item in data)
        {
            if (item.packageName.Equals(Application.identifier)) return item;
        }
        return null;
    }
}
