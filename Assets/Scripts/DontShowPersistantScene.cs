using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontShowPersistantScene : MonoBehaviour {
	
	void Start () {
        if (MakePersistentObject.Instance)
        {
            MakePersistentObject.Instance.gameObject.SetActive(false);
        }
    }
    
}
