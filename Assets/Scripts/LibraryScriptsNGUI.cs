using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LibraryScriptsNGUI : MonoBehaviour
{
    public GameObject imageItem;
    public GameObject scrollView;
    public Text TextTitle;
    public UIButton satan;

    const int deScale = 0;
    const bool USE_PACK = false;
    const int clone = 1;
    public static TemplateDrawingList templateDrawingList;
    private static string title;
    public Button BtnBack;

    public enum MODE { CATEGORY, TEMPLATE, SPECIAL };
    public static MODE mode;
    private IDisposable cancelCoroutineBackBtnAndroid;

    void Awake()
    {
        GFs.LoadCategoryList();
        GFs.LoadAllTemplateList();

        if (Application.platform == RuntimePlatform.Android)
        {
            cancelCoroutineBackBtnAndroid = Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) == true)
            .Subscribe((long xs) =>
            {
                if (mode == MODE.CATEGORY)
                {
                    GFs.GoHomeSceneScripts();
                }
                else
                {
                    mode = MODE.CATEGORY;
                    GFs.BackToPreviousScene();
                }
            });
        }

        BtnBack.onClick = new Button.ButtonClickedEvent();
        BtnBack.onClick.AddListener(() =>
        {
            if (mode == MODE.CATEGORY)
            {
                GFs.GoHomeSceneScripts();
            }
            else
            {
                mode = MODE.CATEGORY;
                GFs.BackToPreviousScene();
            }
        });
    }

    void Start()
    {
        if (mode == MODE.CATEGORY )
        {
            TextTitle.text = "Thư Viện";
        }
        else if(mode == MODE.TEMPLATE)
        {
            TextTitle.text = title;
        }

        MainThreadDispatcher.StartUpdateMicroCoroutine(Load());
    }

    IEnumerator Load()
    {
        Debug.Log(mode == MODE.SPECIAL);
        UITexture rimageOri = imageItem.transform.Find("icon").GetComponent<UITexture>();
        int widthOri = rimageOri.width;
        int heightOri = rimageOri.height;
        var categorys = GVs.CATEGORY_LIST.data;
        int imageCount = 0;
        if (mode == MODE.CATEGORY)
        {
            imageCount = categorys.Count;
        }
        else if (mode == MODE.TEMPLATE)
        {
            imageCount = templateDrawingList.Count();
        }
        var app_path = GFs.getAppDataDirPath();
        var categoryDirPath = app_path + GVs.CATEGORY_LIST.dir + "/";
        yield return null;

        if (mode == MODE.CATEGORY || mode == MODE.TEMPLATE)
        {
            int nSpecTpl = 0;
            if(mode == MODE.TEMPLATE && title.Trim()=="Giáng sinh")
            {
                nSpecTpl = 1; //1 satan
            }

            for (int j = 0; j < clone; j++)
                for (int i = 0; i < imageCount; i++)
                {
                    try
                    {
                        if (imageItem == null) break;
                        GameObject go = Instantiate(imageItem) as GameObject;
                        var c = go.GetComponent<UITexture>().color;
                        go.GetComponent<UITexture>().color = new Color(c.r, c.g, c.b, 0);
                        go.transform.GetComponent<TweenAlpha>().delay = 0.035f * (i+nSpecTpl) + 0.5f;
                        go.transform.SetParent(imageItem.transform.parent.transform);
                        go.transform.localScale = imageItem.transform.localScale;
                        UITexture rimage = go.transform.Find("icon").GetComponent<UITexture>();
                        UILabel text = go.transform.Find("itemLabel").GetComponent<UILabel>();
                        Texture2D texture = null;
                        Category category = null;
                        TemplateDrawing template = null;
                        if (mode == MODE.CATEGORY)
                        {
                            category = categorys[i];                            
                            texture = GFs.LoadPNGFromPath(categoryDirPath + category.image);
                            text.text = category.name;
                        }
                        else
                        {
                            template = templateDrawingList.Get(i);
                            var dirPath = app_path + templateDrawingList.dir + "/";
                            texture = GFs.LoadPNGFromPath(dirPath + "/" + template.thumb);
                        }
                        
                        rimage.mainTexture = texture;

                        float width = texture.width;
                        float height = texture.height;
                        float ratio = width / height;

                        var w = widthOri;
                        var h = heightOri;

                        if (ratio > 1)
                        {
                            w = widthOri;
                            h = (int)(w * height / width);

                            TextureScale.Bilinear(texture, w >> deScale, h >> deScale);

                            rimage.width = w;
                            rimage.height = h;
                        }
                        else
                        {
                            h = widthOri;
                            w = (int)(h * width / height);

                            TextureScale.Bilinear(texture, w >> deScale, h >> deScale);
                            rimage.width = w;
                            rimage.height = h;
                        }

                        texture.Compress(true);
                        go.GetComponent<UIButton>().onClick.Clear();
                        go.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
                        {
                            if (mode == MODE.CATEGORY)
                            {
                                var categoryID = category._id;
                                TemplateDrawingList templateDrawingList = GVs.TEMPLATE_LIST_ALL_CATEGORY[categoryID];
                                LibraryScriptsNGUI.templateDrawingList = templateDrawingList;
                                LibraryScriptsNGUI.mode = MODE.TEMPLATE;
                                title = category.name;
                                if (category.name.Trim() == "Giáng sinh")
                                {

                                }
                                GVs.SCENE_MANAGER.loadTemplateNGUIScene();
                            }
                            else
                            {
                                var dirPath = GFs.getAppDataDirPath() + "/" + templateDrawingList.dir + "/";
                                var thumbPath = dirPath + template.thumb;
                                var imgPath = dirPath + template.image;
                                DrawingScripts.imgModelPath = imgPath;
                                DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_MODEL;
                                HistoryNGUIScripts.AddHistoryItem(new HistoryModel(imgPath, thumbPath, HistoryModel.IMAGETYPE.MODEL));
                                GVs.SCENE_MANAGER.loadDrawingScene();
                            }
                        }));
                        go.SetActive(true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogErrorFormat("Error is {0}", e.ToString());
                        Debug.LogErrorFormat("Stacktrace is {0}", e.StackTrace.ToString());
                    }
                }
            if (satan != null && title.Trim() == "Giáng sinh")
            {
                satan.GetComponent<TweenAlpha>().delay = 0.035f * (0) + 0.5f;
                var c = satan.GetComponent<UITexture>().color;
                satan.GetComponent<UITexture>().color = new Color(c.r, c.g, c.b, 0);
                satan.gameObject.SetActive(true);
                satan.onClick.Clear();
                satan.onClick.Add(new EventDelegate(() =>
                {
                    TextAsset asset = Resources.Load("satan") as TextAsset;
                    Texture2D tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
                    tex.LoadImage(asset.bytes);
                    Mat mat = new Mat(tex.height, tex.width, CvType.CV_8UC4);
                    Utils.texture2DToMat(tex, mat);
                    DrawingScripts.texModel = tex;
                    DrawingScripts.image = mat;
                    DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_SPECIAL;
                    GVs.SCENE_MANAGER.loadDrawingScene();
                }));
            }

            Destroy(imageItem);            
            scrollView.GetComponent<UIGrid>().Reposition();            
        }        
    }

    private void OnDisable()
    {
        if (cancelCoroutineBackBtnAndroid != null)
            cancelCoroutineBackBtnAndroid.Dispose();
    }
}
