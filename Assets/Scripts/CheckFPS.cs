using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UniRx;

public class CheckFPS : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        // Observable.EveryUpdate().Subscribe(_ =>
        // {
        //     Debug.Log("Every update: " + (int)(Time.deltaTime * 1000));
        // });


        // Observable.EveryFixedUpdate().Subscribe(_ =>
        // {
        //     Debug.Log("EveryFixedUpdate: " + (int)(Time.deltaTime * 1000));
        // });
    }
    long time = DateTime.Now.Millisecond;
    // Update is called once per frame

    void FixedUpdate()
    {
        // Debug.Log("FixedUpdate: " + (int)(Time.deltaTime * 1000));
        // Debug.Log(DateTime.Now.Millisecond - time);
        // time = DateTime.Now.Millisecond;
    }

    void Update()
    {
        Debug.Log("Update: " + (int)(Time.deltaTime * 1000));
        // Debug.Log(DateTime.Now.Millisecond - time);
        // time = DateTime.Now.Millisecond;
    }
}
