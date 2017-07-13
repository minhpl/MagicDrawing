using Assets.OpenCVForUnity.Examples.MagicDrawing;
using OpenCVForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class LibraryScripts : MonoBehaviour
{
    public GameObject imageItem;
    public int imageCount = 9;
    const int deScale = 1;
    //Mat tempMat;
    public static LibraryScripts Instance;
    void Awake()
    {
        Instance = this;
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
        StartCoroutine( Load());
    }

    IEnumerator Load()
    {
        RawImage rimageOri = imageItem.transform.Find("RImage").GetComponent<RawImage>();
        int widthOri = rimageOri.texture.width;
        imageCount = GVs.DRAWING_TEMPLATE_LIST_MODEL.Count();
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int j = 0; j < 5; j++)
            for (int i = 0; i < imageCount; i++)
            {
                GameObject go = Instantiate(imageItem) as GameObject;
                go.transform.parent = imageItem.transform.parent.transform;
                go.transform.localScale = imageItem.transform.localScale;
                RawImage rimage = go.transform.Find("RImage").GetComponent<RawImage>();

                Texture2D texture = GFs.LoadPNG(GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + GVs.DRAWING_TEMPLATE_LIST_MODEL.Get(i).image);
                //TextureScale.Point(texture, 100, 50);
                float width = texture.width;
                float height = texture.height;
                //Debug.LogFormat("width = {0}, height = {1}", width, height);
                float ratio = width / height;
                rimage.texture = texture;
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
                yield return new WaitForEndOfFrame();
            }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Utilities.Log("Time excution: {0}", elapsedMs);
    }

    // Update is called once per frame
    //void Update () {

    //}
}
