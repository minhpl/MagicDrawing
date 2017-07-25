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
    private int CamWidth = 4800;
    private int CamHeight = 6400;
    // Use this for initialization
    private WebCamTextureToMatHelper webcamTextureTomat;
    void Start () {
        rawImgCam = goCam.GetComponent<RawImage>();
        webcamTextureTomat = goCam.GetComponent<WebCamTextureToMatHelper>();
        webcamTextureTomat.Init(null, webcamTextureTomat.requestWidth, webcamTextureTomat.requestHeight, webcamTextureTomat.requestIsFrontFacing);
        //WebCamDevice[] devices = WebCamTexture.devices;
        //if (devices.Length == 0)
        //{
        //    Utilities.Log("No camera detected");
        //    camAvailable = false;
        //    return;
        //}
        //Utilities.Log("how many camera {0}", devices.Length);

        //for (int i = 0; i < devices.Length; i++)
        //{
        //    if (devices[i].isFrontFacing == isFronFacing)
        //    {
        //        webcamTex = new WebCamTexture(devices[i].name, CamWidth, CamHeight);
        //        webCamDevice = devices[i];
        //        break;
        //    }               
        //}        
        //if (webcamTex == null)
        //{
        //    Utilities.Log("Unable to find back camera");
        //    webcamTex = new WebCamTexture(devices[0].name, CamWidth, CamHeight);
        //    webCamDevice = devices[0];            
        //}
        //webcamTex.Play();
        ////if (webCamDevice.isFrontFacing == true)
        ////    rawImgCam.rectTransform.localScale = new Vector3(-1, 1, 1);
        ////if(webCamDevice.isFrontFacing == false)
        ////    rawImgCam.rectTransform.localScale = new Vector3(-1, 1, 1);
        //rawImgCam.texture = webcamTex;
        //camAvailable = true;
        //var width = webcamTex.width;
        //var height = webcamTex.height;
        //var ratio = width / (float)height;
        //goCam.GetComponent<AspectRatioFitter>().aspectRatio = ratio;
        
        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }

    IEnumerator Worker()
    {
        while (true)
        {
            yield return null;
            //if(camAvailable)
            {

                webcamTex = webcamTextureTomat.GetWebCamTexture();
                var width = webcamTex.width;
                var height = webcamTex.height;
                var ratio = width / (float)height;
                goCam.GetComponent<AspectRatioFitter>().aspectRatio = ratio;
                Utilities.Log("width is {0}, height is {1}", width, height);
                rawImgCam.texture = webcamTex;

                //long time = DateTime.Now.Millisecond;
                float scaleY = webcamTex.videoVerticallyMirrored ? -1f : 1f;
                int orient = webcamTex.videoRotationAngle;
                Utilities.Log("orient is {0}, videoVerticallyMirrored is {1}", orient, webcamTex.videoVerticallyMirrored);
                rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
                //rawImgCam.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
                //Utilities.Log("Excution time is {0}", DateTime.Now.Millisecond - time);
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

        isFronFacing = !isFronFacing;
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == isFronFacing)
            {
                webCamDevice = devices[i];
                webcamTex = new WebCamTexture(devices[i].name, 480, 680);
                
                break;
            }
        }

        if (webcamTex == null && devices.Length > 0)
        {
            webcamTex = new WebCamTexture(devices[0].name, 480, 680);
            webCamDevice = devices[0];
        }
            

        webcamTex.Play();
        rawImgCam.texture = webcamTex;
        if (webCamDevice.isFrontFacing==true)
            rawImgCam.rectTransform.localScale = new Vector3(-1, 1, 1);
        //else
        //    rawImgCam.rectTransform.localScale = new Vector3(1, -1, 1);
        rawImgCam.texture = webcamTex;
        //optimizationWebCamTextureToMatHelper.Init(null, optimizationWebCamTextureToMatHelper.requestWidth,
        //    optimizationWebCamTextureToMatHelper.requestHeight, !optimizationWebCamTextureToMatHelper.requestIsFrontFacing);
    }       
}
