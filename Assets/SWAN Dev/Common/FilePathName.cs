using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FilePathName
{
	public enum SaveFormat
	{
		GIF = 0,
		JPG,
		PNG,
	}

	public string GetSaveDirectory()
	{
		//GIF store in Virtual Memory
		//Available path: Application.persistentDataPath, Application.temporaryCachePath, Application.dataPath
		//Do not allow sub-Folder under the path. If you need to view gif, you can filter the file names to include .gif only.
		#if UNITY_EDITOR
		return Application.dataPath; 
		#else
		return Application.persistentDataPath;
		#endif
	}

	public string GeFileNameWithoutExt()
	{
		return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
	}

	public string GetGifFileName()
	{
		string timestamp = GeFileNameWithoutExt();
		return "GIF_" + timestamp;
	}
	public string GetGifFullPath()
	{
		return GetSaveDirectory() + "/" + GetGifFileName() + ".gif";
	}
	public string GetDownloadedGifSaveFullPath()
	{
		return GetSaveDirectory() + "/" + GetGifFileName() + ".gif";
	}

	public string GetJpgFileName()
	{
		string timestamp = GeFileNameWithoutExt();
		return "JPG_" + timestamp;
	}
	public string GetJpgFullPath()
	{
		return GetSaveDirectory() + "/" + GetJpgFileName() + ".jpg";
	}

	public string GetPngFileName()
	{
		string timestamp = GeFileNameWithoutExt();
		return "PNG_" + timestamp;
	}
	public string GetPngFullPath()
	{
		return GetSaveDirectory() + "/" + GetPngFileName() + ".png";
	}

	public byte[] ReadFileToBytes(string fullPath)
	{
		return File.ReadAllBytes(fullPath);
	}

	public void WriteBytesToFile(string toFullpath, byte[] byteArray)
	{
		File.WriteAllBytes(toFullpath, byteArray);
	}

	public void FileStreamTo(string fullpath, byte[] byteArray)
	{
		using( FileStream fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write) )
		{
			fs.Write(byteArray, 0, byteArray.Length);
		}
	}

	public string SaveTextureAs(Texture2D texture2D, SaveFormat format = SaveFormat.JPG)
	{
		string savePath = string.Empty;
		switch(format)
		{
		case SaveFormat.JPG:
			savePath = GetJpgFullPath();
			WriteBytesToFile(savePath, texture2D.EncodeToJPG(90));
			break;
		case SaveFormat.PNG:
			savePath = GetPngFullPath();
			WriteBytesToFile(savePath, texture2D.EncodeToPNG());
			break;
		case SaveFormat.GIF:
			//savePath = ProGifTexture2DsToGIF.Instance.Save(new List<Texture2D>{texture2D}, texture2D.width, texture2D.height, 1, 0, 10);
			break;
		}
		return savePath;
	}

//	public string SaveTexturesAsGIF(List<Texture2D> textureList, int width, int height, int fps, int loop, int quality,
//		Action<int, string> onFileSaved = null, Action<int, float> onFileSaveProgress = null, 
//		ProGifTexture2DsToGIF.ResolutionHandle resolutionHandle = ProGifTexture2DsToGIF.ResolutionHandle.ResizeKeepRatio)
//	{
//		return ProGifTexture2DsToGIF.Instance.Save(textureList, width, height, fps, loop, quality, onFileSaved, onFileSaveProgress, resolutionHandle);
//	}

}
