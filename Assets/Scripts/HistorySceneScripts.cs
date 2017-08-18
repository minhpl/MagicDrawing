using Newtonsoft.Json;
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
    public enum IMAGETYPE { SNAP, MODEL};
    public string filePath;
    public string thumbPath;
    public IMAGETYPE imgType;
    public HistoryModel(string filePath,string thumbPath, IMAGETYPE imgType = IMAGETYPE.SNAP)
    {
        this.filePath = filePath;
        this.imgType = imgType;
        this.thumbPath = thumbPath;
    }
}

public class HistorySceneScripts : MonoBehaviour {	
    public static LinkedList<HistoryModel> history = null;
    private const string KEY = "history";    
    public GameObject item;
    void Start () {
        if (history == null)
        {
            var json = PlayerPrefs.GetString(KEY);            
            history = JsonConvert.DeserializeObject<LinkedList<HistoryModel>>(json);            
        }
        MainThreadDispatcher.StartUpdateMicroCoroutine(load());
    }       

    IEnumerator load()
    {
        yield return null;
        if(history==null)
        {
            yield break;            
        }
        for (var a = history.First; a != null; a = a.Next)
        {           
            yield return null;
            var historyModel = a.Value;            
            GameObject cloneItem = Instantiate(item);
            cloneItem.transform.parent = item.transform.parent;
            cloneItem.transform.localScale = item.transform.localScale;
            var thumbPath = historyModel.thumbPath;
            var filePath = historyModel.filePath;
			string loadPath = null;
			if (historyModel.imgType == HistoryModel.IMAGETYPE.SNAP) {
				loadPath = filePath;
			} else {
				loadPath = thumbPath;
			}
			Texture2D texture = GFs.LoadPNGFromPath(loadPath);
            Mat image = new Mat(texture.height, texture.width, CvType.CV_8UC4);
            Utils.texture2DToMat(texture, image);
            var rimgGameObject = cloneItem.transform.Find("rimg");
            rimgGameObject.GetComponent<RawImage>().texture = texture;
            rimgGameObject.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;
            cloneItem.SetActive(true);
            cloneItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                if(historyModel.imgType == HistoryModel.IMAGETYPE.MODEL)
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
            });
        }
        Destroy(item);
    }
    public static void AddHistoryItem(HistoryModel historyModel)
    {
        const int MAXHISTORY = 30;
        if (history==null)
        {
            var jsonLoad = PlayerPrefs.GetString(KEY);
            if (!String.IsNullOrEmpty(jsonLoad))
                history = JsonConvert.DeserializeObject<LinkedList<HistoryModel>>(jsonLoad);
            if (history == null)
                history = new LinkedList<HistoryModel>();
        }                        
        while (history.Count >= MAXHISTORY)
            history.RemoveLast();
        history.AddFirst(historyModel);
        var jsonSave = JsonConvert.SerializeObject(history);
        PlayerPrefs.SetString(KEY, jsonSave);
        PlayerPrefs.Save();
    }
}
