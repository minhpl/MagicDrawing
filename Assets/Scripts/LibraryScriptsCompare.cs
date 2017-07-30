using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LibraryScriptsCompare : MonoBehaviour
{
    public GameObject imageItem;
    public int imageCount = 9;
    const int deScale = 1;
    //Mat tempMat;    

    void Awake()
    {        
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
        StartCoroutine(Load());        
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        //Utilities.Log("Time pass Load function: {0}", elapsedMs);        
    }

    IEnumerator Load()
    {
        List<Texture2D> LstTexture = new List<Texture2D>();

        RawImage rimageOri = imageItem.transform.Find("RImage").GetComponent<RawImage>();
        int widthOri = (int)rimageOri.rectTransform.rect.width;
        imageCount = GVs.DRAWING_TEMPLATE_LIST_MODEL.Count();
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int j = 0; j < 1; j++)
            for (int i = 0; i < imageCount; i++)
            {
                yield return new WaitForEndOfFrame();
                GameObject go = Instantiate(imageItem) as GameObject;
                go.transform.SetParent(imageItem.transform.parent.transform);
                go.transform.localScale = imageItem.transform.localScale;
                RawImage rimage = go.transform.Find("RImage").GetComponent<RawImage>();
                //a = GVs.DRAWING_TEMPLATE_LIST_MODEL.Get(i);     

                var drawTemplateModel = GVs.DRAWING_TEMPLATE_LIST_MODEL.Get(i);
                go.GetComponent<DataBind>().drawingTemplateModel = drawTemplateModel;
                Texture2D texture = GFs.LoadPNG(GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + drawTemplateModel.thumb);
                //TextureScale.Point(texture, 100, 50);
                float width = texture.width;
                float height = texture.height;
                //Debug.LogFormat("width = {0}, height = {1}", width, height);
                float ratio = width / height;
               
                if (ratio > 1)
                {
                    TextureScale.Bilinear(texture, widthOri >> deScale, (int)(widthOri * height / width) >> deScale);
                    //rimage.rectTransform.localScale = new Vector3(1, height / width, 1);
                    rimage.rectTransform.sizeDelta = new Vector2(widthOri, widthOri * height / width);
                }
                else
                {
                    TextureScale.Bilinear(texture, (int)(widthOri * width / height) >> deScale, widthOri >> deScale);
                    //rimage.rectTransform.localScale = new Vector3(ratio, 1, 1);
                    rimage.rectTransform.sizeDelta = new Vector2(widthOri * width / height, widthOri);
                }
                texture.Compress(true);
                rimage.texture = texture;
                LstTexture.Add(texture);
                
                
                go.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnItemClicked(go);
                });

                go.SetActive(true);
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


        //Texture2D atlas = new Texture2D(1, 1,TextureFormat.Alpha8,false);
        //var LstRect = atlas.PackTextures(LstTexture.ToArray(), 3, 2048, false);
        //Debug.LogFormat("Listrect size is {0}, listTexture size is {1}", LstRect.Length, LstTexture.Count);
        //for (int i = 0; i < LstRect.Length; i++)
        //{
        //    Debug.LogFormat("Texture Size is ({0} , {1}), rect size is ({2} , {3})", LstTexture[i].width, LstTexture[i].height, LstRect[i].width * atlas.width, LstRect[i].height * atlas.height);
        //}
        ////System.IO.File.WriteAllBytes(Application.dataPath + "/" + "picture5886.png", atlas.EncodeToPNG());
        //atlas.Compress(true);
        //Debug.LogFormat("atlas: width = {0}, height  {1}", atlas.width, atlas.height);

        //Component[] LstRawImage = imageItem.transform.parent.GetComponentsInChildren(typeof(RawImage));
        //for (int i = 0; i < LstRawImage.Length; i++)
        //{
        //    RawImage RawImage = (RawImage)LstRawImage[i];
        //    Destroy(RawImage.texture);
        //    RawImage.texture = atlas;
        //    RawImage.uvRect = LstRect[i];
        //}

        //Debug.Log(Application.dataPath);
        


        Destroy(imageItem);
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        //Utilities.Log("Time excution: {0}", elapsedMs);        
        //imageItem.GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    OnClickedGameObject(imageItem);
        //});
    }

    void OnItemClicked(GameObject go)
    {
        Debug.LogFormat("name is {0}", go.GetComponent<DataBind>().drawingTemplateModel.thumb);
        //GVs.PREV_SCENE.Add(this.gameObject.scene.buildIndex);
        GVs.CURRENT_MODEL = go.GetComponent<DataBind>().drawingTemplateModel;
        GVs.SCENE_MANAGER.loadDrawingScene();
    }    


    public void onAppBtnClicked()
    {
        Destroy(GameObject.Find("Canvas"));
        GVs.SCENE_MANAGER.loadLibraryScene();
    }
}
