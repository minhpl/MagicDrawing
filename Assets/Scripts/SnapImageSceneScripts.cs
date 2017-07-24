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
    OptimizationWebCamTextureToMatHelper optimizationWebCamTextureToMatHelper;
    private Texture2D texCam;
    private Color32[] colorsBuffer;
    private RawImage rawImgCam;
    // Use this for initialization
    void Start () {
        rawImgCam = goCam.GetComponent<RawImage>();
        optimizationWebCamTextureToMatHelper = gameObject.GetComponent<OptimizationWebCamTextureToMatHelper>();
        if (!optimizationWebCamTextureToMatHelper.IsInited())
        {
            optimizationWebCamTextureToMatHelper.Init(null, 480, 680, optimizationWebCamTextureToMatHelper.requestIsFrontFacing);            
        }

        Mat camMat = optimizationWebCamTextureToMatHelper.GetMat();
        var camWidth = camMat.width();
        var camHeight = camMat.height();
        var camRatio = camWidth / (float)camHeight;

        var rawImgCamera = goCam.GetComponent<RawImage>();
        var width = rawImgCamera.rectTransform.rect.width;
        var height = rawImgCamera.rectTransform.rect.height;
        var ratio = width / (float)height;

        if(camRatio < ratio)
        {
        	
        }

        //goCam.transform.localScale = new Vector3(camWidth, camHeight, 1);
        


        // Debug.LogFormat("camwidth is {0}, camheight is {1}", camMat.width(), camMat.height());
        // Debug.LogFormat("width is {0}, height is {1}", width, height);

        texCam = new Texture2D(camMat.width(), camMat.height(), TextureFormat.RGBA32, false);
        colorsBuffer = new Color32[camMat.width() * camMat.height()];

        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }

    IEnumerator Worker()
    {
        while (true)
        {
            yield return null;
            if (optimizationWebCamTextureToMatHelper.IsPlaying() && optimizationWebCamTextureToMatHelper.DidUpdateThisFrame())
            {
                long time = DateTime.Now.Millisecond;
                Mat rgbaMat = optimizationWebCamTextureToMatHelper.GetMat();
                Utils.matToTexture2D(rgbaMat, texCam, colorsBuffer);
                //WebCamTexture wct = optimizationWebCamTextureToMatHelper.GetWebCamTexture();
                //texCam.SetPixels(wct.GetPixels());
                //texCam.Apply();
                rawImgCam.texture = texCam;                
                Utilities.Log("{0}", DateTime.Now.Millisecond - time);
            }
        }
    }



    private void OnDisable()
    {
        optimizationWebCamTextureToMatHelper.Stop();
        optimizationWebCamTextureToMatHelper.Dispose();
    }

    public void OnChangeCameraButton()
    {
        optimizationWebCamTextureToMatHelper.Init(null, optimizationWebCamTextureToMatHelper.requestWidth,
            optimizationWebCamTextureToMatHelper.requestHeight, !optimizationWebCamTextureToMatHelper.requestIsFrontFacing);
    }       
}
