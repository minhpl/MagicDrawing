using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSs : MonoBehaviour
{
    private float updateInterval = 0.5f;
    private float accum = 0.0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft;

    // Use this for initialization
    void Start()
    {
        timeleft = updateInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            GetComponent<Text>().text = "FPS - " + (accum / frames).ToString("f2");
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}
