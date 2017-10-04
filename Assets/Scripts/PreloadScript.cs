using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UniRx;

public class PreloadScript : MonoBehaviour
{
    public GameObject goLogo;
    public UISlider uiDownload;
    public UILabel text;
    public GameObject popupRequireNetwork;
    public GameObject licensePanel;
    public UIInput inputCode;
    public UILabel labelMsg;
    public GameObject[] btnControls;
    IDisposable cancelCorountineDownloadData;
    IDisposable cancelCorountineLoadSoundButton;
    bool ready1 = false;   //ready download data
    IDisposable cancelCorountineQuitApplication;

    private const float PROGRESS_DOWNLOAD_AVARTAR_PERCENT = 0.03f;

    private void Awake()
    {
        cancelCorountineQuitApplication = GFs.BackButtonAndroidQuitApplication();
    }

    // Use this for initialization
    void Start()
    {
        GFs.LoadData();
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

        StartCheckApp();
    }

    private void OnDisable()
    {
        if (cancelCorountineDownloadData != null)
            cancelCorountineDownloadData.Dispose();
        if (cancelCorountineLoadSoundButton != null)
            cancelCorountineLoadSoundButton.Dispose();
        if (cancelCorountineQuitApplication != null)
            cancelCorountineQuitApplication.Dispose();
    }

    public void StartCheckApp()
    {
        if (NET.NetWorkIsAvaiable()) popupRequireNetwork.SetActive(false);
        if (GVs.LICENSE_CODE.Equals(""))
        {

            if (!NET.NetWorkIsAvaiable())
            {
                popupRequireNetwork.SetActive(true);
                return;
            }

            HTTPRequest.Instance.Request(GVs.CHECK_AVAIABLE_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
            {
                try
                {
                    ResModel resModel = JsonUtility.FromJson<ResModel>(data);
                    if (resModel.success == 0)
                    {
                        GVs.LICENSE_CODE = "******";
                        StartCheckApp();
                    }
                    else
                    {
                        licensePanel.SetActive(true);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log(data);
                    Debug.Log(e);
                }
            });
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
                popupRequireNetwork.SetActive(true);
            }
            else if (GVs.CATEGORY_LIST == null || GVs.CATEGORY_LIST.Count() == 0)
            {
                popupRequireNetwork.SetActive(true);
            }
            else if (GVs.TEMPLATE_LIST_ALL_CATEGORY == null || GVs.TEMPLATE_LIST_ALL_CATEGORY.Count == 9)
            {
                popupRequireNetwork.SetActive(true);
            }
            else if (GVs.OTHER_APP_LIST_MODEL == null || GVs.OTHER_APP_LIST_MODEL.Count() == 9)
            {
                popupRequireNetwork.SetActive(true);
            }
            else
            {
                GVs.SCENE_MANAGER.StartHomeScene();
            }
            return;
        }
        goLogo.SetActive(true);
        uiDownload.gameObject.SetActive(true);
        uiDownload.numberOfSteps = 1001;
        popupRequireNetwork.SetActive(false);

        var streamDownloadAvartarProfile = Observable.Create<float>((IObserver<float> observer) =>
        {
            HTTPRequest.Instance.Request(GVs.GET_ALL_AVATA_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
            {
                try
                {
                    Debug.Log(data);
                    GVs.AVATA_LIST_MODEL = (JsonUtility.FromJson<AvataListModel>(data));
                    GFs.SaveAvatas();
                    HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_AVATAS))), (data2, progress) =>
                    {
                        uiDownload.value = progress * PROGRESS_DOWNLOAD_AVARTAR_PERCENT;
                        if (progress == 1 || progress == -404)
                        {
                            observer.OnCompleted();
                        }
                    });
                }
                catch (System.Exception e)
                {
                    observer.OnError(e);
                }
            });
            return Disposable.Create(() =>
            {

            });
        });

        streamDownloadOtherApp.Subscribe(_ => { }, () =>
        {
            streamDownloadAvartarProfile.Subscribe(_ => { }, () =>
            {
                cancelCorountineDownloadData = Observable.FromCoroutine(DownloadData).Subscribe();
            });
        });

    }

    private IObservable<float> streamDownloadOtherApp = Observable.Create<float>((IObserver<float> observer) =>
    {
        HTTPRequest.Instance.Request(GVs.GET_ALL_OTHER_APP_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
        {
            try
            {
                GVs.OTHER_APP_LIST_MODEL = (JsonUtility.FromJson<OtherAppListModel>(data));
                HTTPRequest.Instance.Download(GVs.DOWNLOAD_OTHER_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_OTHER_APP_ICON))), (data2, process) =>
                {
                    if (process == 1 || process == -404)
                    {
                        if (process == 1)
                            GFs.SaveOtherAppList();
                        observer.OnCompleted();
                    }
                });
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                observer.OnError(e);
            }
        });
        return Disposable.Empty;
    });

    IEnumerator DownloadData()
    {
        //try
        //{
        //    if (GVs.CATEGORY_LIST != null && GVs.TEMPLATE_LIST_ALL_CATEGORY != null)
        //    {
        //        var numCategory = GVs.CATEGORY_LIST.Count();
        //        var NumtemplateList = GVs.TEMPLATE_LIST_ALL_CATEGORY.Count;
        //        if (numCategory == NumtemplateList && numCategory != 0)
        //        {
        //            Utilities.Log("Ready");
        //            ready1 = true;
        //            uiDownload.value = 1;
        //            StartCoroutine(WaitForStartHome());
        //            yield break;
        //        }
        //    }
        //    else
        //    {
        //        PlayerPrefs.DeleteAll();
        //        PlayerPrefs.Save();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError(ex);
        //}


        Utilities.Log("Waiting for downloading");
        Dictionary<string, TemplateDrawingList> templateListsAllCategory = new Dictionary<string, TemplateDrawingList>();
        List<IObservable<float>> ListStreamDownloadTemplate = new List<IObservable<float>>();

        while (!NET.NetWorkIsAvaiable())
        {
            yield return new WaitForSeconds(0.5f);
        }

        popupRequireNetwork.SetActive(false);
        const float PROGRESS_DOWNLOAD_ALL_CATEGORY = 0.03f;
        const float PROGRESS_DOWNLOAD_CATEGORY_AVATA = 0.03f;
        const float PROGRESS_MAX = 1f;
        float currentProgress = PROGRESS_DOWNLOAD_AVARTAR_PERCENT;
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
                    var volumeCategoryProgress = (PROGRESS_MAX - PROGRESS_DOWNLOAD_ALL_CATEGORY - PROGRESS_DOWNLOAD_CATEGORY_AVATA - PROGRESS_DOWNLOAD_AVARTAR_PERCENT)
                    / (float)listCategory.Count;
                    for (int i = 0; i < listCategory.Count; i++)
                    {
                        var category = listCategory[i];
                        string id = category._id;
                        var index = i;
                        stream = Observable.Create<float>((IObserver<float> observer) =>
                        {
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
                                        Debug.Log(process);
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
                            popupRequireNetwork.SetActive(false);
                            ready1 = true;
                            StartCoroutine(WaitForStartHome());
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
            popupRequireNetwork.SetActive(true);
        }
    }


    IEnumerator WaitForStartHome()
    {
        yield return null;
        while (!ready1)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);
        GVs.SCENE_MANAGER.StartHomeScene();
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
            popupRequireNetwork.SetActive(true);
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

                    GVs.LICENSE_CODE = code;
                    GFs.SaveLicenseCode();
                    btnControls[0].SetActive(false);
                    btnControls[1].SetActive(true);
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
