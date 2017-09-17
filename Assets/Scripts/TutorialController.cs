using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{    
    public void OnSkipBtnClicked()
    {
        GVs.SCENE_MANAGER.loadPreloadScene();
    }
}
