using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonButtonSound : MonoBehaviour {
    public AudioSource audioSource { get { return GetComponent<AudioSource>(); } }
    public static SingletonButtonSound Instance;

    void Awake () {
        Debug.Log(this.gameObject.name);
        this.InstantiateController();
        this.gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = Resources.Load("button") as AudioClip;
    }

    private void InstantiateController()
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
    }
}
