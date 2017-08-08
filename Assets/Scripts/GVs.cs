using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GVs {	
    public static string APP_PATH;
    public static SceneManagerScript SCENE_MANAGER = new SceneManagerScript();
    public static string SERVER_URL = "http://118.71.225.28:3000/";
    public static string ACTIVE_LICENSE_URL = SERVER_URL + "users/activeLicense";
    public static string CHECK_LICENSE_URL = SERVER_URL + "users/checkLicense";
    public static string GET_ALL_AVATA_URL = SERVER_URL + "users/drawing/getAllAvata";
    public static string DOWNLOAD_URL = SERVER_URL + "users/drawing/download";
    public static string GET_ALL_TEMPLATE_URL = SERVER_URL + "users/drawing/getAllTemplate";

    public static string KEY = "12345678123456781234567812345678";
    public static string IV = "1234567812345678";
    public static AvataListModel AVATA_LIST_MODEL = new AvataListModel();
    public static int CURRENT_USER_EDIT = 0;

    public static int PROFILE_STATE = 0;
    public static int PROFILE_EDIT = 1;
    public static int PROFILE_ADD = 2;
    public static int PROFILE_CHANGE_AVATA = 3;

    public static DrawingTemplateListModel DRAWING_TEMPLATE_LIST_MODEL = new DrawingTemplateListModel();

    //-----------------------
    public static Stack<int> TRACE_SCENE = new Stack<int>();
    public static DrawingTemplateModel CURRENT_MODEL;    
 
}
