using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvataScript : MonoBehaviour
{

    public List<UITexture> avatas;
    // Use this for initialization
    void Start()
    {
        avatas = new List<UITexture>();
        avatas.Add(transform.GetComponentInChildren<UITexture>());
        for (int i = 0; i < 11; i++)
        {
            UITexture ui = Instantiate(avatas[0]) as UITexture;
            ui.transform.parent = transform;
            ui.transform.localScale = Vector3.one;
            avatas.Add(ui);
        }
        for (int i = 0; i < GVs.AVATA_LIST_MODEL.Count(); i++)
        {
            if (avatas.Count > i && avatas[i] != null)
            {
                GFs.LoadPNG(GVs.AVATA_LIST_MODEL.dir + '/' + GVs.AVATA_LIST_MODEL.Get(i).image, avatas[i]);
                avatas[i].transform.name = GVs.AVATA_LIST_MODEL.Get(i)._id;
                UIEventListener.Get(avatas[i].transform.gameObject).onClick += SelectAvataOnClick;
            }
        }
        ResetTween();
        TweenAlpha.Begin(transform.parent.gameObject, 0, 0);
        transform.parent.GetComponent<UIPanel>().depth = 10;
        //GetComponent<UIGrid>().Reposition();
    }

    public void PlayTween()
    {
        for (int i = 0; i < avatas.Count; i++)
        {
            PlayTweenAvata ptAvata = avatas[i].GetComponent<PlayTweenAvata>();
            ptAvata.SetDelay(0.03f * i);
            ptAvata.Play();
        }
    }

    public void ResetTween()
    {
        for (int i = 0; i < avatas.Count; i++)
        {
            PlayTweenAvata ptAvata = avatas[i].GetComponent<PlayTweenAvata>();
            ptAvata.Reset();
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
        for (int i = 0; i < avatas.Count; i++)
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
        ResetTween();
        TweenAlpha.Begin(transform.parent.gameObject, 0.3f, 0);
    }
}
