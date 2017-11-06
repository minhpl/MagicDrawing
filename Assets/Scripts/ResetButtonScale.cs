using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButtonScale : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        UIButtonScale ui = GetComponent<UIButtonScale>();
        if (ui == null) return;
        ui.hover = Vector3.one * 0.95f;
        ui.pressed = Vector3.one * 0.95f;
        ui.duration = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
