using OpenCVForUnity;
using OpenCVForUnityExample;
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Behaviors;
using TouchScript.Layers;
using TouchScript.Layers.UI;
using TouchScript;
using TouchScript.Gestures;
using System.IO;
using TMPro;

public class DrawingScripts : MonoBehaviour {
    public GameObject goCam;
    public GameObject goModel;
    public Slider sliderLine;
    public Slider sliderContrast;
    public Slider sliderTest;
    public TextMeshProUGUI txtTimeTMPro;
    public Button backBtn;
    public GameObject panelComfirm;
    public GameObject eventSystem;
    public TapGesture tapGesture;
    public Button tickBtn;
    public Button cancelBtn;
    public UnityEngine.UI.Text txtComfirmText;
    public Canvas canvas;
    public Button Button_Recording;
    public GameObject pause;
    public GameObject textTimeTMPro;
    private Threshold threshold;
    private AdaptiveThreshold athreshold;   
    WarpPerspective warpPerspective;
    public static Mat image;
    public static string imgModelPath = null;
    private Color32[] colorsBuffer;
    private Texture2D texEdges;
    private Mat edges;
    public static Texture2D texModel;
    private Texture2D texCam;
    private Texture2D texCamCrop;
    RawImage rimgcam;
    RawImage rimgmodel;
    Utilities utilities;
    WebCamTextureToMatHelper webCamTextureToMatHelper;
    bool loaded = false;
    WebcamVideoCapture webcamVideoCapture;
    Mat warp;
    Mat displayMat;
    Color32[] bufferColor;
    private float opaque = 0.25f;
    private OpenCVForUnity.Rect cropRect;
    //private float opaque = 0.4f;
    public enum DRAWMODE { DRAW_MODEL, DRAW_IMAGE};
    public static DRAWMODE drawMode = DRAWMODE.DRAW_MODEL;
    public enum FILTERMODE { LINE,BLEND};
    public static FILTERMODE filtermode = FILTERMODE.LINE;
    private System.Diagnostics.Stopwatch stopWatch;
    private bool isRecording = false;
    private IDisposable cancelCorountineWorker;
    private IDisposable cancelCorountineTurnOffTouchInput;
    private IDisposable cancelCorountineBlinkTime;
    private IDisposable cancelCoroutineBackBtnAndroid;
    private void Awake()
    {
        filtermode = FILTERMODE.LINE;
        if (MakePersistentObject.Instance)
            MakePersistentObject.Instance.gameObject.SetActive(false);
        int delayTime = 200;
        var onSliderLineValueStream = sliderLine.onValueChanged.AsObservable();
        onSliderLineValueStream.Sample(TimeSpan.FromMilliseconds(delayTime)).Subscribe((float f) => { OnLineSliderValueChange(sliderLine); });
        var onSliderContrastValueStream = sliderContrast.onValueChanged.AsObservable();
        onSliderContrastValueStream.Sample(TimeSpan.FromMilliseconds(delayTime)).Subscribe((float f) =>
        {
            OnContrastSliderValueChange(sliderContrast);
        });
        backBtn.onClick = new Button.ButtonClickedEvent();
        backBtn.onClick.AddListener(() =>
        {
            panelComfirm.SetActive(true);
        });
        var cancelPopup = panelComfirm.transform.Find("cancel").GetComponent<Button>();
        cancelPopup.onClick.AddListener(() =>
        {
            panelComfirm.SetActive(false);
        });

        cancelCoroutineBackBtnAndroid = Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) == true).Subscribe(_ =>
        {
            panelComfirm.SetActive(true);
        });

        var agreePopup = panelComfirm.transform.Find("agree").GetComponent<Button>();
        agreePopup.onClick.AddListener(() =>
        {
            if ( webcamVideoCapture.filePath!=null)
            {
                if(webcamVideoCapture!=null && webcamVideoCapture.writer!=null)
                {                    
                    webcamVideoCapture.writer.release();
                    File.Delete(webcamVideoCapture.filePath);
                }                
            }
            GFs.BackToPreviousScene();            
        });

        tickBtn.onClick.AddListener(() =>
        {
            if (!cancelBtn.gameObject.activeSelf)
            {
                stopWatch.Stop();
                webCamTextureToMatHelper.Pause();
            }
            else
            {
                OnTickConfirmBtnClicked();
            }
        });

        cancelBtn.onClick.AddListener(() =>
        {
            webCamTextureToMatHelper.Play();
            stopWatch.Start();
        });

        if (sliderTest)
        {
            var onSliderTeststream = sliderTest.onValueChanged.AsObservable();
            onSliderTeststream.Subscribe((float i) =>
            {
                var scale = 1 + i;
                Utilities.Log("Scale is {0}", scale);
                rimgcam.rectTransform.localScale = new Vector3(scale, scale, scale);
            });
        }

        Button_Recording.onClick.AddListener(() =>
        {
            if (!pause.activeSelf)
            {
                isRecording = true;
                stopWatch.Start();
                if (cancelCorountineBlinkTime != null)
                    cancelCorountineBlinkTime.Dispose();
            }
            else
            {
                isRecording = false;
                stopWatch.Stop();
                cancelCorountineBlinkTime = Observable.FromCoroutine(blinkTime).Subscribe();
            }
        }); 
    }
 
    void Start () {        
        rimgcam = goCam.GetComponent<RawImage>();
        rimgmodel = goModel.GetComponent<RawImage>();
        rimgmodel.color = new Color(255, 255, 255, opaque);
        webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
        warpPerspective = gameObject.GetComponent<WarpPerspective>();        
        utilities = new Utilities();
        threshold = GetComponent<Threshold>();
        GFs.LoadCategoryList();
        GFs.LoadAllTemplateList();        
        MainThreadDispatcher.StartUpdateMicroCoroutine(loadCameraAndModel());        
    }    
    void OnDestroy()
    {
        if (cancelCorountineWorker != null)
            cancelCorountineWorker.Dispose();
        webCamTextureToMatHelper.Stop();
        webCamTextureToMatHelper.Dispose();
    }
    IEnumerator loadCameraAndModel()
    {
        yield return null;
        if (!webCamTextureToMatHelper.IsInitialized())
        {            
            webCamTextureToMatHelper.onInitialized.AddListener(() => {
                var rgbaMat = webCamTextureToMatHelper.GetMat();
                var captureWidth = rgbaMat.width();
                var captureHeight = rgbaMat.height();
                var captureRatio = captureWidth / (float)captureHeight;

                //var aspectRatioFitter = goCam.GetComponent<AspectRatioFitter>();
                //aspectRatioFitter.aspectRatio = (float)rgbaMat.width() / (float)rgbaMat.height();
                //aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

                warpPerspective.Init(webCamTextureToMatHelper.GetMat());
                Mat camMat = webCamTextureToMatHelper.GetMat();
                            
                var rawImageCamera = goCam.GetComponent<RawImage>();
                int rawImageWidth = (int)rawImageCamera.rectTransform.rect.width;
                int rawImageHeight = (int)rawImageCamera.rectTransform.rect.height;
                var matWidth = rgbaMat.width();
                var matHeight = rgbaMat.height();

                var rawImageRatio = rawImageWidth / (float)rawImageHeight;
                int cropWidth = 0, cropHeight = 0;
                if (rawImageRatio > captureRatio)
                {
                    cropWidth = captureWidth;
                    cropHeight = (int)(cropWidth / rawImageRatio);
                }
                else
                {                                     
                    cropHeight = captureHeight;
                    cropWidth = (int)(cropHeight * rawImageRatio);
                }

                int offsetX = matWidth * ((captureWidth - cropWidth) >> 1) / captureWidth;
                int offsetY = matHeight * ((captureHeight - cropHeight) >> 1) / captureHeight;
                int subWidth = matWidth - (offsetX << 1);
                int subHeight = matHeight - (offsetY << 1);
                cropRect = new OpenCVForUnity.Rect(offsetX, offsetY, subWidth, subHeight);

                texCamCrop = new Texture2D(subWidth, subHeight, TextureFormat.RGBA32, false);
                texCam = new Texture2D(matWidth, matHeight, TextureFormat.RGBA32, false);
                bufferColor = new Color32[subWidth * subHeight];
                var size = new Size(subWidth, subHeight);
                webcamVideoCapture = new WebcamVideoCapture(size, true);
                cancelCorountineWorker = Observable.FromMicroCoroutine(Worker).Subscribe();
                stopWatch = new System.Diagnostics.Stopwatch();
            });
            webCamTextureToMatHelper.Initialize(null, 640, 480, true, 60);
        }        

        if (drawMode == DRAWMODE.DRAW_MODEL)
        {
            string imgPath;
            if (imgModelPath != null)
            {
                imgPath = imgModelPath;
            }
            else
            {
                var categoryID = GVs.CATEGORY_LIST.data[5]._id;
                imgPath = GFs.getAppDataDirPath() + GVs.TEMPLATE_LIST_ALL_CATEGORY[categoryID].dir + "/" + "C06T001.jpg";        
            }
              
            image = Imgcodecs.imread(imgPath, Imgcodecs.IMREAD_UNCHANGED);
            Imgproc.cvtColor(image, image, Imgproc.COLOR_BGRA2RGBA);
            float w = image.width();
            float h = image.height();
            var rat = w / h;
            var restrictMaxSize = 640;
            if (rat > 1)
            {
                w = restrictMaxSize;
                h = w / rat;
            }
            else
            {
                h = restrictMaxSize;
                w = h * rat;
            }
            
            Imgproc.resize(image, image, new Size(w, h), 0, 0, Imgproc.INTER_AREA);            
            texModel = new Texture2D(image.width(), image.height(), TextureFormat.RGBA32, false);            
            Utils.matToTexture2D(image, texModel);
            texModel.Compress(true);
        }
        else
        {
            
        }

        float width = image.width();
        float heigh = image.height();
        float modelAreaWidth = rimgmodel.rectTransform.rect.width;
        float modelAreaHeight = rimgmodel.rectTransform.rect.height;        
        float ratio = width / heigh;
        float ratioDisplay = modelAreaWidth / modelAreaHeight;
        if (ratio > ratioDisplay)
        {
            var newWidth = modelAreaWidth;
            var newHeight = modelAreaWidth * (heigh / width);
            rimgmodel.rectTransform.sizeDelta = new Vector2(newWidth - modelAreaWidth, newHeight - modelAreaHeight);
        }
        else
        {
            var newHeight = modelAreaHeight;
            var newWidth = modelAreaHeight * ratio;
            rimgmodel.rectTransform.sizeDelta = new Vector2(newWidth - modelAreaWidth, newHeight - modelAreaHeight);
        }
       
        athreshold = GetComponent<AdaptiveThreshold>();
        athreshold.setParameter(sliderLine.value);
        texEdges = new Texture2D(image.width(), image.height(), TextureFormat.ARGB32, false);
        edges = athreshold.adapTiveThreshold(image);
        Mat redMat = utilities.makeMonoAlphaMat(edges,Utilities.MonoColor.RED);         
        colorsBuffer = new Color32[edges.width() * edges.height()];
        Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
        rimgmodel.texture = texEdges;
        utilities = new Utilities();
        goModel.SetActive(true);
        loaded = true;

        cancelCorountineTurnOffTouchInput = Observable.FromMicroCoroutine(turnOffTouchInput).Subscribe();
    }

    IEnumerator turnOffTouchInput()
    {
        while (eventSystem && eventSystem.GetComponent<TouchScriptInputModule>() == null)
        {
            yield return null;
        }
        eventSystem.GetComponent<TouchScriptInputModule>().enabled = false;
    }

    public void OnContrastSliderValueChange(Slider slider)
    {
        float percent = slider.value / 100f;
        var c = rimgmodel.color;
        rimgmodel.color = new Color(c.r, c.g, c.b, percent);
    }

    public void OnLineSliderValueChange(Slider slider)
    {
        if (loaded)
        {
            if (slider)
            {
                athreshold.setParameter(slider.value);
                edges = athreshold.adapTiveThreshold(image);
                Mat redMat = utilities.makeMonoAlphaMat(edges,Utilities.MonoColor.RED);
                Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
                rimgmodel.texture = texEdges;
            }            
        }
    }

    private int numberFrame = 0;
    
    IEnumerator Worker()
    {
        while (true)
        {    
            yield return null;
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = webCamTextureToMatHelper.GetMat();
                warp = warpPerspective.warpPerspective(rgbaMat);
                displayMat = warp.submat(cropRect);
                Utils.matToTexture2D(displayMat, texCam, bufferColor);
                //Utils.matToTexture2D(warp, texCam, bufferColor);
                if (isRecording)
                {
                    numberFrame++;

                    webcamVideoCapture.write(displayMat);
                    var timeLapse = (int)stopWatch.Elapsed.TotalSeconds;
                    string minSec = string.Format("{0}:{1:00}", (int)(timeLapse / 60f), (int)timeLapse % 60);                    
                    txtTimeTMPro.text = minSec;
                }
                rimgcam.texture = texCam;
            }
        }
    }
 
    public void ScaleGoCam(float scaleX)
    {
        RawImage a = goCam.GetComponent<RawImage>();
        a.rectTransform.localScale = new Vector3(scaleX, 1, 1);
        var w = a.rectTransform.rect.width;
        var needw = w * scaleX;                   
    }
    private void OnDisable()
    {
        if (cancelCorountineTurnOffTouchInput != null)
            cancelCorountineTurnOffTouchInput.Dispose();
        if (cancelCorountineBlinkTime!=null)
            cancelCorountineBlinkTime.Dispose();
        if (cancelCoroutineBackBtnAndroid!=null)
            cancelCoroutineBackBtnAndroid.Dispose();
        image.release();
        image.Dispose();
        edges.Dispose();
        image = null;
        if (!preserveTexture) {
            Destroy(texCam);
        }            
        Destroy(texEdges);
        Destroy(texModel);
        Destroy(webCamTextureToMatHelper);
        if(webcamVideoCapture != null)
        {
            webcamVideoCapture.filePath = null;
            if(webcamVideoCapture.writer != null)
            {
                webcamVideoCapture.writer.release();
            }
        }                  
    }
    public void OnContrastBtnClicked()
    {      
        if (filtermode == FILTERMODE.LINE)
            filtermode = FILTERMODE.BLEND;
        else if (filtermode == FILTERMODE.BLEND)
            filtermode = FILTERMODE.LINE;
        if (filtermode == FILTERMODE.LINE)
        {
            rimgmodel.color = new Color(255, 255, 255, opaque);
            rimgmodel.texture = texEdges;
        }
        else if (filtermode == FILTERMODE.BLEND)
        {
            OnContrastSliderValueChange(sliderContrast);
            rimgmodel.texture = texModel;        
        }        
    }
    public void OnSliderBtnClicked()
    {
        filtermode = FILTERMODE.LINE;
        rimgmodel.color = new Color(255, 255, 255, opaque);
        Mat redMat = utilities.makeMonoAlphaMat(edges, Utilities.MonoColor.RED);
        Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
        rimgmodel.texture = texEdges;
        rimgmodel.GetComponent<ScreenTransformGesture>().enabled = false;
        rimgmodel.GetComponent<Transformer>().enabled = false;       
    }
    bool preserveTexture = false;
    public void OnTickConfirmBtnClicked()
    {
        Mat resultMat = warp.submat(cropRect);
        Texture2D resultTexture = new Texture2D(cropRect.width, cropRect.height, TextureFormat.BGRA32, false);
        Utils.matToTexture2D(resultMat, resultTexture);

        string name = null;
        if (WebcamVideoCapture.filenameWithoutExt != null)
        {
            name = String.Format("{0}.png", WebcamVideoCapture.filenameWithoutExt);
        }
        else
        {
            name = String.Format("{0}.png", DateTime.Now.ToString(Utilities.customFmts));
        }
        var masterPieceDirPath = GFs.getMasterpieceDirPath();
        var fullPath = masterPieceDirPath + name;
        File.WriteAllBytes(fullPath, resultTexture.EncodeToPNG());
        WebcamVideoCapture.filename = null;
        WebcamVideoCapture.filenameWithoutExt = null;
        ResultScripts.texture = resultTexture;
        ResultScripts.mode = ResultScripts.MODE.FISRT_RESULT;        
        if (webcamVideoCapture != null)
            ResultScripts.videoPath = webcamVideoCapture.filePath;

        Debug.LogFormat("Number frame is "+numberFrame);
        var lengthInSeconds = numberFrame / (float)WebcamVideoCapture.FPS;
        if (lengthInSeconds < 3)
        {
            webcamVideoCapture.writer.release();
            webcamVideoCapture.writer.Dispose();
            Debug.Log("Video deleted");
            File.Delete(webcamVideoCapture.filePath);
            ResultScripts.videoPath = null;
        }

        GVs.SCENE_MANAGER.loadResultScene();
    }
    public void OnPushBtnClicked()
    {
        if(filtermode == FILTERMODE.LINE)
        {
            Mat blueMat = utilities.makeMonoAlphaMat(edges, Utilities.MonoColor.BLUE);
            Utils.matToTexture2D(blueMat, texEdges, colorsBuffer);
            rimgmodel.texture = texEdges;
        }        
        rimgmodel.GetComponent<ScreenTransformGesture>().enabled = true;
        rimgmodel.GetComponent<Transformer>().enabled = true;        
    }
    public void OnPushActiveBtnClicked()
    {
        if (filtermode == FILTERMODE.LINE)
        {
            Mat redMat = utilities.makeMonoAlphaMat(edges, Utilities.MonoColor.RED);
            Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
            rimgmodel.texture = texEdges;
        }
        rimgmodel.GetComponent<ScreenTransformGesture>().enabled = false;
        rimgmodel.GetComponent<Transformer>().enabled = false;        
    }


    IEnumerator blinkTime()
    {
        yield return null;
        while(!isRecording)
        {
            yield return new WaitForSeconds(1);
            //textTime.SetActive(!textTime.activeSelf);
            textTimeTMPro.SetActive(!textTimeTMPro.activeSelf);
        }
    }
}
