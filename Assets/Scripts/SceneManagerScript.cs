using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManagerScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartArchivementScene()
    {
        SceneManager.LoadScene("FinishTrain", LoadSceneMode.Single);
    }

    public void StartHomeScene()
    {
        SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
    }

    public void StartEditProfileScene()
    {
        SceneManager.LoadScene("EditProfileScene", LoadSceneMode.Single);
    }

    public void StartProfileScene()
    {
        SceneManager.LoadScene("ProfileScene", LoadSceneMode.Single);
    }


    public void StartAvataScene()
    {
        SceneManager.LoadScene("AvataScene", LoadSceneMode.Single);
    }
    public void GameBackScene()
    {
        if (GVs.GAME_MODE == GVs.SINGLE_MODE || GVs.GAME_MODE == GVs.GENERAL_MODE) SceneManager.LoadScene("CategoryScene", LoadSceneMode.Single);
        else SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
    }

    /* */
    public void StartMainModeScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        // StartCoroutine(LoadScene());
        // StartCoroutine(SwitchScene());
    }
    AsyncOperation async;
    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync("Main");
        async.allowSceneActivation = false;
        yield return async;
        Debug.Log("Loading complete");
    }

    IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(2f);
        if (async != null)
            async.allowSceneActivation = true;
    }

    public void loadHomeScene()
    {
        Debug.Log("heerererer");
        SceneManager.LoadScene("HomeScene");
    }

    public void loadMasterpieceCreationScene(){
    	SceneManager.LoadScene("MasterpieceCreationScene");	
    }

    public void loadLibraryScene(){
    	SceneManager.LoadScene("LibraryScene");
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

   	public void loadLibraryNGUIScene(){
    	SceneManager.LoadScene("LibraryNGUIScene");
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
    }

    public void loadHistoryScene(){
    	SceneManager.LoadScene("HistoryScene");			
    }

    public void loadHistoryNGUIScene()
    {
        SceneManager.LoadScene("HistoryNGUIScene");
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

    public void loadTemplateScene()
    {
        SceneManager.LoadScene("TemplateScene");
    }

    public void loadTemplateNGUIScene()
    {
        SceneManager.LoadScene("TemplateNGUIScene");
    }


}
