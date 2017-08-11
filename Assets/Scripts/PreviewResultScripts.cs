using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using System.IO;
using System;

public class PreviewResultScripts : MonoBehaviour {
    public Canvas canvas;
    public GameObject panel;
    public RawImage rawImg;    
    public static Texture2D texture;
    //public static float ratio;
	void Start () {        
        {                                
            var canvasRect = canvas.GetComponent<RectTransform>().rect;
            var canvasRatHW = canvasRect.height / canvasRect.width;
            var panelRectTransform = panel.GetComponent<RectTransform>();
            var width = panelRectTransform.rect.width;
            var height = width * canvasRatHW;
            panelRectTransform.sizeDelta = new Vector2(width, height);
            var aspectRatioFitter = rawImg.gameObject.GetComponent<AspectRatioFitter>();
            if(texture!=null)
            {
                aspectRatioFitter.aspectRatio = (float)texture.width / (float)texture.height;
                aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                rawImg.texture = texture;
            }
        }
	}

    private void OnDisable()
    {
        if (preserveTexture)
        {
            Utilities.Log("here0");
            return;
        }
        Utilities.Log("why here");
        Destroy(texture);        
    }

    bool preserveTexture = false;
    public void OnOkBtnClicked()
    {
        preserveTexture = true;
        ResultScripts.texture = texture;
        Mat a = new Mat(texture.height, texture.width,CvType.CV_8UC3);       
        Utils.texture2DToMat(texture, a );
        Imgproc.cvtColor(a, a, Imgproc.COLOR_RGB2BGR);
        string fullPath = null;
        string name = String.Format("video_{0}.png", DateTime.Now.ToString(Utilities.customFmts));
        if (Application.platform == RuntimePlatform.Android)
        {
            fullPath = GVs.androidDir + name;
            if (!Directory.Exists(GVs.androidDir))
            {
                Directory.CreateDirectory(GVs.androidDir);
            }
            Utilities.Log("filename is {0}", fullPath);
        }
        else
        {
            fullPath = GVs.pcDir + name;
            if (!Directory.Exists(GVs.pcDir))
            {
                Directory.CreateDirectory(GVs.pcDir);
            }
            Utilities.Log("filename is {0}", fullPath);
        }
        Imgcodecs.imwrite(fullPath, a);
        GVs.SCENE_MANAGER.loadResultScene();
    }
}
