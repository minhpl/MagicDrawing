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
            //string androidMagicBookFolder = "/storage/emulated/0/DCIM/MagicDrawing/";
            //string libraryFolder = androidMagicBookFolder + "library";
            //var files = Directory.GetFiles(libraryFolder, "*.png");
            //foreach(var file in files)
            //{
            //    Utilities.Log("File name is {0}", file);
            //}
        }
        else
        {
            GVs.APP_PATH = Application.persistentDataPath;
        }
        GFs.LoadTemplateList();
        var watch = System.Diagnostics.Stopwatch.StartNew();
        coroutine = Load();
        MainThreadDispatcher.StartUpdateMicroCoroutine(coroutine);        
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        //Utilities.Log("Time pass Load function: {0}", elapsedMs);        
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

        var imageCount = GVs.DRAWING_TEMPLATE_LIST_MODEL.Count();
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
                //yield return new WaitForEndOfFrame();
                yield return null;
                if (imageItem == null) break;
                GameObject go = Instantiate(imageItem) as GameObject;
                go.transform.SetParent(imageItem.transform.parent.transform);
                go.transform.localScale = imageItem.transform.localScale;
                RawImage rimage = go.transform.Find("RImage").GetComponent<RawImage>();

                var drawTemplateModel = GVs.DRAWING_TEMPLATE_LIST_MODEL.Get(i);
                go.GetComponent<DataBind>().drawingTemplateModel = drawTemplateModel;
                Texture2D texture = GFs.LoadPNG(GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + drawTemplateModel.thumb);
                //TextureScale.Point(texture, 100, 50);
                float width = texture.width;
                float height = texture.height;
                //Debug.LogFormat("width = {0}, height = {1}", width, height);
                float ratio = width / height;

                var w = widthOri;
                var h = widthOri;
                if (ratio > 1)
                {
                    w = widthOri >> deScale;
                    h = (int)(widthOri * height / width) >> deScale;

                    TextureScale.Bilinear(texture, widthOri >> deScale, (int)(widthOri * height / width) >> deScale);
                    //rimage.rectTransform.localScale = new Vector3(1, height / width, 1);
                    rimage.rectTransform.sizeDelta = new Vector2(widthOri, widthOri * height / width);
                }
                else
                {
                    w = (int)(widthOri * width / height) >> deScale;
                    h = widthOri >> deScale;
                    TextureScale.Bilinear(texture, (int)(widthOri * width / height) >> deScale, widthOri >> deScale);
                    //rimage.rectTransform.localScale = new Vector3(ratio, 1, 1);
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
                //var offsetWidth = 1 / 2048;
                //var offsetHeight = 1 / 2048;
                //var a = rimage.uvRect;
                //a.x += offsetWidth;
                //a.y += offsetHeight;
                //a.width -= 2 * offsetWidth;
                //a.height -= 2 * offsetHeight;
                LstTexture.Add(texture);
                go.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnItemClicked(go);
                });
                //var filePath = GVs.APP_PATH + "/" + GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + GVs.DRAWING_TEMPLATE_LIST_MODEL.Get(i).image;
                //Mat a = Imgcodecs.imread(filePath);
                //float width = a.width();
                //float height = a.height();
                //float ratio = width / height;
                //if (ratio > 1)
                //    Imgproc.resize(a, tempMat, new Size(widthOri, widthOri * height / width));
                //else Imgproc.resize(a, tempMat, new Size(widthOri * width / height, widthOri));
                //Texture2D texture2d = new Texture2D(tempMat.width(), tempMat.height());
                //Utils.matToTexture2D(tempMat, texture2d);            
                //rimage.texture = texture2d;
                //rimage.rectTransform.sizeDelta = new Vector2(tempMat.width(), tempMat.height());
                //go.SetActive(true);                          
            }

        if(USE_PACK && imageItem!=null)
        {
            int freeArea = 0;
            freeArea = Area - area2048 * num2048;

            if (freeArea > area1024)
            {
                //Debug.LogFormat("here");
                num2048 += 1;
                numRectIn2048.Add(imageCount * clone - tempNumPacked);
                //numRectIn2048 = clone * imageCount - numRectInOther;
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
                //Debug.LogFormat("Count is {0}", count);
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
                //atlas.Compress(true);
                //Debug.LogFormat("Atlas size is {0}", atlasSize);
                //Debug.LogFormat("Leng of rect array is {0}", rects.Length);
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

            //Debug.LogFormat("Number Area2048 need is {0}, Area1024 need is {1}, Area512 need is {2}", num2048, num1024, num512);            
        }
        //Destroy(imageItem.transform.parent.GetComponent<ContentSizeFitter>());
        //Destroy(imageItem.transform.parent.GetComponent<GridLayoutGroup>());
        Destroy(imageItem);        
        //watch.Stop();
        //var elapsedMs = watch.ElapsedMilliseconds;
        //Utilities.Log("Time excution: {0}", elapsedMs);
        //imageItem.GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    OnClickedGameObject(imageItem);
        //});
    }

    void OnItemClicked(GameObject go)
    {
        //Debug.LogFormat("name is {0}", go.GetComponent<DataBind>().drawingTemplateModel.thumb);
        //GVs.PREV_SCENE.Add(this.gameObject.scene.buildIndex);
        GVs.CURRENT_MODEL = go.GetComponent<DataBind>().drawingTemplateModel;
        DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_MODEL;
        GVs.SCENE_MANAGER.loadDrawingScene();
    }    

    public void OnAppBtnClicked()
    {
        StopCoroutine(coroutine);
        Destroy(GameObject.Find("Canvas"));
        GVs.SCENE_MANAGER.loadCollectionScene();
        //SceneManager.LoadScene("LibrarySceneCompare");
    }

    public void OnCamBtnClicked()
    {
        GVs.SCENE_MANAGER.loadSnapImageScene();
    }
}
