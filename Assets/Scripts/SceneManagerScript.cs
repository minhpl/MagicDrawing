using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript  {

	public void loadHomeScne()
	{
		SceneManager.LoadScene("HomeScene");
	}

    public void loadMasterpieceCreationScene(){
    	SceneManager.LoadScene("MasterpieceCreationScene");	
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

    public void loadGalleryScene()
    {
        SceneManager.LoadScene("GalleryScene");
    }

    public void loadSplashScene(){
    	SceneManager.LoadScene("SplashScene");	
    }

    public void loadTutorialScene(){
    	SceneManager.LoadScene("Tutorial");
    }

    public void loadCollectionScene()
    {
        SceneManager.LoadScene("CollectionScene");
    }

    public void loadDrawingScene()
    {
        SceneManager.LoadScene("DrawingScene");
    }
}
