using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButtonSingletonScript : MonoBehaviour
{
    public static SoundButtonSingletonScript Instance;
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
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != Instance)
        {
            Destroy(this.gameObject);
        }

        this.gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("button") as AudioClip;
        audioSource.playOnAwake = false;
    }
}
