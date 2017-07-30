using OpenCVForUnity;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ResultScripts : MonoBehaviour {

    public static string videoname = null;
    VideoCapture cap;
    bool isPlayable = false;
    bool isPlaying = true;
    public RawImage rimg;

	void Start () {

        
        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }

    Texture2D texture;
    Color32[] buffer;


    IEnumerator Worker()
    {
        isPlaying = true;

        isPlayable = true;
        if (videoname == null) isPlayable = false;

        cap = new VideoCapture(videoname);
        cap.open(videoname);

        Utilities.Log("video name is {0}", videoname);

        if (!cap.isOpened())
        {
            Utilities.Log("Xin chao sai lam");
            isPlayable = false;
        }

        if (isPlayable)
        {
            Mat frame = new Mat();

            for (;;)
            {
                yield return null;
                cap.read(frame);                
                if (frame.empty())
                {
                    Utilities.Log("Xin chao sai lam blank frame");
                    break;
                }

                if (texture == null) { 
                    texture = new Texture2D(frame.width(), frame.height(), TextureFormat.RGBA32, false);
                    buffer = new Color32[frame.width() * frame.height()];
                }

                Utils.matToTexture2D(frame, texture,buffer);
                rimg.texture = texture;
                //Utilities.Log("Mat width is {0}, height is {1}", frame.width(), frame.height());
            }
        }
        isPlaying = false;
        //isPlayable = true;
        cap.release();
        yield return null;
    }


    public void OnPlayBtnClicked()
    {
        if (isPlaying) return;
        MainThreadDispatcher.StartUpdateMicroCoroutine(Worker());
    }

}
