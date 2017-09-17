using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHidePanel : MonoBehaviour
{

    public int depth = 0;
    void Start()
    {
        TweenAlpha.Begin(gameObject, 0, 0);
        StartCoroutine(ChangeDepth());
    }
    IEnumerator ChangeDepth()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1);
        UIPanel panel = GetComponent<UIPanel>();
        if (panel != null)
        {
            panel.depth = depth;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
