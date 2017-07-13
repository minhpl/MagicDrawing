using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class NET
{
    public static bool NetWorkIsAvaiable()
    {
        return (Application.internetReachability != NetworkReachability.NotReachable);
    }
}
