using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript;

public class TestTourchScripts : MonoBehaviour {

    private void OnEnable()
    {
        if (TouchManager.Instance != null)
            TouchManager.Instance.PointersPressed += pointersPressedHandler;
    }

    private void OnDisable()
    {
        if (TouchManager.Instance != null)
            TouchManager.Instance.PointersPressed -= pointersPressedHandler;
    }

    private void pointersPressedHandler(object sender, PointerEventArgs e)
    {
        foreach (var pointer in e.Pointers)
            Debug.Log(pointer.Id + " touched down at " + pointer.Position);
    }

}
