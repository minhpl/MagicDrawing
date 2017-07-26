using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MakePersistentObject : MonoBehaviour {

    public static MakePersistentObject Instance;


    private void Awake()
    {
        
    }

    private void Start()
    {
        this.InstantiateController();
        //Debug.LogFormat("Xin chao, Active Scene name is {0}", SceneManager.GetActiveScene().name);
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
