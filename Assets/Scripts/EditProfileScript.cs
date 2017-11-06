using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditProfileScript : MonoBehaviour
{

    public UILabel title;
    public UIInput username;
    public UITexture avata;
    // Use this for initialization
    void Start()
    {
        if (GVs.PROFILE_STATE == GVs.PROFILE_ADD)
        {
            title.text = "Tạo nhân vật";
            if (GVs.NEW_USER_MODEL == null)
            {
                GVs.NEW_USER_MODEL = new UserModel("");
                GVs.NEW_USER_MODEL.avata = GVs.AVATA_LIST_MODEL.dir + '/' + GVs.AVATA_LIST_MODEL.Get(1).image;
            }
            if (GVs.AVATA_LIST_MODEL != null && GVs.AVATA_LIST_MODEL.Count() > 1)
            {
                GFs.LoadPNG(GVs.NEW_USER_MODEL.avata, avata);
            }
        }
        else
        {
            GFs.LoadPNG(GVs.CURRENT_USER_EDIT_MODEL.avata, avata);
            username.value = GVs.CURRENT_USER_EDIT_MODEL.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FinishOnClick()
    {
        if (!username.value.Trim().Equals(""))
        {
            if (GVs.PROFILE_STATE == GVs.PROFILE_ADD)
            {

                GVs.NEW_USER_MODEL.name = username.value.Trim();
                GVs.USER_LIST_MODEL.Add(GVs.NEW_USER_MODEL);
                GVs.CURRENT_USER_MODEL = GVs.NEW_USER_MODEL;
                GFs.SaveUsers();
            }
            else
            {
                GVs.CURRENT_USER_EDIT_MODEL.name = username.value.Trim();
                GVs.USER_LIST_MODEL.Set(GVs.CURRENT_USER_EDIT_MODEL, GVs.CURRENT_USER_EDIT);
                GFs.SaveUsers();
            }
        }
        GVs.NEW_USER_MODEL = null;
        GVs.SCENE_MANAGER.StartProfileScene();
    }

    public void CancelOnClick()
    {
        GVs.NEW_USER_MODEL = null;
        GVs.SCENE_MANAGER.StartProfileScene();
    }

    public void ChangeAvataOnClick()
    {
        GVs.SCENE_MANAGER.StartAvataScene();
    }
}
