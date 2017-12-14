using OpenCVForUnity;
using OpenCVForUnityExample;
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Behaviors;
using TouchScript.Layers.UI;
using TouchScript.Gestures;
using System.IO;
using TMPro;
using System.Threading;
using System.Collections.Generic;
using Spine.Unity;

public class DrawingScripts : MonoBehaviour
{
    public GameObject goCam;
    public GameObject goModel;
    public Slider sliderLine;
    public Slider sliderContrast;
    public UnityEngine.UI.Text txtTime;
    public Button backBtn;
    public GameObject pnlCfirm;
    public Button agr;
    public Button cancel;
    public GameObject eventSystem;
    public Button tickBtn;
    public Button cancelBtn;
    public UnityEngine.UI.Text txtComfirmText;
    public Canvas canvas;
    public Button Button_Recording;
    public GameObject pause;
    public TextMeshProUGUI timeCounterSnap;
    public GameObject Pnl_Snap;
    public GameObject Pnl_Tool;
    public AudioSource audioSource;
    public Image img_progress_cutvideo;
    public Button getPosSize;
    public Toggle btnTurnRecord;
    public Toggle btnStopRecord;
    public RawImage preview;
    public RawImage imgUserDraw;
    public RawImage background;
    public GameObject reindeer;
    public GameObject pnSpec;  
    public AudioSource chrimasSong;
    public RawImage bigStar;
    public RawImage smallStar;
    public ParticleSystem firework;
    public ParticleSystem snowCircle;
    public ParticleSystem snowFlower;
    public UIPlayTween[] popupPlayTween;
    public SkeletonAnimation skeletonAnimation;
    public ScreenTransformGesture scrTransGes;

    private Threshold threshold;
    private AdaptiveThreshold athreshold;
    WarpPerspective warpPerspective;
    public static Mat image;
    public static string imgModelPath = null;
    private Color32[] colorsBuffer;
    private Texture2D texEdges;
    private Mat edges;
    public static Texture2D texModel;
    private Texture2D texCam;
    private Texture2D texCamDisplay;
    RawImage rimgcam;
    RawImage rimgmodel;
    Utilities utilities;
    WebCamTextureToMatHelper wcHelper;
    bool loaded = false;
    WebcamVideoCapture webcamVideoCapture;
    Mat warp;
    Mat displayMat;
    Color32[] bufferColor;
    private float opaque = 0.25f;
    private OpenCVForUnity.Rect cropRect;
    //private float opaque = 0.4f;
    public enum DRAWMODE { DRAW_MODEL, DRAW_IMAGE, DRAW_SPECIAL };
    public static DRAWMODE drawMode = DRAWMODE.DRAW_SPECIAL;
    public enum FILTERMODE { LINE, BLEND };
    public static FILTERMODE filtermode = FILTERMODE.LINE;
    private System.Diagnostics.Stopwatch CountVidRec;
    private bool isRecording = false;
    private IDisposable cancelCorountineWorker;
    private IDisposable cancelCorountineTurnOffTouchInput;
    private IDisposable cancelCorountineBlinkTime;
    private IDisposable cancelCoroutineBackBtnAndroid;
    private IDisposable cancelCorountineSnapImage;
    private int numberFrameSave = 0;
    private Mat frame;
    private Size size;
    private const int MAX_LENGTH_RESULT_VIDEO = 30;
    private string nameMasterPiece = null;
    private string nameNoExt = "default";
    private void Awake()
    {
        firework.Stop();
        snowCircle.Stop();
        snowFlower.Stop();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        frame = new Mat();
        filtermode = FILTERMODE.LINE;
        if (MakePersistentObject.Instance)
            MakePersistentObject.Instance.gameObject.SetActive(false);
        int delayTime = 200;
        var onSliderLineValueStream = sliderLine.onValueChanged.AsObservable();
        onSliderLineValueStream.Sample(TimeSpan.FromMilliseconds(delayTime)).Subscribe((float f) => { OnLineSliderValueChange(sliderLine); });
        var onSliderContrastValueStream = sliderContrast.onValueChanged.AsObservable();
        onSliderContrastValueStream.Sample(TimeSpan.FromMilliseconds(delayTime)).Subscribe((float f) =>
        {
            OnContrastSliderValueChange(sliderContrast);
        });

        backBtn.onClick = new Button.ButtonClickedEvent();
        backBtn.onClick.AddListener(() =>
        {
            pnlCfirm.SetActive(true);
            for (int i = 0; i < popupPlayTween.Length; i++)
            {
                popupPlayTween[i].Play(true);
            }
        });

        cancel.onClick.AddListener(() =>
        {
            for (int i = 0; i < popupPlayTween.Length; i++)
            {
                popupPlayTween[i].Play(false);
            }
            popupPlayTween[0].onFinished.Add(new EventDelegate(() =>
            {
                pnlCfirm.SetActive(false);
                popupPlayTween[0].onFinished.Clear();
            }));

        });

        if (Application.platform == RuntimePlatform.Android)
        {
            cancelCoroutineBackBtnAndroid = Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) == true).Subscribe(_ =>
            {
                pnlCfirm.SetActive(true);
                for (int i = 0; i < popupPlayTween.Length; i++)
                {
                    popupPlayTween[i].Play(true);
                }
            });
        }

        agr.onClick.AddListener(() =>
        {
            if (cancelGVidP != null)
                cancelGVidP.Dispose();
            if (cancelSaveMP2 != null)
                cancelSaveMP2.Dispose();

            if (webcamVideoCapture.filePath != null)
            {
                if (webcamVideoCapture != null && webcamVideoCapture.writer != null)
                {
                    webcamVideoCapture.writer.release();
                    File.Delete(webcamVideoCapture.filePath);
                }
            }

            for (int i = 0; i < popupPlayTween.Length; i++)
            {
                popupPlayTween[i].Play(false);
            }
            popupPlayTween[0].onFinished.Add(new EventDelegate(() =>
            {
                pnlCfirm.SetActive(false);
                popupPlayTween[0].onFinished.Clear();
                GFs.BackToPreviousScene();
            }));
        });

        tickBtn.onClick.AddListener(() =>
        {

            if (!cancelBtn.gameObject.activeSelf)
            {
                CountVidRec.Stop();
                wcHelper.Pause();
            }
            else
            {
                IDisposable cancelCorountineSnapImage = Observable.FromCoroutine(CountDown).Subscribe();
                backBtn.gameObject.SetActive(false);
            }
        });
        cancelBtn.onClick.AddListener(() =>
        {
            wcHelper.Play();
            CountVidRec.Start();
        });

        Button_Recording.onClick.AddListener(() =>
        {
            if (!pause.activeSelf)
            {
                isRecording = true;
                CountVidRec.Start();
                if (cancelCorountineBlinkTime != null)
                    cancelCorountineBlinkTime.Dispose();
            }
            else
            {
                isRecording = false;
                CountVidRec.Stop();
                cancelCorountineBlinkTime = Observable.FromCoroutine(blinkTime).Subscribe();
            }
        });
    }

    void Start()
    {
        var size = skeletonAnimation.GetComponent<MeshRenderer>().bounds.size;
        rimgcam = goCam.GetComponent<RawImage>();
        rimgmodel = goModel.GetComponent<RawImage>();
        rimgmodel.color = new Color(255, 255, 255, opaque);
        wcHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
        warpPerspective = gameObject.GetComponent<WarpPerspective>();
        utilities = new Utilities();
        threshold = GetComponent<Threshold>();
        GFs.LoadCategoryList();
        GFs.LoadAllTemplateList();
        MainThreadDispatcher.StartUpdateMicroCoroutine(loadCameraAndModel());
        Everyplay.Initialize();
    }

    int texModelW;
    int texModelH;

    IDisposable cancelGVidP;
    IDisposable cancelSaveMP2;

    private void OnDisable()
    {
        if (Everyplay.IsRecording() || Everyplay.IsPaused())
        {
            Utilities.Log("Here in Ondestroy: is recording? {0}, is paused ? {1}", Everyplay.IsRecording(), Everyplay.IsPaused());
            Everyplay.StopRecording();
        }
        Everyplay.StopRecording();
        Utilities.Log("Here");
        if (cancelCorountineWorker != null)
            cancelCorountineWorker.Dispose();
        if (cancelGVidP != null)
            cancelGVidP.Dispose();
        if (cancelSaveMP2 != null)
            cancelSaveMP2.Dispose();
        wcHelper.Stop();
        wcHelper.Dispose();
        if (cancelCorountineTurnOffTouchInput != null)
            cancelCorountineTurnOffTouchInput.Dispose();
        if (cancelCorountineBlinkTime != null)
            cancelCorountineBlinkTime.Dispose();
        if (cancelCoroutineBackBtnAndroid != null)
            cancelCoroutineBackBtnAndroid.Dispose();
        if (cancelCorountineSnapImage != null)
            cancelCorountineSnapImage.Dispose();
        image.release();
        image.Dispose();
        edges.Dispose();
        image = null;
        if (!preserveTexture)
        {
            Destroy(texCam);
        }
        Destroy(texEdges);
        Destroy(texModel);
        Destroy(wcHelper);
        if (webcamVideoCapture != null)
        {
            webcamVideoCapture.filePath = null;
            if (webcamVideoCapture.writer != null && !webcamVideoCapture.writer.IsDisposed)
            {
                webcamVideoCapture.writer.release();
            }
        }
    }
    IEnumerator loadCameraAndModel()
    {
        yield return null;
        if (!wcHelper.IsInitialized())
        {
            wcHelper.onInitialized.AddListener(() =>
            {
                var rgbaMat = wcHelper.GetMat();
                var captureWidth = rgbaMat.width();
                var captureHeight = rgbaMat.height();
                var captureRatio = captureWidth / (float)captureHeight;
                warpPerspective.Init(wcHelper.GetMat());
                Mat camMat = wcHelper.GetMat();

                var rawImageCamera = goCam.GetComponent<RawImage>();
                int rawImageWidth = (int)rawImageCamera.rectTransform.rect.width;
                int rawImageHeight = (int)rawImageCamera.rectTransform.rect.height;
                var matWidth = rgbaMat.width();
                var matHeight = rgbaMat.height();

                var rawImageRatio = rawImageWidth / (float)rawImageHeight;
                int cropWidth = 0, cropHeight = 0;
                if (rawImageRatio > captureRatio)
                {
                    cropWidth = captureWidth;
                    cropHeight = (int)(cropWidth / rawImageRatio);
                }
                else
                {
                    cropHeight = captureHeight;
                    cropWidth = (int)(cropHeight * rawImageRatio);
                }

                int offsetX = matWidth * ((captureWidth - cropWidth) >> 1) / captureWidth;
                int offsetY = matHeight * ((captureHeight - cropHeight) >> 1) / captureHeight;
                int subWidth = matWidth - (offsetX << 1);
                int subHeight = matHeight - (offsetY << 1);
                cropRect = new OpenCVForUnity.Rect(offsetX, offsetY, subWidth, subHeight);

                texCamDisplay = new Texture2D(subWidth, subHeight, TextureFormat.RGBA32, false);
                texCam = new Texture2D(matWidth, matHeight, TextureFormat.RGBA32, false);
                bufferColor = new Color32[subWidth * subHeight];
                size = new Size(subWidth, subHeight);
                webcamVideoCapture = new WebcamVideoCapture(size, true);
                cancelCorountineWorker = Observable.FromMicroCoroutine(Worker).Subscribe();
                CountVidRec = new System.Diagnostics.Stopwatch();
            });
            wcHelper.Initialize(null, 640, 640, true, 60);
        }

        if (drawMode == DRAWMODE.DRAW_SPECIAL)
        {
            scrTransGes.Type = TransformGesture.TransformType.Translation | TransformGesture.TransformType.Scaling;
        }
        else
        {
            scrTransGes.Type = TransformGesture.TransformType.Rotation | TransformGesture.TransformType.Scaling | TransformGesture.TransformType.Translation;
        }

        if (drawMode == DRAWMODE.DRAW_MODEL || GVs.isTest)
        {
            string imgPath;
            if (imgModelPath != null)
            {
                imgPath = imgModelPath;
            }
            else
            {
                var categoryID = GVs.CATEGORY_LIST.data[5]._id;                
                imgPath = GFs.getAppDataDirPath() + GVs.TEMPLATE_LIST_ALL_CATEGORY[categoryID].dir + "/" + "C07T001.png";
                Debug.Log(imgPath);
            }

            texModel = GFs.LoadPNGFromPath(imgPath);
            if(GVs.isTest)
            {
                TextAsset asset = Resources.Load("satan") as TextAsset;                
                texModel.LoadImage(asset.bytes);
            }

            int w = texModel.width;
            int h = texModel.height;
            texModelW = w;
            texModelH = h;
            var rat = w / (float)h;
            var restrictMaxSize = 640;
            if (rat > 1)
            {
                w = restrictMaxSize;
                h = (int)(w / rat);
            }
            else
            {
                h = restrictMaxSize;
                w = (int)(h * rat);
            }

            TextureScale.Bilinear(texModel, (int)w, (int)h);
            image = new Mat(h, w, CvType.CV_8UC4);
            Utils.texture2DToMat(texModel, image);
            Imgproc.cvtColor(image, image, Imgproc.COLOR_BGRA2RGBA);
            texModel.Compress(true);
        }
        else
        {            
            texModelW = texModel.width;
            texModelH = texModel.height;
        }

        rimgmodel.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        rimgmodel.GetComponent<AspectRatioFitter>().aspectRatio = image.width() / (float)image.height();
        rimgmodel.GetComponent<AspectRatioFitter>().enabled = true;

        athreshold = GetComponent<AdaptiveThreshold>();
        athreshold.setParameter(sliderLine.value);
        texEdges = new Texture2D(image.width(), image.height(), TextureFormat.ARGB32, false);
        edges = athreshold.adapTiveThreshold(image);
        Mat redMat = utilities.makeMonoAlphaMat(edges, Utilities.MonoColor.RED);
        colorsBuffer = new Color32[edges.width() * edges.height()];
        Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
        rimgmodel.texture = texEdges;
        utilities = new Utilities();
        goModel.SetActive(true);
        loaded = true;

        cancelCorountineTurnOffTouchInput = Observable.FromMicroCoroutine(turnOffTouchInput).Subscribe();
    }

    IEnumerator turnOffTouchInput()
    {
        while (eventSystem && eventSystem.GetComponent<TouchScriptInputModule>() == null)
        {
            yield return null;
        }
        eventSystem.GetComponent<TouchScriptInputModule>().enabled = false;
    }

    public void OnContrastSliderValueChange(Slider slider)
    {
        float percent = slider.value / 100f;
        var c = rimgmodel.color;
        rimgmodel.color = new Color(c.r, c.g, c.b, percent);
    }

    public void OnLineSliderValueChange(Slider slider)
    {
        if (loaded)
        {
            if (slider)
            {
                athreshold.setParameter(slider.value);
                edges = athreshold.adapTiveThreshold(image);
                Mat redMat = utilities.makeMonoAlphaMat(edges, Utilities.MonoColor.RED);
                Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
                rimgmodel.texture = texEdges;
            }
        }
    }
    int numberFrame = 0;

    IEnumerator Worker()
    {
        while (true)
        {
            yield return null;
            if (wcHelper.IsPlaying() && wcHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = wcHelper.GetMat();
                warp = warpPerspective.warpPerspective(rgbaMat);
                displayMat = warp.submat(cropRect);
                Utils.matToTexture2D(displayMat, texCamDisplay, bufferColor);

                if (isRecording)
                {
                    numberFrame++;
                    if (numberFrame % GVs.skipFrame == 0)
                    {
                        numberFrameSave++;
                        webcamVideoCapture.write(displayMat);
                    }
                    var timeLapse = (int)CountVidRec.Elapsed.TotalSeconds;
                    string minSec = string.Format("{0}:{1:00}", (int)(timeLapse / 60f), (int)timeLapse % 60);
                    txtTime.text = minSec;
                }
                rimgcam.texture = texCamDisplay;
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (isRecording)
        {
            if (focus == false)
                CountVidRec.Stop();
            else
                CountVidRec.Start();
        }
    }

    public void OnContrastBtnClicked()
    {
        if (filtermode == FILTERMODE.LINE)
            filtermode = FILTERMODE.BLEND;
        else if (filtermode == FILTERMODE.BLEND)
            filtermode = FILTERMODE.LINE;
        if (filtermode == FILTERMODE.LINE)
        {
            rimgmodel.color = new Color(255, 255, 255, opaque);
            rimgmodel.texture = texEdges;
        }
        else if (filtermode == FILTERMODE.BLEND)
        {
            OnContrastSliderValueChange(sliderContrast);
            rimgmodel.texture = texModel;
        }
    }

    public void OnSliderBtnClicked()
    {
        filtermode = FILTERMODE.LINE;
        rimgmodel.color = new Color(255, 255, 255, opaque);
        Mat redMat = utilities.makeMonoAlphaMat(edges, Utilities.MonoColor.RED);
        Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
        rimgmodel.texture = texEdges;
        rimgmodel.GetComponent<ScreenTransformGesture>().enabled = false;
        rimgmodel.GetComponent<Transformer>().enabled = false;
    }
    bool preserveTexture = false;
    IEnumerator CountDown()
    {
        if (webcamVideoCapture.writer != null && !webcamVideoCapture.writer.IsDisposed)
        {
            webcamVideoCapture.writer.release();
        }
        wcHelper.Play();
        Pnl_Snap.SetActive(true);
        Pnl_Tool.SetActive(false);
        // backBtn.gameObject.SetActive(false);
        goModel.SetActive(false);
        float periods = 1f;
        yield return new WaitForSeconds(periods);
        timeCounterSnap.text = "2";
        yield return new WaitForSeconds(periods);
        timeCounterSnap.text = "1";
        yield return new WaitForSeconds(periods);
        timeCounterSnap.text = null;
        wcHelper.Pause();
        wcHelper.Stop();
        audioSource.Play();
        Pnl_Snap.SetActive(false);


        if (WebcamVideoCapture.filenameWithoutExt != null)
        {
            nameNoExt = WebcamVideoCapture.filenameWithoutExt;
        }
        else
        {
            nameNoExt = DateTime.Now.ToString(Utilities.customFmts);
        }
        nameMasterPiece = nameNoExt + ".png";

        if (drawMode == DRAWMODE.DRAW_SPECIAL)
        {
            isolateBoundary();
        }
        else
        {
            yield return SaveMasterPiece2();
        }
    }

    OpenCVForUnity.Rect posPMat;

    void isolateBoundary()
    {
        var offsetMin = rimgmodel.rectTransform.offsetMin;
        var offsetMax = rimgmodel.rectTransform.offsetMax;
        var left = offsetMin.x;
        var bottom = -offsetMin.y + rimgcam.rectTransform.rect.size.y;
        var top = -offsetMax.y;
        var right = offsetMax.x + rimgcam.rectTransform.rect.size.x;
        var width = right - left;
        var height = bottom - top;
        var center_x = left + width / 2;
        var center_y = top + height / 2;
        var scale = rimgmodel.transform.localScale.x;
        var real_width = width * scale;
        var real_height = height * scale;
        var real_left = center_x - (real_width / 2);
        var real_top = center_y - (real_height / 2);

        var scal = cropRect.width / rimgcam.rectTransform.rect.size.x;
        int x = (int)(real_left * scal);
        int y = (int)(real_top * scal);
        int w = (int)(real_width * scal);
        int h = (int)(real_height * scal);

        int x_end = x + w > cropRect.width ? cropRect.width : x + w;
        int x_begin = x < 0 ? 0 : x;
        int y_end = y + h > cropRect.height ? cropRect.height : y + h;
        int y_begin = y < 0 ? 0 : y;

        Mat mask = new Mat();
        Imgproc.resize(image, mask, new Size(real_width * scal, real_height * scal));

        List<Mat> channels = new List<Mat>();
        Core.split(mask, channels);
        channels[0] = channels[3];
        channels[1] = channels[3];
        channels[2] = channels[3];
        channels[3] = channels[3];
        Core.merge(channels, mask);
        Mat cropBoundaryMat2 = new Mat();
        Mat mask2 = mask.colRange(x_begin - x, x_end - x).rowRange(y_begin - y, y_end - y);

        Mat kernel = Imgproc.getStructuringElement(Imgproc.MORPH_CROSS, new Size(8, 8));
        Imgproc.morphologyEx(mask2, mask2, Imgproc.MORPH_DILATE, kernel);

        Mat cropBoundary = displayMat.colRange(x_begin, x_end).rowRange(y_begin, y_end);
        cropBoundary.copyTo(cropBoundaryMat2, mask2);

        var kernel2 = Imgproc.getStructuringElement(Imgproc.MORPH_CROSS, new Size(70, 70));
        Mat bg = new Mat();
        Imgproc.morphologyEx(cropBoundary, bg, Imgproc.MORPH_CLOSE, kernel2);
        var kernel3 = Imgproc.getStructuringElement(Imgproc.MORPH_CROSS, new Size(50, 50));
        Imgproc.morphologyEx(bg, bg, Imgproc.MORPH_CLOSE, kernel3);

        Mat backgroundMat3 = displayMat.clone();
        bg.copyTo(backgroundMat3.colRange(x_begin, x_end).rowRange(y_begin, y_end), mask2);
        Imgproc.GaussianBlur(backgroundMat3, backgroundMat3, new Size(10, 10), 0);
        Texture2D bgTture = new Texture2D(backgroundMat3.width(), backgroundMat3.height(), TextureFormat.BGRA32, false);
        Utils.matToTexture2D(backgroundMat3, bgTture);
        Imgproc.resize(cropBoundaryMat2, cropBoundaryMat2, new Size(texModelW, texModelH));

        var txtASpine = (Texture2D)skeletonAnimation.gameObject.GetComponent<MeshRenderer>().material.mainTexture;
        Mat txtASpineM = new Mat(txtASpine.height, txtASpine.width, CvType.CV_8UC4);
        Utils.texture2DToMat(txtASpine, txtASpineM);
        cropBoundaryMat2.copyTo(txtASpineM.submat(0, cropBoundaryMat2.height(), 0, cropBoundaryMat2.width()));
        Texture2D texture = new Texture2D(txtASpine.width, txtASpine.height, TextureFormat.RGBA32, false);
        Utils.matToTexture2D(txtASpineM, texture);
        skeletonAnimation.gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
        //-----------------------------------------------------------------------------------------------------------------------                
        rimgcam.texture = bgTture;
        //Animation
        imgUserDraw.gameObject.transform.position = goModel.transform.position;
        imgUserDraw.rectTransform.sizeDelta = rimgmodel.rectTransform.rect.size;
        Texture2D userDrawTexture = new Texture2D(cropBoundaryMat2.width(), cropBoundaryMat2.height(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(cropBoundaryMat2, userDrawTexture);
        imgUserDraw.texture = userDrawTexture;
        imgUserDraw.gameObject.SetActive(true);
        //---------------------------------
        skeletonAnimation.gameObject.SetActive(true);
        skeletonAnimation.gameObject.SetActive(false);
        var sizeAnimation = skeletonAnimation.gameObject.GetComponent<MeshRenderer>().bounds.size;
        var sizeAnimation2 = new Vector2(sizeAnimation.x, sizeAnimation.y);
        Debug.Log(sizeAnimation);

        var widthUserDraw = imgUserDraw.rectTransform.sizeDelta.x;
        var scal_ = sizeAnimation.x / widthUserDraw;
        var posAnimation = skeletonAnimation.gameObject.transform.position;
        var newPos = new Vector3(posAnimation.x, posAnimation.y + sizeAnimation.y / 2f);

        var seq = LeanTween.sequence();
        seq.append(() =>
        {
            if (Everyplay.IsRecording() || Everyplay.IsPaused())
            {
                Everyplay.StopRecording();
            }
            Everyplay.StartRecording();
            chrimasSong.Play();
            chrimasSong.loop = false;
            LeanTween.move(imgUserDraw.gameObject, newPos, 2f);
            LeanTween.scale(imgUserDraw.gameObject, new Vector3(scal_, scal_, scal_), 2f);
        });
        seq.append(2);
        seq.append(() =>
        {
            Utilities.Log("is recording0 ? {0}", Everyplay.IsRecording());
            LeanTween.alpha(rimgcam.rectTransform, 0, 3f);
            LeanTween.alpha(background.rectTransform, 1, 3f);
        });
        seq.append(3f);
        seq.append(() =>
        {
            firework.gameObject.SetActive(true);
            snowCircle.gameObject.SetActive(true);
            snowFlower.gameObject.SetActive(true);

            firework.GetComponent<ParticleSystem>().Play(true);
            LeanTween.alpha(bigStar.rectTransform, 1, 0.4f).setLoopPingPong();
            LeanTween.alpha(smallStar.rectTransform, 1, 0.4f).setLoopPingPong().setDelay(0.2f);
            LeanTween.rotateZ(reindeer, -reindeer.transform.localRotation.eulerAngles.z, 10).setLoopClamp();
            skeletonAnimation.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "spine";
            skeletonAnimation.gameObject.SetActive(true);
            skeletonAnimation.AnimationName = "animation";
            imgUserDraw.gameObject.SetActive(false);


            Observable.Timeout<float>(Observable.Never<float>(), TimeSpan.FromSeconds(GVs.timeAni)).Subscribe((float f) =>
            { }, (Exception ex) =>
            {
                Everyplay.StopRecording();
                cancelGVidP = Observable.FromCoroutine(GetVideoPath).Subscribe((_) => { }, () =>
                {
                    ScreenshotHelper.iCaptureScreen((Texture2D tex_) =>
                    {
                        Utilities.Log("snap image width is {0}, height is {1}", tex_.width, tex_.height);
                        var animPath = GFs.getMasterpieceDirPath() + nameNoExt + "_anim.png";
                        File.WriteAllBytes(animPath, tex_.EncodeToPNG());
                        Destroy(tex_);

                        cancelSaveMP2 = Observable.FromMicroCoroutine(SaveMasterPiece2).Subscribe();
                    });
                });
            });
        });
    }

    IEnumerator GetVideoPath()
    {
        yield return null;
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            yield break;
        string everyplayDir = null;
#if UNITY_IOS
        var root = new DirectoryInfo(Application.persistentDataPath).Parent.FullName;
        everyplayDir = root + "/tmp/Everyplay/session";
#elif UNITY_ANDROID
        var root = new DirectoryInfo(Application.temporaryCachePath).FullName;
        everyplayDir = root + "/sessions";
#endif
        var files = new DirectoryInfo(everyplayDir).GetFiles("*.mp4", SearchOption.AllDirectories);
        var videoLocation = "";
        // Should only be one video, if there is one at all
        foreach (var file in files)
        {
#if UNITY_ANDROID
            //videoLocation = "file://" + file.FullName;
            videoLocation = file.FullName;
#else
            videoLocation = file.FullName;
#endif
            Utilities.LogFormat("Videos Location is {0}", videoLocation);
            //break;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            WWW www = new WWW("file://" + videoLocation);
            yield return www;
        }
        var mpdir = GFs.getMasterpieceDirPath();
        var file_name = mpdir + nameNoExt + ".mp4";
        //EveryplayLocalSave.SaveTo(file_name);        
        yield return EveryplayLocalSave.SaveToAsync(file_name);
        Utilities.Log("The path is {0}, is file exist ? {1}", file_name, File.Exists(file_name));
        Utilities.Log("The original is {0}, is file exist ? {1}", videoLocation, File.Exists(videoLocation));
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    Handheld.PlayFullScreenMovie(file_name);
        //}
        //else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    Handheld.PlayFullScreenMovie("file://" + file_name);
        //}
    }

    IEnumerator SaveMasterPiece2()
    {
        pnSpec.SetActive(false);
        firework.gameObject.SetActive(false);
        snowCircle.gameObject.SetActive(false);
        snowFlower.gameObject.SetActive(false);
        skeletonAnimation.gameObject.SetActive(false);

        Debug.Log("Here, xin chao tat cac moi nguoi tren tg nay");
        yield return null;
        var _numberFrameSave = numberFrameSave;
        goCam.GetComponent<RawImage>().texture = null;

        Mat resultMat = warp.submat(cropRect);
        Texture2D resultTexture = new Texture2D(cropRect.width, cropRect.height, TextureFormat.BGRA32, false);
        GFs.load_APP_PATH_VAR();
        var logoPath = GFs.getlogoPath();
        Texture2D txtWMark = GFs.LoadPNGFromPath(logoPath);
        Mat logo = new Mat(txtWMark.height, txtWMark.width, CvType.CV_8UC4);
        var width = resultMat.width();
        int newWidthlogo = (int)(width / 5f);
        int newHeightlogo = (int)(logo.height() * (newWidthlogo / (float)logo.width()));
        Utils.texture2DToMat(txtWMark, logo);        
        Imgproc.resize(logo, logo, new Size(newWidthlogo, newHeightlogo));
        posPMat = new OpenCVForUnity.Rect(10, resultMat.height() - logo.height() - 10, logo.width(), logo.height());
        var rect = resultMat.submat(posPMat);
        Mat maskCopyMask = new Mat(logo.height(), logo.width(), CvType.CV_8UC1);
        Core.extractChannel(logo, maskCopyMask, 3);
        maskCopyMask = maskCopyMask - new Scalar(230);
        logo.copyTo(rect, maskCopyMask);
        Imgproc.cvtColor(logo, logo, Imgproc.COLOR_RGBA2BGR); //3 channel for frames of video
        Utils.matToTexture2D(resultMat, resultTexture);       
        var masterPieceDirPath = GFs.getMasterpieceDirPath();
        var imagePath = masterPieceDirPath + nameMasterPiece;
        File.WriteAllBytes(imagePath, resultTexture.EncodeToPNG());

        DecorateScene.texture = texCamDisplay;
        DecorateScene.imagePath = imagePath;
        ResultScripts.texture = resultTexture;
        ResultScripts.mode = ResultScripts.MODE.FISRT_RESULT;
        ResultScripts.imagePath = imagePath;
        if (webcamVideoCapture != null)
            ResultScripts.videoPath = webcamVideoCapture.filePath;

        var lengthInSeconds = _numberFrameSave / (float)WebcamVideoCapture.FPS;
        if (lengthInSeconds < 3)
        {
            webcamVideoCapture.writer.release();
            webcamVideoCapture.writer.Dispose();
            File.Delete(webcamVideoCapture.filePath);
            ResultScripts.videoPath = null;
        }
        var maxNumberFrame = MAX_LENGTH_RESULT_VIDEO * WebcamVideoCapture.FPS;
        var redundanceFrame = _numberFrameSave - maxNumberFrame;

        img_progress_cutvideo.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        LeanTween.rotateAround(img_progress_cutvideo.gameObject, Vector3.forward, 360, 1)
            .setOnStart(() => { img_progress_cutvideo.gameObject.SetActive(true); })
            .setRepeat(-1).setEaseLinear();

        var cutvideo = Observable.Start(() =>
        {
            if (lengthInSeconds >= 3)
            {
                var filePath1 = masterPieceDirPath + WebcamVideoCapture.filenameWithoutExt + ".avi";
                var filePath2 = masterPieceDirPath + WebcamVideoCapture.filenameWithoutExt + "_2.avi";
                System.IO.File.Move(filePath1, filePath2);
                var writer = new VideoWriter(filePath1, VideoWriter.fourcc('M', 'J', 'P', 'G'), WebcamVideoCapture.FPS, size);
                VideoCapture cap = new VideoCapture();
                cap.open(filePath2);
                if (redundanceFrame > 0)
                {
                    Debug.LogFormat("number frame of first video is {0}", cap.get(7));
                    var count = 0;
                    var count2 = 0;

                    if (maxNumberFrame > redundanceFrame)
                    {
                        float ratio = _numberFrameSave / (float)redundanceFrame;
                        int ratioFloor = (int)Math.Floor(ratio);
                        int j = 1;
                        float du = 0;
                        for (; ; j++)
                        {
                            cap.read(frame);
                            if (frame.empty())
                            {
                                break;
                            }

                            count++;
                            if (count != ratioFloor)
                            {
                                count2++;
                                rect = frame.submat(posPMat);
                                logo.copyTo(rect, maskCopyMask);                                
                                
                                writer.write(frame);
                            }
                            else
                            {
                                ratioFloor = (int)Math.Floor(ratio + du);
                                du = ratio + du - ratioFloor;                                
                                count = 0;
                            }

                            if (count2 >= maxNumberFrame)
                                break;
                        }                        
                    }
                    else
                    {
                        float ratio = _numberFrameSave / (float)maxNumberFrame;
                        int ratioFloor = (int)Math.Floor(ratio);
                        count = 0;
                        float du = 0;
                        int j = 1;
                        for (; ; j++)
                        {
                            cap.read(frame);
                            if (frame.empty())
                            {
                                break;
                            }

                            count++;
                            if (count == ratioFloor)
                            {
                                count2++;
                                rect = frame.submat(posPMat);
                                logo.copyTo(rect, maskCopyMask);
                                writer.write(frame);
                                ratioFloor = (int)Math.Floor(ratio + du);
                                du = ratio + du - ratioFloor;
                                count = 0;                                
                            }

                            if (count2 >= maxNumberFrame)
                                break;
                        }                        
                    }                    
                }
                else
                {
                    for (; ; )
                    {
                        cap.read(frame);                        
                        if (frame.empty())
                        {
                            break;
                        }
                        rect = frame.submat(posPMat);
                        logo.copyTo(rect, maskCopyMask);                        
                        writer.write(frame);
                    }

                }
                logo.release();
                logo.Dispose();
                writer.release();
                writer.Dispose();
                cap.release();
                File.Delete(filePath2);
            }
            Thread.Sleep(500);
        });
        Observable.WhenAll(cutvideo)
            .ObserveOnMainThread().Subscribe(_ =>
            {
                img_progress_cutvideo.gameObject.SetActive(false);
                GVs.SCENE_MANAGER.loadDecorateScene();
            });
    }
    int iii = 0;
    public void OnPushBtnClicked()
    {
        if (filtermode == FILTERMODE.LINE)
        {
            Mat blueMat = utilities.makeMonoAlphaMat(edges, Utilities.MonoColor.BLUE);
            Utils.matToTexture2D(blueMat, texEdges, colorsBuffer);
            rimgmodel.texture = texEdges;
        }
        rimgmodel.GetComponent<AspectRatioFitter>().enabled = false;
        rimgmodel.GetComponent<ScreenTransformGesture>().enabled = true;
        rimgmodel.GetComponent<Transformer>().enabled = true;
    }
    public void OnPushActiveBtnClicked()
    {
        if (filtermode == FILTERMODE.LINE)
        {
            Mat redMat = utilities.makeMonoAlphaMat(edges, Utilities.MonoColor.RED);
            Utils.matToTexture2D(redMat, texEdges, colorsBuffer);
            rimgmodel.texture = texEdges;
        }
        rimgmodel.GetComponent<ScreenTransformGesture>().enabled = false;
        rimgmodel.GetComponent<Transformer>().enabled = false;
    }

    IEnumerator blinkTime()
    {
        yield return null;

        var textTimeGameObj = txtTime.gameObject;

        while (!isRecording)
        {
            yield return new WaitForSeconds(1);
            textTimeGameObj.SetActive(!textTimeGameObj.activeSelf);
        }
    }
}
