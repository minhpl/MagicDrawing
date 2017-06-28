using UnityEngine;
using System.Collections;
using OpenCVForUnityExample;
using UnityEngine.UI;
using System.IO;
using System;
using Assets.OpenCVForUnity.Examples.MagicDrawing;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;

namespace MagicDrawing
{
    /// <summary>
    /// ComicFilter example.
    /// referring to the http://dev.classmethod.jp/smartphone/opencv-manga-2/.
    /// </summary>
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    [RequireComponent(typeof(LaplaceEdgeDetector))]
    [RequireComponent(typeof(SobelEdgeDetector))]
    [RequireComponent(typeof(ScharrEdgeDetector))]
    [RequireComponent(typeof(CannyEdgeDetector))]
    [RequireComponent(typeof(Threshold))]
    [RequireComponent(typeof(WarpPerspective))]
    [RequireComponent(typeof(AdaptiveThreshold))]
    public class MagicDrawing : MonoBehaviour
    {
        public Slider slider;

        /// <summary>
        /// The gray mat.
        /// </summary>
        Mat grayMat;

        /// <summary>
        /// The line mat.
        /// </summary>
        Mat lineMat;

        /// <summary>
        /// The mask mat.
        /// </summary>
        Mat maskMat;

        /// <summary>
        /// The background mat.
        /// </summary>
        Mat bgMat;

        /// <summary>
        /// The dst mat.
        /// </summary>
        Mat dstMat;

        /// <summary>
        /// The gray pixels.
        /// </summary>
        byte[] grayPixels;

        /// <summary>
        /// The mask pixels.
        /// </summary>
        byte[] maskPixels;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// The web cam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;
        
        LaplaceEdgeDetector laplaceEdgeDetector;
        SobelEdgeDetector sobelEdgeDetector;
        ScharrEdgeDetector scharrEdgeDetector;
        CannyEdgeDetector cannyEdgeDetector;
        Threshold threshold;
        AdaptiveThreshold aDaptiveThreshold;
        WarpPerspective warpPerspective;
        
        Mat snapImage;
        int stage = 1;      //1: hiển thị camera như bình thường và có nút chụp lại ảnh
                            //2: chụp lại ảnh sau đó hiển thị ảnh tĩnh 
                            //3: hiển thị ảnh đã tách biên trên nền video capture từ camera, với điều kiện camera đã
        Mat EdgeDetectedMat;
        Mat mergedMat;
        Utilities utilities;
            
        // Use this for initialization
        void Start()
        {            
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
            webCamTextureToMatHelper.Init();
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();           
            
            laplaceEdgeDetector = gameObject.GetComponent<LaplaceEdgeDetector>();
            sobelEdgeDetector = gameObject.GetComponent<SobelEdgeDetector>();
            scharrEdgeDetector = gameObject.GetComponent<ScharrEdgeDetector>();
            cannyEdgeDetector = gameObject.GetComponent<CannyEdgeDetector>();
            threshold = gameObject.GetComponent<Threshold>();
            warpPerspective = gameObject.GetComponent<WarpPerspective>();
            aDaptiveThreshold = gameObject.GetComponent<AdaptiveThreshold>();

            snapImage = new Mat();            
            EdgeDetectedMat = new Mat();
            //thresholdMat = new Mat();
            utilities = new Utilities();
            warpPerspective.Init(rgbaMat);
            mergedMat = new Mat();
            slider.onValueChanged.AddListener(delegate { slider_onValueChanged(); });
        }

        void slider_onValueChanged()
        {
            float sliderValue = slider.value;
            sobelEdgeDetector.setParameter(sliderValue);

            EdgeDetectedMat = sobelEdgeDetector.sobelEdgeDetect(snapImage);
            sobelEdgeDetector.adapTiveThreshold(EdgeDetectedMat, EdgeDetectedMat);
        }

        /// <summary>
        /// Raises the web cam texture to mat helper inited event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInited()
        {
            Debug.Log("OnWebCamTextureToMatHelperInited");
            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();
            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
            gameObject.GetComponent<Renderer>().material.mainTexture = texture;
            gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = webCamTextureMat.width();
            float height = webCamTextureMat.height();

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }

            grayMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1);
            lineMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1);
            maskMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1);

            //create a striped background.
            bgMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1, new Scalar(255));
            for (int i = 0; i < bgMat.rows() * 2.5f; i = i + 4)
            {
                Imgproc.line(bgMat, new Point(0, 0 + i), new Point(bgMat.cols(), -bgMat.cols() + i), new Scalar(0), 1);
            }

            dstMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1);

            grayPixels = new byte[grayMat.cols() * grayMat.rows() * grayMat.channels()];
            maskPixels = new byte[maskMat.cols() * maskMat.rows() * maskMat.channels()];
        }

        /// <summary>
        /// Raises the web cam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed()
        {
            Debug.Log("OnWebCamTextureToMatHelperDisposed");

            grayMat.Dispose();
            lineMat.Dispose();
            maskMat.Dispose();

            bgMat.Dispose();
            dstMat.Dispose();
            grayPixels = null;
            maskPixels = null;
        }

        /// <summary>
        /// Raises the web cam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }

        void comicFilterOriginal()
        {
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = webCamTextureToMatHelper.GetMat();
                Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
                //                      Utils.webCamTextureToMat (webCamTexture, grayMat, colors);
                bgMat.copyTo(dstMat);
                Imgproc.GaussianBlur(grayMat, lineMat, new Size(3, 3), 0);
                grayMat.get(0, 0, grayPixels);
                for (int i = 0; i < grayPixels.Length; i++)
                {
                    maskPixels[i] = 0;
                    if (grayPixels[i] < 70)
                    {
                        grayPixels[i] = 0;
                        maskPixels[i] = 1;
                    }
                    else if (70 <= grayPixels[i] && grayPixels[i] < 120)
                    {
                        grayPixels[i] = 100;
                    }
                    else
                    {
                        grayPixels[i] = 255;
                        maskPixels[i] = 1;
                    }
                }
                grayMat.put(0, 0, grayPixels);
                maskMat.put(0, 0, maskPixels);
                grayMat.copyTo(dstMat, maskMat);
                Imgproc.Canny(lineMat, lineMat, 20, 120);
                lineMat.copyTo(maskMat);
                Core.bitwise_not(lineMat, lineMat);
                lineMat.copyTo(dstMat, maskMat);
                //          Imgproc.putText (dstMat, "W:" + dstMat.width () + " H:" + dstMat.height () + " SO:" + Screen.orientation, new Point (5, dstMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar (0), 2, Imgproc.LINE_AA, false);
                //      Imgproc.cvtColor(dstMat,rgbaMat,Imgproc.COLOR_GRAY2RGBA);
                //              Utils.matToTexture2D (rgbaMat, texture);
                Utils.matToTexture2D(dstMat, texture, webCamTextureToMatHelper.GetBufferColors());
            }
        }
        
        void Update()
        {
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = webCamTextureToMatHelper.GetMat();
                if (stage == 1)
                {                                    
                    //Debug.Log(rgbaMat.channels());
                    Utils.matToTexture2D(rgbaMat, texture, webCamTextureToMatHelper.GetBufferColors());
                }
                else if(stage==2)
                {
                    Utils.matToTexture2D(snapImage,texture,webCamTextureToMatHelper.GetBufferColors());
                }
                else if(stage==3)
                {
                    Mat warpPerspectiveResult = warpPerspective.warpPerspective(rgbaMat);
                    //Debug.LogFormat("width2 is {0}", warpPerspectiveResult.size().width);
                    //Utils.matToTexture2D(warpPerspectiveResult, texture);                    
                    //EdgeDetectedMat = laplaceEdgeDetector.laplaceEdgeDetect(snapImage);
                    //laplaceEdgeDetector.adapTiveThreshold(EdgeDetectedMat, EdgeDetectedMat);      
                    //EdgeDetectedMat = sobelEdgeDetector.sobelEdgeDetect(snapImage);
                    //sobelEdgeDetector.adapTiveThreshold(EdgeDetectedMat, EdgeDetectedMat);
                    //EdgeDetectedMat = scharrEdgeDetector.scharrEdgeDetect(snapImage);
                    //scharrEdgeDetector.adapTiveThreshold(EdgeDetectedMat, EdgeDetectedMat);
                    //EdgeDetectedMat = cannyEdgeDetector.edgeDetect(snapImage);
                    //cannyEdgeDetector.adapTiveThreshold(EdgeDetectedMat, EdgeDetectedMat);                            
                    //EdgeDetectedMat = aDaptiveThreshold.adapTiveThreshold(snapImage);
                    //EdgeDetectedMat = threshold.threshold(snapImage);
                    utilities.OverlayOnRGBMat(EdgeDetectedMat, warpPerspectiveResult, mergedMat);
                    Utils.matToTexture2D(mergedMat, texture);   
                }
            }
        }

        /// <summary>
        /// Raises the disable event.
        /// </summary>
        void OnDisable()
        {
            webCamTextureToMatHelper.Dispose();
        }

        /// <summary>
        /// Raises the back button event.
        /// </summary>
        public void OnBackButton()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene("OpenCVForUnityExample");
#else
             Application.LoadLevel ("OpenCVForUnityExample");
#endif
        }

        /// <summary>
        /// Raises the play button event.
        /// </summary>
        public void OnPlayButton()
        {
            webCamTextureToMatHelper.Play();
        }

        /// <summary>
        /// Raises the pause button event.
        /// </summary>
        public void OnPauseButton()
        {
            webCamTextureToMatHelper.Pause();
        }

        /// <summary>
        /// Raises the stop button event.
        /// </summary>
        public void OnStopButton()
        {
            webCamTextureToMatHelper.Stop();
        }

        /// <summary>
        /// Raises the change camera button event.
        /// </summary>
        public void OnChangeCameraButton()
        {
            webCamTextureToMatHelper.Init(null, webCamTextureToMatHelper.requestWidth, webCamTextureToMatHelper.requestHeight, !webCamTextureToMatHelper.requestIsFrontFacing);
        }
        
        public Button btnRecord;

        public void OnSnapBtnClicked()
        {            
             Mat rgbaMat = webCamTextureToMatHelper.GetMat();
             rgbaMat.copyTo(snapImage);           
             stage = 2;
        }

        public void onBackBtnClicked()
        {
            if (stage>1) 
                stage = stage - 1;
        }        

        public void OnEdgeIsolationBtnClicked()
        {                        
            EdgeDetectedMat = sobelEdgeDetector.sobelEdgeDetect(snapImage);
            sobelEdgeDetector.adapTiveThreshold(EdgeDetectedMat, EdgeDetectedMat);
            stage = 3;
        }
    }
}
