using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LukeWaffel.AndroidGallery;
using OpenCVForUnity;
using Kakera;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using System.Threading;

public class MainToolsNGUIScripts : MonoBehaviour {

    public UIButton uiBtnLibrary;
    public UIButton uiBtnCam;
    public UIButton uiBtnGallery;
    public UIButton uiBtnHistory;
    public UIButton uiBtnHome;
    public GameObject progressWaitLongTask;

    IDisposable cancelCorountineBackBtnAndroid;

    [SerializeField]
    private Unimgpicker imagePicker;

    // Use this for initialization
    void Start()
    {
        //imagePicker = new Unimgpicker();
        uiBtnLibrary.onClick.Add(new EventDelegate(() =>
        {
            LibraryScriptsNGUI.mode = LibraryScriptsNGUI.MODE.CATEGORY;
            GVs.SCENE_MANAGER.loadLibraryNGUIScene();
        }));

        uiBtnCam.onClick.Add(new EventDelegate(() =>
        {
            GVs.SCENE_MANAGER.loadSnapImageScene();
        }));

        uiBtnGallery.onClick.Add(new EventDelegate(() =>
        {

            //imagePicker.Show("Select Image", "unimgpicker", 1024);

            if (Application.platform == RuntimePlatform.Android)
            {
               // Utilities.Log("Here 0, xin chao the gioi");
               // IDisposable cancelCorountineBackBtnAndroid = Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) == true)
               //.Subscribe(_ => {
               //    Utilities.Log("Here, xin chao the gioi");
               //    int i = GVs.TRACE_SCENE.Pop();                   
               //    SceneManager.LoadScene(i);
               //});

                AndroidGallery.Instance.OpenGallery(ImageLoaded);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                imagePicker.Show("Select Image", "unimgpicker", 1024);
            }
            else
            {
                progressWaitLongTask.SetActive(true);
                Observable.FromCoroutine(WaitSomeTime).DoOnCompleted(() =>
                {
                    progressWaitLongTask.SetActive(false);
                });                
            }
        }));

        uiBtnHistory.onClick.Add(new EventDelegate(() =>
        {
            GVs.SCENE_MANAGER.loadHistoryNGUIScene();
        }));

        uiBtnHome.onClick.Add(new EventDelegate(() =>
        {
            GVs.TRACE_SCENE.Clear();
            GVs.SCENE_MANAGER.loadHomeScene();
        }));

        imagePicker.Completed += (string path) =>
        {
            progressWaitLongTask.SetActive(true);
            Texture2D texture = GFs.LoadPNGFromPath(path);
            Mat image = new Mat(texture.height, texture.width, CvType.CV_8UC3);
            Utils.texture2DToMat(texture, image);
            DrawingScripts.image = image;
            DrawingScripts.texModel = texture;
            DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
            progressWaitLongTask.SetActive(false);
            GVs.SCENE_MANAGER.loadDrawingScene();
            HistoryNGUIScripts.AddHistoryItem(new HistoryModel(path, path, HistoryModel.IMAGETYPE.SNAP));
        };
    }

    private void OnDisable()
    {
        Debug.Log("when call this ????");
        if(cancelCorountineBackBtnAndroid!=null)
            cancelCorountineBackBtnAndroid.Dispose();
    }

    void ImageLoaded()
    {
        progressWaitLongTask.SetActive(true);
        Texture2D texture = (Texture2D)AndroidGallery.Instance.GetTexture();
        Mat image = new Mat(texture.height, texture.width,CvType.CV_8UC4);
        Utils.texture2DToMat(texture, image);        
        var path = AndroidGallery.Instance.getPath();        
        DrawingScripts.image = image;
        DrawingScripts.texModel = texture;
        DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
        progressWaitLongTask.SetActive(false);
        GVs.SCENE_MANAGER.loadDrawingScene();
        HistoryNGUIScripts.AddHistoryItem(new HistoryModel(path, path, HistoryModel.IMAGETYPE.SNAP));
    }

    IEnumerator WaitSomeTime()
    {
        yield return new WaitForSeconds(2);
    }
}
