using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ShareFacebook : MonoBehaviour
{
    public Slider progressSlider;
    public GameObject panelInputShareFacebook;
    public InputField InputCommentFacebook;
    public Button btnOk;
    public Button btnCancel;
    public RawImage avartarFacebook;
    public Text txtFacebookName;
    public Button btnLogoutFacebook;
    public GameObject panelNotification;
    public Text textNotification;
    public GameObject gameObjectPlayerShareFB;
    public GameObject btnStopPlayerShareFB;
    public GameObject overlayProcessShareFB;
    public Button btnCancelUploadFB;

    private IDisposable cancelStreamHideNotificationPanel;
    private IObservable<float> streamHideNotificationPanel;
    private float timeOutSeconds = 4f;
    public static string filePath;
    public enum mode { SHARE_VIDEO, SHARE_IMAGE};
    public static mode ShareMODE = mode.SHARE_VIDEO;
    void Awake()
    {
        
        if (!FB.IsInitialized)
        {            
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {            
            FB.ActivateApp();
            FB.Mobile.ShareDialogMode = ShareDialogMode.WEB;
        }

        btnOk.onClick.AddListener(() =>
        {
            overlayProcessShareFB.SetActive(true);
            btnOk.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(false);
            StartCoroutine(StartUpload());            
        });

        btnCancel.onClick.AddListener(() =>
        {            
            panelInputShareFacebook.SetActive(false);
        });

        btnLogoutFacebook.onClick.AddListener(() =>
        {
            panelInputShareFacebook.SetActive(false);
            cancelCorountineLogOut = Observable.FromCoroutine(LogOut).Timeout(TimeSpan.FromSeconds(3)).Subscribe(_ => { }, _ =>
            {
                Debug.LogFormat("Error is: {0}", _.ToString());
            }, ()=>
            {
                Debug.LogFormat("Logout successfull hehehe");
            });            
        });

        btnCancelUploadFB.onClick.AddListener(() =>
        {
            var go = GameObject.Find("UnityFacebookSDKPlugin");
            Debug.Log(go.gameObject.name);
            Destroy(go);
        });

        streamHideNotificationPanel = Observable.Create<float>((IObserver<float> observer) =>
        {
            return Disposable.Empty;
        });

        var rimgPlayerShareFB = gameObjectPlayerShareFB.GetComponent<RawImage>();
        rimgPlayerShareFB.texture = ResultScripts.texture;
        var aspectRatioFitterPlayerShareFB = gameObjectPlayerShareFB.GetComponent<AspectRatioFitter>();
        aspectRatioFitterPlayerShareFB.aspectRatio = (float)Screen.width / (float)Screen.height;
        var btnPlayerShareFB = gameObjectPlayerShareFB.GetComponent<Button>();
        if (!string.IsNullOrEmpty(ResultScripts.videoPath))
        {
            btnStopPlayerShareFB.SetActive(true);
            var moviePlayer = gameObjectPlayerShareFB.GetComponent<MoviePlayer>();
            moviePlayer.Load(ResultScripts.videoPath);
            moviePlayer.play = false;
            moviePlayer.loop = false;
            btnPlayerShareFB.onClick.AddListener(() =>
            {
                btnStopPlayerShareFB.SetActive(!btnStopPlayerShareFB.activeSelf);
            });

            btnPlayerShareFB.onClick.AddListener(() =>
            {
                if (btnStopPlayerShareFB.activeSelf == false)
                {
                    moviePlayer.play = true;
                    moviePlayer.loop = true;
                    rimgPlayerShareFB.texture = null;
                }
                else
                {
                    moviePlayer.play = false;
                    moviePlayer.videoFrame = 0;
                    rimgPlayerShareFB.texture = ResultScripts.texture;
                }
            });
        }
        else
        {
            btnStopPlayerShareFB.gameObject.SetActive(false);
        }
    }

    IDisposable cancelCorountineLogOut;

    IEnumerator LogOut()
    {
        yield return null;
        FB.LogOut();
        while (FB.IsLoggedIn)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Debug.LogFormat("LogOut is successfully?  {0}", !FB.IsLoggedIn);
    }

    public void InitCallback()
    {
        if (FB.IsInitialized)
        {         
            FB.ActivateApp();
            FB.Mobile.ShareDialogMode = ShareDialogMode.WEB;
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    public void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {           
            Time.timeScale = 0;
        }
        else
        {         
            Time.timeScale = 1;
        }
    }

    public void onlogin()
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };             
        FB.LogInWithPublishPermissions(new List<string>() {"publish_actions"}, callbackLoginWithPubplishPerm);
    }


    public void callbackLoginWithPubplishPerm(ILoginResult result)
    {
        if (result == null)
        {
            Debug.Log("null");
            return;
        }

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error Response:\n" + result.Error);
        }
        else if (result.Cancelled)
        {
            Debug.Log("Cancelled Response:\n" + result.RawResult);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            onLoggedInSuccess();
        }
    }   

    public void onLoggedInSuccess()
    {
        Debug.Log("here1");
        FB.API("/me?fields=name", HttpMethod.GET, (IGraphResult a) =>
        {         
            var name = (string)a.ResultDictionary["name"];
            txtFacebookName.text = name;
            Debug.LogFormat("name is {0}", name);
        });

        FB.API("me/picture", HttpMethod.GET, (IGraphResult a) =>
        {          
            avartarFacebook.texture = a.Texture;
        });

        btnOk.gameObject.SetActive(true);
        btnCancel.gameObject.SetActive(true);
        panelInputShareFacebook.SetActive(true);
        InputCommentFacebook.text = null;        
    }

    private IEnumerator StartUpload()
    {        
        yield return new WaitForEndOfFrame();
        var url = "file:///" + filePath;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url = "file://" + filePath;
        }
        Debug.Log("url is: " + url);
        WWW www = new WWW(url);
        while (!www.isDone)
        {
            yield return null;
            Debug.Log("progress : " + www.progress);
            progressSlider.value = www.progress;
        }

        Debug.Log("size : " + www.size / 1024 / 1024);
        var wwwForm = new WWWForm();
        wwwForm.AddBinaryData("filevideo", www.bytes, "Video.MOV", "multipart/form-data");
        var message = InputCommentFacebook.text;

        if (!string.IsNullOrEmpty(message))
        {
            
        }

        if (ShareMODE == mode.SHARE_VIDEO)
        {
            wwwForm.AddField("description", message);
            wwwForm.AddField("title", message);
            FB.API("me/videos", HttpMethod.POST, HandleResultUploadVideo, wwwForm);
        }
        else
        {            
            wwwForm.AddField("caption", message);
            FB.API("me/photos", HttpMethod.POST, HandleResultUploadVideo, wwwForm);
        }                
    }

    void HandleResultUploadVideo(IResult result)
    {
        if (result == null)
        {
            Debug.Log("null");
            return;
        }

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error Response here is:\n" + result.Error);
        }
        else if (result.Cancelled)
        {
            Debug.Log("Cancelled Response:\n" + result.RawResult);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {            
            Debug.Log("HandleResultUploadVideo: " + result.RawResult);
            Debug.LogFormat("Result tostring is {0}", result.ToString());

            overlayProcessShareFB.SetActive(false);
            panelInputShareFacebook.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if(cancelStreamHideNotificationPanel!=null)
            cancelStreamHideNotificationPanel.Dispose();
        if(cancelCorountineLogOut!=null)
            cancelCorountineLogOut.Dispose();
    }
}
