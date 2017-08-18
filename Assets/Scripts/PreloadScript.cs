using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PreloadScript : MonoBehaviour
{
    public UISlider uiDownload;
    public UILabel text;
    public GameObject popup;

    public GameObject licensePanel;
    public UIInput inputCode;
    public UILabel labelMsg;

    public GameObject[] btnControls;
    // Use this for initialization
    void Start()
    {
        // getApplicationContext().getFilesDir().getAbsolutePath()
        if (Application.platform == RuntimePlatform.Android)
        {
            GVs.APP_PATH = "data/data/com.plusstudio.MagicBookEng/files";
        }
        else
        {
            GVs.APP_PATH = Application.persistentDataPath;
        }
        uiDownload.numberOfSteps = 101;
        GFs.LoadData();
        if (GVs.LICENSE_CODE.Equals(""))
        {
            licensePanel.SetActive(true);
        }
        else
        {
            GVs.SCENE_MANAGER.StartHomeScene();
            // Init();
        }
    }
    /*
        public void Init()
        {
            if (!NET.NetWorkIsAvaiable())
            {
                if (!File.Exists(GVs.APP_PATH + "/svm.txt"))
                {
                    popup.SetActive(true);
                }
                else if (GVs.AVATA_LIST_MODEL == null || GVs.AVATA_LIST_MODEL.Count() == 0)
                {
                    popup.SetActive(true);
                }
                else if (GVs.CATEGORY_LIST_MODEL == null || GVs.CATEGORY_LIST_MODEL.Count() == 9)
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
            HTTPRequest.Instance.Request(GVs.GET_ALL_CATEGORY_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
           {
               try
               {
                   GVs.CATEGORY_LIST_MODEL = JsonUtility.FromJson<CategoryListModel>(data);
                   GVs.CATEGORY_LIST_DOWNLOADED_MODEL.Update(GVs.CATEGORY_LIST_MODEL);
                   GFs.SaveCategories();
               }
               catch (System.Exception e)
               {
                   Debug.Log(data);
                   Debug.Log(e);
               }
           });

            HTTPRequest.Instance.Request(GVs.GET_WORD_BY_CATEGORY_URL, JsonUtility.ToJson(new ReqModel(new CategoryWordsModel("C01"))), (data) =>
            {
                try
                {
                    GVs.CATEGORY_WORD_LIST_MODEL.Add(JsonUtility.FromJson<CategoryWordsModel>(data));
                    Debug.Log(data);
                    GFs.SaveCategories();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            });

            HTTPRequest.Instance.Request(GVs.GET_JUNIOR_CATEGORY_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
            {
                try
                {
                    GVs.JUNIOR_CATEGORY_LIST_MODEL = JsonUtility.FromJson<CategoryListModel>(data);
                    Debug.Log(data);
                    GFs.SaveJunior();
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            });

            if (!GVs.CATEGORY_LIST_DOWNLOADED_MODEL.IsDownloaded("C01"))
            {
                DownloadCategory("C01");
            }
            else
            {
                DownLoadML();
            }
        }

        public void DownloadCategory(string cat)
        {
            HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_CATEGORY, cat))), (data, process) =>
            {
                uiDownload.value = process;
                text.text = data;
                if (process == 1 || process == -404)
                {
                    if (process == 1)
                    {
                        GVs.CATEGORY_LIST_DOWNLOADED_MODEL.Downloaded(cat);
                        GFs.SaveCategories();
                    }
                    DownLoadML();
                }

            });
        }

        private void DownLoadML()
        {
            if (NET.NetWorkIsAvaiable())
                HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_ML))), (data, process) =>
               {
                   if (process == 1 || process == -404)
                   {
                       DownloadAvatas();
                   }
               });
        }

        private void DownloadAvatas()
        {
            HTTPRequest.Instance.Request(GVs.GET_ALL_AVATA_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
            {
                try
                {
                    GVs.AVATA_LIST_MODEL = (JsonUtility.FromJson<AvataListModel>(data));
                    GFs.SaveAvatas();
                    uiDownload.value = 0;
                    text.text = "Cập nhật dữ liệu";
                    HTTPRequest.Instance.Download(GVs.DOWNLOAD_URL, JsonUtility.ToJson(new ReqModel(new DownloadModel(DownloadModel.DOWNLOAD_AVATAS))), (data2, process) =>
                    {
                        uiDownload.value = process;
                        if (process == 1 || process == -404)
                        {
                            StartHome();
                        }
                    });
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            });
        }


        private void StartHome()
        {
            GVs.SCENE_MANAGER.StartHomeScene();
        }

        // Update is called once per frame
        void Update()
        {

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
        */
}
