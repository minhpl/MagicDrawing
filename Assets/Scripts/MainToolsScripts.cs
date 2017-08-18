using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LukeWaffel.AndroidGallery;
using OpenCVForUnity;
using Kakera;

public class MainToolsScripts : MonoBehaviour {

    public Button btnLibrary;
    public Button btnCam;
    public Button btnGallary;
    public Button btnHistory;

    [SerializeField]
    private Unimgpicker imagePicker;

    // Use this for initialization
    void Start () {
        //imagePicker = new Unimgpicker();
        btnLibrary.onClick.AddListener(() =>
        {
            GVs.SCENE_MANAGER.loadLibraryScene();
        });

        btnCam.onClick.AddListener(()=>{
            GVs.SCENE_MANAGER.loadSnapImageScene();
        });

        btnGallary.onClick.AddListener(() =>
        {           
            if (Application.platform==RuntimePlatform.Android)
            {
                AndroidGallery.Instance.OpenGallery(ImageLoaded);                                
            }
            else if (Application.platform==RuntimePlatform.IPhonePlayer)
            {
                
                imagePicker.Show("Select Image", "unimgpicker", 1024);
            }
            else
            {
                GVs.SCENE_MANAGER.loadGalleryScene();
            }
        });

        btnHistory.onClick.AddListener(() =>
        {
            GVs.SCENE_MANAGER.loadHistoryScene();
        });

        imagePicker.Completed += (string path) =>
        {
            Utilities.Log("Path here is {0}", path);
//            
			Texture2D texture = GFs.LoadPNGFromPath(path);
			Mat image = new Mat(texture.height,texture.width,CvType.CV_8UC3);
			Utils.texture2DToMat(texture,image);
            DrawingScripts.image = image;
            DrawingScripts.texModel = texture;
            DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
            GVs.SCENE_MANAGER.loadDrawingScene();
            HistorySceneScripts.AddHistoryItem(new HistoryModel(path, path, HistoryModel.IMAGETYPE.SNAP));
        };
    }

    void ImageLoaded()
    {
        Texture2D texture = (Texture2D)AndroidGallery.Instance.GetTexture();
        Mat image = new Mat(texture.height, texture.width,CvType.CV_8UC4);
        Utils.textureToMat(texture, image);
        var path = AndroidGallery.Instance.getPath();        
        DrawingScripts.image = image;
        DrawingScripts.texModel = texture;
        DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
        GVs.SCENE_MANAGER.loadDrawingScene();
        HistorySceneScripts.AddHistoryItem(new HistoryModel(path, path, HistoryModel.IMAGETYPE.SNAP));
    }
}
