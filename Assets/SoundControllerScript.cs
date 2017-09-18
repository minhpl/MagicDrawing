using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControllerScript : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audio;
    private UIButton[] buttons;
    public static SoundControllerScript Instance
    {
        get;
        private set;
    }
    void Awake()
    {
        if (Instance != null)
        { 
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        Init();
    }
    public void Init()
    {
        StopAllCoroutines();
        StartCoroutine(InitButtonSound());
    }

    public IEnumerator InitButtonSound(float delay = 0.5f)
    {
        audio.clip = audioClip;
        yield return new WaitForSeconds(delay);
        buttons = FindObjectsOfType(typeof(UIButton)) as UIButton[];
        foreach (UIButton item in buttons)
        {
            UIPlaySound uIPlaySound = item.gameObject.GetComponent<UIPlaySound>();
            // if (uIPlaySound == null) uIPlaySound = item.gameObject.AddComponent<UIPlaySound>();
            item.onClick.Add(new EventDelegate(() =>
            {
                if (GVs.SOUND_SYSTEM == 1)
                {
                    audio.Play();
                }
            }));
        }
    }
}
