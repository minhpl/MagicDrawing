using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class HTTPRequest : MonoBehaviour
{
    public static string ERROR = "error";
    public static HTTPRequest Instance
    {
        get;
        private set;
    }
    public delegate void Handler(string data);
    public delegate void HandlerDownload(string data, float process = -1f);

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #region API
    /// <summary>
    /// Send HTTP request to specified url
    /// </summary>
    /// <param name="url">URL.</param>
    /// <param name="handler">Handler.</param>
    public void Request(string url, string reqModel, Handler handler, Action timeOutCallback = null, float timeOut = 20f)
    {
        StartCoroutine(RequestProcessor(url, reqModel, handler, timeOutCallback, timeOut));
    }

    public void Download(string url, string reqModel, HandlerDownload handler, Action timeOutCallback = null, float timeOut = 20f)
    {
        StartCoroutine(RequestDơnloader(url, reqModel, handler, timeOutCallback, timeOut));
    }


    #endregion

    #region Processor
    private IEnumerator RequestProcessor(string url, string reqModel, Handler handler, Action timeOutCallback, float timeOut = 20f)
    {
        WWWForm form = new WWWForm();
        string data = reqModel;
        data = AesEncryptor.encode(data, GVs.KEY, GVs.IV);
        form.AddField("data", data);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.Send();

        while (!www.isDone && timeOut > 0)
        {
            timeOut -= Time.deltaTime;
            yield return null;
        }

        if (timeOut <= 0)
        {
            if (timeOutCallback != null)
                timeOutCallback();
            else
                handler("time out");
            yield break;
        }

        string error = www.error;
        if (string.IsNullOrEmpty(error))
        {
            if (handler != null)
            {
                try
                {

                    if (url.Equals(GVs.DOWNLOAD_URL))
                    {
                        if (www.downloadedBytes != 0)
                        {
                            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/file.ios", www.downloadHandler.data);
                        }
                        else
                        {
                            handler("zero data");
                        }
                    }
                    else
                    {
                        if (www.downloadedBytes != 0)
                        {
                            //Debug.Log("call success");
                            string resData = www.downloadHandler.text;
                            resData = AesEncryptor.decode(resData, GVs.KEY, GVs.IV);
                            handler(resData);
                        }
                        else
                        {
                            handler("zero data");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    handler("null data");
                }
            }
        }
        else
        {
            handler("null data");
        }

        www.Dispose();
    }



    private IEnumerator RequestDơnloader(string url, string reqModel, HandlerDownload handler, Action timeOutCallback, float timeOut = 20f)
    {
        WWWForm form = new WWWForm();
        string data = reqModel;
        data = AesEncryptor.encode(data, GVs.KEY, GVs.IV);
        form.AddField("data", data);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.Send();

        while (!www.isDone)
        {
            timeOut -= Time.deltaTime;

            if (www.downloadProgress > 0.95)
                handler("Xử lý dữ liệu", www.downloadProgress);
            else
                handler("Đang tải dữ liệu", www.downloadProgress);
            yield return null;
        }
        if (www.isDone)
        {
        }
        if (timeOut <= 0)
        {
            // if (timeOutCallback != null)
            //     timeOutCallback();
            // else
            //     handler("time out");
            // yield break;
        }
        string error = www.error;
        if (www.responseCode == 200)
        {
            if (string.IsNullOrEmpty(error))
            {
                if (handler != null)
                {
                    try
                    {
                        if (www.downloadedBytes != 0)
                        {
                            string sInfo = www.GetResponseHeader("downloadInfo");
                            sInfo = AesEncryptor.decode(sInfo, GVs.KEY, GVs.IV);
                            DownloadModel downloadInfo = JsonUtility.FromJson<DownloadModel>(sInfo);
                            string fn = downloadInfo.filename;
                            string dir = downloadInfo.dir;
                            bool compress = downloadInfo.compress;
                            // Debug.Log(DateTime.Now.Second);
                            if (!Directory.Exists(GVs.APP_PATH + "/" + dir))
                            {
                                Directory.CreateDirectory(GVs.APP_PATH + "/" + dir);
                            }
                            File.WriteAllBytes(GVs.APP_PATH + "/" + dir + fn, www.downloadHandler.data);
                            // Debug.Log(DateTime.Now.Second);
                            if (compress)
                            {
                                ZipUtil.Unzip(GVs.APP_PATH + "/" + dir + fn, GVs.APP_PATH + "/" + dir);
                                File.Delete(GVs.APP_PATH + "/" + dir + fn);
                            }
                            else
                            {
                                Debug.Log("download ml: " + (GVs.APP_PATH + "/" + dir + fn));
                            }
                            //Debug.Log(GVs.APP_PATH);
                            // Debug.Log(DateTime.Now.Second);
                            handler("Hoàn thành", 1);
                        }
                        else
                        {
                            handler("zero data");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        handler("null data");
                    }
                }
            }
            else
            {
                handler("null data");
            }
        }
        else
        {
            handler("Dữ liệu không tồn tại", -404);
        }
        www.Dispose();
    }
    #endregion
}
