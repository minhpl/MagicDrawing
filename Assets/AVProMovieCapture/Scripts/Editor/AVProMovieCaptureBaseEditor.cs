using UnityEngine;
using UnityEditor;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2013 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[CustomEditor(typeof(AVProMovieCaptureBase))]
public class AVProMovieCaptureBaseEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GUI.color = Color.yellow;
		GUILayout.BeginVertical("box");
		GUILayout.TextArea("Error: This is not a component, this is the base class.\n\nPlease add one of the components, eg:\nAVProMovieCaptureFromScene / AVProMovieCaptureFromCamera etc");
		GUILayout.EndVertical();
	}	
}