using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

class GFs
{
    public static void LoadTemplateList()
    {
        if (PlayerPrefs.HasKey("TEMPLATE_DRAWING_LIST"))
        {
            string s = PlayerPrefs.GetString("TEMPLATE_DRAWING_LIST");
            GVs.DRAWING_TEMPLATE_LIST = JsonUtility.FromJson<TemplateDrawingList>(s);
        }
        else
            SaveTemplateList();
    }

    public static void SaveTemplateList()
    {
        PlayerPrefs.SetString("TEMPLATE_DRAWING_LIST", JsonUtility.ToJson(GVs.DRAWING_TEMPLATE_LIST));
        PlayerPrefs.Save();
    }

    public static void SaveCategoryList()
    {
        PlayerPrefs.SetString("CATEGORY_LIST", JsonUtility.ToJson(GVs.CATEGORY_LIST));
        PlayerPrefs.Save();
    }

    public static void LoadCategoryList()
    {
        if (PlayerPrefs.HasKey("CATEGORY_LIST"))
        {
            string s = PlayerPrefs.GetString("CATEGORY_LIST");
            GVs.CATEGORY_LIST = JsonUtility.FromJson<CategoryList>(s);
        }
        else
            SaveCategoryList();
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;
        Debug.Log(GVs.APP_PATH + "/" + filePath);
        if (File.Exists(GVs.APP_PATH + "/" + filePath))
        {
            fileData = File.ReadAllBytes(GVs.APP_PATH + "/" + filePath);
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


}
