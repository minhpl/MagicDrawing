//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using UnityEngine.SceneManagement;

//public class SoundButtonClick : MonoBehaviour
//{
//    void Start()
//    {
//        if (SingletonButtonSound.Instance == null)
//            this.gameObject.AddComponent<SingletonButtonSound>();
        
        
//        var listGameObjects = new List<GameObject>();
//        var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

//        foreach (var rootGO in rootGameObjects)
//        {
//            var UIButtonArrays = rootGO.GetComponentsInChildren<UIButton>(true);
//            var ButtonArrays = rootGO.GetComponentsInChildren<Button>(true);

//            foreach (var uibtn in UIButtonArrays)
//            {
//                uibtn.onClick.Add(new EventDelegate(() =>
//                {
//                    SingletonButtonSound.Instance.audioSource.Play();
//                }));
//            }

//            foreach (var btn in ButtonArrays)
//            {
//                btn.onClick.AddListener(() =>
//                {
//                    SingletonButtonSound.Instance.audioSource.Play();
//                });
//            }
//        }

//    }
//}