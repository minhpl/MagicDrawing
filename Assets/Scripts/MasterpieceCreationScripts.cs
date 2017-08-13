using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MasterpieceCreationScripts : MonoBehaviour {
    private string dirPath;
    public GameObject imageItem;
    public GameObject canvas;
    public GridLayoutGroup gridLayoutGroup;
	void Start () {
        MainThreadDispatcher.StartUpdateMicroCoroutine(LoadMasterpieceDrawing());
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var canvasRat = (float)canvasRect.width / (float)canvasRect.height;
        var cellSize = gridLayoutGroup.cellSize;
        var newwidth = cellSize.x;
        var newheight = newwidth / canvasRat;
        gridLayoutGroup.cellSize = new Vector2(newwidth, newheight);                
    }	
    IEnumerator LoadMasterpieceDrawing()
    {
        yield return null;
        if (Application.platform == RuntimePlatform.Android)
        {
            dirPath = GVs.androidDir;
        }
        else
        {
            dirPath = GVs.pcDir;
        }
        var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".png"));
        foreach (var f in files)
        {
            yield return null;
            GameObject go = Instantiate(imageItem) as GameObject;
            go.transform.SetParent(imageItem.transform.parent.transform);
            go.transform.localScale = imageItem.transform.localScale;
            go.SetActive(true);
            Texture2D texture = GFs.LoadPNGFromPath(f);
            GameObject masterpiece = go.transform.Find("masterpiece").gameObject;
            var databind = go.GetComponent<DataBind>();
            databind.imageFileName = f;
            string videoPath = dirPath + Path.GetFileNameWithoutExtension(f)+".avi";
            if (File.Exists(videoPath))
            {
                databind.videoFileName = videoPath;
            }
            else Utilities.Log(videoPath);
            var rimg = masterpiece.GetComponent<RawImage>();
            var aspectratioFitter = masterpiece.GetComponent<AspectRatioFitter>();
            var widthImg = texture.width;
            var heightImg = texture.height;
            aspectratioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            aspectratioFitter.aspectRatio = (float)widthImg / (float)heightImg;
            var scale = 1 + GVs.ridTopPercent;
            rimg.rectTransform.localScale = new Vector3(scale, scale, scale);
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                GVs.SCENE_MANAGER.loadResultScene();
            });
            rimg.texture = texture;    
        }
        Destroy(imageItem);
    }
}
