using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AvataListModel
{
    public int success;
    public AvataModel[] avatas;
    public string dir;
    public bool downloaded = false;
    public bool needUpdate = false;

    public AvataListModel()
    {
        avatas = new AvataModel[0];
    }

    public AvataModel Get(int i)
    {
        return avatas[i];
    }

    public int Count()
    {
        return avatas.Length;
    }
    public void Update(AvataModel[] avs)
    {
        for (int i = 0; i < avs.Length; i++)
        {
            bool find = false;
            for (int j = 0; j < avatas.Length; j++)
            {
                if (avs[i]._id.Equals(avatas[j]._id))
                {
                    if (downloaded)
                    {
                        if (avs[i]._v != avatas[j]._v)
                        {
                            needUpdate = true;
                        }
                    }
                    avatas[j] = avs[i];
                    find = true;
                }
            }
            if (find == false)
            {
                avatas = avs;
                if (downloaded)
                {
                    needUpdate = true;
                }
                break;
            }
        }
    }

    public void Updated()
    {
        needUpdate = false;
    }
}

[Serializable]
public class AvataModel
{
    public string _id;
    public string image;
    public int _v;
}
