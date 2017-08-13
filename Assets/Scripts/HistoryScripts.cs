using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
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
    public HistoryModel()
    {

    }
    public HistoryModel(string filePath,string thumbPath, IMAGETYPE imgType = IMAGETYPE.SNAP)
    {
        this.filePath = filePath;
        this.imgType = imgType;
        this.thumbPath = thumbPath;
    }
}

public class HistoryScripts : MonoBehaviour {	
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

            Texture2D texture = GFs.LoadPNGFromPath(thumbPath);

            cloneItem.GetComponent<RawImage>().texture = texture;
        }
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
                        
        if (history.Count == MAXHISTORY)
            history.RemoveLast();
        history.AddFirst(historyModel);
        var jsonSave = JsonConvert.SerializeObject(history);
        PlayerPrefs.SetString(KEY, jsonSave);
        PlayerPrefs.Save();
    }

}
