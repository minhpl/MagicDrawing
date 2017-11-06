using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{

    public int speed = 50;
    GameObject go;
    int w = 0;
    void Start()
    {
        go = gameObject;
        w = go.GetComponent<UITexture>().width;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v3 = go.transform.localPosition;
        v3.x += Time.deltaTime * (speed / 2);
        if (v3.x > 1080 / 2 + w / 2)
        {
            v3.x = -1080 / 2 - w / 2;
        }
        go.transform.localPosition = v3;

    }
}
