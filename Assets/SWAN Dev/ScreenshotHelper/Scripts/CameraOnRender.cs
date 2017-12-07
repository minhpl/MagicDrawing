/// <summary>
/// By SwanDEV 2017
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraOnRender : MonoBehaviour
{
	[HideInInspector] public RenderTexture m_RenderTexture;

	[HideInInspector] public bool m_ToCapture = true;
	private Action<Texture2D> _onCaptureCallback;

	void Start()
	{
		m_RenderTexture = new RenderTexture(4, 4, 24);
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination);
		if(!m_ToCapture)
		{
			return;
		}
		m_ToCapture = false;

		if(m_RenderTexture.width != source.width || m_RenderTexture.height != source.height)
		{
			m_RenderTexture = new RenderTexture(source.width, source.height, 24);
		}
		Graphics.Blit(source, m_RenderTexture);

		if(_onCaptureCallback != null)
		{
			_onCaptureCallback(GetLastTexture2D());
			_onCaptureCallback = null;
		}
	}

	public void SetOneCapture(Action<Texture2D> onCaptured)
	{
		_onCaptureCallback = onCaptured;
		m_ToCapture = true;
	}

	public Texture2D GetLastTexture2D()
	{
		return _RenderTextureToTexture2D(m_RenderTexture);
	}

	private Texture2D _RenderTextureToTexture2D(RenderTexture source)
	{
		RenderTexture.active = source;
		Texture2D tex = new Texture2D(source.width, source.height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
		tex.Apply();
		RenderTexture.active = null;
		return tex;
	}

	public void Clear()
	{
		_onCaptureCallback = null;

		if(m_RenderTexture != null)
		{
			RenderTexture.Destroy(m_RenderTexture);
		}

		Destroy(this);
	}
}
