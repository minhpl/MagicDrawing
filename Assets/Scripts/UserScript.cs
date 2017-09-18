using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScript : MonoBehaviour
{

    public UILabel lblUsername;
    public UITexture userAvata;
    public GameObject userItem;
    List<GameObject> users = new List<GameObject>();
    List<UIToggle> toggleUsers = new List<UIToggle>();

    public GameObject goUser;
    public GameObject goUserInfo;

    public UIInput uiUsername;
    public UILabel lblCode;

    public UIToggle togSoundSystem;
    public UIToggle togSoundBG;
    public GameObject deleteUserPopup;
    public GameObject deleteUserButton;
    public GameObject goAvataPanel;

    public UITexture avata;
    public Texture defaultAvata;
    public AvataScript avataScript;

    void Start()
    {
        defaultAvata = avata.mainTexture;
        avataScript.handle = (UITexture tex) =>
        {
            avata.mainTexture = tex.mainTexture;
        };
        InitUser();
        SetUser();
        lblCode.text = GVs.LICENSE_CODE;
        togSoundSystem.value = GVs.SOUND_SYSTEM == 1 ? true : false;
        togSoundBG.value = GVs.SOUND_BG == 1 ? true : false;

        togSoundBG.onChange.Add(new EventDelegate(() =>
        {
            if (togSoundBG.value == true)
            {
                AudioSource audioSource = null;
                if(SoundBackgroundSingletonScripts.Instance!=null)
                    audioSource =  SoundBackgroundSingletonScripts.Instance.audioSource;
                if (audioSource!=null && !audioSource.isPlaying)
                    audioSource.Play();
            }
            else
            {
                SoundBackgroundSingletonScripts.Instance.audioSource.Stop();
            }
        }));
    }
    private void InitUser()
    {
        for (int i = 0; i < users.Count; i++) Destroy(users[i]);
        users.Clear();
        toggleUsers.Clear();
        int count = GVs.USER_LIST_MODEL.Count();
        Vector3 v3 = userItem.transform.localPosition;
        // userItem.transform.Find("Toggle").GetComponent<UIToggle>().startsActive = true;
        v3.y += 252;
        for (int i = 0; i < count; i++)
        {
            v3.y -= 252;
            GameObject go = Instantiate(userItem) as GameObject;
            if (i > 0) userItem.transform.Find("line").gameObject.SetActive(true);
            else userItem.transform.Find("line").gameObject.SetActive(false);
            go.transform.parent = userItem.transform.parent.transform;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = v3;
            go.transform.Find("username").gameObject.GetComponent<UILabel>().text = GVs.USER_LIST_MODEL.Get(i).name;
            try
            {
                GFs.LoadPNG(GVs.USER_LIST_MODEL.Get(i).avata, go.transform.Find("avata").GetComponent<UITexture>());
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
            go.SetActive(true);
            // EventDelegate.Set(go.GetComponent<UIButton>().onClick, EditProfileOnCLick);
            go.transform.Find("option").Find("id").gameObject.GetComponent<UILabel>().text = i + "";
            if (GVs.CURRENT_USER_MODEL.id.Equals(GVs.USER_LIST_MODEL.Get(i).id))
            {
                go.transform.Find("Toggle").GetComponent<UIToggle>().startsActive = true;
            }
            toggleUsers.Add(go.transform.Find("Toggle").GetComponent<UIToggle>());
            UIEventListener.Get(go.transform.Find("option").gameObject).onClick += OpenUserInfoOnClick;
            users.Add(go);
        }
        // userItem.transform.parent.GetComponent<UIScrollView>().Scroll(-20);
        // userItem.transform.parent.GetComponent<UIScrollView>().Scroll(20);
    }

    private void SetUser()
    {
        lblUsername.text = GVs.CURRENT_USER_MODEL.name;
        if (GVs.CURRENT_USER_MODEL.avata == null || GVs.CURRENT_USER_MODEL.avata.Equals(""))
        {
            userAvata.mainTexture = defaultAvata;
        }
        else
        {
            GFs.LoadPNG(GVs.CURRENT_USER_MODEL.avata, userAvata);
        }
    }
    public void OpenUserInfoOnClick(GameObject go)
    {
        deleteUserButton.SetActive(true);
        if (GVs.USER_LIST_MODEL.Count() <= 1)
            deleteUserButton.SetActive(false);
        string s = go.transform.Find("id").gameObject.GetComponent<UILabel>().text;
        if (!s.Equals(""))
        {
            GVs.PROFILE_STATE = GVs.PROFILE_EDIT;
            int id = int.Parse(s);
            SetCurrentUser(id);
            OpenUserInfor(id);
        }
        else
        {
            OpenUserInfor();
        }
    }

    private void SetCurrentUser(int id)
    {
        GVs.CURRENT_USER_EDIT = id;
        GVs.CURRENT_USER_EDIT_MODEL = GVs.USER_LIST_MODEL.Get(GVs.CURRENT_USER_EDIT).Clone();
        if (GVs.USER_LIST_MODEL.Count() == 1)
        {
            GVs.CURRENT_USER_MODEL = GVs.USER_LIST_MODEL.Get(GVs.CURRENT_USER_EDIT).Clone();
            GVs.CURRENT_USER_EDIT_MODEL = GVs.CURRENT_USER_MODEL;
        }
    }

    public void OpenAddUserOnClick()
    {
        GVs.NEW_USER_MODEL = new UserModel("");
        GVs.PROFILE_STATE = GVs.PROFILE_ADD;
        deleteUserButton.SetActive(false);
        OpenUserInfor();
    }

    void OpenUserInfor(int id = -1)
    {
        TweenAlpha.Begin(goUserInfo, 0f, 0);
        goUserInfo.SetActive(true);
        TweenAlpha.Begin(goUserInfo, 0.3f, 1);
        TweenAlpha.Begin(goUser, 0.3f, 0);
        if (id >= 0)
        {
            if (GVs.USER_LIST_MODEL.Get(id).avata != null && !GVs.USER_LIST_MODEL.Get(id).avata.Equals(""))
            {
                avata.mainTexture = GFs.LoadPNG(GVs.USER_LIST_MODEL.Get(id).avata);
            }
            else
            {
                avata.mainTexture = defaultAvata;
            }
            uiUsername.value = GVs.USER_LIST_MODEL.Get(id).name;
        }
        else
        {
            avata.mainTexture = defaultAvata;
        }
    }

    public void CloseUserInforOnClick()
    {
        TweenAlpha.Begin(goUserInfo, 0.3f, 0);
        goUser.SetActive(true);
        TweenAlpha.Begin(goUser, 0.3f, 1);
        uiUsername.value = "";
    }

    public void OpenUserAvata()
    {
        TweenAlpha.Begin(goAvataPanel, 0f, 0);
        goAvataPanel.SetActive(true);
        TweenAlpha.Begin(goAvataPanel, 0.3f, 1);
    }
    public void CloseUserAvata()
    {
        TweenAlpha.Begin(goAvataPanel, 0.3f, 0);
    }


    public void SaveOnClick()
    {
        if (!uiUsername.value.Trim().Equals(""))
        {
            if (GVs.PROFILE_STATE == GVs.PROFILE_ADD)
            {

                GVs.NEW_USER_MODEL.name = uiUsername.value.Trim();
                GVs.USER_LIST_MODEL.Add(GVs.NEW_USER_MODEL);
                GVs.CURRENT_USER_MODEL = GVs.NEW_USER_MODEL;
                GFs.SaveUsers();
            }
            else
            {
                GVs.CURRENT_USER_EDIT_MODEL.name = uiUsername.value.Trim();
                GVs.USER_LIST_MODEL.Set(GVs.CURRENT_USER_EDIT_MODEL, GVs.CURRENT_USER_EDIT);
                if (GVs.CURRENT_USER_EDIT_MODEL.id.Equals(GVs.CURRENT_USER_MODEL.id))
                {
                    GVs.CURRENT_USER_MODEL = GVs.CURRENT_USER_EDIT_MODEL;
                }
                GFs.SaveUsers();
            }
            InitUser();
            SetUser();
        }
        GVs.NEW_USER_MODEL = null;
        CloseUserInforOnClick();
    }

    public void OnUserSelect()
    {
        for (int i = 0; i < toggleUsers.Count; i++)
        {
            if (toggleUsers[i].value)
            {
                GVs.CURRENT_USER_MODEL = GVs.USER_LIST_MODEL.Get(i);
                GFs.SaveUsers();
                SetUser();
            }
        }
    }

    public void DeleteUser()
    {
        deleteUserPopup.SetActive(true);
    }

    public void OnDeleteAllow()
    {
        bool val = GVs.CURRENT_USER_EDIT_MODEL.id.Equals(GVs.CURRENT_USER_MODEL.id);
        GVs.USER_LIST_MODEL.Delete(GVs.CURRENT_USER_EDIT);
        if (val) SetCurrentUser(0);
        GFs.SaveUsers();
        InitUser();
        InitUser();
        CloseUserInforOnClick();
        deleteUserPopup.SetActive(false);
    }

    public void OnDeleteDeny()
    {
        deleteUserPopup.SetActive(false);
    }

    public void SoundOnChange()
    {
        GVs.SOUND_SYSTEM = togSoundSystem.value ? 1 : 0;
        GVs.SOUND_BG = togSoundBG.value ? 1 : 0;
        GFs.SaveSoundConfig();
    }
}
