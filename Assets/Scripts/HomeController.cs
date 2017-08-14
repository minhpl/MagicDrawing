using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour {

    private void Awake()
    {
        var clickStream = Observable.EveryUpdate().Where(_ => Input.touchCount!=0);
        clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250))).Where(xs => xs.Count >= 2)
            .Subscribe(xs =>
            {
                Utilities.Log("Double click detected");
            });
        
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        //MainThreadDispatcher.StartCoroutine(Worker());
    }

    private IEnumerator Worker()
    {        
        yield return null;
        Utilities.Log("mouse press is {0}", Input.GetMouseButtonDown(0));
        Utilities.Log("touch is {0}", Input.touchCount);
        Utilities.Log("Is touch supported? {0}", Input.touchSupported);
        MainThreadDispatcher.StartCoroutine(Worker());
        if (Input.GetKey("up"))
            print("up arrow key is held down");
    }

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
        if(GVs.DRAWING_TEMPLATE_LIST!=null)
        {
            var imageCount = GVs.DRAWING_TEMPLATE_LIST.Count();
            if (imageCount > 0)
            {
                Utilities.Log("Ready");
                //ready = true;
                //return;
            }
        }        

        Utilities.Log("Waiting for downloading");

        //if (NET.NetWorkIsAvaiable())
        //    HTTPRequest.Instance.Request(GVs.GET_ALL_TEMPLATE_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
        //    {
        //        Debug.Log("data: " + data);
        //        GVs.DRAWING_TEMPLATE_LIST_MODEL = JsonUtility.FromJson<DrawingTemplateListModel>(data);
        //        GFs.SaveTemplateList();
        //        HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_TEMPLATE))), (d, process) =>
        //        {
        //            Utilities.Log("downloading");
        //            if (process == 1)
        //            {
        //                ready = true;
        //                Utilities.Log("Downloaded");
        //            }
        //        });
        //    });
        //else
        //    Utilities.Log("Why network not available");

        HTTPRequest.Instance.Request(GVs.GET_ALL_CATEGORY_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
        {
            try
            {
                Debug.Log(data);
                GVs.CATEGORY_LIST = JsonConvert.DeserializeObject <CategoryList>(data.ToString());
                Debug.Log(GVs.CATEGORY_LIST.data.Count);
                GFs.SaveCategoryList();
            }
            catch (Exception e)
            {
                Debug.LogFormat("cannot deserialize data to object, error is {0}", e.ToString());
            }
        });

        HTTPRequest.Instance.Request(GVs.GET_TEMPLATE_BY_CATEGORY_URL, JsonUtility.ToJson(new ReqModel(new CategoryRequest("C01"))), (data) =>
        {
            try
            {
                TemplateDrawingList templatelist= JsonConvert.DeserializeObject<TemplateDrawingList>(data);
                templatelist.dir = templatelist.dir + "/C01";
                GVs.DRAWING_TEMPLATE_LIST = templatelist;
                GFs.SaveTemplateList();
                ready = true;
            }
            catch(Exception e)
            {
                Debug.LogFormat("Error : {0}", e.ToString());
            }
        });
        HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_CATEGORY_AVATA))), (d, process) =>
        {

        });
        HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_CATEGORY, "C01"))), (d, process) =>
        {

        });
    }
    private bool ready = false;

    public void loadHistory1Scene()
    {
        if (ready)
        {
            GVs.SCENE_MANAGER.loadMasterpieceCreationScene();
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
