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

public class DrawingScripts : MonoBehaviour {
    public GameObject goCam;
    public GameObject goModel;
    public Slider sliderLine;
    public Slider sliderContrast;
    public Slider sliderTest;
    public Text txtTime;
    public Button backBtn;
    public GameObject panelComfirm;
    public GameObject eventSystem;
    public TapGesture tapGesture;
    public Button tickBtn;
    public Button cancelBtn;
    public Text txtComfirmText;
    public Canvas canvas;
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
    RawImage rimgcam;
    RawImage rimgmodel;
    Utilities utilities;
    WebCamTextureToMatHelper webCamTextureToMatHelper;
    bool loaded = false;
    WebcamVideoCapture webcamVideoCapture;
    Mat warp;
    private float opaque = 0.25f;
    private OpenCVForUnity.Rect cropRect;
    //private float opaque = 0.4f;
    public enum DRAWMODE { DRAW_MODEL, DRAW_IMAGE};
    public static DRAWMODE drawMode = DRAWMODE.DRAW_MODEL;
    public enum FILTERMODE { LINE,BLEND};
    public static FILTERMODE filtermode = FILTERMODE.LINE;
    private System.Diagnostics.Stopwatch stopWatch;
    private void Awake()
    {
        filtermode = FILTERMODE.LINE;
        if (MakePersistentObject.Instance)
            MakePersistentObject.Instance.gameObject.SetActive(false);
        int delayTime = 100;
        var onSliderLineValueStream = sliderLine.onValueChanged.AsObservable();
        //onSliderValueStream.Buffer(onSliderValueStream.Throttle(TimeSpan.FromMilliseconds(delayTime)))
        //    .Subscribe(list => Debug.LogFormat("list count is {0}", list[list.Count - 1]));
        //onSliderLineValueStream.Buffer(onSliderLineValueStream.Throttle(TimeSpan.FromMilliseconds(delayTime)))
        //    .Subscribe(delegate (IList<float> i) { OnLineSliderValueChange(sliderLine); });
        onSliderLineValueStream.Sample(TimeSpan.FromMilliseconds(delayTime)).Subscribe((float f) => { OnLineSliderValueChange(sliderLine); });
        //slider.onValueChanged.AddListener(delegate { ValueChangeCheck(slider); });       
        var onSliderContrastValueStream = sliderContrast.onValueChanged.AsObservable();
        //onSliderContrastValueStream.Buffer(onSliderContrastValueStream.Throttle(TimeSpan.FromMilliseconds(delayTime)))
        //    .Subscribe(delegate (IList<float> i) { OnContrastSliderValueChange(sliderContrast); });
        onSliderContrastValueStream.Sample(TimeSpan.FromMilliseconds(delayTime)).Subscribe((float f) => {
            OnContrastSliderValueChange(sliderContrast);
        });
        backBtn.onClick.RemoveAllListeners();
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

        tickBtn.onClick.AddListener(() =>
        {
            if(!cancelBtn.gameObject.activeSelf)
            {
                stopWatch.Stop();
                webCamTextureToMatHelper.Pause();
            }
            else
            {
                OnTickBtnClicked();
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


    //    var clickStream = Observable.EveryUpdate()
    //.Where(_ => Input.GetMouseButtonDown(0));

    //    clickStream.Subscribe((long i) =>
    //    {
    //        Debug.Log("press detected");
    //    });

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
                var aspectRatioFitter = goCam.GetComponent<AspectRatioFitter>();
                aspectRatioFitter.aspectRatio = (float)rgbaMat.width() / (float)rgbaMat.height();
                aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

                warpPerspective.Init(webCamTextureToMatHelper.GetMat());
                Mat camMat = webCamTextureToMatHelper.GetMat();
                texCam = new Texture2D(camMat.width(), camMat.height(), TextureFormat.RGBA32, false);

                var rawImageCamera = goCam.GetComponent<RawImage>();
                int camWidth = (int)rawImageCamera.rectTransform.rect.width;
                int camHeight = (int)rawImageCamera.rectTransform.rect.height;
                int widthCanvas = (int)canvas.GetComponent<RectTransform>().rect.width;
                int heightCanvas = (int)canvas.GetComponent<RectTransform>().rect.height;
                var matWidth = rgbaMat.width();
                var matHeight = rgbaMat.height();

                int offsetX = matWidth * ((camWidth - widthCanvas) >> 1) / camWidth;
                int offsetY = matHeight * ((camHeight - heightCanvas) >> 1) / camHeight;
                int subWidth = matWidth - (offsetX << 1);
                int subHeight = matHeight - (offsetY << 1);
                cropRect = new OpenCVForUnity.Rect(offsetX, offsetY, subWidth, subHeight);

                var size = new Size(matWidth, matHeight);
                webcamVideoCapture = new WebcamVideoCapture(size, true);
                MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
                stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
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
    private float currentSliderValue = 0;
    public void OnLineSliderValueChange(Slider slider)
    {
        if (loaded)
        {
            if (slider)
            {
                currentSliderValue = slider.value;
                athreshold.setParameter(slider.value);
                edges = athreshold.adapTiveThreshold(image);
                Mat redMat = utilities.makeMonoAlphaMat(edges,Utilities.MonoColor.RED);
                Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
                rimgmodel.texture = texEdges;
            }            
        }
    }    
    
    IEnumerator Worker()
    {
        while (true)
        {    
            yield return null;
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = webCamTextureToMatHelper.GetMat();
                warp = warpPerspective.warpPerspective(rgbaMat);
                Utils.matToTexture2D(warp, texCam, webCamTextureToMatHelper.GetBufferColors());
                rimgcam.texture = texCam;

                webcamVideoCapture.write(warp);
                var timeLapse = stopWatch.Elapsed.Seconds;
                string minSec = string.Format("{0}:{1:00}", (int)timeLapse / 60, (int)timeLapse % 60);
                txtTime.text = minSec;            
            }
        }
    }
 
    public void ScaleGoCam(float scaleX)
    {
        RawImage a = goCam.GetComponent<RawImage>();
        a.rectTransform.localScale = new Vector3(scaleX, 1, 1);
        var w = a.rectTransform.rect.width;
        var needw = w * scaleX;       
        //Debug.Log(a.rectTransform.rect.ToString());      
    }
    private void OnDisable()
    {        
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
        if (webcamVideoCapture != null && webcamVideoCapture.writer != null)
            webcamVideoCapture.writer.release();      
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
    public void OnTickBtnClicked()
    {
        //var rawImageCamera = goCam.GetComponent<RawImage>();
        //int width = (int)rawImageCamera.rectTransform.rect.width;
        //int height = (int)rawImageCamera.rectTransform.rect.height;
        //int widthCanvas = (int)canvas.GetComponent<RectTransform>().rect.width;
        //int heightCanvas = (int)canvas.GetComponent<RectTransform>().rect.height;
        //var matWidth = warp.width();
        //var matHeight = warp.height();

        //int offsetX = matWidth* ((width - widthCanvas) >> 1) / width;
        //int offsetY = matHeight * ((height - heightCanvas) >> 1) / height;
        //int subWidth = matWidth - (offsetX << 1);
        //int subHeight = matHeight - (offsetY << 1);
       
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
}
