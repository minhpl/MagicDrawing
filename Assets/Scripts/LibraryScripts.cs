using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LibraryScripts : MonoBehaviour
{
    public GameObject imageItem;
    const int deScale = 1;
    const bool USE_PACK = true;
    const int clone = 5;
    //Mat tempMat;
    public static LibraryScripts Instance;
    IEnumerator coroutine;
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
        //tempMat = new Mat();
        if (Application.platform == RuntimePlatform.Android)
        {
            GVs.APP_PATH = "/data/data/com.MinhViet.ProductName/files";
        }
        else
        {
            GVs.APP_PATH = Application.persistentDataPath;
        }
        GFs.LoadTemplateList();
        GFs.LoadCategoryList();

        var watch = System.Diagnostics.Stopwatch.StartNew();
        coroutine = Load();
        MainThreadDispatcher.StartUpdateMicroCoroutine(coroutine);        
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;      
    }

    IEnumerator Load()
    {                      
        //Need
        List<Texture2D> LstTexture = new List<Texture2D>();
        List<GameObject> LstGameObject = new List<GameObject>();
        List<int> numRectIn2048 = new List<int>();
        int numRectInOther = 0;
        int deviant2048 = (1 << 22) / 10;
        int deviant1024 = (1 << 20) / 8;
        int deviant512 = (1 << 18) / 5;
        RawImage rimageOri = imageItem.transform.Find("RImage").GetComponent<RawImage>();
        int widthOri = (int)rimageOri.rectTransform.rect.width;
        var imageCount = GVs.DRAWING_TEMPLATE_LIST.Count();
        imageCount = GVs.CATEGORY_LIST.data.Count;
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
        
        for (int j = 0; j < clone; j++)
            for (int i = 0; i < imageCount; i++)
            {                
                yield return null;
                if (imageItem == null) break;
                GameObject go = Instantiate(imageItem) as GameObject;
                go.transform.SetParent(imageItem.transform.parent.transform);
                go.transform.localScale = imageItem.transform.localScale;
                RawImage rimage = go.transform.Find("RImage").GetComponent<RawImage>();

                var categorys = GVs.CATEGORY_LIST.data;
                Category category = categorys[i];
                var dir = GVs.CATEGORY_LIST.dir;

                var drawTemplateModel = GVs.DRAWING_TEMPLATE_LIST.Get(i);
                Texture2D texture = GFs.LoadPNG(dir + "/" + category.image);                
                float width = texture.width;
                float height = texture.height;
                float ratio = width / height;

                var w = widthOri;
                var h = widthOri;
                if (ratio > 1)
                {
                    w = widthOri >> deScale;
                    h = (int)(widthOri * height / width) >> deScale;
                    TextureScale.Bilinear(texture, widthOri >> deScale, (int)(widthOri * height / width) >> deScale);                    
                    rimage.rectTransform.sizeDelta = new Vector2(widthOri, widthOri * height / width);
                }
                else
                {
                    w = (int)(widthOri * width / height) >> deScale;
                    h = widthOri >> deScale;
                    TextureScale.Bilinear(texture, (int)(widthOri * width / height) >> deScale, widthOri >> deScale);
                    rimage.rectTransform.sizeDelta = new Vector2(widthOri * width / height, widthOri);
                }
                var area = w * h;
                Area += area;
                tempArea = tempArea - area;
                if(tempArea < 0)
                {
                    num2048 += 1;
                    tempArea = area2048 - area;                    
                    numRectIn2048.Add(j * imageCount + i + 1 - tempNumPacked);
                    tempNumPacked = j * imageCount + i + 1;
                }

                texture.Compress(true);
                rimage.texture = texture;                                
                go.SetActive(true);                
                LstGameObject.Add(go);               
                LstTexture.Add(texture);

                go.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var dirPath = GVs.APP_PATH + "/" + GVs.DRAWING_TEMPLATE_LIST.dir + "/";
                    var thumbPath = dirPath + drawTemplateModel.thumb;
                    var imgPath = dirPath + drawTemplateModel.image;
                    Debug.LogFormat("on click: img path is {0}", imgPath);
                    DrawingScripts.imgModelPath = imgPath;                    
                    DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_MODEL;
                    HistorySceneScripts.AddHistoryItem(new HistoryModel(imgPath, thumbPath, HistoryModel.IMAGETYPE.MODEL));
                    GVs.SCENE_MANAGER.loadDrawingScene();
                });                                                       
            }

        if(USE_PACK && imageItem!=null)
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
                    RawImage rimg = go.GetComponentInChildren<RawImage>(true);
                    Destroy(rimg.texture);
                    rimg.texture = atlas;
                    rimg.uvRect = rect;
                }
            }         
        }
        Destroy(imageItem);                
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
}
