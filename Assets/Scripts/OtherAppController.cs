using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherAppController : MonoBehaviour
{
    public GameObject btnRateMyApp;
    public GameObject item;
    void Start()
    {
        GVs.OTHER_APP_LIST_MODEL.Clean();
        float h = 1920 - Screen.height * 1080.0f / Screen.width;
        Vector3 lp = item.transform.parent.transform.localPosition;
        lp.y -= h / 2;
        item.transform.parent.transform.localPosition = lp;
        Vector3 v3 = item.transform.localPosition;
        for (int i = 0; i < GVs.OTHER_APP_LIST_MODEL.Count(); i++)
        {
            GameObject go = Instantiate(item);
            go.transform.parent = item.transform.parent.transform;
            go.transform.localScale = Vector3.one;
            v3.y -= 220 * i;
            go.transform.localPosition = v3;
            go.transform.Find("name").GetComponent<UILabel>().text = GVs.OTHER_APP_LIST_MODEL.Get(i).name;
            GFs.LoadPNG(GVs.OTHER_APP_LIST_MODEL.dir + "/" + GVs.OTHER_APP_LIST_MODEL.Get(i).image, go.GetComponent<UITexture>());
            go.name = GVs.OTHER_APP_LIST_MODEL.Get(i)._id;
            go.SetActive(true);
            UIEventListener.Get(go).onClick += GoStore;
        }
        item.transform.parent.transform.GetComponent<UIGrid>().Reposition();
        if (btnRateMyApp != null)
        {
            UIEventListener.Get(btnRateMyApp).onClick += RateMyApp;
        }
    }

    public void GoStore(GameObject go)
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + GVs.OTHER_APP_LIST_MODEL.Get(go.name).packageName);
#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/id" + GVs.OTHER_APP_LIST_MODEL.Get(go.name).appStore);
#endif
    }
    public void RateMyApp(GameObject go)
    {
        OtherAppModel oam = GVs.OTHER_APP_LIST_MODEL.GetMyApp();
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + oam.packageName);
#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/id" + oam.appStore);
#endif
    }
}
