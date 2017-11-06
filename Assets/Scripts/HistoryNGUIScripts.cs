﻿using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HistoryModel
{
    public enum IMAGETYPE { SNAP, MODEL };
    public string filePath;
    public string thumbPath;
    public IMAGETYPE imgType;
    public HistoryModel(string filePath, string thumbPath, IMAGETYPE imgType = IMAGETYPE.SNAP)
    {
        this.filePath = filePath;
        this.imgType = imgType;
        this.thumbPath = thumbPath;
    }
}

public class HistoryNGUIScripts : MonoBehaviour
{
    public static LinkedList<HistoryModel> history = null;
    private const string KEY = "history_";
    public GameObject item;
    public UIGrid uiGrid;
    private IDisposable cancelCoroutineBackBtnAndroid;
    private IDisposable cancelLoad;
    public Button btnBack;
    private void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            cancelCoroutineBackBtnAndroid = Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) == true).Subscribe(_ =>
            {
                GVs.SCENE_MANAGER.loadLibraryNGUIScene();
            });
        }


        btnBack.onClick = new Button.ButtonClickedEvent();
        btnBack.onClick.AddListener(() =>
        {
            GVs.SCENE_MANAGER.loadLibraryNGUIScene();
        });
    }

    void Start()
    {
        //if (history == null)
        {
            var user_id = GVs.CURRENT_USER_MODEL.id;

            var json = PlayerPrefs.GetString(getUserHistoryKey());
            history = JsonConvert.DeserializeObject<LinkedList<HistoryModel>>(json);
        }
        cancelLoad = Observable.FromMicroCoroutine(load).Subscribe();
    }

    static private string getUserHistoryKey()
    {
        var user_id = GVs.CURRENT_USER_MODEL.id;
        return KEY + user_id.ToString();
    }


    IEnumerator load()
    {
        yield return null;
        if (history == null)
        {
            yield break;
        }
        for (var a = history.First; a != null; a = a.Next)
        {
            yield return null;
            try
            {
                var historyModel = a.Value;
                GameObject cloneItem = Instantiate(item);
                cloneItem.transform.parent = item.transform.parent;
                cloneItem.transform.localScale = item.transform.localScale;
                var thumbPath = historyModel.thumbPath;
                var filePath = historyModel.filePath;
                string loadPath = null;
                if (historyModel.imgType == HistoryModel.IMAGETYPE.SNAP)
                {
                    loadPath = filePath;
                }
                else
                {
                    loadPath = thumbPath;
                }
                Texture2D texture = GFs.LoadPNGFromPath(loadPath);
                Mat image = new Mat(texture.height, texture.width, CvType.CV_8UC4);
                Utils.texture2DToMat(texture, image);
                var rimgGameObject = cloneItem.transform.Find("icon");
                rimgGameObject.GetComponent<UITexture>().mainTexture = texture;
                cloneItem.SetActive(true);
                uiGrid.Reposition();

                cloneItem.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
                {
                    if (historyModel.imgType == HistoryModel.IMAGETYPE.MODEL)
                    {
                        DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_MODEL;
                        DrawingScripts.imgModelPath = filePath;
                        AddHistoryItem(new HistoryModel(filePath, thumbPath, HistoryModel.IMAGETYPE.MODEL));
                    }
                    else
                    {
                        DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
                        DrawingScripts.image = image;
                        DrawingScripts.texModel = texture;
                        AddHistoryItem(new HistoryModel(filePath, thumbPath, HistoryModel.IMAGETYPE.MODEL));
                    }
                    GVs.SCENE_MANAGER.loadDrawingScene();
                }));
            }
            catch (System.Exception e)
            {
                Debug.LogFormat("Error is {0}", e.ToString());
                Debug.LogFormat("Stack trace is {0}", e.StackTrace.ToString());
            }
        }
        Destroy(item);
    }

    public void OnDisable()
    {
        if (cancelCoroutineBackBtnAndroid != null)
            cancelCoroutineBackBtnAndroid.Dispose();
        if (cancelLoad != null)
            cancelLoad.Dispose();
    }

    public static void AddHistoryItem(HistoryModel historyModel)
    {
        const int MAXHISTORY = 30;
        if (history == null)
        {
            var jsonLoad = PlayerPrefs.GetString(getUserHistoryKey());
            if (!String.IsNullOrEmpty(jsonLoad))
                history = JsonConvert.DeserializeObject<LinkedList<HistoryModel>>(jsonLoad);
            if (history == null)
                history = new LinkedList<HistoryModel>();
        }
        for (var a = history.First; a != null; a = a.Next)
        {
            if (a.Value.filePath == historyModel.filePath)
            {
                history.Remove(a);
                break;
            }
        }
        //var a = history.Find(historyModel);
        //if (a != null) history.Remove(a);
        history.AddFirst(historyModel);
        while (history.Count > MAXHISTORY)
            history.RemoveLast();
        var jsonSave = JsonConvert.SerializeObject(history);
        PlayerPrefs.SetString(getUserHistoryKey(), jsonSave);
        PlayerPrefs.Save();
    }
}
