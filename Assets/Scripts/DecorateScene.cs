using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Facebook.Unity;
using System.Threading;

public class DecorateScene : MonoBehaviour
{
    public static string videoPath = null;
    public static string imagePath = null;
    public static string animPath = null;
    public static Texture2D texture;    
    public static string title;        
    public RawImage rimg;
    public GameObject panel;    
    public Canvas canvas;
    public Button BackButton;                  
    public AudioSource audioSource;
    public Button backBtn;
    public Button btnTest;
    public Camera cam;
    public GameObject imageItem;
    public GameObject khung;
    public RawImage khungAnh1;
    public RawImage khungAnh2;
    public Canvas canvasSnap;

    private Color32[] buffer;
    private Texture2D texVideo;
    private Mat frame;
    private AspectRatioFitter rawImageAspect;
    public enum MODE { FISRT_RESULT, REWATCH_RESULT, ANIM };
    public static MODE mode;
    private int FPS = 60;
    private float currentFPS = 0;
    private float ratioImage = 1;
    private GameObject tempPanel = null;
    private IDisposable cancelCorountineBackButtonAndroid;
    LTDescr ltdescr_ScaleComfirmDeletePanel;
    LTDescr ltdescr_AlphaComfirmDeletePanel;
    private void Awake()
    {
        GFs.LoadData();
        btnTest.onClick.AddListener(() =>
        {
            if (tempPanel != null)
                Destroy(tempPanel);
            tempPanel = Instantiate(panel);
            var rect = tempPanel.transform.GetComponent<RectTransform>().rect;
            tempPanel.transform.parent = canvasSnap.transform;
            tempPanel.transform.localScale = Vector3.one;          
            tempPanel.transform.localPosition = panel.transform.localPosition;
            tempPanel.GetComponent<RectTransform>().anchorMax = Vector2.one;
            tempPanel.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            tempPanel.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            tempPanel.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            ScreenshotHelper.iCaptureWithCamera(cam, (Texture2D txt) =>
             {
                 TextureScale.Bilinear(txt, texture.width, texture.height);                 
                 //var path = new FilePathName().SaveTextureAs(txt, FilePathName.SaveFormat.JPG);                 
                 ScreenshotHelper.iClear();
             });
        });

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
         if (texture != null)
        {
            rimg.texture = texture;

            panel.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;

        }
        loadFramesList();
    }

    void loadFramesList()
    {
        var framelist = GVs.listFrame.data;
        var dir = GVs.listFrame.dir;
        dir = GFs.getAppDataDirPath() + dir + "/";
        for (int i = 0; i < framelist.Count; i++)
        {
            var frame = framelist[i];
            var thumbPath = dir + frame.thumb;
            Texture2D texture = GFs.LoadPNGFromPath(thumbPath);
            GameObject clone = Instantiate(imageItem, imageItem.transform.parent);
            clone.SetActive(true);

            var thumb = clone.transform.Find("thumb");
            thumb.gameObject.GetComponent<RawImage>().texture = texture;
            thumb.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;

            clone.GetComponent<Button>().onClick.AddListener(() =>
            {
                var imgP = dir + frame.image1;
                var imgP2 = dir + frame.image2;
                Texture2D txImg = GFs.LoadPNGFromPath(imgP);
                Texture2D txImg2 = GFs.LoadPNGFromPath(imgP2);
                txImg.Compress(true);
                txImg2.Compress(true);
                Destroy(khungAnh1.texture);
                Destroy(khungAnh2.texture);
                khungAnh1.texture = txImg;
                khungAnh2.texture = txImg2;
                khung.GetComponent<RectTransform>().localScale = Vector3.one;
                khung.transform.localEulerAngles = Vector3.zero;
                khung.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                khung.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                var rect = khung.GetComponent<RectTransform>().rect;
                rect.position = Vector3.zero;
                khungAnh1.GetComponent<AspectRatioFitter>().aspectRatio = txImg.width / (float)txImg.height;
                khungAnh2.GetComponent<AspectRatioFitter>().aspectRatio = txImg2.width / (float)txImg2.height;
                khungAnh1.GetComponent<AspectRatioFitter>().enabled = true;
                khungAnh2.GetComponent<AspectRatioFitter>().enabled = true;
                khung.SetActive(true);
                //textureKhungAnh = txImg;
                //mode = SnapMode.FRAME;
            });
        }

        var txt = imageItem.transform.Find("text");
        txt.gameObject.SetActive(true);
        imageItem.GetComponent<Button>().onClick.AddListener(() =>
        {           
            khung.SetActive(false);            
        });
    }            

    private void OnDisable()
    {        
        if (cancelCorountineBackButtonAndroid != null)
            cancelCorountineBackButtonAndroid.Dispose();
        videoPath = null;

        Destroy(khungAnh1.texture);
        Destroy(khungAnh2.texture);
    }
}
