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
using WW;

public class HomeController : MonoBehaviour {
    private IDisposable cancelCorountineQuitApplication;
    public GameObject requireNetworl_panel;
    public UIButton BtnClose;
    public UIButton BtnX;
    IDisposable cancelCorountineDownloadData;


    private void Awake()
    {


        debug.log("Important message. Color it red and show in the console");

        debug.log(1, "Debug message from a specific category. Custom color and important level (will always print)");

        debug.log(2, "Debug message from a specific category, moderate importantce (marked as debug level 5)", 5);

        debug.log(3, "Debug message, same as previous but will also print gameObject containing this call, it's name as well as highlight it from console+click)", 1, gameObject);





        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

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
    .Where(s => s.EndsWith(".avi"));
        foreach (var videoPath in videoFiles)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(videoPath);
            var imageCorresponding = masterPieceDirPath + fileNameWithoutExtension + ".png";
            if (!File.Exists(imageCorresponding))
            {
                File.Delete(videoPath);
            }
        }

        GFs.LoadData();
        try
        {
            if (GVs.CATEGORY_LIST != null && GVs.TEMPLATE_LIST_ALL_CATEGORY != null)
            {
                var numCategory = GVs.CATEGORY_LIST.Count();
                var NumtemplateList = GVs.TEMPLATE_LIST_ALL_CATEGORY.Count;
                if (numCategory == NumtemplateList && numCategory != 0)
                {
                    Utilities.Log("Ready");
                    ready1 = true;
                    ready2 = true;
                    return;
                }
            }
            else
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }


        if (!NET.NetWorkIsAvaiable())
        {
            requireNetworl_panel.SetActive(true);
        }

        cancelCorountineDownloadData = Observable.FromCoroutine(DownloadData).Subscribe();

    }

    IEnumerator DownloadData()
    {
        Utilities.Log("Waiting for downloading");
        Dictionary<string, TemplateDrawingList> templateListsAllCategory = new Dictionary<string, TemplateDrawingList>();
        List<IObservable<string>> ListStreamDownloadTemplate = new List<IObservable<string>>();

        while (!NET.NetWorkIsAvaiable())
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (NET.NetWorkIsAvaiable())
            HTTPRequest.Instance.Request(GVs.GET_ALL_CATEGORY_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
            {
                Debug.LogFormat("Data is {0}", data);
                try
                {
                    GVs.CATEGORY_LIST = JsonConvert.DeserializeObject<CategoryList>(data.ToString());
                    GFs.SaveCategoryList();

                    var listCategory = GVs.CATEGORY_LIST.data;
                    for (int i = 0; i < listCategory.Count; i++)
                    {
                        var category = listCategory[i];
                        string id = category._id;
                        var index = i;
                        var stream = Observable.Create<string>((IObserver<string> observer) =>
                        {
                            Debug.LogFormat("Start download tempplate {0}", index);

                            HTTPRequest.Instance.Request(GVs.GET_TEMPLATE_BY_CATEGORY_URL, JsonUtility.ToJson(new ReqModel(new CategoryRequest(id))), (templates) =>
                            {
                                try
                                {
                                    Debug.LogFormat("templates data : {0}",templates);
                                    TemplateDrawingList templatelist = JsonConvert.DeserializeObject<TemplateDrawingList>(templates);
                                    templatelist.dir = templatelist.dir + "/" + id;
                                    GVs.DRAWING_TEMPLATE_LIST = templatelist;
                                    GFs.SaveAllTemplateList();
                                    templateListsAllCategory[id] = templatelist;

                                    HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_CATEGORY, id))), (d, process) =>
                                    {
                                        if (process == 1)
                                        {
                                            //Thread.Sleep(2000);
                                            Debug.LogFormat("call from thread {0}", index);
                                            observer.OnCompleted();
                                        }
                                    });
                                }
                                catch (Exception e)
                                {
                                    Utilities.Log("Error : {0}", e.ToString());
                                }
                            });
                            return Disposable.Create(() =>
                            {
                                Debug.LogFormat("observer {0} has unsubscribed", index);
                            });
                        });
                        ListStreamDownloadTemplate.Add(stream);
                    }

                    //Observable.Concat()

                    //ListStreamDownloadTemplate[0].Subscribe();

                    Observable.Concat(ListStreamDownloadTemplate).Subscribe(_ => { }, () =>
                     {
                         GVs.TEMPLATE_LIST_ALL_CATEGORY = templateListsAllCategory;
                         GFs.SaveAllTemplateList();
                         Utilities.Log("all downloaded");
                         var json = JsonConvert.SerializeObject(templateListsAllCategory);
                         ready1 = true;
                     });
                }
                catch (Exception e)
                {
                    Utilities.Log("cannot deserialize data to object, error is {0}", e.ToString());
                }
            });
        else
        {
            requireNetworl_panel.SetActive(true);
        }

        HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_CATEGORY_AVATA))), (d, process) =>
        {
            if (process == 1)
            {
                ready2 = true;
            }

        });

        //HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_CATEGORY, "C01"))), (d, process) =>
        //{

        //});     
    }


    private void Start()
    {
        if(Application.platform==RuntimePlatform.Android)
        {
            cancelCorountineQuitApplication = Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) == true).Subscribe(_ =>
            {
                Application.Quit();
            });
        }        
    }

    private bool ready1 = false;
    private bool ready2 = false;

    public void loadMasterpieceCreationScene()
    {
        if (ready1 && ready2)
        {
            GVs.SCENE_MANAGER.loadMasterpieceCreationnNGUIScene();
        }
    }
    public void loadLibrary()
    {
        if (ready1 && ready2)
        {
            LibraryScriptsNGUI.mode = LibraryScriptsNGUI.MODE.CATEGORY;            
            SceneManager.LoadSceneAsync("LibraryNGUIScene");
            //GVs.SCENE_MANAGER.loadLibraryNGUIScene();
        }
    }

    private void OnDisable()
    {
        if (cancelCorountineQuitApplication != null)
            cancelCorountineQuitApplication.Dispose();

        if (cancelCorountineDownloadData != null)
            cancelCorountineDownloadData.Dispose();
    }
}
