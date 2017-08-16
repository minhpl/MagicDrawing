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
        }        
        if(mode==MODE.FISRT_RESULT && BackButton!=null)
        {
            BackButton.onClick = new Button.ButtonClickedEvent();
            BackButton.onClick.AddListener(() =>
            {
                GVs.TRACE_SCENE.Pop();
                GVs.TRACE_SCENE.Pop();
                GVs.TRACE_SCENE.Pop();
                int i = GVs.TRACE_SCENE.Pop();                
                SceneManager.LoadScene(i);
            });
            //BackButton.onClick.RemoveAllListeners();
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
        Destroy(texture);
        Destroy(texVideo);
    }

    IEnumerator Worker()
    {
        isPlaying = true;
        bool isPlayable = true;
        if (string.IsNullOrEmpty(videoPath)) isPlayable = false;
        cap = new VideoCapture(videoPath);
        cap.open(videoPath);
        if (!cap.isOpened())
        {
            isPlayable = false;
        }      

        if (isPlayable)
        {            
            btnPlay.SetActive(false);
            Mat frame = new Mat();

            //int count = 0;
            float a = 0;
            for (;;)
            {  
                currentFPS = 1 / Time.deltaTime;
                if (a < 1)
                {
                    a = a + FPS / currentFPS - 1;
                }
                
                if(a>1)
                {
                    a = a - 1;
                    continue;
                }                

                yield return null;
                cap.read(frame);                
                if (frame.empty())
                {
                    break;
                }
                if (texVideo == null) {
                    texVideo = new Texture2D(frame.width(), frame.height(), TextureFormat.BGRA32, false);
                    buffer = new Color32[frame.width() * frame.height()];
                }
                Imgproc.cvtColor(frame, frame, Imgproc.COLOR_BGRA2RGBA);
                Utils.matToTexture2D(frame, texVideo, buffer);
                rimg.texture = texVideo;                
            }
        }
        cap.release();
        isPlaying = false;
        btnPlay.SetActive(true);        
        rimg.texture = texture;
        yield return null;
    }
    public void OnPlayBtnClicked()
    {
        if (isPlaying) return;
        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }

    public void OnBackBtnClicked()
    {
        
    }
}
