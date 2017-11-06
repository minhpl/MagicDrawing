//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UniRx;
//using UnityEngine;
//using OpenCVForUnity;
//using UnityEngine.UI;

//public class GallerySceneScripts : MonoBehaviour {
//    private string mainImagesPath = null;
//    private string excludeDirPath = null;
//    private string MPModelDirPath = null;
//    public GameObject item;
//    void Start () {        
//        if (Application.platform == RuntimePlatform.Android)
//        {
//            //mainImagesPath = GVs.androidMainImagesDirPath;
//            excludeDirPath = GVs.androidDirMPiece;
//            MPModelDirPath = GVs.androidDirMPModel;
//        }            
//        else
//        {
//            //mainImagesPath = GVs.pcMainImagesDirPath;
//            excludeDirPath = GVs.pcDirMPiece;
//            MPModelDirPath = GVs.pcDirMPModel;
//            Debug.LogFormat("excludeDirPath is {0}", excludeDirPath);
//        }
            
//        MainThreadDispatcher.StartUpdateMicroCoroutine(load());
//    }

//    IEnumerator load()
//    {
//        yield return null;
//        var mainImages = Directory.GetFiles(mainImagesPath, "*.*", SearchOption.AllDirectories)
//     .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
//        Debug.LogFormat("main images {0}", mainImages.Count());
//        var MPModelImage = Directory.GetFiles(MPModelDirPath, "*.*", SearchOption.AllDirectories)
//     .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
//        Debug.LogFormat("MPModelImage {0}", MPModelImage.Count());
//        var excludeImamge = Directory.GetFiles(excludeDirPath, "*.*", SearchOption.AllDirectories)
//     .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
//        Debug.LogFormat("excludeImamge {0}", excludeImamge.Count());

//        var files = (mainImages.Except(excludeImamge)).Union(MPModelImage).ToArray();

//        foreach (var file in files)
//        {
//            yield return null;
//            GameObject cloneItem = Instantiate(item, item.transform.parent);
//            cloneItem.transform.localScale = item.transform.localScale;
//            Mat image = Imgcodecs.imread(file, Imgcodecs.CV_LOAD_IMAGE_UNCHANGED);              
//            Imgproc.cvtColor(image, image, Imgproc.COLOR_BGRA2RGBA);
//            Texture2D texture = new Texture2D(image.width(), image.height(), TextureFormat.RGBA32, false);
//            Utils.matToTexture2D(image, texture);
//            var rimgGameObject = cloneItem.transform.Find("rimg");            
//            cloneItem.SetActive(true);
//            rimgGameObject.GetComponent<AspectRatioFitter>().aspectRatio = (float)image.width() / (float)image.height();            
//            var imgSize = rimgGameObject.GetComponent<RawImage>().rectTransform.rect;
//            Debug.LogFormat("{0} {1}", imgSize.width, imgSize.height);
//            TextureScale.Bilinear(texture,(int)imgSize.width,(int)imgSize.height);
//            texture.Compress(true);
//            rimgGameObject.GetComponent<RawImage>().texture = texture;
//            cloneItem.GetComponent<Button>().onClick.AddListener(() =>
//            {
//                DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
//                DrawingScripts.image = image;
//                DrawingScripts.texModel = texture;
//                HistorySceneScripts.AddHistoryItem(new HistoryModel(file, file, HistoryModel.IMAGETYPE.SNAP));
//                GVs.SCENE_MANAGER.loadDrawingScene();
//            });
//        }
//    }
//}
