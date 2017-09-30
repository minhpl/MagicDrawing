using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUpdateAppScript : MonoBehaviour
{
    public GameObject goUpdatePopup;
    public UIPlayTween[] updatePlayTween;
    private OtherAppModel oam;
    // Use this for initialization
    void Start()
    {
        if (!NET.NetWorkIsAvaiable()) return;
        HTTPRequest.Instance.Request(GVs.CHECK_UPDATE_APP_URL, JsonUtility.ToJson(new ReqModel()), (data) =>
        {
            Debug.Log(data);
            ResModel resModel = JsonUtility.FromJson<ResModel>(data);
            if (resModel.success == 1)
            {
                GFs.LoadOtherAppList();
                oam = GVs.OTHER_APP_LIST_MODEL.GetMyApp();
                if (oam != null)
                {
                    goUpdatePopup.SetActive(true);
                    GFs.PlayTweens(updatePlayTween, true);
                }

            }
        });
    }
    public void UpdateAppOnClick()
    {
        if (oam == null) return;
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + oam.packageName);
#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/id" + oam.appStore);
#endif
        CancelAppOnClick();
    }

    public void CancelAppOnClick()
    {
        GFs.PlayTweens(updatePlayTween, false, () =>
        {
            goUpdatePopup.SetActive(false);
        });
    }
}
