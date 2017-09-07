using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MasterpieceCreationNGUIScripts : MonoBehaviour {
    private string dirPathMP;
    public GameObject item;
    public GameObject canvas;
    public UIGrid uiGrid;
    private IDisposable cancelCoroutineBackButtonAndroid;
    private IDisposable cancelMCoroutineLoadMasterpiece;
    private void Awake()
    {
        cancelCoroutineBackButtonAndroid = GFs.BackButtonAndroidGoHome();
    }

    void Start () {
        cancelMCoroutineLoadMasterpiece = Observable.FromMicroCoroutine(LoadMasterpieceDrawing).Subscribe();
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var canvasRat = (float)canvasRect.width / (float)canvasRect.height;        
        UIPanel uiPanel = item.GetComponent<UIPanel>();
        uiPanel.clipping = UIDrawCall.Clipping.SoftClip;        
        var width = uiPanel.width;        
        var newheight = width / canvasRat;
        uiPanel.SetRect(0, 0, width, newheight);
        BoxCollider boxCollider = item.GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(width, newheight, 0);
        boxCollider.center = Vector3.zero;
        var padding = 50;
        uiGrid.cellHeight = newheight + padding;
    }

    private void OnDisable()
    {
        if (cancelCoroutineBackButtonAndroid != null)
            cancelCoroutineBackButtonAndroid.Dispose();
        if (cancelMCoroutineLoadMasterpiece != null)
            cancelMCoroutineLoadMasterpiece.Dispose();
    }

    IEnumerator LoadMasterpieceDrawing()
    {        
        yield return null;       
        dirPathMP = GFs.getMasterpieceDirPath();        
        var imagefiles = Directory.GetFiles(dirPathMP, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => s.EndsWith(".png"));
        foreach (var f in imagefiles)
        {
            yield return null;           
            GameObject go = Instantiate(item) as GameObject;
            go.transform.SetParent(item.transform.parent.transform);
            go.transform.localScale = item.transform.localScale;
            go.SetActive(true);
            uiGrid.Reposition();
            Texture2D texture = GFs.LoadPNGFromPath(f);
            GameObject masterpiece = go.transform.Find("icon").gameObject;
            var fileNameWithouExtension = Path.GetFileNameWithoutExtension(f);
            string videoPath = dirPathMP + fileNameWithouExtension + ".avi";
            if (!File.Exists(videoPath))
            {
                videoPath = null;
            }
            var uiTexture = masterpiece.GetComponent<UITexture>();          
            var widthImg = texture.width;
            var heightImg = texture.height;

            Debug.Log(f);
            go.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
            {
                ResultScripts.texture = texture;
                ResultScripts.videoPath = videoPath;
                ResultScripts.imagePath = f;
                ResultScripts.mode = ResultScripts.MODE.REWATCH_RESULT;
                var datetime = DateTime.ParseExact(fileNameWithouExtension, Utilities.customFmts, new CultureInfo(0x042A));
                var datemonthyear = string.Format("{0}", datetime.Date.ToString("d-M-yyyy"));
                Debug.Log(datemonthyear);
                ResultScripts.title = datemonthyear;
                GVs.SCENE_MANAGER.loadResultScene();
            })); 

            uiTexture.mainTexture = texture;
        }
        Destroy(item);

        var videoFiles = Directory.GetFiles(dirPathMP, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => s.EndsWith(".avi"));
        foreach(var videoPath in videoFiles)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(videoPath);
            var imageCorresponding = dirPathMP + fileNameWithoutExtension + ".png";
            if (!File.Exists(imageCorresponding))
            {
                File.Delete(videoPath);
            }
        }
    }
}
