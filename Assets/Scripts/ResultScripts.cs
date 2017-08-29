using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultScripts : MonoBehaviour {
    public static string videoPath = null;
    public static Texture2D texture;
    public static string title;
    //public static Size sizeVideo;
    //public static float ratioVideo = 1;
    VideoCapture cap;    
    private Color32[] buffer;
    public RawImage rimg;
    public GameObject panel;
    public GameObject btnPlay;
    public Canvas canvas;
    public Button BackButton;
    public Text tit;
    public RawImage rimgTitle;
    public MoviePlayer moviePlayer;
    public Button btnShareFacebooks;
    private Texture2D texVideo;
    private Mat frame;
    private AspectRatioFitter rawImageAspect;
    public enum MODE { FISRT_RESULT,REWATCH_RESULT};
    public static MODE mode;
    private int FPS = 60;
    private float currentFPS = 0;
    private float ratioImage = 1;
    private void Awake()
    {
        if (mode == MODE.REWATCH_RESULT)
        {
            tit.text = title;
            rimgTitle.gameObject.SetActive(false);
            tit.gameObject.SetActive(true);
        }        
        if(mode==MODE.FISRT_RESULT && BackButton!=null)
        {
            rimgTitle.gameObject.SetActive(true);
            tit.gameObject.SetActive(false);
            BackButton.onClick = new Button.ButtonClickedEvent();
            BackButton.onClick.AddListener(() =>
            {
                GVs.TRACE_SCENE.Pop();
                GVs.TRACE_SCENE.Pop();
                GVs.TRACE_SCENE.Pop();
                int i = GVs.TRACE_SCENE.Pop();                
                SceneManager.LoadScene(i);
            });           
        }

        btnShareFacebooks.onClick.AddListener(() =>
        {
            ShareFacebook.filePath = videoPath;
            var shareFacebook = GetComponent<ShareFacebook>();
            shareFacebook.onlogin();
        });
    }

    void Start () {
        rawImageAspect = rimg.GetComponent<AspectRatioFitter>();
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var ratioCanvas = (float)canvasRect.width / canvasRect.height;
        ratioImage = ratioCanvas;
        var panelAspect = panel.GetComponent<AspectRatioFitter>();
        panelAspect.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
        panelAspect.aspectRatio = ratioCanvas;
        if(texture!=null)
        {            
            rimg.texture = texture;
        }
        if (!string.IsNullOrEmpty(videoPath))
        {
            frame = new Mat();
            btnPlay.SetActive(true);
            moviePlayer.Load(videoPath);
            moviePlayer.play = false;
            moviePlayer.loop = true;
        }
        else btnPlay.SetActive(false);    
    }

    private void OnDisable()
    {       
        videoPath = null;        
        if (texture != null)
            Destroy(texture);
        if (texVideo != null)
            Destroy(texVideo);
        moviePlayer.Unload();
        ShareFacebook.filePath = null;
    }

    public void OnPlayBtnClicked()
    {
        rimg.texture = null;
        var isPlay = moviePlayer.play;
        if (isPlay == false)
        { 
            moviePlayer.play = true;
        }
        else
        {
            moviePlayer.play = false;            
        }        
    }
}
