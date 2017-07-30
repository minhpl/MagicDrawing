using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
public class PreviewResultScripts : MonoBehaviour {
    public Canvas canvas;
    public GameObject panel;
    public RawImage rawImg;    
    public static Texture2D texture;
    //public static float ratio;
	void Start () {        
        {
            rawImg.texture = texture;                       
            var canvasRect = canvas.GetComponent<RectTransform>().rect;
            var canvasRatHW = canvasRect.height / canvasRect.width;
            var panelRectTransform = panel.GetComponent<RectTransform>();
            var width = panelRectTransform.rect.width;
            var height = width * canvasRatHW;
            panelRectTransform.sizeDelta = new Vector2(width, height);

            var aspectRatioFitter = rawImg.gameObject.GetComponent<AspectRatioFitter>();
            if(texture!=null)
                aspectRatioFitter.aspectRatio = (float)texture.width / (float)texture.height;
            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        }
	}

    private void OnDisable()
    {
        Destroy(texture);        
    }

    public void OnOkBtnClicked()
    {
        Debug.Log("Here");
        GVs.SCENE_MANAGER.loadResultScene();
    }
}
