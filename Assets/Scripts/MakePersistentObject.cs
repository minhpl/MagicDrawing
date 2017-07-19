using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePersistentObject : MonoBehaviour {

    public static MakePersistentObject Instance;


    private void Awake()
    {
        this.InstantiateController();
    }

    private void Start()
    {
        //Debug.LogFormat("Xin chao dong chi");
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
            if (!Instance.gameObject.activeSelf) Instance.gameObject.SetActive(true);
            Destroy(this.gameObject);
        }
    }

}
