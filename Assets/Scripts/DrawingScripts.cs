using OpenCVForUnity;
using OpenCVForUnityExample;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DrawingScripts : MonoBehaviour {
    public GameObject goCam;
    public GameObject goModel;
    public Slider sliderLine;
    public Slider sliderContrast;
    public Threshold threshold;
    public AdaptiveThreshold athreshold;
    WarpPerspective warpPerspective;
    public static Mat image;
    private Color32[] colorsBuffer;
    private Texture2D texEdges;
    public static Texture2D texModel;
    private Texture2D texCam;
    RawImage rimgcam;
    RawImage rimgmodel;
    Utilities utilities;
    WebCamTextureToMatHelper webCamTextureToMatHelper;
    bool loaded = false;
    WebcamVideoCapture webcamCapture;
    Mat warp;

    public enum DRAWMODE { DRAW_MODEL, DRAW_IMAGE};
    public static DRAWMODE drawMode = DRAWMODE.DRAW_MODEL;
    public enum FILTERMODE { LINE,BLEND};
    public static FILTERMODE filtermode = FILTERMODE.LINE;
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

        //var heavyMethod2 = Observable.Start(() =>
        //{
        //    // heavy method...
        //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
        //    return 10;
        //});
    }

    void OnSliderValueChaned(Slider slider)
    {
        Debug.LogFormat("Slider value is {0}", slider.value);
    }

    // Use this for initialization
    void Start () {
        rimgcam = goCam.GetComponent<RawImage>();
        rimgmodel = goModel.GetComponent<RawImage>();
        webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
        warpPerspective = gameObject.GetComponent<WarpPerspective>();        

        utilities = new Utilities();

        threshold = GetComponent<Threshold>();
 
        if (Application.platform == RuntimePlatform.Android)
        {
            GVs.APP_PATH = "/data/data/com.MinhViet.ProductName/files";
        }
        else
        {
            GVs.APP_PATH = Application.persistentDataPath;
        }

        threshold = GetComponent<Threshold>();
        GFs.LoadTemplateList();

        MainThreadDispatcher.StartUpdateMicroCoroutine(loadModel());
        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
        //var heavyMethod = Observable.Start(() =>
        //{
        //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
        //    sliderLine.value = 1;
        //    return 10;
        //});
    }    

    void OnDestroy()
    {
        webCamTextureToMatHelper.Stop();
        webCamTextureToMatHelper.Dispose();
    }

    IEnumerator loadModel()
    {
        yield return null;

        if (!webCamTextureToMatHelper.IsInitialized())
        {
            int w = (int)goCam.GetComponent<RawImage>().rectTransform.rect.width;
            int h = (int)goCam.GetComponent<RawImage>().rectTransform.rect.height;
            
            webCamTextureToMatHelper.onInitialized.AddListener(() => {
                var rgbaMat = webCamTextureToMatHelper.GetMat();
                var aspectRatioFitter = goCam.GetComponent<AspectRatioFitter>();
                aspectRatioFitter.aspectRatio = (float)rgbaMat.width() / (float)rgbaMat.height();
                aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                warpPerspective.Init(webCamTextureToMatHelper.GetMat());
                Mat camMat = webCamTextureToMatHelper.GetMat();
                texCam = new Texture2D(camMat.width(), camMat.height(), TextureFormat.RGBA32, false);
            });

            webCamTextureToMatHelper.Initialize(null, 640, 480, webCamTextureToMatHelper.requestedIsFrontFacing);
        }

        if (drawMode == DRAWMODE.DRAW_MODEL)
        {
            var model = GVs.CURRENT_MODEL;
            string imgPath;
            if (model != null)
            {
                imgPath = GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + model.image;
            }
            else
            {
                imgPath = GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + "T0027.jpg";
            }
            imgPath = GVs.APP_PATH + "/" + imgPath;
            Debug.LogFormat("image path is {0}", imgPath);
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
            Utilities.Log("img loaded have width is {0}, height is {1}", image.width(), image.height());
            Imgproc.resize(image, image, new Size(w, h), 0, 0, Imgproc.INTER_AREA);
            Utilities.Log("img loaded have width is {0}, height is {1}", image.width(), image.height());
            texModel = new Texture2D(image.width(), image.height(), TextureFormat.RGBA32, false);
            //texModel = new Texture2D(image.width(), image.height());            
            Utils.matToTexture2D(image, texModel);
            texModel.Compress(true);
        }
        else
        {

        }

        Utilities.Log("img loaded have width is {0}, height is {1}", image.width(), image.height());

        float width = image.width();
        float heigh = image.height();

        float modelAreaWidth = rimgmodel.rectTransform.rect.width;
        float modelAreaHeight = rimgmodel.rectTransform.rect.height;
        Debug.LogFormat("ModelAreaWidth = {0}, ModelAreaHeight = {1}", modelAreaWidth, modelAreaHeight);

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

        Imgproc.cvtColor(image, image, Imgproc.COLOR_BGRA2RGBA);
        athreshold = GetComponent<AdaptiveThreshold>();
        athreshold.setParameter(sliderLine.value);
        texEdges = new Texture2D(image.width(), image.height(), TextureFormat.ARGB32, false);
        Mat edges = athreshold.adapTiveThreshold(image);
        Mat redMat = utilities.makeMonoAlphaMat(edges);         
        colorsBuffer = new Color32[edges.width() * edges.height()];
        Utils.matToTexture2D(redMat, texEdges, colorsBuffer);

        rimgmodel.texture = texEdges;
        utilities = new Utilities();
        goModel.SetActive(true);
        loaded = true;  
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
                Mat edges = athreshold.adapTiveThreshold(image);
                Mat redMat = utilities.makeMonoAlphaMat(edges);
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
                if(isRecording)
                {
                    //Utilities.Log("Mat width = {0}, warp width = {1}, Mat height = {2}, ward height = {3}",rgbaMat.width(), warp.width(), rgbaMat.height(),warp.height());
                    webcamCapture.write(warp);
                }
            }
        }
    }
    public bool isRecording = false;
    public void StartRecordVideo()
    {
        Utilities.Log("Start video recording");
        Mat m = webCamTextureToMatHelper.GetMat();
        webcamCapture = new WebcamVideoCapture(m.size());
        isRecording = true;
    }    
    public void StopVideoRecording()
    {
        if(isRecording)
        {
            Utilities.Log("Stop recording video");
            isRecording = false;            
            webcamCapture.writer.release();
        }
    }
    public void ScaleGoCam(float scaleX)
    {
        RawImage a = goCam.GetComponent<RawImage>();
        a.rectTransform.localScale = new Vector3(scaleX, 1, 1);
        var w = a.rectTransform.rect.width;
        var needw = w * scaleX;       
        Debug.Log(a.rectTransform.rect.ToString());      
    }

    private void OnDisable()
    {
        image.release();
        image.Dispose();
        image = null;
        if (!TickBtnClicked) {
            Destroy(texCam);
        }            
        Destroy(texEdges);
        Destroy(texModel);
        Destroy(webCamTextureToMatHelper);
        StopVideoRecording();
    }

    public void OnContrastBtnClicked()
    {
        if (filtermode == FILTERMODE.LINE)
            filtermode = FILTERMODE.BLEND;
        else if (filtermode == FILTERMODE.BLEND)
            filtermode = FILTERMODE.LINE;

        if (filtermode == FILTERMODE.LINE)
        {
            rimgmodel.color = new Color(255, 255, 255, 1);
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
        rimgmodel.color = new Color(255, 255, 255, 1);
        rimgmodel.texture = texEdges;
    }

    bool TickBtnClicked = false;
    public void OnTickBtnClicked()
    {
        TickBtnClicked = true;
        GVs.SCENE_MANAGER.loadPreviewResultScene();        
        PreviewResultScripts.texture = texCam;
        if(webcamCapture!=null)
            ResultScripts.videoname = webcamCapture.filename;
    }
}
