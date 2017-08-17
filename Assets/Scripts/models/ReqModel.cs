using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ReqModel
{
    public string license = GVs.LICENSE_CODE;
    public string deviceId = "" + SystemInfo.deviceUniqueIdentifier;
    public string deviceName = "" + SystemInfo.deviceName;
    public string deviceType = "" + SystemInfo.deviceType;
    public string deviceModel = "" + SystemInfo.deviceModel;
    public string data = "";

    public ReqModel() { }

    public ReqModel(string license, int con)
    {
        this.license = license;
    }

    public ReqModel(object data)
    {
        this.data = JsonUtility.ToJson(data);
    }

    public ReqModel(string license, object data)
    {
        this.license = license;
        this.data = JsonUtility.ToJson(data);
    }
}