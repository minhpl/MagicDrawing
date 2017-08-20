using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using System.IO;
using System;
using UniRx;

public class PreviewResultScripts : MonoBehaviour {
    public Canvas canvas;
    public GameObject panel;
    public RawImage rawImg;    
    public static Texture2D texture;
    //public static float ratio;
    void Start()
    {
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var canvasRatHW = canvasRect.height / canvasRect.width;
        var panelRectTransform = panel.GetComponent<RectTransform>();
        var width = panelRectTransform.rect.width;
        var height = width * canvasRatHW;
        panelRectTransform.sizeDelta = new Vector2(width, height);
        var aspectRatioFitter = rawImg.gameObject.GetComponent<AspectRatioFitter>();
        if (texture != null)
        {
            aspectRatioFitter.aspectRatio = (float)texture.width / (float)texture.height;
            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            //var scale = 1 + GVs.ridTopPercent;
            //rawImg.rectTransform.localScale = new Vector3(scale, scale, scale);

            var ridTopPercent = GVs.ridTopPercent;
            rawImg.rectTransform.pivot = new Vector2(0.5f, 1 - ridTopPercent);
            var h = rawImg.rectTransform.rect.height * rawImg.rectTransform.localScale.y;
            var ridTop = ridTopPercent * h;
            Debug.Log(h);
            var position = rawImg.rectTransform.localPosition;
            rawImg.rectTransform.localPosition = new Vector3(position.x, position.y + ridTop, position.z);

            var scale = 1 + GVs.bonusScale;
            Utilities.Log("Scale is {0}", scale);
            rawImg.rectTransform.localScale = new Vector3(scale, scale, scale);

            rawImg.texture = texture;
        }        
    }
    private void OnDisable()
    {
        if (preserveTexture)
        {
            return;
        }        
        Destroy(texture);        
    }
    bool preserveTexture = false;
    public void OnOkBtnClicked()
    {
        preserveTexture = true;
        ResultScripts.mode = ResultScripts.MODE.FISRT_RESULT;
        Mat a = new Mat(texture.height, texture.width,CvType.CV_8UC3);       
        Utils.texture2DToMat(texture, a );
        Imgproc.cvtColor(a, a, Imgproc.COLOR_RGB2BGR);
        string fullPath = null;
        string name = null;
        if (WebcamVideoCapture.filenameWithoutExt != null)
        {
            name = String.Format("{0}.png", WebcamVideoCapture.filenameWithoutExt);
        }            
        else
        {
            name = String.Format("{0}.png", DateTime.Now.ToString(Utilities.customFmts));
        }
        
        var masterPieceDirPath = GFs.getMasterpieceDirPath();
        fullPath = masterPieceDirPath + name;
        if (!Directory.Exists(masterPieceDirPath))
        {
            Directory.CreateDirectory(masterPieceDirPath);
        }  

        Utilities.Log("filename is {0}", fullPath);
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			System.IO.File.WriteAllBytes (fullPath, texture.EncodeToPNG());
		} else {
			Imgcodecs.imwrite(fullPath, a);   
		}

             
        WebcamVideoCapture.filename = null;
        WebcamVideoCapture.filenameWithoutExt = null;
        ResultScripts.mode = ResultScripts.MODE.FISRT_RESULT;
        GVs.SCENE_MANAGER.loadResultScene();
    }
}
