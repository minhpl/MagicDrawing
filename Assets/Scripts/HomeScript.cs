using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HomeScript : MonoBehaviour
{
    // Use this for initialization
    public GameObject[] btnControls;
    void Start()
    {


        // PlayerPrefs.DeleteAll();

        // new TrainSVM().doTrain();

        for (int i = 0; i < btnControls.Length; i++)
        {
            Vector3 v3 = btnControls[i].transform.localPosition;
            TweenPosition.Begin(btnControls[i], 0, new Vector3(v3.x + 1080 / 2, v3.y, v3.x));
            TweenAlpha.Begin(btnControls[i], 0, 0);
        }
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < btnControls.Length; i++)
        {
            btnControls[i].SetActive(true);
            Vector3 v3 = btnControls[i].transform.localPosition;
            TweenAlpha.Begin(btnControls[i], 0.1f, 1);
            TweenPosition.Begin(btnControls[i], 0.3f, new Vector3(v3.x - 1080 / 2, v3.y, v3.x));
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void Train()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}

