using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvataScript : MonoBehaviour
{

    public GameObject[] avatas;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < GVs.AVATA_LIST_MODEL.Count(); i++)
        {
            if (avatas.Length > i && avatas[i] != null)
            {
                GFs.LoadPNG(GVs.AVATA_LIST_MODEL.dir + '/' + GVs.AVATA_LIST_MODEL.Get(i).image, avatas[i].GetComponent<UITexture>());
                avatas[i].SetActive(true);
                UIEventListener.Get(avatas[i]).onClick += SelectAvataOnClick;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectAvataOnClick(GameObject go)
    {
        string avata = "";
        for (int i = 0; i < avatas.Length; i++)
        {
            if (avatas[i].Equals(go))
            {
                avata = GVs.AVATA_LIST_MODEL.dir + '/' + GVs.AVATA_LIST_MODEL.Get(i).image;
            }
        }
        if (GVs.PROFILE_STATE == GVs.PROFILE_ADD)
        {
            GVs.NEW_USER_MODEL.avata = avata;
            GVs.SCENE_MANAGER.StartEditProfileScene();
        }
        else if (GVs.PROFILE_STATE == GVs.PROFILE_EDIT)
        {
            GVs.CURRENT_USER_EDIT_MODEL.avata = avata;
            GVs.SCENE_MANAGER.StartEditProfileScene();
        }
        else
        {
            GVs.CURRENT_USER_MODEL.avata = avata;
            GVs.USER_LIST_MODEL.Update(GVs.CURRENT_USER_MODEL);
            GFs.SaveUsers();
            GVs.SCENE_MANAGER.StartProfileScene();
        }
    }
}
