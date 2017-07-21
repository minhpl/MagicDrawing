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






        var heavyMethod = Observable.Start(() =>
        {
            // heavy method...
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
            return 10;
        });

        var heavyMethod2 = Observable.Start(() =>
        {
            // heavy method...
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
            return 30;
        });

        // Join and await two other thread values
        Observable.WhenAll(heavyMethod, heavyMethod2)
            .ObserveOnMainThread() // return to main thread
            .Subscribe(xs =>
            {
                // Unity can't touch GameObject from other thread
                // but use ObserveOnMainThread, you can touch GameObject naturally.
                Debug.Log(xs[0] + ":" + xs[1]);
            });


        Debug.LogFormat("hiiiiiiiiiiiiiiiiiiiiiiiiii");
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
            Debug.LogFormat("{0}{1}", width, heigh);
            webCamTextureToMatHelper.Init(null, 640, 480, webCamTextureToMatHelper.requestIsFrontFacing);
            warpPerspective.Init(webCamTextureToMatHelper.GetMat());                    
        }       

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
        StartCoroutine(loadModel());
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
        Texture2D texture;
        string imgPath;
        if (model != null)
        {
            imgPath = GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + model.image;
            texture = GFs.LoadPNG(imgPath);            
        }
        else
        {
            imgPath = GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + "T0027.jpg";
            texture = GFs.LoadPNG(GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + "T0027.jpg");
            Debug.LogFormat("");
        }

        var rimgmodel = goModel.GetComponent<RawImage>();
        var rimgCam = goModel.GetComponent<RawImage>();
        
        float width = texture.width;
        float heigh = texture.height;
        float modelAreaWidth = rimgmodel.rectTransform.rect.width;
        float modelAreaHeight = rimgmodel.rectTransform.rect.height;
        
        float ratio = width / heigh;
        float ratioDisplay = modelAreaWidth / modelAreaHeight;

        var ratioWidth = width / modelAreaWidth;
        var ratioHeight = heigh / modelAreaHeight;

        if (ratio > ratioDisplay)
        {
            rimgmodel.rectTransform.localScale = new Vector3(1, (modelAreaWidth / modelAreaHeight) *(heigh/width), 1);
        }
        else
        {
            rimgmodel.rectTransform.localScale = new Vector3((modelAreaHeight / modelAreaWidth) * ratio, 1, 1);
        }       

        imgPath = GVs.APP_PATH + "/" + imgPath;
        image = Imgcodecs.imread(imgPath, Imgcodecs.IMREAD_UNCHANGED);
        Imgproc.cvtColor(image, image, Imgproc.COLOR_BGRA2RGBA);
        athreshold = GetComponent<AdaptiveThreshold>();
        texEdges = new Texture2D(image.width(), image.height(),TextureFormat.ARGB32,false);

        //Mat edges = athreshold.adapTiveThreshold(image);                
        //colorsBuffer = new Color32[edges.width() * edges.height()];
        //Utils.matToTexture2D(edges, texEdges, colorsBuffer);

        rimgmodel.texture = texture;                
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

        ValueChangeCheck(slider);
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
            currentSliderValue = slider.value;
            if (!updateAble) return;
            Debug.LogFormat("slider {0}", slider == null);
            /*
            if (slider)
                athreshold.setParameter(slider.value);
            Mat edges = athreshold.adapTiveThreshold(image);

            Mat tempMat = new Mat();
            Core.bitwise_not(edges, tempMat);
            List<Mat> listMat = new List<Mat>();
            Mat zeroMat = Mat.zeros(tempMat.size(), CvType.CV_8U);
            listMat.Add(tempMat);
            listMat.Add(zeroMat);
            listMat.Add(zeroMat);
            listMat.Add(tempMat);
            Mat redMat = new Mat();
            Core.merge(listMat, redMat);


            utilities.set(edges);
            colorsBuffer = new Color32[edges.width() * edges.height()];
            Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
            var rimgmodel = goModel.GetComponent<RawImage>();
            rimgmodel.texture = texEdges;
            redMat.Dispose();
            */
            if (job != null)
            {
                updateAble = false;
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

        Debug.LogFormat("Xin chao tat ca");
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

    // Update is called once per frame
    //void Update()
    void Update()
    {
        if (finishJob)
        {
            AfterProcess( _redMat);
        }
        if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
        {
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();
            //if (isRecording && webcamCapture != null)
            //{
            //    webcamCapture.write(rgbaMat);
            //}
            //Mat a = warpPerspective.warpPerspective(rgbaMat);
            Utils.matToTexture2D(rgbaMat, texCam, webCamTextureToMatHelper.GetBufferColors());
            goCam.GetComponent<RawImage>().texture = texCam;
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
