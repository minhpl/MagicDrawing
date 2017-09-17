using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UniRx;

public class PreloadScript : MonoBehaviour
{
    public UISlider uiDownload;
    public UILabel text;
    public GameObject popup;

    public GameObject licensePanel;
    public UIInput inputCode;
    public UILabel labelMsg;

    public GameObject[] btnControls;

    IDisposable cancelCorountineDownloadData;
    IDisposable cancelCorountineLoadSoundButton;
    bool ready1 = false;   //ready download data
    bool ready2 = false;   //ready load sound from resources 
    private void Awake()
    {
        if(ClickSound.audioClip!=null)
        {
            cancelCorountineLoadSoundButton = Observable.FromCoroutine(loadSoundButton).Subscribe(_ => { }, () => { ready2 = true; });
        }
    }


    public static AudioSource audioSource;
    IEnumerator loadSoundButton()
    {
        var request = Resources.LoadAsync("button");
        yield return request;
        var audioClip = request.asset as AudioClip;
        ClickSound.audioClip = audioClip;
        audioSource.clip = audioClip;
        ready2 = true;
    }

    // Use this for initialization
    void Start()
    {
        if (GVs.DEBUG) Debug.Log("Application.persistentDataPath;: " + Application.persistentDataPath);
        // getApplicationContext().getFilesDir().getAbsolutePath()
        if (Application.platform == RuntimePlatform.Android)
        {
            GVs.APP_PATH = "data/data/" + Application.identifier + "/files";
        }
        else
        {
            GVs.APP_PATH = Application.persistentDataPath;
        }
        text.text = "Cập nhật dữ liệu";
        uiDownload.numberOfSteps = 1001;
        GFs.LoadData();
        StartCheckApp();
    }

    private void OnDisable()
    {
        if (cancelCorountineDownloadData != null)
            cancelCorountineDownloadData.Dispose();
        if (cancelCorountineLoadSoundButton != null)
            cancelCorountineLoadSoundButton.Dispose();
    }

    public void StartCheckApp()
    {
        if (NET.NetWorkIsAvaiable()) popup.SetActive(false);
        if (GVs.LICENSE_CODE.Equals(""))
        {
            licensePanel.SetActive(true);
        }
        else
        {
            Init();
        }
    }

    public void Init()
    {
        if (!NET.NetWorkIsAvaiable())
        {
            if (GVs.AVATA_LIST_MODEL == null || GVs.AVATA_LIST_MODEL.Count() == 0)
            {
                popup.SetActive(true);
            }
            else if (GVs.CATEGORY_LIST == null || GVs.CATEGORY_LIST.Count() == 0)
            {
                popup.SetActive(true);
            }
            else if (GVs.TEMPLATE_LIST_ALL_CATEGORY == null || GVs.TEMPLATE_LIST_ALL_CATEGORY.Count == 9)
            {
                popup.SetActive(true);
            }
            else
            {
                GVs.SCENE_MANAGER.StartHomeScene();
            }
            return;
        }
        uiDownload.gameObject.SetActive(true);
        popup.SetActive(false);


        text.text = "Cập nhật dữ liệu";
        DownloadAvatas();
    }

    private void DownloadAvatas()
    {
        HTTPRequest.Instance.Request(GVs.GET_ALL_AVATA_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
        {
            try
            {
                Debug.Log(data);
                GVs.AVATA_LIST_MODEL = (JsonUtility.FromJson<AvataListModel>(data));
                GFs.SaveAvatas();
                HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_AVATAS))), (data2, process) =>
                {
                    if (process == 1 || process == -404)
                    {
                        DownloadCat();
                    }
                });
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        });
    }

    private void DownloadCat()
    {
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
                    StartCoroutine(WaitForStartHome());
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

        cancelCorountineDownloadData = Observable.FromCoroutine(DownloadData).Subscribe();
    }


    IEnumerator DownloadData()
    {
        Utilities.Log("Waiting for downloading");
        Dictionary<string, TemplateDrawingList> templateListsAllCategory = new Dictionary<string, TemplateDrawingList>();
        List<IObservable<float>> ListStreamDownloadTemplate = new List<IObservable<float>>();

        while (!NET.NetWorkIsAvaiable())
        {
            yield return new WaitForSeconds(0.5f);
        }
        popup.SetActive(false);
        const float PROGRESS_DOWNLOAD_ALL_CATEGORY = 0.03f;
        const float PROGRESS_DOWNLOAD_CATEGORY_AVATA = 0.03f;
        const float PROGRESS_MAX = 1f;
        float currentProgress = 0;
        if (NET.NetWorkIsAvaiable())
        {

            HTTPRequest.Instance.Request(GVs.GET_ALL_CATEGORY_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
            {

                IObservable<float> stream = null;
                currentProgress += PROGRESS_DOWNLOAD_ALL_CATEGORY;
                uiDownload.value = currentProgress;
                try
                {
                    GVs.CATEGORY_LIST = JsonConvert.DeserializeObject<CategoryList>(data.ToString());
                    GFs.SaveCategoryList();

                    var listCategory = GVs.CATEGORY_LIST.data;
                    var volumeCategoryProgress = (PROGRESS_MAX - PROGRESS_DOWNLOAD_ALL_CATEGORY - PROGRESS_DOWNLOAD_CATEGORY_AVATA) / (float)listCategory.Count;
                    for (int i = 0; i < listCategory.Count; i++)
                    {
                        var category = listCategory[i];
                        string id = category._id;
                        var index = i;
                        stream = Observable.Create<float>((IObserver<float> observer) =>
                        {
                            Debug.LogFormat("Start download tempplate {0}", index);

                            HTTPRequest.Instance.Request(GVs.GET_TEMPLATE_BY_CATEGORY_URL, JsonUtility.ToJson(new ReqModel(new CategoryRequest(id))), (templates) =>
                            {
                                try
                                {
                                    Debug.LogFormat("templates data : {0}", templates);
                                    TemplateDrawingList templatelist = JsonConvert.DeserializeObject<TemplateDrawingList>(templates);
                                    templatelist.dir = templatelist.dir + "/" + id;
                                    GVs.DRAWING_TEMPLATE_LIST = templatelist;
                                    GFs.SaveAllTemplateList();
                                    templateListsAllCategory[id] = templatelist;

                                    HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_CATEGORY, id))), (d, process) =>
                                    {
                                        var plusProgress = volumeCategoryProgress * process;
                                        var temp = currentProgress + plusProgress;
                                        uiDownload.value = temp;
                                        if (process == 1)
                                        {
                                            plusProgress = volumeCategoryProgress * process;
                                            currentProgress += plusProgress;
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

                    stream = Observable.Create<float>((IObserver<float> observer) =>
                    {
                        HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_CATEGORY_AVATA))), (d, process) =>
                        {
                            var plusProgress = PROGRESS_DOWNLOAD_CATEGORY_AVATA * process;
                            var temp = currentProgress + plusProgress;
                            uiDownload.value = temp;
                            if (process == 1)
                            {
                                observer.OnCompleted();
                            }
                        });
                        return null;
                    });
                    ListStreamDownloadTemplate.Add(stream);

                    Observable.Concat(ListStreamDownloadTemplate).Subscribe(_ => { }, () =>
                        {
                            GVs.TEMPLATE_LIST_ALL_CATEGORY = templateListsAllCategory;
                            GFs.SaveAllTemplateList();
                            Utilities.Log("all downloaded");
                            var json = JsonConvert.SerializeObject(templateListsAllCategory);
                            popup.SetActive(false);
                            ready1 = true;
                        });
                }
                catch (Exception e)
                {
                    Utilities.Log("cannot deserialize data to object, error is {0}", e.ToString());
                }
            });
        }
        else
        {
            popup.SetActive(true);
        }
    }

    IEnumerator WaitForStartHome()
    {
        yield return null;
        while (!ready1 || !ready2)
        {
            yield return null;
            GVs.SCENE_MANAGER.StartHomeScene();            
        }
    }


    // Update is called once per frame    
    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private IEnumerator StartApp()
    {
        yield return new WaitForSeconds(1);
        GVs.SCENE_MANAGER.StartHomeScene();
    }

    private ResModel resLicenseModel;
    public void ActiveLicenseOnClick()
    {
        string code = inputCode.value.Trim().ToUpper();

        if (!NET.NetWorkIsAvaiable())
        {
            popup.SetActive(true);
        }
        else
            HTTPRequest.Instance.Request(GVs.ACTIVE_LICENSE_URL, JsonUtility.ToJson(new ReqModel(code, 1)), (data) =>
            {
                try
                {
                    Debug.Log(data);
                    resLicenseModel = JsonUtility.FromJson<ResModel>(data);
                    inputCode.gameObject.SetActive(false);
                    labelMsg.text = resLicenseModel.msg;
                    if (resLicenseModel.success == 1)
                    {
                        GVs.LICENSE_CODE = code;
                        GFs.SaveLicenseCode();
                        btnControls[0].SetActive(false);
                        btnControls[1].SetActive(true);
                    }
                    else
                    {
                        btnControls[0].SetActive(false);
                        btnControls[2].SetActive(true);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            });
    }

    public void TryAgainOnClick()
    {
        inputCode.value = "";
        inputCode.gameObject.SetActive(true);
        labelMsg.text = "";
        btnControls[0].SetActive(true);
        btnControls[2].SetActive(false);
    }
    public void ContinueOnClick()
    {
        licensePanel.SetActive(false);
        Init();
    }
}
