using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBackgroundSingletonScripts : MonoBehaviour
{

    public static SoundBackgroundSingletonScripts Instance;
    public AudioSource audioSource
    {
        get
        {
            return GetComponent<AudioSource>();
        }
    }


    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        //if (GVs.SOUND_BG == 1)
        //{
            //audioSource.Play();
        //}
    }

}
