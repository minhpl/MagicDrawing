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
        if (PlayerPrefs.HasKey("DRAWING_TEMPLATE_LIST"))
        {
            string s = PlayerPrefs.GetString("DRAWING_TEMPLATE_LIST");
            GVs.DRAWING_TEMPLATE_LIST_MODEL = JsonUtility.FromJson<DrawingTemplateListModel>(s);
        }
        SaveTemplateList();
    }

    public static void SaveTemplateList()
    {
        PlayerPrefs.SetString("DRAWING_TEMPLATE_LIST", JsonUtility.ToJson(GVs.DRAWING_TEMPLATE_LIST_MODEL));
        PlayerPrefs.Save();
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;
        //Utilities.Log(GVs.APP_PATH + "/" + filePath);
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
