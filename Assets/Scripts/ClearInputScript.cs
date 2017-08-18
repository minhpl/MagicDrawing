using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearInputScript : MonoBehaviour
{
    public UIInput input;
    public void ClearOnClick()
    {
        input.value = "";
    }
}
