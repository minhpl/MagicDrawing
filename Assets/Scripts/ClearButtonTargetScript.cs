using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButtonTargetScript : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        try
        {
            UIButton uiButton = gameObject.GetComponent<UIButton>();
            if (uiButton != null)
            {
                uiButton.tweenTarget = null;
            }
        }
        catch (System.Exception e)
        {
        }
    }

}
