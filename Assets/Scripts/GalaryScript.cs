using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaryScript : MonoBehaviour {
    public GameObject imageItem;
    public UIScrollView uiScrollView;
    public int imageCount = 800;
    const int deScale = 1;
    const int clone = 5;
	// Use this for initialization
	void Start () {
        GFs.LoadAllTemplateList();
        StartCoroutine(load());
    }

    private IEnumerator load()
    {
        yield return null;
        UITexture rimageOri = imageItem.transform.Find("image").GetComponent<UITexture>();
        int widthOri = rimageOri.width;
        Debug.LogFormat("width = {0}", widthOri);
        imageCount = GVs.DRAWING_TEMPLATE_LIST.Count();

        Vector3 v3 = imageItem.transform.localPosition;
        v3.y += 340;
        float x = v3.x;

        var watch = System.Diagnostics.Stopwatch.StartNew();

        for (int j = 0; j < clone; j++)
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
                Texture2D texture = GFs.LoadPNG(GVs.DRAWING_TEMPLATE_LIST.dir + "/" + GVs.DRAWING_TEMPLATE_LIST.Get(i).thumb);


                var drawTemplateModel = GVs.DRAWING_TEMPLATE_LIST.Get(i);
                go.GetComponent<DataBind>().drawingTemplateModel = drawTemplateModel;

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

                EventDelegate.Set(go.GetComponent<UIButton>().onClick, delegate () { OnItemClicked(go); }); 
                uiScrollView.Scroll(20);
                go.SetActive(true);
            }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.LogFormat("Time excution: {0}", elapsedMs);
    }

    void OnItemClicked(GameObject go)
    {
        //Debug.LogFormat("name is {0}", go.GetComponent<DataBind>().drawingTemplateModel.thumb);
        //GVs.PREV_SCENE.Add(this.gameObject.scene.buildIndex);
        var drawTemplateModel = go.GetComponent<DataBind>().drawingTemplateModel;
        var dirPath = GFs.getAppDataDirPath() + "/" + GVs.DRAWING_TEMPLATE_LIST.dir + "/";
        var thumbPath = dirPath + drawTemplateModel.thumb;
        var imgPath = dirPath + drawTemplateModel.image;
        DrawingScripts.imgModelPath = imgPath;
        GVs.SCENE_MANAGER.loadDrawingScene();
    }


    public void onAppBtnClicked()
    {
        GVs.SCENE_MANAGER.loadLibraryScene();
    }
}
