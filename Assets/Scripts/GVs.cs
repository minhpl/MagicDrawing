using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
class GVs
{
    public static bool DEBUG = true;
    public static string LICENSE_CODE = "PUWCSL";
    public static string APP_PATH =
#if UNITY_ANDROID
        "data/data/com.MinhViet.Drawing/files";
#elif UNITY_IOS || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
		Application.persistentDataPath;
#endif
    public static string TRAIN_PATH;

    public static SceneManagerScript SCENE_MANAGER = new SceneManagerScript();
    public static string SERVER_URL = "http://developer.magicbook.vn/";
    //public static string SERVER_URL = "http://113.23.71.223:3000/";
    public static string LOGIN_URL = SERVER_URL + "login";
    public static string ACTIVE_LICENSE_URL = SERVER_URL + "users/activeLicense";
    public static string CHECK_LICENSE_URL = SERVER_URL + "users/checkLicense";
    public static string GET_ALL_CATEGORY_URL = SERVER_URL + "users/drawing/getAllCategory";
    public static string GET_JUNIOR_CATEGORY_URL = SERVER_URL + "users/drawing/getJuniorCategory";
    public static string GET_WORD_BY_CATEGORY_URL = SERVER_URL + "users/drawing/getWordByCategory";
    public static string GET_ALL_AVATA_URL = SERVER_URL + "users/english/getAllAvata";
    public static string DOWNLOAD_URL = SERVER_URL + "users/drawing/download";

    public static string GET_TEMPLATE_BY_CATEGORY_URL = GVs.SERVER_URL + "users/drawing/getTemplateByCategory";
    public static string KEY = "12345678123456781234567812345678";
    public static string IV = "1234567812345678";

    public static int CURRENT_LEVEL = 1;

    public static int CURRENT_GAME_MODE = 1;
    public static int GAME_MODE = 1;
    public static int SINGLE_MODE = 1;
    public static int GENERAL_MODE = 2;
    public static int JUNIOR_MODE = 3;
    public static int VERSUS_MODE = 4;
    public static int SURVIVAL_MODE = 5;

    public static UserListModel USER_LIST_MODEL = new UserListModel();
    public static UserModel CURRENT_USER_MODEL = USER_LIST_MODEL.Get(0);
    public static UserModel CURRENT_USER_EDIT_MODEL;
    public static UserModel NEW_USER_MODEL;
    public static int CURRENT_USER_EDIT = 0;
    public static int PROFILE_STATE = 0;
    public static int PROFILE_EDIT = 1;
    public static int PROFILE_ADD = 2;
    public static int PROFILE_CHANGE_AVATA = 3;
    public static int SOUND_SYSTEM = 1;
    public static int SOUND_BG = 1;

    public static AvataListModel AVATA_LIST_MODEL = new AvataListModel();
    //-----------------------
    public static Stack<int> TRACE_SCENE = new Stack<int>();
    public static LinkedList<string> history = new LinkedList<string>();
    public const float ridTopPercent = 0.09f;
    public const float bonusScale = 1.2f;
    public const float DURATION_TWEEN_UNIFY = 0.3f;
    public const float DELAY_TWEEN_UNIFY = 0.2f;
    public static CategoryList CATEGORY_LIST = new CategoryList();
    public static Dictionary<string, TemplateDrawingList> TEMPLATE_LIST_ALL_CATEGORY;
    public static TemplateDrawingList DRAWING_TEMPLATE_LIST = new TemplateDrawingList();
}
public class COLOR
{
    public static string RED = "#eb4051";
    public static string BLUE = "#0066ff";
    public static string CYAN = "#11cfe8";
    public static string GRAY = "#5d5d5d";
    public static string YELLOW = "#ffd916";
}
