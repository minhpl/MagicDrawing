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
    public Slider slider;    

    public Threshold threshold;
    public AdaptiveThreshold athreshold;
    WarpPerspective warpPerspective;
    public Mat image;
    private Color32[] colorsBuffer;
    private Texture2D texEdges;
    private Texture2D texCam;
    Utilities utilities;
    WebCamTextureToMatHelper webCamTextureToMatHelper;
    bool loaded = false;
    WebcamVideoCapture webcamCapture;

    private void Awake()
    {
        if (MakePersistentObject.Instance)
            MakePersistentObject.Instance.gameObject.SetActive(false);

        int delayTime = 100;
        var onSliderValueStream = slider.onValueChanged.AsObservable();
        //onSliderValueStream.Buffer(onSliderValueStream.Throttle(TimeSpan.FromMilliseconds(delayTime)))
        //    .Subscribe(list => Debug.LogFormat("list count is {0}", list[list.Count - 1]));
        onSliderValueStream.Buffer(onSliderValueStream.Throttle(TimeSpan.FromMilliseconds(delayTime)))
            .Subscribe(delegate { ValueChangeCheck(slider); });
        //onSliderValueStream.Sample(TimeSpan.FromMilliseconds(delayTime)).Subscribe(list => Debug.Log(list));        
        //slider.onValueChanged.AddListener(delegate { ValueChangeCheck(slider); });       
    }

    void OnSliderValueChaned(Slider slider)
    {
        Debug.LogFormat("Slider value is {0}", slider.value);
    }


    // Use this for initialization
    void Start () {        
        webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
        warpPerspective = gameObject.GetComponent<WarpPerspective>();        
        if(!webCamTextureToMatHelper.IsInited())
        {
            int width = (int)goCam.GetComponent<RawImage>().rectTransform.rect.width;
            int heigh = (int)goCam.GetComponent<RawImage>().rectTransform.rect.height;            
            webCamTextureToMatHelper.Init(null, 640, 480, webCamTextureToMatHelper.requestIsFrontFacing);
            warpPerspective.Init(webCamTextureToMatHelper.GetMat());                    
        }
        utilities = new Utilities();

        Mat camMat = webCamTextureToMatHelper.GetMat();
        texCam = new Texture2D(camMat.width(), camMat.height(), TextureFormat.RGBA32, false);

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
    }
    Job job;

    void OnDestroy()
    {
        webCamTextureToMatHelper.Stop();
        webCamTextureToMatHelper.Dispose();
    }

    IEnumerator loadModel()
    {
        yield return null;
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
        image = Imgcodecs.imread(imgPath, Imgcodecs.IMREAD_UNCHANGED);

        var rimgmodel = goModel.GetComponent<RawImage>();
        var rimgCam = goModel.GetComponent<RawImage>();

        float width = image.width();
        float heigh = image.height();

        float modelAreaWidth = rimgmodel.rectTransform.rect.width;
        float modelAreaHeight = rimgmodel.rectTransform.rect.height;
        Debug.LogFormat("ModelAreaWidth = {0}, ModelAreaHeight = {1}", modelAreaWidth, modelAreaHeight);

        float ratio = width / heigh;
        float ratioDisplay = modelAreaWidth / modelAreaHeight;

        var ratioWidth = width / modelAreaWidth;
        var ratioHeight = heigh / modelAreaHeight;

        if (ratio > ratioDisplay)
        {
            rimgmodel.rectTransform.localScale = new Vector3(1, (modelAreaWidth / modelAreaHeight) * (heigh / width), 1);
        }
        else
        {
            rimgmodel.rectTransform.localScale = new Vector3((modelAreaHeight / modelAreaWidth) * ratio, 1, 1);
        }
        
        Imgproc.cvtColor(image, image, Imgproc.COLOR_BGRA2RGBA);
        athreshold = GetComponent<AdaptiveThreshold>();
        athreshold.setParameter(slider.value);
        texEdges = new Texture2D(image.width(), image.height(), TextureFormat.ARGB32, false);
        Mat edges = athreshold.adapTiveThreshold(image);
        Mat redMat = new Mat();
        utilities.makeMonoAlphaMat(edges, redMat);
        colorsBuffer = new Color32[edges.width() * edges.height()];
        Utils.matToTexture2D(redMat, texEdges, colorsBuffer);

        rimgmodel.texture = texEdges;
        utilities = new Utilities();
        goModel.SetActive(true);
        loaded = true;


        job = new Job();
        job.rimgmodel = goModel.GetComponent<RawImage>();
        job.athreshold = athreshold;
        job.utilities = utilities;
        job.image = image;
        job.rimgmodel = rimgmodel;
        job.texEdges = texEdges;
    }

    bool updateAble = true;
    IEnumerator DelayUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        updateAble = true;
    }
    private float currentSliderValue = 0;
    public void ValueChangeCheck(Slider slider)
    {        
        if (loaded)
        {
            if (slider)
            currentSliderValue = slider.value;
            if (!updateAble) return;
            
            if (job != null)
            {
                updateAble = false;
                if (slider)
                    job.athreshold.setParameter(slider.value);
                job.Start(( redMat) =>
                {
                    _redMat = redMat;
                    finishJob = true;
                });
            }
        }
    }
    bool finishJob = false;
    Mat  _redMat;

    public void AfterProcess(Mat redMat)
    {        
        finishJob = false;
        
        Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
        var rimgmodel = goModel.GetComponent<RawImage>(); ;
        rimgmodel.texture = texEdges;
        redMat.Dispose();
        if (currentSliderValue != job.athreshold._sliderValue)
        {
            updateAble = true;
            ValueChangeCheck(slider);
        }
        else
            StartCoroutine(DelayUpdate());
    }

    IEnumerator Worker()
    {
        while (true)
        {    
            yield return null;
            if (finishJob)
            {
                AfterProcess(_redMat);
            }
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = webCamTextureToMatHelper.GetMat();
                //if (isRecording && webcamCapture != null)
                //{
                //    webcamCapture.write(rgbaMat);
                //}
                Mat a = warpPerspective.warpPerspective(rgbaMat);
                Utils.matToTexture2D(a, texCam, webCamTextureToMatHelper.GetBufferColors());
                goCam.GetComponent<RawImage>().texture = texCam;
            }
        }
    }


    public bool isRecording = false;
    public void StartRecordVideo()
    {        
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

    private void OnValidate()
    {
        //Debug.Log("Xin chao validator");
    }
}
