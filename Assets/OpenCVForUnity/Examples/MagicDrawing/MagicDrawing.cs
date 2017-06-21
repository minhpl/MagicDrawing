using UnityEngine;
using System.Collections;
using OpenCVForUnityExample;
using UnityEngine.UI;
using System.IO;
using System;

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
    [RequireComponent(typeof(EdgeDetector))]
    public class MagicDrawing : MonoBehaviour
    {
        public UISlider slider_width_perspective;
        public UISlider slider_height_scale;
        public UISlider slider_heigh_redundancy;
        public UILabel label_width_perspective;
        public UILabel label_height_scale;
        public UILabel label_parameter;

        public UITexture uITexture1;
        public UITexture uITexture2;
        public UITexture uITexture3;
        public UITexture uITexture4;

        Texture2D texture1;
        Texture2D texture2;
        Texture2D texture3;
        Texture2D texture4;

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

        EdgeDetector cannyEdgeDetector;

        int w;
        int h;

        Mat src_mat;
        Mat dst_mat;
        Mat perspectiveTransform;
        Size size;
        Mat warpPerspectiveResult;
        OpenCVForUnity.Rect myROI;
        Mat scaled_height_mat;
        Mat resultImage;
        int newHeight;
        float widthScale = 1;
        float heightScale = 1;
        int needWidthInDe = 0;
        int topWidthAfter = 0;
        int heightRedundancy = 0;
        const int MAX_HEIGHT_REDUNDANCY = 200;
        bool isUseSnappedImage;
        double alpha = 0.5;
        double beta;
        Mat blendedImage;
        Mat imreadImage;
        string androidDir = "/storage/emulated/0/DCIM/MagicDrawing";


        Mat snapImage;
        int stage = 1;      //1: hiển thị camera như bình thường và có nút chụp lại ảnh
                            //2: chụp lại ảnh sau đó hiển thị ảnh tĩnh 
                            //3: hiển thị ảnh đã tách biên trên nền video capture từ camera, với điều kiện camera đã
        Mat EdgeIsolationMat;
        Mat redChannelMat;
        Mat matTexture1;
        Mat matTexture2;
        Mat matTexture3;
        Mat matTexture4;

        void warpPerspectiveTunner()
        {
            beta = 1 - alpha;
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();
            w = rgbaMat.cols();
            h = rgbaMat.rows();
            int topWidth = 400;
            int botWidth = 272;
            topWidth = (int)(topWidth / widthScale);
            int differWidth = topWidth - botWidth;
            int halfWidth = differWidth / 2;
            needWidthInDe = (int)(((float)halfWidth / (float)topWidth) * w);
            topWidthAfter = w - needWidthInDe * 2;

            size = rgbaMat.size();
            src_mat.put(0, 0,
                    0, 0,
                    w, 0,
                    w, h,
                    0, h);
            dst_mat.put(0, 0,
                    needWidthInDe, 0,
                    w - needWidthInDe, 0,
                    w, h,
                    0, h);

            perspectiveTransform = Imgproc.getPerspectiveTransform(src_mat, dst_mat);
        }

        void VideoRecorder()
        {
            // VideoCapture vc = new VideoCapture(0);
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();            
            string filename = "E:\\WorkspaceMinh\\MagicDrawing\\x64\\Release\\out.avi";
            int codec = VideoWriter.fourcc('M', 'J', 'P', 'G');
            double fps = 25.0;
            VideoWriter writer = new VideoWriter(filename, codec, fps, rgbaMat.size());

            // select desired codec (must be available at runtime)
            // framerate of the created video stream
            // name of the output video file
        }

        // Use this for initialization
        void Start()
        {
            //xx = new Mat();
            isRecording = false;
            isUseSnappedImage = false;
            beta = 1 - alpha;
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
            webCamTextureToMatHelper.Init();
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();

            //Debug.LogFormat("xin chao {0}", rgbaMat != null);

            blendedImage = new Mat(rgbaMat.rows(), rgbaMat.cols(), CvType.CV_32FC3);
            imreadImage = new Mat();            

            src_mat = new Mat(4, 2, CvType.CV_32FC1);
            dst_mat = new Mat(4, 2, CvType.CV_32FC1);
            
            warpPerspectiveTunner();
            myROI = new OpenCVForUnity.Rect((int)needWidthInDe, heightRedundancy, (int)(topWidthAfter), h - heightRedundancy);

            newHeight = (int)(h / heightScale);
            scaled_height_mat = new Mat();
            warpPerspectiveResult = new Mat();
            resultImage = new Mat();
            
            EventDelegate.Add(slider_width_perspective.onChange, onSliderWidthPerspectiveChange);
            EventDelegate.Add(slider_height_scale.onChange, onSliderHeightScaleChange);
            EventDelegate.Add(slider_heigh_redundancy.onChange, onSliderHeightRedundancyChange);

            cannyEdgeDetector = gameObject.GetComponent<EdgeDetector>();

            grayMatForCanny = new Mat();
            snapImage = new Mat();            
            redChannelMat = new Mat();
            //matTexture1 = new Mat();
        }

        void onSliderHeightRedundancyChange()
        {
            heightRedundancy = (int)(MAX_HEIGHT_REDUNDANCY * slider_heigh_redundancy.value);
            //Debug.LogFormat("height redundancy is {0}, height is {1}", heightRedundancy,h);
            label_parameter.text = string.Format("WarpPerspective destination: ({0},0),({1},0),(w,h),(0,h) \n" +
                "Height scale is {2} \nHeigt_redundancy = {3}", needWidthInDe, w - needWidthInDe, heightScale, heightRedundancy);
            myROI = new OpenCVForUnity.Rect((int)needWidthInDe, heightRedundancy, (int)(topWidthAfter), h - heightRedundancy);
            // myROI = new OpenCVForUnity.Rect((int)needWidthInDe, heightRedundancy, (int)(topWidthAfter), h);
        }

        void onSliderWidthPerspectiveChange()
        {
            widthScale = slider_width_perspective.value;       
            warpPerspectiveTunner();    
            myROI = new OpenCVForUnity.Rect((int)needWidthInDe, heightRedundancy, (int)(topWidthAfter), h - heightRedundancy);
            label_parameter.text = string.Format("WarpPerspective destination: ({0},0),({1},0),(w,h),(0,h) \n" +
                "Height scale is {2} \nHeigt_redundancy = {3}", needWidthInDe, w - needWidthInDe, heightScale, heightRedundancy);
        }

        void onSliderHeightScaleChange()
        {
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();
            heightScale = slider_height_scale.value;
            newHeight = (int)(rgbaMat.rows() / heightScale);
            label_parameter.text = string.Format("WarpPerspective destination: ({0},0),({1},0),(w,h),(0,h) \n" +
                "Height scale is {2} \nHeigt_redundancy = {3}", needWidthInDe, w - needWidthInDe, heightScale, heightRedundancy);
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

            matTexture1 = new Mat();
            texture1 = new Texture2D(uITexture1.mainTexture.width, uITexture1.mainTexture.height, TextureFormat.RGBA32, false);
            uITexture1.mainTexture = texture1;

            matTexture2 = new Mat();
            texture2 = new Texture2D(uITexture2.mainTexture.width, uITexture2.mainTexture.height, TextureFormat.RGBA32, false);
            uITexture2.mainTexture = texture2;

            matTexture3 = new Mat();
            texture3 = new Texture2D(uITexture3.mainTexture.width, uITexture3.mainTexture.height, TextureFormat.RGBA32, false);
            uITexture3.mainTexture = texture3;

            matTexture4 = new Mat();
            texture4 = new Texture2D(uITexture4.mainTexture.width, uITexture4.mainTexture.height, TextureFormat.RGBA32, false);
            uITexture4.mainTexture = texture4;
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

            blendedImage.Dispose();
            imreadImage.Dispose();
            src_mat.Dispose();
            dstMat.Dispose();
            perspectiveTransform.Dispose();
            warpPerspectiveResult.Dispose();
            scaled_height_mat.Dispose();
            resultImage.Dispose();
            grayPixels = null;
            maskPixels = null;
            //cannyEdgeDetector.Dispose();
        }

        /// <summary>
        /// Raises the web cam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }

        void comicFilter()
        {
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();
            Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
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
            //Utils.matToTexture2D(dstMat, texture, webCamTextureToMatHelper.GetBufferColors());
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

        void magicDrawing()
        {            
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();
            Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
            bgMat.copyTo(dstMat);
            Imgproc.GaussianBlur(grayMat, lineMat, new Size(7, 7), 1.5, 1.5);
            Imgproc.Canny(lineMat, lineMat, 0, 30, 3, true);
            Core.bitwise_not(lineMat, lineMat);
        }

        public GameObject goooo;

        void blendWithComicFilterImage()
        {
            Imgproc.cvtColor(lineMat, imreadImage, 9);
            Core.addWeighted(resultImage, alpha, imreadImage, beta, 0.0, blendedImage);
            Utils.matToTexture2D(blendedImage, texture, webCamTextureToMatHelper.GetBufferColors());
        }

        // Update is called once per frame


        Mat grayMatForCanny;

        void CannyEdgeDetector()
        {
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();
            Mat canny = cannyEdgeDetector.Canny(rgbaMat);
            //Imgproc.cvtColor(rgbaMat, grayMatForCanny, Imgproc.COLOR_RGB2GRAY);
            //Debug.LogFormat("hello {0}",grayMatForCanny.cols());
            //cannyEdgeDetector.Canny(rgbaMat);
            Utils.matToTexture2D(canny, texture, webCamTextureToMatHelper.GetBufferColors());
        }

        void convertOneChannelMattoRed(Mat oneChannelMat)
        {
            
        }
        //xx = new Mat();

        void Update()
        {
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {
                if(stage == 1)
                {
                    Mat rgbaMat = webCamTextureToMatHelper.GetMat();
                    Utils.matToTexture2D(rgbaMat, texture, webCamTextureToMatHelper.GetBufferColors());
                    Debug.LogFormat("Fuck all: {0}", uITexture1 != null);
                    Imgproc.resize(rgbaMat, matTexture1, new Size((double)texture1.width, (double)texture1.height));                    
                    Utils.matToTexture2D(matTexture1, texture1);
                    Utils.matToTexture2D(matTexture1, texture2);
                    Utils.matToTexture2D(matTexture1, texture3);
                    Utils.matToTexture2D(matTexture1, texture4);
                }
                else if(stage==2)
                {
                    Utils.matToTexture2D(snapImage,texture,webCamTextureToMatHelper.GetBufferColors());
                }
                else if(stage==3)
                {
                    EdgeIsolationMat = cannyEdgeDetector.Canny(snapImage);
                    convertOneChannelMattoRed(EdgeIsolationMat);
                    //Mat rgbaMat = webCamTextureToMatHelper.GetMat();
                    //Imgproc.warpPerspective(rgbaMat, warpPerspectiveResult, perspectiveTransform, size);
                    //Imgproc.resize(warpPerspectiveResult, scaled_height_mat, new Size(rgbaMat.width(), newHeight))1;
                    //Utils.matToTexture2D(EdgeIsolationMat, texture, webCamTextureToMatHelper.GetBufferColors());
                    Utils.matToTexture2D(EdgeIsolationMat, texture, webCamTextureToMatHelper.GetBufferColors());
                }
                // CannyEdgeDetector();
                // if(isRecording)
                // {
                //     Debug.LogFormat("Starting recording");
                //     writer.write(rgbaMat);
                // }                
                //Imgproc.warpPerspective(rgbaMat, warpPerspectiveResult, perspectiveTransform, size);
                //Imgproc.resize(warpPerspectiveResult, scaled_height_mat, new Size(rgbaMat.width(), newHeight));
                //if (scaled_height_mat == null)
                //{
                //    Debug.LogFormat("scaled_height_mat is null here");
                //}
                //if (myROI == null)
                //{
                //    Debug.LogFormat("myROI is null here");
                //}
                //Mat croppedImage = scaled_height_mat.submat(myROI);
                //Imgproc.resize(croppedImage, resultImage, new Size(texture.width, texture.height));
                ////Utils.matToTexture2D(resultImage, texture, webCamTextureToMatHelper.GetBufferColors());
                //if (!isUseSnappedImage)
                //{
                //    Utils.matToTexture2D(resultImage, texture, webCamTextureToMatHelper.GetBufferColors());
                //}
                //else
                //{
                //    blendWithComicFilterImage();
                //}
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

        public void onSnapAndComicFilter()
        {
            isUseSnappedImage = true;
            comicFilter();
        }

        /// <summary>
        /// Raises the change camera button event.
        /// </summary>
        public void OnChangeCameraButton()
        {
            webCamTextureToMatHelper.Init(null, webCamTextureToMatHelper.requestWidth, webCamTextureToMatHelper.requestHeight, !webCamTextureToMatHelper.requestIsFrontFacing);
        }

        VideoWriter writer;
        public bool isRecording;
        public Button btnRecord;

        public void RecordVideo()
        {
            //Debug.LogFormat("Xin chao, dang ghi hinh lai video");
            //Directory.CreateDirectory("/storage/emulated/0/DCIM/MagicDrawing/");        
            if(!Directory.Exists(androidDir))
            {
                Directory.CreateDirectory(androidDir);
            }
            string dateTime = DateTime.Now.ToString();
            string videoName = /*dateTime + */"a.mp4";
            string fileName = androidDir + "/" + videoName;

            if (writer == null)
            {
                Mat rgbaMat = webCamTextureToMatHelper.GetMat();
                //string filename = "E:\\WorkspaceMinh\\MagicDrawing\\x64\\Release\\out.avi";
                int codec = VideoWriter.fourcc('M', 'J', 'P', 'G');
                double fps = 25.0;
                writer = new VideoWriter(fileName, codec, fps, rgbaMat.size());
            }
            isRecording = !isRecording;
            if (isRecording)
            {
                btnRecord.GetComponentInChildren<Text>().text = "Stop Recording";
            }
        }

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
            stage = 3;
        }
    }
}