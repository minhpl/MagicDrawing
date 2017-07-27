using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using OpenCVForUnity;
using UnityEngine.UI;
using System;

public class SnapImageSceneScripts : MonoBehaviour
{

    public GameObject goCam;
    private RawImage rawImgCam;
    private bool camAvailable;
    private WebCamTexture webcamTex;
    private WebCamDevice webCamDevice;
    private bool isFronFacing = true;
    private int requestWidth = 1920;
    private int requestHeight = 1920;
    // Use this for initialization
    private WebCamTextureToMatHelper webcamTextureTomat;

    Texture2D texture;
    void Start()
    {
        if (MakePersistentObject.Instance)
            MakePersistentObject.Instance.gameObject.SetActive(false);

        rawImgCam = goCam.GetComponent<RawImage>();
        webcamTextureTomat = goCam.GetComponent<WebCamTextureToMatHelper>();
        webcamTextureTomat.Init(null, requestWidth, requestHeight, isFronFacing);

        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        //WebCamDevice[] devices = WebCamTexture.devices;
        //if (devices.Length == 0)
        //{
        //    Utilities.Log("No camera detected");
        //    camAvailable = false;
        //    return;
        //}

        //for (int i = 0; i < devices.Length; i++)
        //{
        //    if (devices[i].isFrontFacing == isFronFacing)
        //    {
        //        webcamTex = new WebCamTexture(devices[i].name, requestWidth, requestHeight);
        //        webCamDevice = devices[i];
        //        break;
        //    }
        //}

        //if (webcamTex == null)
        //{
        //    Utilities.Log("Unable to find back camera");
        //    webcamTex = new WebCamTexture(devices[0].name, requestWidth, requestHeight);
        //    webCamDevice = devices[0];
        //}

        webcamTex = webcamTextureTomat.GetWebCamTexture();
        webcamTex.Play();
        rawImgCam.texture = webcamTex;

        //rawImgCam.texture = null;
        var widthCam = webcamTex.width;
        var heightCam = webcamTex.height;

        var ratioWH = widthCam / (float)heightCam;
        var ratioHW = heightCam / (float)widthCam;
        Utilities.Log("Webcam width is {0}, webcam height is {1}", widthCam, heightCam);

        var widthDisplay = rawImgCam.rectTransform.rect.width;
        var heightDisplay = rawImgCam.rectTransform.rect.height;
        var ratioDisplay = widthDisplay / (float)heightDisplay;
        Utilities.Log("Display Initial width is {0}, height is {1}", widthDisplay, heightDisplay);


        int orient = webcamTex.videoRotationAngle;
        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
        Utilities.Log("orient here is {0}", orient);
        bool isVerticalMirror = webcamTex.videoVerticallyMirrored;
        Utilities.Log("is vertical mirror {0}", isVerticalMirror);

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

        widthDisplay = rawImgCam.rectTransform.rect.width;
        heightDisplay = rawImgCam.rectTransform.rect.height;

        Utilities.Log("Display later in start function have width is {0}, height is {1}", widthDisplay, heightDisplay);


        if (isFronFacing && (orient == 90 || orient == 270))
        {
            rawImgCam.rectTransform.localScale = new Vector3(1, -1, 1);
        }


        camAvailable = true;
        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }

    IEnumerator Worker()
    {
        while (true)
        {
            yield return null;
            if (useWebcamTexture ==false && camAvailable && webcamTextureTomat.IsPlaying() && webcamTextureTomat.DidUpdateThisFrame())
            {
                Mat a = webcamTextureTomat.GetMat();
                texture = new Texture2D(a.width(), a.height(), TextureFormat.RGBA32, false);

                Utils.fastMatToTexture2D(a, texture);
                rawImgCam.texture = texture;
            }
            else if(useWebcamTexture==true)
            {
                rawImgCam.texture = webcamTex;
            }
        }
    }

    private void OnDisable()
    {
        if(webcamTex)
            webcamTex.Stop();
        webcamTex = null;
    }

    public void OnChangeCameraButton()
    {
        webcamTex.Stop();
        webcamTex = null;
        WebCamDevice[] devices = WebCamTexture.devices;
        rawImgCam.rectTransform.sizeDelta = new Vector2(0, 0);
        rawImgCam.transform.localScale = new Vector3(1, 1, 1);
        //rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        isFronFacing = !isFronFacing;
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == isFronFacing)
            {
                webCamDevice = devices[i];
                webcamTex = new WebCamTexture(devices[i].name, requestWidth, requestHeight);
                break;
            }
        }

        if (webcamTex == null && devices.Length > 0)
        {
            webcamTex = new WebCamTexture(devices[0].name, requestWidth, requestHeight);
            webCamDevice = devices[0];
        }

        webcamTex.Play();

        var widthCam = webcamTex.width;
        var heightCam = webcamTex.height;

        var ratioWH = widthCam / (float)heightCam;
        var ratioHW = heightCam / (float)widthCam;
        Utilities.Log("Webcam width is {0}, webcam height is {1}", widthCam, heightCam);

        var widthDisplay = rawImgCam.rectTransform.rect.width;
        var heightDisplay = rawImgCam.rectTransform.rect.height;
        var ratioDisplay = widthDisplay / (float)heightDisplay;

        Utilities.Log("Display in change function have width is {0}, height is {1}", widthDisplay, heightDisplay);

        int orient = webcamTex.videoRotationAngle;
        Utilities.Log("orient is {0}", orient);
        bool isVerticalMirror = webcamTex.videoVerticallyMirrored;
        Utilities.Log("is vertical mirror {0}", isVerticalMirror);

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


        widthDisplay = rawImgCam.rectTransform.rect.width;
        heightDisplay = rawImgCam.rectTransform.rect.height;

        Utilities.Log("Display later in change function have width is {0}, height is {1}", widthDisplay, heightDisplay);

        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
        if (isFronFacing && (orient == 270 || orient == 90))
        {
            rawImgCam.transform.localScale = new Vector3(1, -1, 1);
        }
        rawImgCam.texture = webcamTex;
    }

    bool useWebcamTexture = false;

    public void OnSnapBtnClicked()
    {
        //useWebcamTexture = true;
        StartCoroutine(Snap());
    }

    IEnumerator Snap()
    {
        webcamTex.Stop();
        webcamTex.requestedWidth = 1920;
        webcamTex.requestedHeight = 1920;
        webcamTex.Play();
        Utilities.Log("webcamtexture width is {0}, height is {1}", webcamTex.width, webcamTex.height);
        rawImgCam.texture = webcamTex;
        int countNonZero = 0;
        int numBlackFrame = 0;
        Mat mat = new Mat(webcamTex.height, webcamTex.width, CvType.CV_8UC4);
        Mat singleChannel = new Mat();
        int skipNonZeroFrame = 0;

        Color32[] buffers = new Color32[webcamTex.width * webcamTex.height];
        rawImgCam.texture = null;
        while (countNonZero == 0 || skipNonZeroFrame < 10)
        {                       
            //if (skipNonZeroFrame > 0)
                //webcamTex.Pause();
            Utils.webCamTextureToMat(webcamTex, mat, buffers);
            Core.extractChannel(mat, singleChannel, 1);            
            countNonZero = Core.countNonZero(singleChannel);
            if (countNonZero > 0)
            {                
                skipNonZeroFrame++;
            }
                
            Utilities.Log("Count non zero of mat is {0}", countNonZero);
            numBlackFrame++;
            yield return null;
            webcamTex.Play();
        }

        numBlackFrame--;
        Utilities.Log("Number black frame is {0}", numBlackFrame);

        Texture2D texture2d = new Texture2D(mat.width(), mat.height(),TextureFormat.RGBA32,false);
        Utils.matToTexture2D(mat, texture2d);
        
        rawImgCam.texture = texture2d;       
    }
}
