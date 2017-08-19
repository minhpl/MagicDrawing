using OpenCVForUnity;
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
    public enum MODE { FISRT_RESULT,REWATCH_RESULT};
    public static MODE mode;
    private int FPS = 60;
    private float currentFPS = 0;
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
        Debug.Log("Mono's Start function [ResultScripts]");
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var ratioDisplay = (float)canvasRect.width / canvasRect.height;
        var panelAspect = panel.GetComponent<AspectRatioFitter>();
        panelAspect.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
        panelAspect.aspectRatio = ratioDisplay;
        if(texture!=null)
        {
            float ratio = (float)texture.width /(float) texture.height;
            var rawImageAspect = rimg.GetComponent<AspectRatioFitter>();
            rawImageAspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            rawImageAspect.aspectRatio = ratio;
            rimg.texture = texture;
            var scale = 1 + GVs.ridTopPercent;
            rimg.rectTransform.localScale = new Vector3(scale, scale, scale);
        }
        if (!string.IsNullOrEmpty(videoPath))
        {
            Debug.LogFormat("Video Path is {0}", videoPath);
            btnPlay.SetActive(true);
        }
        else btnPlay.SetActive(false);

    }
    private void OnDisable()
    {
        videoPath = null;
        if(texture!=null)
            Destroy(texture);
        if(texVideo!=null)
            Destroy(texVideo);
    }

    IEnumerator Worker()
    {
        Utilities.Log("heere1");
        isPlaying = true;
        if (string.IsNullOrEmpty(videoPath)) yield break;
        cap = new VideoCapture(videoPath);
        cap.open(videoPath);

        if (!cap.isOpened())
        {
            yield break;
        }
        Utilities.Log("heere2");
        btnPlay.SetActive(false);
        Mat frame = new Mat();
        float a = 0;
        for (;;)
        {
            cap.read(frame);
            if (frame.empty())
            {                
                Utilities.Log("heere5");
                break;
            }

            currentFPS = 1 / Time.deltaTime;
            if (a < 1)
            {
                if (currentFPS > 0)
                    a = a + FPS / currentFPS - 1;
            }

            while (a > 1)
            {
                a = a - 1;
                continue; 
            }

            Utilities.Log("heere3");
            yield return null;
                       
            if (texVideo == null)
            {
                texVideo = new Texture2D(frame.width(), frame.height(), TextureFormat.BGRA32, false);
                buffer = new Color32[frame.width() * frame.height()];
            }
            Imgproc.cvtColor(frame, frame, Imgproc.COLOR_BGRA2RGBA);
            Utils.matToTexture2D(frame, texVideo, buffer);
            rimg.texture = texVideo;              
        }

        cap.release();
        Utilities.Log("heere5");
        isPlaying = false;
        btnPlay.SetActive(true);
        rimg.texture = texture;
        Utilities.Log("heere6");
        yield return null;
    }

    //void playVideo()
    //{
    //    var heavyMethod2 = Observable.Start(() =>
    //    {
    //        cap = new VideoCapture(videoPath);
    //        cap.open(videoPath);
    //    });

    //}

    public void OnPlayBtnClicked()
    {
        if (isPlaying) return;
        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }

}
