using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    private IDisposable cancelCorountineQuitApplication;
    public GameObject requireNetworl_panel;
    public UIButton BtnClose;
    public UIButton BtnX;
    public UIButton BtnQuitApp;
    public GameObject downloadPopUp;
    public UISlider UISliderProgressDownload;
    IDisposable cancelCorountineDownloadData;
    public GameObject root;

    private void Awake()
    {
        GFs.LoadData();
        if (TutorialController.isAddedSoundButtonEvent == false)
            GFs.addButtonSoundEventOnSceneLoad();

        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.Save();
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        BtnQuitApp.onClick.Add(new EventDelegate(() =>
        {
            Application.Quit();
        }));

        BtnClose.onClick.Add(new EventDelegate(() =>
        {
            if (NET.NetWorkIsAvaiable())
            {
                requireNetworl_panel.SetActive(false);
            }
        }));

        BtnX.onClick.Add(new EventDelegate(() =>
        {
            if (NET.NetWorkIsAvaiable())
            {
                requireNetworl_panel.SetActive(false);
            }
        }));

        var masterPieceDirPath = GFs.getMasterpieceDirPath();
        if (!Directory.Exists(masterPieceDirPath))
        {
            Directory.CreateDirectory(masterPieceDirPath);
        }

        var snapImageDirPath = GFs.getSnapImageDirPath();
        if (!Directory.Exists(snapImageDirPath))
        {
            Directory.CreateDirectory(snapImageDirPath);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            GVs.APP_PATH = "data/data/" + Application.identifier + "/files";
        }
        else
        {
            GVs.APP_PATH = Application.persistentDataPath;
        }
        if (MakePersistentObject.Instance)
        {
            MakePersistentObject.Instance.gameObject.SetActive(false);
        }

        var videoFiles = Directory.GetFiles(masterPieceDirPath, "*.*", SearchOption.TopDirectoryOnly)
    .Where(s => s.EndsWith(".avi") || s.EndsWith(".mp4"));
        foreach (var videoPath in videoFiles)
        {       
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(videoPath);
            var imageCorresponding = masterPieceDirPath + fileNameWithoutExtension + ".png";
            var anim = masterPieceDirPath + fileNameWithoutExtension + "_anim.png";
            if (!File.Exists(imageCorresponding) && !File.Exists(anim))
            {
                if (videoPath.EndsWith(".mp4"))
                    Utilities.Log("THIS VIDEO IS DELETED : "+videoPath);
                File.Delete(videoPath);
                Utilities.Log("Deleted video is {0}",videoPath);
            }
        }
    }

    private void Start()
    {
        cancelCorountineQuitApplication = GFs.BackButtonAndroidQuitApplication();
    }
    public void loadMasterpieceCreationScene()
    {
        GVs.SCENE_MANAGER.loadMasterpieceCreationnNGUIScene();
    }
    public void loadLibrary()
    {
        LibraryScriptsNGUI.mode = LibraryScriptsNGUI.MODE.CATEGORY;
        SceneManager.LoadSceneAsync("LibraryNGUIScene");
    }

    private void OnDisable()
    {
        if (cancelCorountineQuitApplication != null)
            cancelCorountineQuitApplication.Dispose();

        if (cancelCorountineDownloadData != null)
            cancelCorountineDownloadData.Dispose();
    }
}
