using BansheeGz.BGSpline.Curve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLeanTween : MonoBehaviour {

    public GameObject image;

    private void Awake()
    {
        var bgcurve = this.gameObject.GetComponent<BGCurve>();
        var lineRenderer = this.gameObject.GetComponent<LineRenderer>();


        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        LeanTween.moveSpline(image.gameObject, positions, 10);

        Debug.Log(bgcurve.PointsCount);
        Debug.Log(lineRenderer.positionCount);
    }
}
