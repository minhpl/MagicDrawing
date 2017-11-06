using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrailRenderer : MonoBehaviour {

    public float width = 1.0f;
    public bool useCurve = true;
    private TrailRenderer tr;

    void Start()
    {
        tr = GetComponent<TrailRenderer>();
        tr.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        AnimationCurve curve = new AnimationCurve();
        if (useCurve)
        {
            curve.AddKey(0.0f, 0.0f);
            curve.AddKey(1.0f, 1.0f);
        }
        else
        {
            curve.AddKey(0.0f, 1.0f);
            curve.AddKey(1.0f, 1.0f);
        }

        tr.widthCurve = curve;
        tr.widthMultiplier = width;
        tr.transform.position = new Vector3(Mathf.Sin(Time.time * 1.51f) * 7.0f, Mathf.Cos(Time.time * 1.27f) * 4.0f, 0.0f);
    }

    private bool isPaused = false;

    void OnGUI()
    {
        GUI.Label(new Rect(25, 20, 200, 30), "Width");
        width = GUI.HorizontalSlider(new Rect(125, 25, 200, 30), width, 0.1f, 1.0f);
        useCurve = GUI.Toggle(new Rect(25, 65, 200, 30), useCurve, "Use Curve");

        if (isPaused)
            GUI.Label(new Rect(100, 100, 50, 30), "Game paused");
    }

    void OnApplicationFocus(bool hasFocus)
    {
        isPaused = !hasFocus;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
    }
}
