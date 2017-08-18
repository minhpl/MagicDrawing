using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileScript : MonoBehaviour
{

    public GameObject userItem;
    public GameObject goAddUser;
    public UIToggle uiToggleEdit;
    public UILabel title;
    // Use this for initialization
    void Start()
    {
        int count = GVs.USER_LIST_MODEL.userModels.Length;
        Vector3 v3 = userItem.transform.localPosition;
        v3.y += 380;
        for (int i = 0; i < count; i++)
        {
            if (i < count - count % 3)
            {
                if (i % 3 == 0)
                {
                    v3.x = -360;
                    v3.y -= 380;
                }
                if (i % 3 == 1) v3.x = 0;
                if (i % 3 == 2) v3.x = 360;
            }
            else
            {
                if (count % 3 == 1)
                {
                    v3.x = 0;
                    v3.y -= 380;
                }
                else
                {

                    if (i % 3 == 0)
                    {
                        v3.x = -360 / 2;
                        v3.y -= 380;
                    }
                    if (i % 3 == 1) v3.x = 360 / 2;
                }
            }
            GameObject go = Instantiate(userItem) as GameObject;
            go.transform.parent = userItem.transform.parent.transform;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = v3;
            go.transform.Find("text").gameObject.GetComponent<UILabel>().text = GVs.USER_LIST_MODEL.Get(i).name;
            GFs.LoadPNG(GVs.USER_LIST_MODEL.Get(i).avata, go.GetComponent<UITexture>());
            go.SetActive(true);
            // EventDelegate.Set(go.GetComponent<UIButton>().onClick, EditProfileOnCLick);
            go.transform.Find("id").gameObject.GetComponent<UILabel>().text = i + "";
            UIEventListener.Get(go).onClick += EditProfileOnCLick;
        }
        v3.y -= 380;
        v3.x = 0;
        goAddUser.transform.localPosition = v3;
        goAddUser.SetActive(true);
        userItem.transform.parent.GetComponent<UIScrollView>().Scroll(-20);
        userItem.transform.parent.GetComponent<UIScrollView>().Scroll(20);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnToggleEditStateChange()
    {
        if (uiToggleEdit.value)
        {
            title.text = "Chọn nhân vật muốn sửa";
            goAddUser.SetActive(false);
        }
        else
        {
            title.text = "Danh sách nhân vật";
            goAddUser.SetActive(true);
        }
    }

    public void EditProfileOnCLick(GameObject go)
    {
        if (uiToggleEdit.value)
        {
            string s = go.transform.Find("id").gameObject.GetComponent<UILabel>().text;
            int i = int.Parse(s);
            GVs.CURRENT_USER_EDIT = i;
            GVs.CURRENT_USER_EDIT_MODEL = GVs.USER_LIST_MODEL.Get(GVs.CURRENT_USER_EDIT).Clone();
            if (GVs.USER_LIST_MODEL.userModels.Length == 1)
            {
                GVs.CURRENT_USER_MODEL = GVs.USER_LIST_MODEL.Get(GVs.CURRENT_USER_EDIT).Clone();
                GVs.CURRENT_USER_EDIT_MODEL = GVs.CURRENT_USER_MODEL;
            }
            GVs.PROFILE_STATE = GVs.PROFILE_EDIT;
            GVs.SCENE_MANAGER.StartEditProfileScene();
        }
        else
        {
            string s = go.transform.Find("id").gameObject.GetComponent<UILabel>().text;
            int i = int.Parse(s);
            GVs.CURRENT_USER_MODEL = GVs.USER_LIST_MODEL.Get(i);
            GFs.SaveUsers();
            GVs.SCENE_MANAGER.StartHomeScene();
        }
    }

    public void CreateUserOnClick()
    {
        GVs.PROFILE_STATE = GVs.PROFILE_ADD;
        GVs.SCENE_MANAGER.StartEditProfileScene();
    }

    public void FinishOnClick()
    {
        GVs.SCENE_MANAGER.StartHomeScene();
    }
}
