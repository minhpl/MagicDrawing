using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MasterpieceCreationScripts : MonoBehaviour {
    private string dirPathMP;
    public GameObject item;
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
       
        dirPathMP = GFs.getMasterpieceDirPath();
        Debug.LogFormat("dir path masterpiece is {0}", dirPathMP);

        var files = Directory.GetFiles(dirPathMP, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => s.EndsWith(".png"));

		var info = new DirectoryInfo(dirPathMP);
		var fileInfo = info.GetFiles();
		foreach (var file in fileInfo) print(file.Name);

		Debug.LogFormat ("Files count is :{0}", files.Count ());
        foreach (var f in files)
        {
            yield return null;
            try
            {
                Debug.Log(f.ToString());
                GameObject go = Instantiate(item) as GameObject;
                go.transform.SetParent(item.transform.parent.transform);
                go.transform.localScale = item.transform.localScale;
                go.SetActive(true);
                Texture2D texture = GFs.LoadPNGFromPath(f);
                GameObject masterpiece = go.transform.Find("RImage").gameObject;

                var fileNameWithouExtension = Path.GetFileNameWithoutExtension(f);
                string videoPath = dirPathMP + fileNameWithouExtension + ".avi";
                if (!File.Exists(videoPath))                
                {
                    videoPath = null;
                }
                var rimg = masterpiece.GetComponent<RawImage>();
                var aspectratioFitter = masterpiece.GetComponent<AspectRatioFitter>();
                var widthImg = texture.width;
                var heightImg = texture.height;
                aspectratioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                aspectratioFitter.aspectRatio = (float)widthImg / (float)heightImg;

                var ridTopPercent = GVs.ridTopPercent;
                rimg.rectTransform.pivot = new Vector2(0.5f, 1 - ridTopPercent);
                var h = rimg.rectTransform.rect.height * rimg.rectTransform.localScale.y;
                var ridTop = ridTopPercent * h;
                Debug.Log(h);
                var position = rimg.rectTransform.localPosition;
                rimg.rectTransform.localPosition = new Vector3(position.x, position.y + ridTop, position.z);

                var scale = 1 + GVs.bonusScale;
                Utilities.Log("Scale is {0}", scale);
                rimg.rectTransform.localScale = new Vector3(scale, scale, scale);

                go.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        ResultScripts.texture = texture;
                        ResultScripts.videoPath = videoPath;
                        ResultScripts.mode = ResultScripts.MODE.REWATCH_RESULT;
                        var datetime = DateTime.ParseExact(fileNameWithouExtension, Utilities.customFmts, new CultureInfo(0x042A));
                        var datemonthyear = string.Format("{0}", datetime.Date.ToString("d-M-yyyy"));
                        Debug.Log(datemonthyear);
                        ResultScripts.title = datemonthyear;
                        GVs.SCENE_MANAGER.loadResultScene();
                    }
                );
                rimg.texture = texture;
            }
            catch (Exception ex)
            {
                Debug.Log("Loi");
                Debug.LogError(ex);
            }
        }
        Destroy(item);
    }
}
