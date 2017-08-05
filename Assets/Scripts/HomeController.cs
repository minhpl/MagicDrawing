using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour {
    void Start () {

        if (MakePersistentObject.Instance)
        {
            MakePersistentObject.Instance.gameObject.SetActive(false);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            GVs.APP_PATH = "/data/data/com.MinhViet.ProductName/files";
        }
        else
        {
            Utilities.Log("here");
            GVs.APP_PATH = Application.persistentDataPath;
        }
        GFs.LoadTemplateList();
        var imageCount = GVs.DRAWING_TEMPLATE_LIST_MODEL.Count();
        if (imageCount > 0) 
        {
            Utilities.Log("Ready");
            ready = true;
            //return;
        }

        Utilities.Log("Waiting for downloading");

        if (NET.NetWorkIsAvaiable())
            HTTPRequest.Instance.Request(GVs.GET_ALL_TEMPLATE_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
            {
                Debug.Log("data: " + data);
                GVs.DRAWING_TEMPLATE_LIST_MODEL = JsonUtility.FromJson<DrawingTemplateListModel>(data);
                GFs.SaveTemplateList();
                HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_TEMPLATE))), (d, process) =>
                {
                    Utilities.Log("downloading");
                    if (process == 1)
                    {
                        ready = true;
                        Utilities.Log("Downloaded");
                    }
                });
            });
        else
            Utilities.Log("Why network not available");
    }
    private bool ready = true;

    public void loadHistory1Scene()
    {
        if (ready)
        {
            GVs.SCENE_MANAGER.loadHistory1Scene();
        }         
    }
    public void loadLibrary()
    {
        if (ready)
        {         
            GVs.SCENE_MANAGER.loadLibraryScene();
        }            
    }
}
