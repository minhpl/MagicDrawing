﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaryScript : MonoBehaviour {

    public GameObject imageItem;
    public UIScrollView uiScrollView;
    public int imageCount = 800;
    const int deScale = 1;
	// Use this for initialization
	void Start () {

        if (Application.platform == RuntimePlatform.Android)
        {
            GVs.APP_PATH = "/data/data/com.MinhViet.ProductName/files";
        }
        else
        {
            GVs.APP_PATH = Application.persistentDataPath;
        }
        GFs.LoadTemplateList();
        UITexture rimageOri = imageItem.transform.Find("image").GetComponent<UITexture>();
        int widthOri = rimageOri.width;
        Debug.LogFormat("width = {0}", widthOri);
        imageCount = GVs.DRAWING_TEMPLATE_LIST_MODEL.Count();

        Vector3  v3 = imageItem.transform.localPosition;
        v3.y += 340;
        float x = v3.x;

        var watch = System.Diagnostics.Stopwatch.StartNew();
              
        for (int j = 0; j < 5; j++)
            for (int i = 0; i < imageCount; i++)
            {
                if (i % 3 == 0)
                {
                    v3.y -= 340;
                    v3.x = x;
                }
                if (i % 3 == 1) v3.x = 0;
                if (i % 3 == 2) v3.x = -x;

                GameObject go = Instantiate(imageItem) as GameObject;
                go.transform.parent = imageItem.transform.parent.transform;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = v3;
                go.GetComponent<UIButton>().tweenTarget = null;

                UITexture rimage = go.transform.Find("image").GetComponent<UITexture>();
                Texture2D texture = GFs.LoadPNG(GVs.DRAWING_TEMPLATE_LIST_MODEL.dir + "/" + GVs.DRAWING_TEMPLATE_LIST_MODEL.Get(i).thumb);
                float width = texture.width;
                float height = texture.height;
                float ratio = width / height;
                rimage.mainTexture = texture;                
                if (ratio > 1)
                {
                    //TextureScale.Bilinear(texture, widthOri >> deScale, (int)(widthOri * height / width) >> deScale);
                    rimage.width = widthOri;
                    rimage.height = (int)(widthOri * height / width);
                    //rimage.rectTransform.localScale = new Vector3(1, height / width, 1);
                    //rimage.rectTransform.sizeDelta = new Vector2(widthOri, widthOri * height / width);
                }
                else
                {
                    //TextureScale.Bilinear(texture, widthOri >> deScale, (int)(widthOri * height / width) >> deScale);
                    rimage.width = (int)(widthOri * width / height);
                    rimage.height = widthOri;
                    //rimage.rectTransform.localScale = new Vector3(ratio, 1, 1);
                    //rimage.rectTransform.sizeDelta = new Vector2(widthOri * width / height, widthOri);
                }

                go.SetActive(true);
                uiScrollView.Scroll(20);
            }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.LogFormat("Time excution: {0}", elapsedMs);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
