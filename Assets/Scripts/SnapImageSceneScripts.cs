using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using OpenCVForUnity;
using UnityEngine.UI;
using System;

public class SnapImageSceneScripts : MonoBehaviour {

    public GameObject goCam;
    private RawImage rawImgCam;
    private bool camAvailable;
    private WebCamTexture webcamTex;
    private WebCamDevice webCamDevice;
    private bool isFronFacing = false;
    private int requestWidth = 480;
    private int requestHeight = 640;
    // Use this for initialization
    private WebCamTextureToMatHelper webcamTextureTomat;
    void Start () {
        rawImgCam = goCam.GetComponent<RawImage>();
        webcamTextureTomat = goCam.GetComponent<WebCamTextureToMatHelper>();
        //webcamTextureTomat.Init(null, webcamTextureTomat.requestWidth, webcamTextureTomat.requestHeight, webcamTextureTomat.requestIsFrontFacing);

        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Utilities.Log("No camera detected");
            camAvailable = false;
            return;
        }

        //Utilities.Log("how many camera {0}", devices.Length);

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == isFronFacing)
            {
                webcamTex = new WebCamTexture(devices[i].name, requestWidth, requestHeight);
                webCamDevice = devices[i];
                break;
            }
        }

        if (webcamTex == null)
        {
            Utilities.Log("Unable to find back camera");
            webcamTex = new WebCamTexture(devices[0].name, requestWidth, requestHeight);
            webCamDevice = devices[0];
        }

        webcamTex.Play();
        rawImgCam.texture = webcamTex;


        var widthCam = webcamTex.width;
        var heightCam = webcamTex.height;

        var ratioWH = widthCam / (float)heightCam;
        var ratioHW = heightCam / (float)widthCam;
        Utilities.Log("Webcam width is {0}, webcam height is {1}", widthCam, heightCam);

        var widthDisplay = rawImgCam.rectTransform.rect.width;
        var heightDisplay = rawImgCam.rectTransform.rect.height;
        var ratioDisplay = widthDisplay / (float)heightDisplay;

        int orient = webcamTex.videoRotationAngle;
        Utilities.Log("orient here is {0}", orient);
        if(orient==90 || orient==270)
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
        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
        camAvailable = true;

      

        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }

    IEnumerator Worker()
    {
        while (true)
        {
            yield return null;
            if (camAvailable)
            {

                webcamTex.Stop();

                webcamTex.requestedWidth = 480;
                webcamTex.requestedHeight = 640;

                webcamTex.Play();

                var widthCam = webcamTex.width;
                var heightCam = webcamTex.height;

                var ratioWH = widthCam / (float)heightCam;
                var ratioHW = heightCam / (float)widthCam;
                Utilities.Log("Webcam here have width is {0}, webcam height is {1}", widthCam, heightCam);
            }
        }
    }

    private void OnDisable()
    {
        //webcamTex.Stop();
        //webcamTex = null;
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

        int orient = webcamTex.videoRotationAngle;
        Utilities.Log("orient is {0}", orient);

        {
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
        }


        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
        if(isFronFacing && (orient == 270 || orient == 90))
        {
            rawImgCam.transform.localScale = new Vector3(1, -1, 1);
        }
        rawImgCam.texture = webcamTex;
    }       
}
