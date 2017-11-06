using CinemaMocap;
using CinemaMocap.Filters;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The main window for Cinema Mocap.
/// </summary>
public class CinemaMocapWindow : EditorWindow
{
    // Mocap profile
    private MocapProfile MocapProfile = null;
    private List<TypeLabelContextData> mocapProfiles = new List<TypeLabelContextData>();
    private int mocapProfileSelection = 0;

    // Output profile
    //private OutputProfile OutputProfile = null;
    private List<TypeLabelContextData> outputProfiles = new List<TypeLabelContextData>();
    private int outputProfileSelection = 0;
   
    // Mapping profile
    SkeletonJointsFilterClippedLegs filter1 = new SkeletonJointsFilterClippedLegs();
    bool useFilter = false;

    // Recording
    private TransformationType transformationType = TransformationType.TransRotLoc;

    private const string SOURCE_FILE = "Assets/Cinema Suite/Cinema Mocap/Resources/Cinema_Mocap_Humanoid.dae";
    private const string SOURCE_FILE_MATRIX = "Assets/Cinema Suite/Cinema Mocap/Resources/Cinema_Mocap_Humanoid_Matrix.dae";
    private const string FILE_DESTINATION = "Assets/Cinema Suite/Cinema Mocap/Animations//{0}.dae";

    private GameObject cinema_mocap_humanoid_prefab;
    private GameObject cinema_mocap_humanoid_instance;
    private NUIInputToRigMapper inputMapper;
    private ColladaRigData rigData;

    private DateTime previousTime;

    #region Language
    private const string TITLE = "Cinema Mocap";
    private const string MENU_ITEM = "Window/Cinema Suite/Cinema Mocap/Cinema Mocap %#m";
    private const string NAME_DUPLICATE_ERROR_MSG = "{0}.dae exists. Saving as {1}.dae...";
    private const string DEVICE_TRACKING = "Device";
    private const string ON = "ON";
    private const string OFF = "OFF";
    private const string VIEWER = "Viewer";

    private string fileName = "Animation";

    private const string NO_MOCAP_PROFILES_FOUND_MSG = "No Mocap Profiles were found. Cinema Mocap will not work properly.";
    private const string NO_OUTPUT_PROFILES_FOUND_MSG = "No Output Profiles were found. Cinema Mocap will not work properly.";

    #endregion

    #region UI
    private const float width = 300f;
    private const float gap = 4f;
    //private bool isAdvancedExposed = false;
    private int delaySelection = 0;

    private readonly GUIContent[] delays = { new GUIContent("0 Seconds"), new GUIContent("3 Seconds"), new GUIContent("5 Seconds"), new GUIContent("10 Seconds") };
    
    #endregion

    #region Enums

    /// <summary>
    /// The kinect profile to load.
    /// </summary>
    private enum KinectProfile
    {
        Kinect1,
        Kinect2
    }

    /// <summary>
    /// COLLADA encoding types (Advanced)
    /// </summary>
    private enum TransformationType
    {
        TransRotLoc,
        Matrix
    }

    #endregion

    /// <summary>
    /// Called when the window is opened.
    /// Initializes all variables and sets up the system.
    /// </summary>
    public void Awake()
    {
        base.title = TITLE;
        this.minSize = new Vector2(680, 400f);

        loadMocapProfiles();
        if (mocapProfiles.Count < 1)
        {
            Debug.LogError(NO_MOCAP_PROFILES_FOUND_MSG);
            return;
        }

        loadOutputProfiles();
        if (outputProfiles.Count < 1)
        {
            Debug.LogError(NO_OUTPUT_PROFILES_FOUND_MSG);
            return;
        }

        rigData = ColladaUtility.ReadRigData(SOURCE_FILE);
        inputMapper = new NUIInputToRigMapper(rigData);

        MocapProfile = Activator.CreateInstance(mocapProfiles[0].Type) as MocapProfile;
        MocapProfile.PoseCaptured += MocapProfile_PoseCaptured;
        MocapProfile.RigData = rigData;

        cinema_mocap_humanoid_prefab = Resources.Load("Cinema_Mocap_Humanoid") as GameObject;
        if (cinema_mocap_humanoid_prefab == null)
        {
            UnityEngine.Debug.LogError("Cinema_Mocap_Humanoid.dae is missing from the Resources folder. This item is required for the system.");
        }
	}

    public void OnEnable()
    {
        rigData = ColladaUtility.ReadRigData(SOURCE_FILE);
        loadMocapProfiles();
        loadOutputProfiles();
    }

    /// <summary>
    /// Load all of the mocap profiles found in the project.
    /// </summary>
    private void loadMocapProfiles()
    {
        mocapProfiles.Clear();

        List<Type> types = CinemaMocapHelper.GetMocapProfiles();
        foreach (Type t in types)
        {
            foreach (MocapProfileAttribute attribute in t.GetCustomAttributes(typeof(MocapProfileAttribute), true))
            {
                mocapProfiles.Add(new TypeLabelContextData(t, attribute.ProfileName));
            }
        }
    }

    /// <summary>
    /// Load all of the output profiles found in the project.
    /// </summary>
    private void loadOutputProfiles()
    {
        outputProfiles.Clear();

        List<Type> types = CinemaMocapHelper.GetOutputProfiles();
        foreach (Type t in types)
        {
            foreach (OutputProfileAttribute attribute in t.GetCustomAttributes(typeof(OutputProfileAttribute), true))
            {
                outputProfiles.Add(new TypeLabelContextData(t, attribute.Name));
            }
        }
    }

    /// <summary>
    /// Update the logic for the window.
    /// </summary>
    protected void Update()
    {
        if(MocapProfile != null)
        {
            MocapProfile.Update();

            if(MocapProfile.IsDeviceOn)
            {
                Repaint();
            }
        }
    }

    /// <summary>
    /// Draw the Window's contents
    /// </summary>
	protected void OnGUI()
    {
        // Organize layout
        Rect profileSelectionArea = new Rect(gap, gap, width, 24);

        // TODO: layout options
        Rect deviceSettingsArea = new Rect(gap, profileSelectionArea.height + profileSelectionArea.y + gap, width, (base.position.height / 2) - profileSelectionArea.height - (gap * 2) - 2);
        //Rect mappingSettingArea = new Rect(gap, deviceSettingsArea.height + deviceSettingsArea.y + gap, width, base.position.height / 3 - (gap + 2));

        Rect captureSettingArea = new Rect(gap, deviceSettingsArea.height + deviceSettingsArea.y + gap, width, base.position.height / 2 - (gap + 2));
        
        Rect displayArea = new Rect(width + (gap * 2), gap, base.position.width - (width + (gap*3)), base.position.height - (gap*2));

        // Draw device selection area
        GUILayout.BeginArea(profileSelectionArea, string.Empty, "box");
            profileSelection();
        GUILayout.EndArea();

        // Draw device settings area
        GUILayout.BeginArea(deviceSettingsArea, string.Empty, "box");
            MocapProfile.DrawDeviceSettings();
        GUILayout.EndArea();

        // Draw the mapping settings area
        //GUILayout.BeginArea(mappingSettingArea, string.Empty, "box");
        //    DrawMappingSettings();
        //GUILayout.EndArea();

        // Draw animation setting area
        GUILayout.BeginArea(captureSettingArea, string.Empty, "box");
            DrawAnimationSettings();
        GUILayout.EndArea();

        // Draw display area
        GUILayout.BeginArea(displayArea);
            MocapProfile.DrawDisplayArea(new Rect(0, 0, displayArea.width, displayArea.height));
        GUILayout.EndArea();
    }

    private void DrawMappingSettings()
    {
        useFilter = EditorGUILayout.Toggle("test", useFilter);
    }

    private void DrawAnimationSettings()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Skeleton"));
        GUIContent[] outputContent = new GUIContent[outputProfiles.Count];
        for (int i = 0; i < outputProfiles.Count; i++)
        {
            outputContent[i] = new GUIContent(outputProfiles[i].Label);
        }

        EditorGUILayout.Popup(outputProfileSelection, outputContent);
        if (GUILayout.Button("GEN", EditorStyles.miniButton))
        {
            createModelPreview();
        }
        EditorGUILayout.EndHorizontal();

        fileName = EditorGUILayout.TextField(new GUIContent("Filename"), fileName);
        
        //isAdvancedExposed = EditorGUILayout.Foldout(isAdvancedExposed, new GUIContent("Advanced"));
        //if (isAdvancedExposed)
        {
            transformationType = (TransformationType)EditorGUILayout.EnumPopup(new GUIContent("Transformation Type"), transformationType);
        }

        EditorGUILayout.Space();
        delaySelection = EditorGUILayout.Popup(new GUIContent("Start Delay"), delaySelection, delays);
        EditorGUI.BeginDisabledGroup(!MocapProfile.IsDeviceOn);
        if (GUILayout.Button(MocapProfile.RecordingState == RecordingState.NotRecording ? new GUIContent("Record") : new GUIContent("Stop")))
        {
            if (MocapProfile.RecordingState == RecordingState.NotRecording)
            {
                int delaySeconds = int.Parse(delays[delaySelection].text.Split(' ')[0]);
                MocapProfile.StartRecording(delaySeconds);
            }
            else
            {
                NUIHumanoidAnimation animation = MocapProfile.StopRecording();
                saveAnimation(animation);
            }
        }
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// Method for drawing and selecting the device profile.
    /// </summary>
    private void profileSelection()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent(DEVICE_TRACKING));

        bool isDeviceActive = (MocapProfile != null) && MocapProfile.IsDeviceOn;
        EditorGUI.BeginDisabledGroup(isDeviceActive);

        GUIContent[] content = new GUIContent[mocapProfiles.Count];
        for(int i = 0; i < mocapProfiles.Count; i++)
        {
            content[i] = new GUIContent(mocapProfiles[i].Label);
        }
        int tempSelection = EditorGUILayout.Popup(mocapProfileSelection, content);

        if (mocapProfileSelection != tempSelection || MocapProfile == null)
        {
            MocapProfile = Activator.CreateInstance(mocapProfiles[tempSelection].Type) as MocapProfile;
            MocapProfile.PoseCaptured += MocapProfile_PoseCaptured;
            MocapProfile.RigData = rigData;

            mocapProfileSelection = tempSelection;
        }

        EditorGUI.EndDisabledGroup();

        bool toggleOn = false;
        Color temp = GUI.color;
        if (MocapProfile.IsDeviceOn)
        {
            GUI.color = Color.green;
            toggleOn = GUILayout.Toggle(MocapProfile.IsDeviceOn, ON, EditorStyles.miniButton);
        }
        else
        {
            GUI.color = Color.red;
            toggleOn = GUILayout.Toggle(MocapProfile.IsDeviceOn, OFF, EditorStyles.miniButton);
        }
        GUI.color = temp;
        EditorGUILayout.EndHorizontal();

        if (toggleOn && !MocapProfile.IsDeviceOn)
        {
            bool result = MocapProfile.TurnOnDevice();

            previousTime = DateTime.Now;

            // TODO: write a routine to wait some time before checking.
            if(!result)
            {
                //Debug.LogWarning("Cinema Mocap: Device failed to turn on.");
            }
        }
        else if (!toggleOn && MocapProfile.IsDeviceOn)
        {
            MocapProfile.TurnOffDevice();
            if (cinema_mocap_humanoid_instance != null && inputMapper != null)
            {
                RealtimeHumanoidPosing poser = cinema_mocap_humanoid_instance.GetComponent<RealtimeHumanoidPosing>();
                poser.SetRotations(inputMapper.GetBaseRotations());
            }
        }

    }

    /// <summary>
    /// Perform cleanup on window close
    /// </summary>
    protected void OnDestroy()
    {
        if (cinema_mocap_humanoid_instance != null)
        {
            UnityEngine.Object.DestroyImmediate(cinema_mocap_humanoid_instance);
        }
        if(MocapProfile != null)
        {
            MocapProfile.Destroy();
        }
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// Once capturing is complete, write out the animation file.
    /// </summary>
    private void saveAnimation(NUIHumanoidAnimation animation) 
    {
        // Check if there is capture data
        if (animation == null)
        {
            UnityEngine.Debug.LogWarning("No capture data was found.");
            return;
        }

        // Reload the rig data and mapper if necessary
        if (rigData == null || inputMapper == null)
        {
            rigData = ColladaUtility.ReadRigData(SOURCE_FILE);
            inputMapper = new NUIInputToRigMapper(rigData);
        }

        // Map captured data to Collada data
        ColladaAnimationData data = inputMapper.GetColladaAnimation(animation);

        // Check filename
        string appendedFileName = string.Format("MoCapHumanoid@{0}", fileName);
        string newFileName = appendedFileName;
        if (System.IO.File.Exists(string.Format(FILE_DESTINATION, appendedFileName)))
        {
            newFileName = getNewFilename(appendedFileName);
            UnityEngine.Debug.LogWarning(string.Format(NAME_DUPLICATE_ERROR_MSG, appendedFileName, newFileName));
        }

        // Save
        if (transformationType == TransformationType.Matrix)
        {
            ColladaUtility.SaveAnimationData(data, SOURCE_FILE_MATRIX, string.Format(FILE_DESTINATION, newFileName), true);
        }
        else
        {
            ColladaUtility.SaveAnimationData(data, SOURCE_FILE, string.Format(FILE_DESTINATION, newFileName), false);
        }
        
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Create an instance of the mocap humanoid and have the model refect the NUI user's movement.
    /// </summary>
    private void createModelPreview()
    {
        if (cinema_mocap_humanoid_instance == null)
        {
            cinema_mocap_humanoid_instance = PrefabUtility.InstantiatePrefab(cinema_mocap_humanoid_prefab) as GameObject;
            cinema_mocap_humanoid_instance.AddComponent<RealtimeHumanoidPosing>();
        }
    }

    void MocapProfile_PoseCaptured(object sender, PoseCapturedEventArgs args)
    {
        // Pose the preview model
        if (inputMapper != null && cinema_mocap_humanoid_instance != null)
        {
            RealtimeHumanoidPosing poser = cinema_mocap_humanoid_instance.GetComponent<RealtimeHumanoidPosing>();

            double delta = (DateTime.Now - previousTime).TotalSeconds;
            previousTime = System.DateTime.Now;

            if(useFilter)
            {
                filter1.FilterSkeleton(args.Skeleton, (float)delta);
            }

            Quaternion[] rotations = inputMapper.GetRotations(args.Skeleton);
            Vector3 position = inputMapper.GetHipPosition(args.Skeleton);

            poser.SetRotations(rotations);
            poser.SetWorldPosition(position);
        }
    }

    /// <summary>
    /// If there is a name conflict. Iterate until we find a new name.
    /// </summary>
    /// <param name="fileName">The original filename</param>
    /// <returns>The new filename.</returns>
    private string getNewFilename(string fileName)
    {
        int i = 1;
        while (System.IO.File.Exists(string.Format("Assets/Cinema Suite/Cinema Mocap/Animations//{0}{1}.dae", fileName,i)))
        {
            i++;
        }
        return string.Format("{0}{1}", fileName, i);
    }

    /// <summary>
    /// Show the Cinema Mocap Window
    /// </summary>
    [MenuItem(MENU_ITEM, false, 20)]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CinemaMocapWindow));
    }

    /// <summary>
    /// A wrapper class for a mocap profile.
    /// </summary>
    private class TypeLabelContextData
    {
        public Type Type;
        public string Label;

        public TypeLabelContextData(Type type, string label)
        {
            Type = type;
            Label = label;
        }
    }
}
