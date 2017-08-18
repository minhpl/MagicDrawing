using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HomeScript : MonoBehaviour
{
    // Use this for initialization
    public GameObject[] btnControls;
    void Start()
    {


        // PlayerPrefs.DeleteAll();

        // new TrainSVM().doTrain();
        for (int i = 0; i < btnControls.Length; i++)
        {
            Vector3 v3 = btnControls[i].transform.localPosition;
            TweenPosition.Begin(btnControls[i], 0, new Vector3(v3.x + 1080 / 2, v3.y, v3.x));
            TweenAlpha.Begin(btnControls[i], 0, 0);
        }
        StartCoroutine(Run());
        // OpenAndroidGallery();
    }

    public void OpenAndroidGallery()
    {
        #region [ Intent intent = new Intent(); ]
        //instantiate the class Intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //instantiate the object Intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        #endregion [ Intent intent = new Intent(); ]
        #region [ intent.setAction(Intent.ACTION_VIEW); ]
        //call setAction setting ACTION_SEND as parameter
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_GET_CONTENT"));
        #endregion [ intent.setAction(Intent.ACTION_VIEW); ]
        //set the type of file
        intentObject.Call<AndroidJavaObject>("setType", "image/*");
        #region [ startActivity(intent); ]
        //instantiate the class UnityPlayer
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //instantiate the object currentActivity
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //call the activity with our Intent
        currentActivity.Call("startActivity", intentObject);
        Debug.Log("call--------------------------------: " + currentActivity.Get<string>("data"));
        #endregion [ startActivity(intent); ]
    }
    IEnumerator Run()
    {
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < btnControls.Length; i++)
        {
            btnControls[i].SetActive(true);
            Vector3 v3 = btnControls[i].transform.localPosition;
            TweenAlpha.Begin(btnControls[i], 0.1f, 1);
            TweenPosition.Begin(btnControls[i], 0.3f, new Vector3(v3.x - 1080 / 2, v3.y, v3.x));
            yield return new WaitForSeconds(0.05f);
        }
    }

}

