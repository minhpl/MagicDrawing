using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using OpenCVForUnity;
using UnityEngine.UI;
using System;
using System.IO;
public class SnapImageSceneScripts : MonoBehaviour
{
    public GameObject goCam;
    private RawImage rawImgCam;
    private bool camAvailable;
    private WebCamTexture webcamTex;
    private WebCamDevice webCamDevice;
    private bool isFronFacing = false;
    private int requestWidth = 1920;
    private int requestHeight = 1920;
    private WebCamTextureToMatHelper webcamTextureTomat;
    Mat snapMat;
    private void Awake()
    {
        if (MakePersistentObject.Instance)
            MakePersistentObject.Instance.gameObject.SetActive(false);
    }
    void Start()
    {
        rawImgCam = goCam.GetComponent<RawImage>();
        ShowCam();
        //MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }
    void ShowCam()
    {
        if (webcamTextureTomat == null)
            webcamTextureTomat = goCam.GetComponent<WebCamTextureToMatHelper>();
        webcamTextureTomat.onInitialized.RemoveAllListeners();
        webcamTextureTomat.onInitialized.AddListener(() =>
        {
            webcamTex = webcamTextureTomat.GetWebCamTexture();
            webcamTex.Play();
            rawImgCam.texture = webcamTex;

            var widthCam = webcamTex.width;
            var heightCam = webcamTex.height;
            var ratioWH = widthCam / (float)heightCam;
            var ratioHW = heightCam / (float)widthCam;
            var widthDisplay = rawImgCam.rectTransform.rect.width;
            var heightDisplay = rawImgCam.rectTransform.rect.height;
            var ratioDisplay = widthDisplay / (float)heightDisplay;

            int orient = webcamTex.videoRotationAngle;

            rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, -orient);
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

            if (isFronFacing && (orient == 90 || orient == 270))
            {
                rawImgCam.rectTransform.localScale = new Vector3(1, -1, 1);
            }

            if (WebCamTexture.devices.Length == 1 && WebCamTexture.devices[0].isFrontFacing == true && orient == 0)
            {
                rawImgCam.rectTransform.localScale = new Vector3(-1, 1, 1);
            }

            camAvailable = true;
        });
        webcamTextureTomat.Initialize(null, requestWidth, requestHeight, isFronFacing);
    }

    private void OnDisable()
    {
        if (webcamTex)
            webcamTex.Stop();
        webcamTex = null;
    }

    public void OnChangeCameraButton()
    {
        webcamTextureTomat.Stop();
        webcamTextureTomat = null;
        rawImgCam.rectTransform.sizeDelta = new Vector2(0, 0);
        rawImgCam.transform.localScale = new Vector3(1, 1, 1);
        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        isFronFacing = !isFronFacing;
        ShowCam();
    }

    public void OnSnapBtnClicked()
    {
        MainThreadDispatcher.StartUpdateMicroCoroutine(Snap());
    }

    IEnumerator Snap()
    {
        //webcamtex.stop();
        webcamTex.requestedWidth = 1920;
        webcamTex.requestedHeight = 1920;
        webcamTex.Play();
        rawImgCam.texture = webcamTex;
        int countNonZero = 0;
        int numBlackFrame = 0;
        int numberFrameSkip = 0;

        snapMat = new Mat(webcamTex.height, webcamTex.width, CvType.CV_8UC4);
        Mat singleChannel = new Mat();
        Color32[] buffers = new Color32[webcamTex.width * webcamTex.height];
        rawImgCam.texture = null;

        while (countNonZero == 0 || numberFrameSkip < 1)
        {
            Utils.webCamTextureToMat(webcamTex, snapMat, buffers);
            Core.extractChannel(snapMat, singleChannel, 1);
            countNonZero = Core.countNonZero(singleChannel);
            if (countNonZero > 0)
            {
                numberFrameSkip++;
            }
            else
            {
                numBlackFrame++;
            }

            Utilities.Log("Count non zero of mat is {0}", countNonZero);
            yield return null;
            webcamTex.Play();
        }

        singleChannel.Dispose();
        snapMat.Dispose();

        Mat rgbaMat = webcamTextureTomat.GetMat();

        rawImgCam.rectTransform.localScale = new Vector3(1, 1, 1);
        rawImgCam.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        rawImgCam.rectTransform.sizeDelta = new Vector2(0, 0);

        var widthCam = rgbaMat.width();
        var heightCam = rgbaMat.height();
        var ratioWH = widthCam / (float)heightCam;
        var ratioHW = heightCam / (float)widthCam;
        var widthDisplay = rawImgCam.rectTransform.rect.width;
        var heightDisplay = rawImgCam.rectTransform.rect.height;
        var ratioDisplay = widthDisplay / (float)heightDisplay;

        var newWidth = 0f;
        var newHeight = 0f;

        if (ratioWH < ratioDisplay)
        {
            newWidth = widthCam;
            newHeight = newWidth / ratioDisplay;
        }
        else
        {
            newHeight = heightCam;
            newWidth = newHeight * ratioDisplay;
        }

        rawImgCam.rectTransform.sizeDelta = new Vector2(0, 0);
        var deltaWidthMat = (int)(widthCam - newWidth);
        var deltaHeightMat = (int)(heightCam - newHeight);
        var matDisplayWidth = widthCam - deltaWidthMat;
        var matDisplayHeight = heightCam - deltaHeightMat;

        var rect = new OpenCVForUnity.Rect(deltaWidthMat / 2, deltaHeightMat / 2, matDisplayWidth, matDisplayHeight);
        snapMat = rgbaMat.submat(rect);

        Utilities.Log("snap image width = {0}, height = {1}", snapMat.width(), snapMat.height());

        Texture2D texRgbaMat = new Texture2D(snapMat.width(), snapMat.height(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(snapMat, texRgbaMat);
        rawImgCam.texture = texRgbaMat;        
    }

    public void OnDrawBtnClicked()
    {
        webcamTextureTomat.Stop();
        webcamTextureTomat = null;
        if (snapMat != null)
        {
            DrawingScripts.image = snapMat;

            var dateTimeNow = DateTime.Now.ToString(Utilities.customFmts);
            string dirPathMPModel = null;
            if (Application.platform == RuntimePlatform.Android)
            {
                dirPathMPModel = GVs.androidDirMPModel;
            }
            else
            {
                dirPathMPModel = GVs.pcDirMPModel;
            }

            if (!Directory.Exists(dirPathMPModel))
            {
                Directory.CreateDirectory(dirPathMPModel);
            }

            var MPModelPath = dirPathMPModel + dateTimeNow + ".png";
            Imgproc.cvtColor(snapMat, snapMat, Imgproc.COLOR_BGR2RGB);
            Imgcodecs.imwrite(MPModelPath, snapMat);
            HistoryScripts.AddHistoryItem(new HistoryModel(MPModelPath, MPModelPath, HistoryModel.IMAGETYPE.SNAP));
            Texture2D texture = new Texture2D(snapMat.width(), snapMat.height(), TextureFormat.BGRA32, false);
            Utils.matToTexture2D(snapMat, texture);
            DrawingScripts.texModel = texture;
            DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
            GVs.SCENE_MANAGER.loadDrawingScene();
        }            
    }

}
