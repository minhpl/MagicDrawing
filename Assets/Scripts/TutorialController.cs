using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public GameObject[] goTutorials;
    int step = 0;
    public GameObject btnBack;
    public GameObject btnNext;
    void Start()
    {

    }

    public void BackOnClick()
    {
        if (step > 0)
        {
            goTutorials[step].SetActive(false);
            goTutorials[--step].SetActive(true);
        }
        if (step == 0)
        {
            btnBack.SetActive(false);
        }
    }

    public void NextOnClick()
    {
        btnBack.SetActive(true);
        if (step + 1 < goTutorials.Length)
        {
            goTutorials[step].SetActive(false);
            goTutorials[++step].SetActive(true);
        }
        else
        {
            GVs.SCENE_MANAGER.StartPreloadScene();
        }
    }

    public void SkipOnClick()
    {
        GVs.SCENE_MANAGER.StartPreloadScene();
    }
}
