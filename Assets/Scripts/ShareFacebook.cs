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
    private Texture2D texture2DFacebookAvartar;
    void Awake()
    {
        if (!FB.IsInitialized)
        {            
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {            
            FB.ActivateApp();
            FB.Mobile.ShareDialogMode = ShareDialogMode.AUTOMATIC;
            FB.Mobile.RefreshCurrentAccessToken((IAccessTokenRefreshResult result) =>
            {
                if (FB.IsLoggedIn)
                {
                    Debug.Log(result.AccessToken.ExpirationTime.ToString());
                }
            });
        }

        btnOk.onClick.AddListener(() =>
        {
            StartCoroutine(StartVideoUpload());
            panelInputShareFacebook.SetActive(false);
            
        });

        btnCancel.onClick.AddListener(() =>
        {            
            panelInputShareFacebook.SetActive(false);
        });

        btnLogoutFacebook.onClick.AddListener(() =>
        {
            cancelCorountineLogOut = Observable.FromCoroutine(LogOut).Timeout(TimeSpan.FromSeconds(3)).Subscribe(_ => { }, _ =>
            {
                Debug.LogFormat("Error is: {0}", _.ToString());
            }, ()=>
            {
                Debug.LogFormat("Logout successfull hehehe");
            });            
        });
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
        Debug.Log("LogOut is successfully");
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {         
            FB.ActivateApp();
            FB.Mobile.ShareDialogMode = ShareDialogMode.AUTOMATIC;
            FB.Mobile.RefreshCurrentAccessToken((IAccessTokenRefreshResult result)=>
            {
                if (FB.IsLoggedIn)
                {
                    Debug.Log(result.AccessToken.ExpirationTime.ToString());
                }
            });            
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
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
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, callbackPublicLogin);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            var check = 0;
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
                if (perm.Equals("publish_actions"))
                {
                    check = 1;

                }
            }            
            if (check == 1)
            {
                StartCoroutine(StartVideoUpload());
            }
            else
            {
                CallFBLoginForPublish();
            }
        }
        else
        {
            Debug.Log("User cancelled login" + result.Error);
        }
    }

    private void CallFBLoginForPublish()
    {
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, callbackPublicLogin);
    }

    private void callbackPublicLogin(ILoginResult result)
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
            FB.API("/me?fields=name", HttpMethod.GET, (IGraphResult a)=>
                {
                    Debug.LogFormat("here1 result is: {0}", a.RawResult);
                    txtFacebookName.text = (string)a.ResultDictionary["name"];
                });

            FB.API("me/picture", HttpMethod.GET, (IGraphResult a) =>
            {
                Debug.LogFormat("here2 result is: {0}", a.RawResult);
                avartarFacebook.texture = a.Texture;
            });

            panelInputShareFacebook.SetActive(true);
        }
    }

    public static string filePath;

    private IEnumerator StartVideoUpload()
    {
        progressSlider.gameObject.SetActive(true);
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
            wwwForm.AddField("description",message);
            wwwForm.AddField("title",message);
        }        
        FB.API("me/videos", HttpMethod.POST, HandleResult, wwwForm);
    }

    void HandleResult(IResult result)
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
            
            Debug.Log("OK Boy:\n" + result.RawResult);
            Debug.LogFormat("Result tostring is {0}", result.ToString());                                                    
        }
    }


    
    private void Update()
    {           
        //avartarFacebook.texture = texture2DFacebookAvartar;
    }

    private void OnDisable()
    {
        cancelCorountineLogOut.Dispose();
    }
}
