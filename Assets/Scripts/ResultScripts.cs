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
    private bool isPlaying = false;
    private Color32[] buffer;
    public RawImage rimg;
    public GameObject panel;
    public GameObject btnPlay;
    public Canvas canvas;
    public Button BackButton;
    public Text tit;
    public RawImage rimgTitle;
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
        }
        else btnPlay.SetActive(false);
    }

    private void OnDisable()
    {       
        if (cancel != null)
            cancel.Dispose();
        videoPath = null;
        if (texture != null)
            Destroy(texture);
        if (texVideo != null)
            Destroy(texVideo);
    }

    IEnumerator Worker()
    {        
        isPlaying = true;
        if (string.IsNullOrEmpty(videoPath)) yield break;
        cap = new VideoCapture(videoPath);
        cap.open(videoPath);

        if (!cap.isOpened())
        {
            
            yield break;
        }        
        btnPlay.SetActive(false);
        cap.read(frame);        
        rawImageAspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        if (frame.empty())
        { 
            rawImageAspect.aspectRatio = ratioImage;
            cap.release();
            yield break;
        }

        float ratioVideo = (float)frame.width() / (float)frame.height();        
        rawImageAspect.aspectRatio = ratioVideo;

        texVideo = new Texture2D(frame.width(), frame.height(), TextureFormat.BGRA32, false);
        buffer = new Color32[frame.width() * frame.height()];
        for (;;)
        {
            yield return null;

            Debug.Log(frame == null);
            cap.read(frame);
            if (frame.empty())
            {                                
                break;
            }

            Imgproc.cvtColor(frame, frame, Imgproc.COLOR_BGRA2RGBA);
            Utils.matToTexture2D(frame, texVideo, buffer);
            rimg.texture = texVideo;              
        }
        cap.release();                       
        rawImageAspect.aspectRatio = ratioImage;
        rimg.texture = texture;
        btnPlay.SetActive(true);
        isPlaying = false;
        yield return null;
    }

    Coroutine worker;
    IDisposable cancel;
    public void OnPlayBtnClicked()
    {
        if (isPlaying) return;
        //MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
        //worker = MainThreadDispatcher.StartCoroutine(Worker());
        //worker = StartCoroutine(Worker());
        cancel = Observable.FromMicroCoroutine(Worker).Subscribe();
    }
}
