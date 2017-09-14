using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvataScript : MonoBehaviour
{

    public UITexture[] avatas;
    // Use this for initialization
    void Start()
    {
        avatas = transform.GetComponentsInChildren<UITexture>();
        Debug.Log(GVs.AVATA_LIST_MODEL.Count());
        for (int i = 0; i < GVs.AVATA_LIST_MODEL.Count(); i++)
        {
            if (avatas.Length > i && avatas[i] != null)
            {
                GFs.LoadPNG(GVs.AVATA_LIST_MODEL.dir + '/' + GVs.AVATA_LIST_MODEL.Get(i).image, avatas[i]);
                avatas[i].transform.name = GVs.AVATA_LIST_MODEL.Get(i)._id;
                UIEventListener.Get(avatas[i].transform.gameObject).onClick += SelectAvataOnClick;
            }
        }
    }
    public delegate void Handle(UITexture texture);
    public Handle handle;
    public void SetHandle(Handle _handle)
    {
        this.handle = _handle;
    }
    public void SelectAvataOnClick(GameObject go)
    {
        string avata = "";
        for (int i = 0; i < avatas.Length; i++)
        {
            if (avatas[i].transform.name.Equals(go.transform.name))
            {
                if (handle != null) handle(avatas[i]);
                avata = GVs.AVATA_LIST_MODEL.dir + '/' + GVs.AVATA_LIST_MODEL.Get(i).image;
            }
        }
        Debug.Log(GVs.PROFILE_STATE);
        if (GVs.PROFILE_STATE == GVs.PROFILE_ADD)
        {
            GVs.NEW_USER_MODEL.avata = avata;
            CloseSelectAvata();
        }
        else if (GVs.PROFILE_STATE == GVs.PROFILE_EDIT)
        {
            GVs.CURRENT_USER_EDIT_MODEL.avata = avata;
            CloseSelectAvata();
        }
        else
        {
            GVs.CURRENT_USER_MODEL.avata = avata;
            GVs.USER_LIST_MODEL.Update(GVs.CURRENT_USER_MODEL);
            GFs.SaveUsers();
            CloseSelectAvata();
        }
    }

    public void CloseSelectAvata()
    {
        Debug.Log(JsonUtility.ToJson(GVs.USER_LIST_MODEL));
        TweenAlpha.Begin(transform.parent.gameObject, 0.3f, 0);
    }
}
