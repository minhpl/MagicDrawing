using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LukeWaffel.AndroidGallery;
using OpenCVForUnity;
using Kakera;

public class MainToolsNGUIScripts : MonoBehaviour {

    public UIButton uiBtnLibrary;
    public UIButton uiBtnCam;
    public UIButton uiBtnGallery;
    public UIButton uiBtnHistory;
    public UIButton uiBtnHome;

    [SerializeField]
    private Unimgpicker imagePicker;

    // Use this for initialization
    void Start()
    {
        //imagePicker = new Unimgpicker();
        uiBtnLibrary.onClick.Add(new EventDelegate(() =>
        {
            LibraryScriptsNGUI.mode = LibraryScriptsNGUI.MODE.CATEGORY;
            GVs.SCENE_MANAGER.loadLibraryNGUIScene();
        }));

        uiBtnCam.onClick.Add(new EventDelegate(() =>
        {
            GVs.SCENE_MANAGER.loadSnapImageScene();
        }));

        uiBtnGallery.onClick.Add(new EventDelegate(() =>
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidGallery.Instance.OpenGallery(ImageLoaded);
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                imagePicker.Show("Select Image", "unimgpicker", 1024);
            }
            else
            {
                GVs.SCENE_MANAGER.loadGalleryScene();
            }
        }));

        uiBtnHistory.onClick.Add(new EventDelegate(() =>
        {
            GVs.SCENE_MANAGER.loadHistoryNGUIScene();
        }));

        uiBtnHome.onClick.Add(new EventDelegate(() =>
        {
            GVs.TRACE_SCENE.Clear();
            GVs.SCENE_MANAGER.loadHomeScene();
        }));

        imagePicker.Completed += (string path) =>
        {
            Texture2D texture = GFs.LoadPNGFromPath(path);
            Mat image = new Mat(texture.height, texture.width, CvType.CV_8UC3);
            Utils.texture2DToMat(texture, image);
            DrawingScripts.image = image;
            DrawingScripts.texModel = texture;
            DrawingScripts.drawMode = DrawingScripts.DRAWMODE.DRAW_IMAGE;
            GVs.SCENE_MANAGER.loadDrawingScene();
            HistoryNGUIScripts.AddHistoryItem(new HistoryModel(path, path, HistoryModel.IMAGETYPE.SNAP));
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
        HistoryNGUIScripts.AddHistoryItem(new HistoryModel(path, path, HistoryModel.IMAGETYPE.SNAP));
    }
}
