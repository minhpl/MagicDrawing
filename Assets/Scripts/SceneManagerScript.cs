using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void loadHomeScne()
	{
		SceneManager.LoadScene("HomeScene");
	}

    public void loadHistory1Scene(){
    	SceneManager.LoadScene("History1Scene");	
    }

    public void loadLibraryScene(){
    	SceneManager.LoadScene("LibraryScene");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
    }

    public void loadHistoryScene(){
    	SceneManager.LoadScene("HistoryScene");			
    }

    public void loadPreviewResultScene()
    {
    	SceneManager.LoadScene("PreviewResultScene");				
    }

    public void loadResultScene(){
    	SceneManager.LoadScene("ResultScene");					
    }

    public void loadSnapImageScene(){
    	SceneManager.LoadScene("SnapImageScene");					
    }

    public void loadSplashScene(){
    	SceneManager.LoadScene("SplashScene");	
    }

    public void loadTutorialScene(){
    	SceneManager.LoadScene("Tutorial");
    }
}
