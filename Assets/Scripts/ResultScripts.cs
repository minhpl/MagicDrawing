using OpenCVForUnity;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
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
    public Text tit;
    private Texture2D texVideo;
    public enum MODE { FISRT_RESULT,REWATCH_RESULT};
    public static MODE mode;    

    private void Awake()
    {
        if (mode == MODE.REWATCH_RESULT)
        {
            tit.text = title;
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
            for (;;)
            {
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
}
