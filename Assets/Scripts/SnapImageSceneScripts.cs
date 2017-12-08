using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using OpenCVForUnity;
using UnityEngine.UI;
using System;
using System.IO;
public class SnapImageSceneScripts : MonoBehaviour
{
    public GameObject goCam;
    private RawImage rawImgCam;
    private bool camAvailable;
    private WebCamTexture webcamTex;
    private WebCamDevice webCamDevice;
    private bool isFronFacing = false;
    private int requestWidth = 1920;
    private int requestHeight = 1920;
    private WebCamTextureToMatHelper wcHelper;
    Mat snapMat;
    public Button btnSnap;
    public Button btnCancel;
    public Button btnContinue;
    public IDisposable cancelCoroutineBackAndroid;
    public GameObject imageItem;
    public RawImage khungAnh;
    public RawImage khungAnh2;
    public GameObject khung;
    public Button btnChapAnh;
    public Camera camera;
    public GameObject scroll;

    public enum SnapMode { NOFRAME, FRAME};
    private SnapMode mode = SnapMode.FRAME;
    private Texture2D textureKhungAnh;
    private Texture2D snapImage;
    IDisposable cancelMCoroutineSnap;
    float oriWidth, oriHeight;
    Texture2D texRgbaMat;

    private void Awake()
    {
        oriWidth = khungAnh.GetComponent<RectTransform>().rect.width;
        oriHeight = khungAnh.GetComponent<RectTransform>().rect.height;
        Debug.LogFormat("Original width is {0}, orginal height is {1}", oriWidth, oriHeight);
        GFs.LoadData();
        if (MakePersistentObject.Instance)
            MakePersistentObject.Instance.gameObject.SetActive(false);

        cancelCoroutineBackAndroid = GFs.BackButtonAndroidGoPreScene();

        btnChapAnh.onClick.AddListener(() =>
        {
            chapMat();
            ScreenshotHelper.iCaptureWithCamera(camera);
        });
    }

    void Start()
    {
        btnSnap.onClick.AddListener(() =>
        {
            btnCancel.gameObject.SetActive(true);
            btnContinue.gameObject.SetActive(true);
            scroll.SetActive(false);
        });

        btnCancel.onClick.AddListener(() =>
        {
            btnContinue.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(false);
            scroll.SetActive(true);
        });

        rawImgCam = goCam.GetComponent<RawImage>();
        ShowCam();
        loadFramesList();             
    }

    void loadFramesList()
    {
        var framelist = GVs.listFrame.data;
        var dir = GVs.listFrame.dir;
        dir = GFs.getAppDataDirPath() + dir + "/";
        for(int i=0;i<framelist.Count;i++)
        {
            var frame = framelist[i];
            var thumbPath = dir + frame.thumb;            
            Texture2D texture = GFs.LoadPNGFromPath(thumbPath);
            GameObject clone = Instantiate(imageItem, imageItem.transform.parent);
            clone.SetActive(true);

            var thumb = clone.transform.Find("thumb");
            thumb.gameObject.GetComponent<RawImage>().texture = texture;
            thumb.GetComponent<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;

            clone.GetComponent<Button>().onClick.AddListener(() => {                
                var imgP = dir + frame.image1;
                var imgP2 = dir + frame.image2;
                Texture2D txImg = GFs.LoadPNGFromPath(imgP);
                Texture2D txImg2 = GFs.LoadPNGFromPath(imgP2);
                Destroy(khungAnh.texture);
                Destroy(khungAnh2.texture);
                khungAnh.texture = txImg;
                khungAnh2.texture = txImg2;
                khung.GetComponent<RectTransform>().localScale = Vector3.one;
                khung.transform.localEulerAngles = Vector3.zero;
                khung.transform.localPosition = Vector3.zero;
                khungAnh.GetComponent<AspectRatioFitter>().aspectRatio = txImg.width / (float)txImg.height;
                khungAnh2.GetComponent<AspectRatioFitter>().aspectRatio = txImg2.width / (float)txImg2.height;
                khungAnh.GetComponent<AspectRatioFitter>().enabled = true;
                khungAnh2.GetComponent<AspectRatioFitter>().enabled = true;
                khung.SetActive(true);
                textureKhungAnh = txImg;
                mode = SnapMode.FRAME;
            });            
        }

        var txt = imageItem.transform.Find("text");
        txt.gameObject.SetActive(true);
        imageItem.GetComponent<Button>().onClick.AddListener(() =>
        {
            mode = SnapMode.NOFRAME;
            khung.SetActive(false);
            Destroy(khungAnh.texture);
        });
    }
        
    void ResizeAndFlipWebcamTexture()
    {
        webcamTex = wcHelper.GetWebCamTexture();
        webcamTex.Play();

        var widthCam = webcamTex.width;
        var heightCam = webcamTex.height;
        var ratioWH = widthCam / (float)heightCam;
        var ratioHW = heightCam / (float)widthCam;
        var widthDisplay = rawImgCam.rectTransform.rect.width;
        var heightDisplay = rawImgCam.rectTransform.rect.height;
        var ratioDisplay = widthDisplay / (float)heightDisplay;

        int orient = webcamTex.videoRotationAngle;

        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
        if (orient == 90 || orient == 270)
        {
            if (ratioHW < ratioDisplay)
            {
                var newHeight = widthDisplay;
                var newWidth = newHeight * ratioWH;

                var heightDelta = newHeight - heightDisplay;
                var widthDelta = newWidth - widthDisplay;
                rawImgCam.rectTransform.sizeDelta = new Vector2(widthDelta, heightDelta);
            }
            else
            {
                var newWidth = heightDisplay;
                var newHeight = newWidth * ratioHW;

                var heightDelta = newHeight - heightDisplay;
                var widthDelta = newWidth - widthDisplay;
                rawImgCam.rectTransform.sizeDelta = new Vector2(widthDelta, heightDelta);
            }
        }
        else
        {
            if (ratioWH < ratioDisplay)
            {
                var newWidth = widthDisplay;
                var newHeight = newWidth * ratioHW;

                rawImgCam.rectTransform.sizeDelta = new Vector2(newWidth - widthDisplay, newHeight - heightDisplay);
            }
            else
            {
                var newHeight = heightDisplay;
                var newWidth = newHeight * ratioWH;

                rawImgCam.rectTransform.sizeDelta = new Vector2(newWidth - widthDisplay, newHeight - heightDisplay);
            }
        }

        if (isFronFacing && (orient == 90 || orient == 270))
        {
            rawImgCam.rectTransform.localScale = new Vector3(1, -1, 1);
        }

        if (WebCamTexture.devices.Length == 1 && WebCamTexture.devices[0].isFrontFacing == true && orient == 0)
        {
            rawImgCam.rectTransform.localScale = new Vector3(-1, 1, 1);
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var localScale = rawImgCam.rectTransform.localScale;
            localScale.y = -localScale.y;
            rawImgCam.rectTransform.localScale = localScale;
        }

    }

    void ShowCam()
    {
        if (wcHelper == null)
            wcHelper = goCam.GetComponent<WebCamTextureToMatHelper>();
        wcHelper.onInitialized.RemoveAllListeners();
        wcHelper.onInitialized.AddListener(() =>
        {
            ResizeAndFlipWebcamTexture();
            rawImgCam.texture = webcamTex;
            camAvailable = true;
        });

        wcHelper.Initialize(null, requestWidth, requestHeight, isFronFacing);
    }

    private void OnDisable()
    {
        if (webcamTex)
            webcamTex.Stop();
        webcamTex = null;
        if (cancelMCoroutineSnap != null)
            cancelMCoroutineSnap.Dispose();
        if (cancelCoroutineBackAndroid != null)
            cancelCoroutineBackAndroid.Dispose();

        Destroy(textureKhungAnh);
     
    }

    public void OnChangeCameraButton()
    {
        wcHelper.Stop();
        wcHelper = null;
        rawImgCam.rectTransform.sizeDelta = new Vector2(0, 0);
        rawImgCam.transform.localScale = new Vector3(1, 1, 1);
        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        isFronFacing = !isFronFacing;
        ShowCam();
        btnContinue.gameObject.SetActive(false);
        btnCancel.gameObject.SetActive(false);
    }

    public void OnSnapBtnClicked()
    {        
        cancelMCoroutineSnap = Observable.FromMicroCoroutine(Snap).Subscribe();
    }

    IEnumerator Snap()
    {
        if (mode == SnapMode.NOFRAME)
        {
            webcamTex.requestedWidth = 1920;
            webcamTex.requestedHeight = 1920;
            webcamTex.Play();
            rawImgCam.texture = webcamTex;
            int countNonZero = 0;
            int numBlackFrame = 0;
            int numberFrameSkip = 0;

            snapMat = new Mat(webcamTex.height, webcamTex.width, CvType.CV_8UC4);
            Mat singleChannel = new Mat();
            Color32[] buffers = new Color32[webcamTex.width * webcamTex.height];
            rawImgCam.texture = null;

            while (countNonZero == 0 || numberFrameSkip < 1)
            {
                Utils.webCamTextureToMat(webcamTex, snapMat, buffers);
                Core.extractChannel(snapMat, singleChannel, 1);
                countNonZero = Core.countNonZero(singleChannel);
                if (countNonZero > 0)
                {
                    numberFrameSkip++;
                }
                else
                {
                    numBlackFrame++;
                }

                Utilities.Log("Count non zero of mat is {0}", countNonZero);
                yield return null;
            }

            singleChannel.Dispose();
            snapMat.Dispose();

            Mat rgbaMat = wcHelper.GetMat();

            rawImgCam.rectTransform.localScale = new Vector3(1, 1, 1);
            rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
            rawImgCam.rectTransform.sizeDelta = new Vector2(0, 0);

            var widthCam = rgbaMat.width();
            var heightCam = rgbaMat.height();
            var ratCamWH = widthCam / (float)heightCam;            
            var wDis = rawImgCam.rectTransform.rect.width;
            var hDis = rawImgCam.rectTransform.rect.height;
            var ratDis = wDis / (float)hDis;

            var newWidth = 0f;
            var newHeight = 0f;

            if (ratCamWH < ratDis)
            {
                newWidth = widthCam;
                newHeight = newWidth / ratDis;
            }
            else
            {
                newHeight = heightCam;
                newWidth = newHeight * ratDis;
            }


            var deltaWidthMat = (int)(widthCam - newWidth);
            var deltaHeightMat = (int)(heightCam - newHeight);
            var matDisW = widthCam - deltaWidthMat;
            var matDisH = heightCam - deltaHeightMat;

            var rect = new OpenCVForUnity.Rect(deltaWidthMat / 2, deltaHeightMat / 2, matDisW, matDisH);
            snapMat = rgbaMat.submat(rect);

            Destroy(texRgbaMat);
            texRgbaMat = new Texture2D(snapMat.width(), snapMat.height(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(snapMat, texRgbaMat);
            rawImgCam.texture = texRgbaMat;
        }
        else if(mode == SnapMode.FRAME)
        {
            ScreenshotHelper.iCaptureWithCamera(camera, (Texture2D texture) =>
             {                 
                 rawImgCam.transform.localScale = Vector3.one;
                 rawImgCam.rectTransform.localEulerAngles = Vector3.zero;
                 rawImgCam.rectTransform.sizeDelta = new Vector2(0, 0);
                 rawImgCam.texture = texture;
                 if (snapImage != null) Destroy(snapImage);
                 snapImage = texture;                
             });           
        }
        
    }

    public void chapMat()
    {
        //if (mode == SnapMode.FRAME)
        
            //var width = khungAnh.GetComponent<RectTransform>().rect.width;
            //var height = khungAnh.GetComponent<RectTransform>().rect.height;

            //var rectTransform = khungAnh.GetComponent<RectTransform>();
            //var offsetMin = rectTransform.offsetMin;
            //var offsetMax = rectTransform.offsetMax;

            //var left = offsetMin.x;
            //var right = orilWidth + offsetMax.x;
            //var top = -offsetMax.y;
            //var bottom = oriHeight - offsetMin.y;
            //var centerX = (left + right) / 2;
            //var centerY = (top + bottom) / 2;

            //var aw = right - left;
            //var ah = bottom - top;
            //var scale = rectTransform.localScale.x;
            //var rotate = rectTransform.localRotation.eulerAngles.z;
            //var real_width = aw * width;
            //var real_height = ah * height;

            //var x = centerX - real_width / 2;
            //var y = centerY - real_height / 2;

            //var _scale = snapMat.width() / orilWidth;
            //int x_ = (int)(x * _scale);
            //int y_ = (int)(y * _scale);
            //int w_ = (int)(real_width * _scale);
            //int h_ = (int)(real_height * _scale);

            //int x_begin = x_ < 0 ? 0 : x_;
            //int y_begin = y_ < 0 ? 0 : y_;
            //int x_end = x_begin + w_ > snapMat.width() ? snapMat.width() : x_begin + w_;
            //int y_end = y_begin + h_ > snapMat.height() ? snapMat.height() : y_begin + h_;
      

        //Mat khungMat = new Mat(textureKhungAnh.height, textureKhungAnh.width, CvType.CV_8UC4);
        //Utils.texture2DToMat(textureKhungAnh, khungMat);
        //Imgproc.resize(khungMat, khungMat, new Size(w_, h_));

        //Debug.LogFormat("left = {0}, right = {1}, top = {2}, botton = {3}", left, right, top, bottom);
        //Debug.LogFormat("aw = {0}, ah = {1}", aw, ah);
        //Debug.Log("minCorner : "+ offsetMin);
        //Debug.Log("maxCorner : " + offsetMax);
    }


    public void OnContinueBtnClicked()
    {  
        string dirPathSnapImage = GFs.getSnapImageDirPath();  
        var dateTimeNow = DateTime.Now.ToString(Utilities.customFmts);
        var MPModelPath = dirPathSnapImage + dateTimeNow + ".png";
        if (mode == SnapMode.NOFRAME)
        {
            if (snapMat != null)
            {
                DrawingScripts.image = snapMat;
                Texture2D texture = new Texture2D(snapMat.width(), snapMat.height(), TextureFormat.BGRA32, false);
                Utils.matToTexture2D(snapMat, texture);
                DrawingScripts.texModel = texture;
                DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    System.IO.File.WriteAllBytes(MPModelPath, texture.EncodeToPNG());
                }
                else
                {
                    Imgproc.cvtColor(snapMat, snapMat, Imgproc.COLOR_BGR2RGB);
                    Imgcodecs.imwrite(MPModelPath, snapMat);
                }
                HistoryNGUIScripts.AddHistoryItem(new HistoryModel(MPModelPath, MPModelPath, HistoryModel.IMAGETYPE.SNAP));
                GVs.SCENE_MANAGER.loadDrawingScene();
            }
        }
        else if (mode == SnapMode.FRAME)
        {
            string save_path = new FilePathName().SaveTextureAs(snapImage, FilePathName.SaveFormat.PNG);
            File.Move(save_path, MPModelPath);            
            Mat mat = new Mat(snapImage.height, snapImage.width, CvType.CV_8UC3);
            Utils.texture2DToMat(snapImage, mat);
            DrawingScripts.texModel = snapImage;
            DrawingScripts.image = mat;
            DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
            HistoryNGUIScripts.AddHistoryItem(new HistoryModel(MPModelPath, MPModelPath, HistoryModel.IMAGETYPE.SNAP));
            GVs.SCENE_MANAGER.loadDrawingScene();
        }
    }

    public void OnCancelBtnClicked()
    {
        rawImgCam.rectTransform.sizeDelta = new Vector2(0, 0);
        rawImgCam.transform.localScale = new Vector3(1, 1, 1);
        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        ResizeAndFlipWebcamTexture();
        rawImgCam.texture = webcamTex;
    }



    void rotateMat(Mat src, Mat des, float angleDegree)
    {
        Point center = new Point(src.cols() / 2f, src.rows() / 2f);
        Mat rot = Imgproc.getRotationMatrix2D(center, angleDegree, 1);
        OpenCVForUnity.Rect bbox = new RotatedRect(center, src.size(), angleDegree).boundingRect();

        rot.put(0, 2, rot.get(0, 2)[0] + bbox.width / 2f - center.x);
        rot.put(1, 2, rot.get(1, 2)[0] + bbox.height / 2f - center.y);

        Mat dst = new Mat();
        Imgproc.warpAffine(src, des, rot, bbox.size());
    }

}
