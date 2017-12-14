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
using TouchScript.Gestures.TransformGestures;

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
    public Button backBtn;
    public Button btCon;
    public Camera cam;
    public GameObject imageItem;
    public GameObject khung;
    public RawImage khungAnh1;
    public RawImage khungAnh2;
    public Canvas canvasSnap;    
    public ScreenTransformGesture tfGes;    

    private Color32[] buffer;
    private Texture2D texVideo;
    private Mat frame;
    private AspectRatioFitter rawImageAspect;

    private int FPS = 60;
    private float currentFPS = 0;
    private float ratioImage = 1;
    private GameObject tempPanel = null;
    private IDisposable cancelCorountineBackButtonAndroid;
    LTDescr ltdescr_ScaleComfirmDeletePanel;
    LTDescr ltdescr_AlphaComfirmDeletePanel;

    public enum MODE {NO_FRAME,FRAME};
    private MODE mode = MODE.NO_FRAME;

    private void Awake()
    {
        GFs.LoadData();
        if (Application.platform == RuntimePlatform.Android)
        {
            cancelCorountineBackButtonAndroid = Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) == true)
                .Subscribe(_ =>
                {
                    GVs.TRACE_SCENE.Pop();
                    GVs.TRACE_SCENE.Pop();
                    int i = GVs.TRACE_SCENE.Pop();
                    Debug.LogFormat("track scene have {0} elements", GVs.TRACE_SCENE.Count);
                    SceneManager.LoadScene(i);
                });
        }

        btCon.onClick.AddListener(() =>
        {
            if(mode == MODE.FRAME)
            {
                if (tempPanel != null)
                    Destroy(tempPanel);
                tempPanel = Instantiate(panel);

                tempPanel.transform.parent = canvasSnap.transform;
                tempPanel.transform.localScale = Vector3.one;
                tempPanel.transform.localPosition = panel.transform.localPosition;
                tempPanel.GetComponent<RectTransform>().anchorMax = Vector2.one;
                tempPanel.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                tempPanel.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                tempPanel.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                cam.gameObject.SetActive(true);
                ScreenshotHelper.iCaptureWithCamera(cam, (Texture2D tex_) =>
                {
                    Mat resultMat = new Mat(tex_.height, tex_.width, CvType.CV_8UC4);
                    Texture2D resultTexture = new Texture2D(tex_.width, tex_.height, TextureFormat.BGRA32, false);
                    Utils.texture2DToMat(tex_, resultMat);
                    Destroy(tex_);
                    Destroy(tempPanel);
                    cam.gameObject.SetActive(false);
                    GFs.load_APP_PATH_VAR();
                    var logoPath = GFs.getlogoPath();
                    Texture2D txtWMark = GFs.LoadPNGFromPath(logoPath);
                    Mat logo = new Mat(txtWMark.height, txtWMark.width, CvType.CV_8UC4);
                    var width = resultMat.width();
                    int newWidthlogo = (int)(width / 5f);
                    int newHeightlogo = (int)(logo.height() * (newWidthlogo / (float)logo.width()));
                    Utils.texture2DToMat(txtWMark, logo);
                    Destroy(txtWMark);
                    Imgproc.resize(logo, logo, new Size(newWidthlogo, newHeightlogo));
                    var rect = resultMat.submat(new OpenCVForUnity.Rect(10, resultMat.height() - logo.height() - 10, logo.width(), logo.height()));
                    Mat maskCopyMask = new Mat(logo.height(), logo.width(), CvType.CV_8UC1);
                    Core.extractChannel(logo, maskCopyMask, 3);
                    maskCopyMask = maskCopyMask - new Scalar(230);
                    logo.copyTo(rect, maskCopyMask);
                    Utils.matToTexture2D(resultMat, resultTexture);
                    ResultScripts.texture = resultTexture;
                    if (imagePath != null)
                    {
                        File.WriteAllBytes(imagePath, resultTexture.EncodeToPNG());
                    }
                    GVs.SCENE_MANAGER.loadResultScene();
                });
            }
            else
            {
                if (texture == null)
                {
                    GVs.SCENE_MANAGER.loadResultScene();
                    return;
                }
                Mat resultMat = new Mat(texture.height, texture.width, CvType.CV_8UC4);
                Texture2D resultTexture = new Texture2D(texture.width, texture.height, TextureFormat.BGRA32, false);
                Utils.texture2DToMat(texture, resultMat);
                Destroy(texture);
                Destroy(tempPanel);
                cam.gameObject.SetActive(false);
                GFs.load_APP_PATH_VAR();
                var logoPath = GFs.getlogoPath();
                Texture2D txtWMark = GFs.LoadPNGFromPath(logoPath);
                Mat logo = new Mat(txtWMark.height, txtWMark.width, CvType.CV_8UC4);
                var width = resultMat.width();
                int newWidthlogo = (int)(width / 5f);
                int newHeightlogo = (int)(logo.height() * (newWidthlogo / (float)logo.width()));
                Utils.texture2DToMat(txtWMark, logo);
                Destroy(txtWMark);
                Imgproc.resize(logo, logo, new Size(newWidthlogo, newHeightlogo));
                var rect = resultMat.submat(new OpenCVForUnity.Rect(10, resultMat.height() - logo.height() - 10, logo.width(), logo.height()));
                Mat maskCopyMask = new Mat(logo.height(), logo.width(), CvType.CV_8UC1);
                Core.extractChannel(logo, maskCopyMask, 3);
                maskCopyMask = maskCopyMask - new Scalar(230);
                logo.copyTo(rect, maskCopyMask);
                Utils.matToTexture2D(resultMat, resultTexture);
                ResultScripts.texture = resultTexture;
                if (imagePath != null)
                {
                    File.WriteAllBytes(imagePath, resultTexture.EncodeToPNG());
                }
                GVs.SCENE_MANAGER.loadResultScene();
            }
        });
    }

    private void TransformGesture_Transformed(object sender, EventArgs e)
    {
        khung.transform.localPosition += tfGes.DeltaPosition;
        khung.transform.localScale += (tfGes.DeltaScale -1f) * Vector3.one;
        khung.transform.localRotation *= Quaternion.AngleAxis(tfGes.DeltaRotation, tfGes.RotationAxis);        
    }

    void Start()
    {
        tfGes.Transformed += TransformGesture_Transformed;
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
                khung.SetActive(true);
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
                
                mode = MODE.FRAME;
            });
        }

        var txt = imageItem.transform.Find("text");
        txt.gameObject.SetActive(true);
        imageItem.GetComponent<Button>().onClick.AddListener(() =>
        {
            mode = MODE.NO_FRAME;
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
        if (mode == MODE.FRAME)
            Destroy(texture);
    }
}
