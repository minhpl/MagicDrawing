using Assets.OpenCVForUnity.Examples.MagicDrawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour {
    void Start () {
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    GVs.APP_PATH = "/data/data/com.MinhViet.ProductName/files";
        //}
        //else
        //{
        //    Utilities.Log("here");
        //    GVs.APP_PATH = Application.persistentDataPath;
        //}
        //GFs.LoadTemplateList();
        //if (NET.NetWorkIsAvaiable())
        //    HTTPRequest.Instance.Request(GVs.GET_ALL_TEMPLATE_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
        //    {
        //        Debug.Log("data: " + data);
        //        GVs.DRAWING_TEMPLATE_LIST_MODEL = JsonUtility.FromJson<DrawingTemplateListModel>(data);
        //        GFs.SaveTemplateList();
        //        HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_TEMPLATE))), (d, process) =>
        //        {
        //            if (process == 1) downloaded = true;
        //        });
        //    });

        if (MakePersistentObject.Instance)
            MakePersistentObject.Instance.gameObject.SetActive(false);

    }
    private bool downloaded = false;
    void Update () {
		
	}
    public void loadHistory1Scene()
    {
        //if (downloaded)
        {
            GVs.PREV_SCENE.Add(this.gameObject.scene.buildIndex);
            GVs.SCENE_MANAGER.loadHistory1Scene();
        }         
    }
    public void loadLibrary()
    {
        //if (downloaded)
        {
            GVs.PREV_SCENE.Add(this.gameObject.scene.buildIndex);
            GVs.SCENE_MANAGER.loadLibraryScene();
        }
            
    }
}
