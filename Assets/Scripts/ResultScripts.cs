﻿using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Facebook.Unity;

public class ResultScripts : MonoBehaviour
{
    public static string videoPath = null;
    public static string imagePath = null;
    public static string animPath = null;
    public static Texture2D texture;
    public static string title;

    private Color32[] buffer;
    public RawImage rimg;
    public GameObject panel;
    public GameObject btnPlay;
    public Button btnStop;
    public Canvas canvas;
    public Button BackButton;
    public UnityEngine.UI.Text tit;
    public RawImage rimgTitle;
    public MoviePlayer moviePlayer;
    public Button btnShareFB;
    public Button btnDelete;
    public GameObject Pnl_Popup;
    public Button OKButton;
    public Button logoutBtn;
    public GameObject overlayer_comfirmDelete;
    public GameObject panel_comfirmDelete;
    public Button btn_okDelete;
    public Button btn_cancelDelete;
    public AudioSource audioSource;
    public Button backBtn;
    public GameObject pnlShareFB;

    private Texture2D texVideo;
    private Mat frame;
    private AspectRatioFitter rawImageAspect;
    public enum MODE { FISRT_RESULT, REWATCH_RESULT, ANIM};
    public static MODE mode;
    private int FPS = 60;
    private float currentFPS = 0;
    private float ratioImage = 1;

    private IDisposable cancelCorountineBackButtonAndroid;
    LTDescr ltdescr_ScaleComfirmDeletePanel;
    LTDescr ltdescr_AlphaComfirmDeletePanel;
    private void Awake()
    {
        if (GVs.SOUND_SYSTEM == 1)
            audioSource.Play();

        backBtn.onClick.AddListener(() =>
        {
            Debug.Log("fuck all");
        });

        btnDelete.onClick.AddListener(() =>
        {
            overlayer_comfirmDelete.SetActive(true);
            ltdescr_ScaleComfirmDeletePanel = LeanTween.scale(panel_comfirmDelete, new Vector3(1f, 1f, 1f), GVs.DURATION_TWEEN_UNIFY)
            .setEase(LeanTweenType.easeOutElastic)
                .setRepeat(2).setLoopPingPong()
                .setOnComplete(() =>
                {
                    ltdescr_ScaleComfirmDeletePanel.pause();
                    ltdescr_ScaleComfirmDeletePanel.setEase(LeanTweenType.easeInQuart);
                }).setOnCompleteOnRepeat(true);

            ltdescr_AlphaComfirmDeletePanel = LeanTween.alpha(panel_comfirmDelete.GetComponent<RectTransform>(), 1, GVs.DURATION_TWEEN_UNIFY).setFrom(0)
            .setRepeat(2).setLoopPingPong().setEase(LeanTweenType.easeOutElastic)
            .setOnComplete(() =>
            {
                ltdescr_AlphaComfirmDeletePanel.pause();
            }).setOnCompleteOnRepeat(true);
        });

        btn_okDelete.onClick.AddListener(() =>
        {
            var a = LeanTween.sequence();
            a.append(ltdescr_ScaleComfirmDeletePanel.resume());
            a.append(() =>
            {
                overlayer_comfirmDelete.SetActive(false);
                moviePlayer.Unload();
                File.Delete(imagePath);
                if (File.Exists(videoPath))
                {
                    File.Delete(videoPath);
                }
                GFs.BackToPreviousScene();
            });
            ltdescr_AlphaComfirmDeletePanel.resume();
        });

        btn_cancelDelete.onClick.AddListener(() =>
        {
            var a = LeanTween.sequence();
            a.append(ltdescr_ScaleComfirmDeletePanel.resume());
            a.append(() =>
            {
                overlayer_comfirmDelete.SetActive(false);
            });
            ltdescr_AlphaComfirmDeletePanel.resume();
        });

        OKButton.onClick.AddListener(() =>
        {
            Pnl_Popup.SetActive(false);
        });

        btnPlay.GetComponent<Button>().onClick.AddListener(() =>
        {
            if(mode == MODE.FISRT_RESULT || mode == MODE.REWATCH_RESULT) { 
                btnStop.gameObject.SetActive(true);
                btnPlay.gameObject.SetActive(false);
            }
        });

        btnStop.GetComponent<Button>().onClick.AddListener(() =>
        {
            btnStop.gameObject.SetActive(false);
            btnPlay.gameObject.SetActive(true);
        });

        btnStop.GetComponent<Button>().onClick.AddListener(() =>
        {
            moviePlayer.play = false;
            moviePlayer.videoFrame = 0;
            rimg.texture = texture;
        });
        
        btnShareFB.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(videoPath))
            {
                ShareFacebook.ShareMODE = ShareFacebook.mode.SHARE_VIDEO;
                ShareFacebook.filePath = videoPath;
            }
            else if(!string.IsNullOrEmpty(animPath))
            {
                ShareFacebook.ShareMODE = ShareFacebook.mode.SHARE_VIDEO;
                ShareFacebook.filePath = animPath;
            }
            else
            {
                ShareFacebook.ShareMODE = ShareFacebook.mode.SHARE_IMAGE;
                ShareFacebook.filePath = imagePath;
            }
            var isVideoExist = File.Exists(videoPath);
            Debug.LogFormat("is video exist ?? {0}", isVideoExist);
            var shareFacebook = GetComponent<ShareFacebook>();
            pnlShareFB.gameObject.SetActive(true);


            if (!FB.IsInitialized)
            {
                FB.Init(shareFacebook.InitCallback, shareFacebook.OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
                FB.Mobile.ShareDialogMode = ShareDialogMode.WEB;
            }

            if (FB.IsLoggedIn)
            {
                shareFacebook.onLoggedInSuccess();
            }
            else
            {
                shareFacebook.onlogin();
            }
        });

        logoutBtn.onClick.AddListener(() =>
        {
            Debug.Log("facebook logout clicked");
            Debug.LogFormat("Before call logout, isloggedIn = {0}", FB.IsLoggedIn);
            FB.LogOut();
            Debug.LogFormat("After call logout, isloggedIn = {0}", FB.IsLoggedIn);
        });

        if (mode == MODE.REWATCH_RESULT)
        {
            tit.text = title;
            rimgTitle.gameObject.SetActive(false);
            tit.gameObject.SetActive(true);
        }

        if (mode == MODE.FISRT_RESULT)
        {
            btnDelete.gameObject.SetActive(false);
        }
        else
        {
            btnDelete.gameObject.SetActive(true);
        }

        Debug.Log(logoutBtn == null);


        moviePlayer.OnStop += MoviePlayer_OnStop;

        if (Application.platform == RuntimePlatform.Android)
        {
            cancelCorountineBackButtonAndroid = Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) == true)
                .Subscribe(_ =>
                {
                    if (mode == MODE.FISRT_RESULT)
                    {
                        if (GVs.TRACE_SCENE.Count > 3)
                        {
                            GVs.TRACE_SCENE.Pop();
                            GVs.TRACE_SCENE.Pop();
                            int i = GVs.TRACE_SCENE.Pop();
                            Debug.LogFormat("track scene have {0} elements", GVs.TRACE_SCENE.Count);
                            SceneManager.LoadScene(i);
                        }
                    }
                    else
                    {
                        GFs.BackToPreviousScene();
                    }
                });
        }
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(videoPath))
        {
            var fileInfo = new FileInfo(videoPath);
            var bytes = fileInfo.Length;
            var kb = bytes >> 10;
            var mb = kb >> 10;
            Debug.LogFormat("File size is {0} bytes, {1} kb, {2} mb", bytes, kb, mb);
        }

        if (mode == MODE.FISRT_RESULT)
        {
            rimgTitle.gameObject.SetActive(true);
            tit.gameObject.SetActive(false);
            BackButton.onClick.RemoveAllListeners();
            BackButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            BackButton.onClick.AddListener(() =>
            {
                if (GVs.TRACE_SCENE.Count > 3)
                {
                    GVs.TRACE_SCENE.Pop();
                    GVs.TRACE_SCENE.Pop();
                    int i = GVs.TRACE_SCENE.Pop();
                    Debug.LogFormat("track scene have {0} elements", GVs.TRACE_SCENE.Count);
                    SceneManager.LoadScene(i);
                }
            });
        }

        rawImageAspect = rimg.GetComponent<AspectRatioFitter>();
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var canvasWidth = canvasRect.width;
        var ratioCanvas = (float)canvasRect.width / canvasRect.height;
        ratioImage = ratioCanvas;
        var panelAspect = panel.GetComponent<AspectRatioFitter>();
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasWidth * 0.68f, 1);
        panelAspect.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
        panelAspect.aspectRatio = ratioCanvas;

        if (texture != null)
        {
            rimg.texture = texture;
        }

        if (mode == MODE.FISRT_RESULT || mode == MODE.REWATCH_RESULT)
        {
            if (!string.IsNullOrEmpty(videoPath))
            {
                frame = new Mat();
                btnPlay.SetActive(true);
                moviePlayer.Load(videoPath);
                moviePlayer.play = false;
                moviePlayer.loop = false;
            }
            else btnPlay.SetActive(false);
        }else if(mode == MODE.ANIM)
        {
            btnPlay.SetActive(true);
        }
    }


    private void OnDisable()
    {
        if (cancelCorountineBackButtonAndroid != null)
            cancelCorountineBackButtonAndroid.Dispose();
        videoPath = null;
        if (texture != null)
            Destroy(texture);
        if (texVideo != null)
            Destroy(texVideo);
        Debug.Log(moviePlayer != null);
        {
            try
            {
                moviePlayer.play = false;
                moviePlayer.loop = false;
                moviePlayer.Unload();
            }
            catch (Exception e)
            {
                Debug.Log("Error" + e.ToString());
            }
        }
        ShareFacebook.filePath = null;
    }

    public void OnPlayBtnClicked()
    {
        if (mode == MODE.FISRT_RESULT || mode == MODE.REWATCH_RESULT)
        {
            rimg.texture = null;
            var isPlay = moviePlayer.play;
            if (isPlay == false)
            {
                moviePlayer.loop = true;
                moviePlayer.play = true;
                moviePlayer.OnLoop += MoviePlayer_OnLoop;
            }
        }
        else if(mode == MODE.ANIM)
        {
            Handheld.PlayFullScreenMovie(animPath);
        }
    }

    private void MoviePlayer_OnPlay(MoviePlayerBase caller)
    {
        moviePlayer.loop = false;
    }

    private void MoviePlayer_OnLoop(MoviePlayerBase caller)
    {
        moviePlayer.loop = false;
        moviePlayer.play = false;
    }

    private void MoviePlayer_OnStop(MoviePlayerBase caller)
    {
        rimg.texture = texture;
        btnStop.gameObject.SetActive(false);
        btnPlay.gameObject.SetActive(true);
    }
}
