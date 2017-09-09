using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareFacebook : MonoBehaviour {
	void Awake ()
	{		
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void onlogin(){
		var perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log (aToken.UserId);
			var check = 0;
			foreach (string perm in aToken.Permissions) {
				Debug.Log (perm);
				if (perm.Equals ("publish_actions")) {
					check = 1;

				}
			}
			//CallFBLoginForPublish ();
			if (check == 1) {
				StartCoroutine (StartVideoUpload ());
			} else {
				CallFBLoginForPublish ();
			}
		} else {
			Debug.Log ("User cancelled login" + result.Error);
		}
	}

	private void CallFBLoginForPublish()
	{
		// It is generally good behavior to split asking for read and publish
		// permissions rather than ask for them all at once.
		//
		// In your own game, consider postponing this call until the moment
		// you actually need it.
		FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, callbackPublicLogin);
        //StartCoroutine(StartVideoUpload());
    }

	private void callbackPublicLogin(ILoginResult result){
		if (result == null)
		{
			Debug.Log("null");
			return;
		}

		// Some platforms return the empty string instead of null.
		if (!string.IsNullOrEmpty(result.Error))
		{
			Debug.Log("Error Response:\n" + result.Error);
		}
		else if (result.Cancelled)
		{
			Debug.Log("Cancelled Response:\n" + result.RawResult);
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			Debug.Log("OK Boy:\n" + result.RawResult);
			StartCoroutine (StartVideoUpload ());
		}	
	}

	private IEnumerator TakeScreenshot()
	{
		yield return new WaitForEndOfFrame();

		var width = Screen.width;
		var height = Screen.height;
		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		byte[] screenshot = tex.EncodeToPNG();

		var wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "InteractiveConsole.png");
		wwwForm.AddField("message", "herp derp.  I did a thing!  Did I do this right?");
		FB.API("me/photos", HttpMethod.POST, HandleResult, wwwForm);
	}

    public static string filePath;

	private IEnumerator StartVideoUpload()
	{
		yield return new WaitForEndOfFrame();
        var url = "file:///" + filePath;
        Debug.Log("url is: " + url);
        WWW www = new WWW(url);
		while(!www.isDone) {
			yield return null;
			Debug.Log("progress : "+www.progress);
		}
		Debug.Log("size : "+www.bytesDownloaded);
		var wwwForm = new WWWForm();
		wwwForm.AddBinaryData("file", www.bytes, "Video.MOV","multipart/form-data");
		wwwForm.AddField("title", "Hello World");
		//wwwForm.AddField("description", ":-) :-)");

		FB.API("me/videos", HttpMethod.POST, HandleResult, wwwForm);
	}

	void HandleResult(IResult result){
		if (result == null)
		{
			Debug.Log("null");
			return;
		}

		// Some platforms return the empty string instead of null.
		if (!string.IsNullOrEmpty(result.Error))
		{
			Debug.Log("Error Response here is:\n" + result.Error);
		}
		else if (result.Cancelled)
		{
			Debug.Log("Cancelled Response:\n" + result.RawResult);
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			Debug.Log("OK Boy:\n" + result.RawResult);
		}
	
	}
}
