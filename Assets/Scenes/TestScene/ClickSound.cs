using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ClickSound : MonoBehaviour
{
    public static AudioClip audioClip = null;        
    private static ResourceRequest resourceRequest = null;
    private AudioSource audioSource {get {return GetComponent<AudioSource>(); }}

    void Start()
    {
        if (audioClip == null)
        {
            audioClip = Resources.Load("button") as AudioClip;            
        }
        
        var uButton = GetComponent<Button>();
        if (uButton != null)
        {
            gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.playOnAwake = false;
            uButton.onClick.AddListener(() => audioSource.Play());
        }
        else
        {
            var nguiButton = GetComponent<UIButton>();
            if (nguiButton != null)
            {
                gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.playOnAwake = false;
                nguiButton.onClick.Add(new EventDelegate(() =>
                {
                    audioSource.Play();
                }));
            }
        }

    }  
}