﻿using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LibraryScriptsNGUI : MonoBehaviour
{
    public GameObject imageItem;
    public GameObject scrollView;
    public UILabel TextTitle;    
    const int deScale = 1;
    const bool USE_PACK = false;
    const int clone = 1;
    public static LibraryScriptsNGUI Instance;
    public static TemplateDrawingList templateDrawingList;
    private static string title;

    public enum MODE {CATEGORY, TEMPLATE};
    private static MODE mode;

    Coroutine coroutine;
    void Awake()
    {
        Instance = this;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }
    // Use this for initialization
    void Start()
    {               
        GFs.LoadCategoryList();
        GFs.LoadAllTemplateList();

        if (mode == MODE.CATEGORY)
        {
            TextTitle.text = "Thư Viện";
        }
        else
        {
            TextTitle.text = title;
        }
        var watch = System.Diagnostics.Stopwatch.StartNew();
        //coroutine = MainThreadDispatcher.StartCoroutine(Load());
        MainThreadDispatcher.StartUpdateMicroCoroutine(Load());
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;      
    }

    IEnumerator Load()
    {                      
        List<Texture2D> LstTexture = new List<Texture2D>();
        List<GameObject> LstGameObject = new List<GameObject>();
        List<int> numRectIn2048 = new List<int>();
        int numRectInOther = 0;
        int deviant2048 = (1 << 22) / 10;
        int deviant1024 = (1 << 20) / 8;
        int deviant512 = (1 << 18) / 5;
        UITexture rimageOri = imageItem.transform.Find("icon").GetComponent<UITexture>();


        //int widthOri = (int)rimageOri.rectTransform.rect.width;
        int widthOri = rimageOri.width;
        int heightOri = rimageOri.height;
        Debug.LogFormat("Width Ori gin is {0}, height is {1}", widthOri,heightOri);

        var watch = System.Diagnostics.Stopwatch.StartNew();
        var area2048 = (1 << 22) - deviant2048;
        var area1024 = (1 << 20) - deviant1024;
        var area512 = (1 << 18) - deviant512;
        int Area = 0;
        var num2048 = 0;
        var num1024 = 0;
        var num512 = 0;                
        int tempArea = area2048;
        int tempNumPacked = 0;
        var categorys = GVs.CATEGORY_LIST.data;
        int imageCount = 0;        
        if (mode == MODE.CATEGORY)
        {            
            imageCount = categorys.Count;
        }
        else
        {
            imageCount = templateDrawingList.Count();
        }

        var app_path = GFs.getAppDataDirPath();        
        var categoryDirPath = app_path + GVs.CATEGORY_LIST.dir + "/";

        for (int j = 0; j < clone; j++)
            for (int i = 0; i < imageCount; i++)
            {               
                yield return null;
                if (imageItem == null) break;
                GameObject go = Instantiate(imageItem) as GameObject;
                go.transform.SetParent(imageItem.transform.parent.transform);
                go.transform.localScale = imageItem.transform.localScale;
                UITexture rimage = go.transform.Find("icon").GetComponent<UITexture>();
                //TextMeshProUGUI textMeshPro = go.transform.Find("textmeshpro").GetComponent<TextMeshProUGUI>();
                UILabel text = go.transform.Find("itemLabel").GetComponent<UILabel>();

            
                Texture2D texture = null;
                Category category = null;
                TemplateDrawing template = null;
                if (mode==MODE.CATEGORY)
                {
                    category = categorys[i];                                        
                    texture = GFs.LoadPNGFromPath(categoryDirPath + category.image);
                    text.text = category.name;
                }
                else
                {                    
                    template = templateDrawingList.Get(i);
                    var dirPath = app_path+ templateDrawingList.dir + "/";
                    texture= GFs.LoadPNGFromPath(dirPath + "/" + template.thumb);
                }

                rimage.mainTexture = texture;
                go.SetActive(true);
                scrollView.GetComponent<UIGrid>().Reposition();

                float width = texture.width;
                float height = texture.height;
                float ratio = width / height;

                Debug.LogFormat("Index {0} : texture width is {1}, height is {2}",i, width, height);

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

                var area = w * h;
                Area += area;
                tempArea = tempArea - area;
                if (tempArea < 0)
                {
                    num2048 += 1;
                    tempArea = area2048 - area;
                    numRectIn2048.Add(j * imageCount + i + 1 - tempNumPacked);
                    tempNumPacked = j * imageCount + i + 1;
                }

                texture.Compress(true);
                go.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
                {
                    if (mode == MODE.CATEGORY)
                    {

                        var categoryID = category._id;
                        TemplateDrawingList templateDrawingList = GVs.TEMPLATE_LIST_ALL_CATEGORY[categoryID];
                        LibraryScriptsNGUI.templateDrawingList = templateDrawingList;
                        LibraryScriptsNGUI.mode = MODE.TEMPLATE;
                        title = category.name;
                        GVs.SCENE_MANAGER.loadTemplateScene();
                    }
                    else
                    {
                        var dirPath = GFs.getAppDataDirPath() + "/" + templateDrawingList.dir + "/";
                        var thumbPath = dirPath + template.thumb;
                        var imgPath = dirPath + template.image;
                        DrawingScripts.imgModelPath = imgPath;
                        DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_MODEL;
                        HistorySceneScripts.AddHistoryItem(new HistoryModel(imgPath, thumbPath, HistoryModel.IMAGETYPE.MODEL));
                        GVs.SCENE_MANAGER.loadDrawingScene();
                    }
                }));
                                                    
                
                LstGameObject.Add(go);
                LstTexture.Add(texture);                
            }

        if (USE_PACK && imageItem != null)
        {
            int freeArea = 0;
            freeArea = Area - area2048 * num2048;
            if (freeArea > area1024)
            {                
                num2048 += 1;
                numRectIn2048.Add(imageCount * clone - tempNumPacked);
            }
            else
            {
                numRectInOther = clone * imageCount - tempNumPacked;
                if (freeArea > area512) num1024 += 1;
                else num512 += 1;
            }
            int padding = 0;
            int index = 0;
            var atlasSize = 2048;
            var offset = (1f / atlasSize);
            for (int i = 0; i < numRectIn2048.Count; i++)
            {
                var count = numRectIn2048[i];                
                var subTextures = LstTexture.GetRange(index, count);
                var subGameObjects = LstGameObject.GetRange(index, count);

                var atlas = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                var rects = atlas.PackTextures(subTextures.ToArray(), padding, 2048, false);
                atlas.Compress(true);
                for (int j = 0; j < rects.Length; j++)
                {
                    GameObject go = subGameObjects[j];
                    var rect = rects[j];
                    rect.x += offset;
                    rect.y += offset;
                    rect.width -= 2 * offset;
                    rect.height -= 2 * offset;
                    RawImage rimg = go.GetComponentInChildren<RawImage>(true);
                    Destroy(rimg.texture);
                    rimg.texture = atlas;
                    rimg.uvRect = rect;
                }
                index = count;
            }
            {
                var subTextures = LstTexture.GetRange(LstTexture.Count - numRectInOther, numRectInOther);
                var subGameObjects = LstGameObject.GetRange(LstTexture.Count - numRectInOther, numRectInOther);
                atlasSize = num1024 > 0 ? 1024 : 512;
                offset = (1f / atlasSize);
                var atlas = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                var rects = atlas.PackTextures(subTextures.ToArray(), padding, atlasSize, false);
                for (int i = 0; i < rects.Length; i++)
                {
                    GameObject go = subGameObjects[i];
                    var rect = rects[i];
                    rect.x += offset;
                    rect.y += offset;
                    rect.width -= 2 * offset;
                    rect.height -= 2 * offset;
                    UITexture rimg = go.GetComponentInChildren<UITexture>(true);
                    //Destroy(rimg.mainTexture);
                    rimg.mainTexture = atlas;
                    rimg.uvRect = rect;
                }
            }         
        }
        Destroy(imageItem);        
    }

    bool ondisable = false;
    private void OnDisable()
    {
        ondisable = true;
    }

    public void OnAppBtnClicked()
    {
        StopCoroutine(coroutine);
        Destroy(GameObject.Find("Canvas"));
    }

    public void OnCamBtnClicked()
    {
        GVs.SCENE_MANAGER.loadSnapImageScene();
    }

    public void OnHistoryBtnClicked()
    {
        GVs.SCENE_MANAGER.loadHistoryScene();
    }

    public void ObBackBtnClicked()
    {
        LibraryScriptsNGUI.mode = LibraryScriptsNGUI.MODE.CATEGORY;
    }
}