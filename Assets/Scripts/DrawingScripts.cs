using OpenCVForUnity;
using OpenCVForUnityExample;
using System;
using System.Collections;
using System.Collections.Generic;
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
            //webCamTextureToMatHelper.Init(null, webCamTextureToMatHelper.requestWidth, width, webCamTextureToMatHelper.requestIsFrontFacing);           
        }       

        Mat camMat = webCamTextureToMatHelper.GetMat();
        texCam = new Texture2D(camMat.width(), camMat.height(), TextureFormat.RGBA32, false);

        threshold = GetComponent<Threshold>();
        Debug.LogFormat("Threshold value is {0}", threshold.thresholdValue);
 
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
            imgPath = GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + "T0005.png";
            texture = GFs.LoadPNG(GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + "T0005.png");
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
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(slider); });
        utilities = new Utilities();        
        goModel.SetActive(true);
        loaded = true;
        ValueChangeCheck(slider);
    }

    public void ValueChangeCheck(Slider slider)
    {
        if (loaded)
        {
            Debug.LogFormat("slider {0}", slider == null);
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
        {
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();
            if (isRecording && webcamCapture!=null)
            {
                webcamCapture.write(rgbaMat);
            }
            Mat a = warpPerspective.warpPerspective(rgbaMat);
            Utils.matToTexture2D(a, texCam, webCamTextureToMatHelper.GetBufferColors());
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
        Debug.Log("Xin chao validator");
    }
}
