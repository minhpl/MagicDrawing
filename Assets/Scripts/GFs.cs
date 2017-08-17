using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

class GFs
{
    private const string ALL_TEMPLATE_LIST = "ALL_TEMPLATE_LIST";
    private const string CATEGORY_LIST = "CATEGORY_LIST";

    public static void LoadAllTemplateList()
    {
        if (PlayerPrefs.HasKey(ALL_TEMPLATE_LIST))
        {
            string s = PlayerPrefs.GetString(ALL_TEMPLATE_LIST);
            //Debug.LogFormat("Load All Template List: {0}", s);
            try
            {         
                GVs.TEMPLATE_LIST_ALL_CATEGORY = JsonConvert.DeserializeObject<Dictionary<string, TemplateDrawingList>>(s);
                AotHelper.EnsureDictionary<string, TemplateDrawingList>();
                //Debug.Log("Deserialized dictionary success");
                //Debug.Log(GVs.TEMPLATE_LIST_ALL_CATEGORY["C02"].templates.Count);
            }
            catch(Exception e)
            {
                Utilities.Log("Error is : {0}", e.ToString());
                Utilities.Log("Trace is : {0}", e.ToString());
            }
        }
        else
            SaveAllTemplateList();
    }

    public static void SaveAllTemplateList()
    {        
        var json = JsonConvert.SerializeObject(GVs.TEMPLATE_LIST_ALL_CATEGORY);        
        PlayerPrefs.SetString(ALL_TEMPLATE_LIST, json);
        PlayerPrefs.Save();
    }

    public static void SaveCategoryList()
    {
        var json = JsonUtility.ToJson(GVs.CATEGORY_LIST);
        PlayerPrefs.SetString(CATEGORY_LIST, json);
        PlayerPrefs.Save();
    }

    public static void LoadCategoryList()
    {
        if (PlayerPrefs.HasKey(CATEGORY_LIST))
        {
            string s = PlayerPrefs.GetString(CATEGORY_LIST);
            GVs.CATEGORY_LIST = JsonUtility.FromJson<CategoryList>(s);            
        }
        else
            SaveCategoryList();
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;
        var appDirPath = GFs.getAppDataDirPath();
        if (File.Exists(appDirPath + "/" + filePath))
        {
            fileData = File.ReadAllBytes(appDirPath + "/" + filePath);
            tex = new Texture2D(2, 2);            
            tex.LoadImage(fileData);    
        }
        else
        {
            Debug.Log("File not existed");
        }
        return tex;
    }

    public static Texture2D LoadPNGFromPath(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;
        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        else
        {
            Debug.Log("File not existed");
        }
        return tex;
    }

    public static string getMasterpieceDirPath()
    {
        const string ANDROID_DIR_PATH_MASTERPIECE = "/storage/emulated/0/DCIM/MagicDrawing/Masterpiece/";
        const string IPHONE_DIR_NAME_MASTERPIECE = "masterpiece";
        const string PC_DIR_NAME_MASTERPIECE = "masterpiece";

        if (Application.platform == RuntimePlatform.Android)
        {
            return ANDROID_DIR_PATH_MASTERPIECE;
        }
        else
        {
            var appPath = getAppDataDirPath();
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return appPath + IPHONE_DIR_NAME_MASTERPIECE+"/";
            }
            else
            {
                return appPath + PC_DIR_NAME_MASTERPIECE+"/";
            }
        }
    }

    public static string getAppDataDirPath()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return "/data/data/com.MinhViet.ProductName/files/";
        }
        else 
        {            
            return Application.persistentDataPath + "/";            
        }   
    }

    public static string getSnapImageDirPath()
    {
        const string IPHONE_DIR_NAME_SNAPIMAGE = "snapimages";
        const string ANDROID_DIR_PATH_SNAPIMAGE = "/storage/emulated/0/DCIM/MagicDrawing/MasterPieceModel/";
        const string PC_DIR_NAME_SNAPIMAGE = "snapimages";
        if (Application.platform == RuntimePlatform.Android)
        {
            return ANDROID_DIR_PATH_SNAPIMAGE;
        }
        else
        {
            var appPath = getAppDataDirPath();
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return appPath + IPHONE_DIR_NAME_SNAPIMAGE+"/";
            }
            else
            {
                return appPath + PC_DIR_NAME_SNAPIMAGE+"/";
            }
        }
    }
}
