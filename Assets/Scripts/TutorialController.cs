using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public static bool isAddedSoundButtonEvent
    {
        get;
        set;
    }
    private void Awake()
    {
        GFs.LoadData();
        GFs.addButtonSoundEventOnSceneLoad();
    }

    public void OnSkipBtnClicked()
    {
        GVs.SCENE_MANAGER.loadPreloadScene();
    }


}
