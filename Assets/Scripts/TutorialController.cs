using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public static bool InstanceLoad
    {
        get;
        private set;
    }
    private void Awake()
    {
        GFs.LoadData();
        SceneManager.GetActiveScene().GetRootGameObjects();

        if (!InstanceLoad)
        {
            InstanceLoad = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    public void OnSkipBtnClicked()
    {
        GVs.SCENE_MANAGER.loadPreloadScene();
    }


    private void OnSceneLoaded(Scene Scene, LoadSceneMode Mode)
    {
        var listGameObjects = new List<GameObject>();
        var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var rootGO in rootGameObjects)
        {
            var UIButtonArrays = rootGO.GetComponentsInChildren<UIButton>(true);
            var ButtonArrays = rootGO.GetComponentsInChildren<Button>(true);

            foreach (var uibtn in UIButtonArrays)
            {
                if (uibtn.GetComponent<AudioSource>() != null)
                    continue;

                uibtn.onClick.Add(new EventDelegate(() =>
                {
                    if (GVs.SOUND_SYSTEM == 1)
                    {
                        SoundButtonSingletonScript.Instance.audioSource.Play();
                    }

                }));
            }

            foreach (var btn in ButtonArrays)
            {
                if (btn.GetComponent<AudioSource>() != null)
                    continue;

                btn.onClick.AddListener(() =>
                {
                    if (GVs.SOUND_SYSTEM == 1)
                    {
                        SoundButtonSingletonScript.Instance.audioSource.Play();
                    }
                });
            }
        }

        if (GVs.SOUND_BG == 1)
        {
            var audioSource = SoundBackgroundSingletonScripts.Instance.audioSource;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            if (Scene.name == "DrawingScene")
                audioSource.Stop();
        }
    }

}
