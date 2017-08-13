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
        var imageCount = GVs.DRAWING_TEMPLATE_LIST_MODEL.Count();
        if (imageCount > 0) 
        {
            Utilities.Log("Ready");
            ready = true;
            return;
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
