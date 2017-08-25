using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using System;

public class GFs
{
    public static void LoadData()
    {
        //PlayerPrefs.DeleteAll();
        LoadUsers();
        LoadAvatas();
        LoadLevel();
        LoadLicenseCode();
        LoadSoundConfig();
		GFs.LoadCategoryList();
		GFs.LoadAllTemplateList();
    }
    public static void SaveUsers()
    {
        PlayerPrefs.SetString("USERS", JsonUtility.ToJson(GVs.USER_LIST_MODEL));
        PlayerPrefs.SetString("CURRENT_USER", JsonUtility.ToJson(GVs.CURRENT_USER_MODEL));
        PlayerPrefs.Save();
    }
    public static void LoadUsers()
    {
        if (PlayerPrefs.HasKey("USERS"))
        {
            string s = PlayerPrefs.GetString("USERS");
            GVs.USER_LIST_MODEL = JsonUtility.FromJson<UserListModel>(s);
        }
        if (PlayerPrefs.HasKey("CURRENT_USER"))
        {
            string s = PlayerPrefs.GetString("CURRENT_USER");
            GVs.CURRENT_USER_MODEL = JsonUtility.FromJson<UserModel>(s);
        }
        SaveUsers();
    }
    public static void SaveAvatas()
    {
        PlayerPrefs.SetString("AVATAS", JsonUtility.ToJson(GVs.AVATA_LIST_MODEL));
        PlayerPrefs.Save();
    }
    public static void LoadAvatas()
    {
        if (PlayerPrefs.HasKey("AVATAS"))
        {
            string s = PlayerPrefs.GetString("AVATAS");
            GVs.AVATA_LIST_MODEL = JsonUtility.FromJson<AvataListModel>(s);
        }
        SaveAvatas();
    }
    public static void LoadLevel()
    {
        if (PlayerPrefs.HasKey("CURRENT_LEVEL"))
        {
            GVs.CURRENT_LEVEL = PlayerPrefs.GetInt("CURRENT_LEVEL");
        }
    }
    public static void SaveLevel()
    {
        PlayerPrefs.SetInt("CURRENT_LEVEL", GVs.CURRENT_LEVEL);
        PlayerPrefs.Save();
    }
    public static void LoadLicenseCode()
    {
        if (PlayerPrefs.HasKey("LICENSE_CODE"))
        {

            GVs.LICENSE_CODE = PlayerPrefs.GetString("LICENSE_CODE");
        }
    }
    public static void SaveLicenseCode()
    {
        PlayerPrefs.SetString("LICENSE_CODE", GVs.LICENSE_CODE);
        PlayerPrefs.Save();
    }

    public static void LoadSoundConfig()
    {
        if (PlayerPrefs.HasKey("SOUND_SYSTEM"))
        {
            GVs.SOUND_SYSTEM = PlayerPrefs.GetInt("SOUND_SYSTEM");
        }
        if (PlayerPrefs.HasKey("SOUND_BG"))
        {
            GVs.SOUND_BG = PlayerPrefs.GetInt("SOUND_BG");
        }
    }
    public static void SaveSoundConfig()
    {
        PlayerPrefs.SetInt("SOUND_BG", GVs.SOUND_BG);
        PlayerPrefs.SetInt("SOUND_SYSTEM", GVs.SOUND_SYSTEM);
        PlayerPrefs.Save();
    }
    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;
        if (File.Exists(GVs.APP_PATH + "/" + filePath))
        {
            fileData = File.ReadAllBytes(GVs.APP_PATH + "/" + filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData);
        }
        else
        {
            Debug.Log(GVs.APP_PATH + "/" + filePath);
            Debug.Log("File not existed");
        }
        return tex;
    }
    public static Texture2D LoadPNG(string filePath, UITexture uiTextture)
    {
        Texture2D tex = null;
        byte[] fileData;
        Debug.Log(GVs.APP_PATH + "/" + filePath);
        if (File.Exists(GVs.APP_PATH + "/" + filePath))
        {
            fileData = File.ReadAllBytes(GVs.APP_PATH + "/" + filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            uiTextture.mainTexture = tex;
        }
        else
        {
            Debug.Log("File not existed");
        }
        return tex;
    }
    public static Color GetColor(string s)
    {
        Color myColor = new Color();
        ColorUtility.TryParseHtmlString(s, out myColor);
        return myColor;
    }

	private const string ALL_TEMPLATE_LIST = "ALL_TEMPLATE_LIST";
    private const string CATEGORY_LIST = "CATEGORY_LIST";
    public static void LoadAllTemplateList()
    {
        if (PlayerPrefs.HasKey(ALL_TEMPLATE_LIST))
        {
            string s = PlayerPrefs.GetString(ALL_TEMPLATE_LIST);
            try
            {         
                GVs.TEMPLATE_LIST_ALL_CATEGORY = JsonConvert.DeserializeObject<Dictionary<string, TemplateDrawingList>>(s);
                AotHelper.EnsureDictionary<string, TemplateDrawingList>();
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
		if (PlayerPrefs.HasKey (CATEGORY_LIST)) {
			try {
				
				string s = PlayerPrefs.GetString (CATEGORY_LIST);				
				GVs.CATEGORY_LIST = JsonUtility.FromJson<CategoryList> (s);				

			} catch (Exception ex) {

			}
		} else
			SaveCategoryList ();
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
