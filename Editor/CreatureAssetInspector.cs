// ------------------------------------------------------------------------------
//  Created by Kestrel Moon Studios. C# Engine. 
//  Copyright (c) 2015 Kestrel Moon Studios. All rights reserved.
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using CreatureModule;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

public class LiveSync
{
    [DllImport("CreatureClientDLL")]
    public static extern bool creatureClient_startConnection();

    [DllImport("CreatureClientDLL")]
    public static extern bool creatureClient_stopConnection();

    [DllImport("CreatureClientDLL")]
    public static extern bool creatureClient_isConnected();

    [DllImport("CreatureClientDLL")]
    public static extern System.IntPtr creatureClient_retrieveRequestExportFilename([MarshalAs(UnmanagedType.LPStr)]string msg_type);
}

[CustomEditor(typeof(CreatureAsset))]
public class CreatureAssetInspector : Editor {
	private SerializedProperty creatureJSON;
	private SerializedProperty compressedCreatureJSON;
	private SerializedProperty flatCreatureData;
	private SerializedProperty creatureMetaJSON;
	private List<string> animation_names;

	[SerializeField]
	public DictionaryOfStringAndAnimation animation_clip_overrides;

	[SerializeField]
	public bool useCompressedAsset;

	CreatureAssetInspector()
	{
		animation_names = new List<string> ();
	}

	void OnEnable () 
	{
		creatureJSON = serializedObject.FindProperty("creatureJSON");
		compressedCreatureJSON = serializedObject.FindProperty("compressedCreatureJSON");
		flatCreatureData = serializedObject.FindProperty ("flatCreatureData");
		creatureMetaJSON = serializedObject.FindProperty("creatureMetaJSON");
	}

    string GetLiveSyncType()
    {
        CreatureAsset creature_asset = (CreatureAsset)target;

        if (!creature_asset.useFlatDataAsset)
        {
            TextAsset text_asset = (TextAsset)creatureJSON.objectReferenceValue;
            if (text_asset)
            {
                if (text_asset.text.Length > 0)
                {
                    return "REQUEST_JSON";
                }
            }
        }
        else
        {
            TextAsset flat_text_asset = (TextAsset)flatCreatureData.objectReferenceValue;
            if (flat_text_asset)
            {
                if (flat_text_asset.text.Length > 0)
                {
                    return "REQUEST_BYTES";
                }
            }
        }

        return "";
    }

	void UpdateData()
	{
		CreatureAsset creature_asset = (CreatureAsset)target;

		TextAsset text_asset = (TextAsset)creatureJSON.objectReferenceValue;
		if (text_asset) {
			if (text_asset.text.Length > 0) {
				creature_asset.ResetState ();
				creature_asset.creatureJSON = text_asset;
				FillAnimationNames ();
			}
		}

		TextAsset compressed_text_asset = (TextAsset)compressedCreatureJSON.objectReferenceValue;
		if (compressed_text_asset) {
			if (compressed_text_asset.text.Length > 0) {
				creature_asset.ResetState ();
				creature_asset.compressedCreatureJSON = compressed_text_asset;
				FillAnimationNames ();
			} 
		}

		TextAsset flat_text_asset = (TextAsset)flatCreatureData.objectReferenceValue;
		if (flat_text_asset) {
			if (flat_text_asset.text.Length > 0) {
				creature_asset.ResetState ();
				creature_asset.flatCreatureData = flat_text_asset;
				FillAnimationNames ();
			} 
		}

		TextAsset meta_text_asset = (TextAsset)creatureMetaJSON.objectReferenceValue;
		if (meta_text_asset) {
			if (meta_text_asset.text.Length > 0) {
				creature_asset.ResetState ();
				creature_asset.creatureMetaJSON = meta_text_asset;
			} 
		}

	}

	void FillAnimationNames()
	{
		CreatureAsset creature_asset = (CreatureAsset)target;
		CreatureManager creature_manager = creature_asset.GetCreatureManager ();
		if (creature_manager == null) {
			return;
		}

		Dictionary<string, CreatureModule.CreatureAnimation> all_animations
			= creature_manager.animations;
		
		animation_names.Clear();
		foreach (string cur_name in all_animations.Keys) {
			animation_names.Add(cur_name);
		}

		animation_clip_overrides = creature_asset.animation_clip_overides;
		if (animation_clip_overrides.Count == 0) {
			foreach (string cur_name in all_animations.Keys) {
				var cur_animation = all_animations[cur_name];
				animation_clip_overrides.Add(cur_name, 
				                             new CreatureAnimationAssetData((int)cur_animation.start_time, 
				                               								(int)cur_animation.end_time));
			}
		}
	}

	override public void OnInspectorGUI () 
	{
		CreatureAsset creature_asset = (CreatureAsset)target;

		serializedObject.Update();

		EditorGUI.BeginChangeCheck();

		EditorGUILayout.PropertyField (creatureJSON);
		EditorGUILayout.PropertyField (compressedCreatureJSON);
		EditorGUILayout.PropertyField (flatCreatureData);
		EditorGUILayout.PropertyField (creatureMetaJSON);

		bool did_change = EditorGUI.EndChangeCheck();

		creature_asset.useCompressedAsset = EditorGUILayout.Toggle ("Use Compressed Asset: ", creature_asset.useCompressedAsset);
		creature_asset.useFlatDataAsset = EditorGUILayout.Toggle ("Use Flat Data Asset: ", creature_asset.useFlatDataAsset);

		if ((creatureJSON.objectReferenceValue || compressedCreatureJSON.objectReferenceValue || flatCreatureData.objectReferenceValue) 
		    && did_change)
		{
			UpdateData();
		}

		if (creatureJSON.objectReferenceValue || compressedCreatureJSON.objectReferenceValue| flatCreatureData.objectReferenceValue)
		{
			FillAnimationNames();

			int i = 1;
			foreach (string cur_name in animation_names) {
				EditorGUILayout.LabelField ("Animation Clip #" + i.ToString () + ":", 
				                            cur_name);

				var cur_start_time = animation_clip_overrides[cur_name].start_frame;
				var cur_end_time = animation_clip_overrides[cur_name].end_frame;
				bool make_point_cache = animation_clip_overrides[cur_name].make_point_cache;
				var cache_approximation = animation_clip_overrides[cur_name].cache_approximation;

				EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
				EditorGUILayout.LabelField("Frame Range:", GUILayout.MaxWidth(100));
				int new_start_frame = EditorGUILayout.IntField(cur_start_time, GUILayout.MaxWidth(50));

				EditorGUILayout.LabelField("to", GUILayout.MaxWidth(20));
				int new_end_frame = EditorGUILayout.IntField(cur_end_time, GUILayout.MaxWidth(50));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginVertical(GUILayout.MaxHeight(50));
				make_point_cache = EditorGUILayout.Toggle("Point Cache:", make_point_cache);

				EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
				EditorGUILayout.LabelField("Cache Approximation:", GUILayout.MaxWidth(150));
				cache_approximation = EditorGUILayout.IntSlider(cache_approximation, 1, 10);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical();

				animation_clip_overrides[cur_name].start_frame = new_start_frame;
				animation_clip_overrides[cur_name].end_frame = new_end_frame;
				animation_clip_overrides[cur_name].make_point_cache = make_point_cache;
				animation_clip_overrides[cur_name].cache_approximation = cache_approximation;

				i++;
			}
		}

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            EditorGUILayout.LabelField("Live Sync Options", GUILayout.MaxHeight(20));
            if (GUILayout.Button("Live Sync"))
            {
                RunLiveSync();
            }
        }

        EditorGUILayout.LabelField("Animation State Options", GUILayout.MaxHeight(20));
        if (GUILayout.Button ("Build State Machine")) 
		{
			CreateStateMachine();
		}

	}

    public void RunLiveSync()
    {
        CreatureAsset creature_asset = (CreatureAsset)target;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if(!LiveSync.creatureClient_isConnected())
            {
                var can_connect = LiveSync.creatureClient_startConnection();
                if(!can_connect)
                {
                    LiveSync.creatureClient_stopConnection();
                }
            }

            if(LiveSync.creatureClient_isConnected())
            {
                var sync_type = GetLiveSyncType();

                if (sync_type.Length > 0)
                {
                    System.IntPtr raw_ptr =
                        LiveSync.creatureClient_retrieveRequestExportFilename(sync_type);
                    string response_str = Marshal.PtrToStringAnsi(raw_ptr);
                    if (response_str.Length == 0)
                    {
                        UnityEditor.EditorUtility.DisplayDialog("Creature Live Sync Error", "Make sure Creature Pro is running in Animate Mode to Live Sync!", "Ok");
                    }
                    else
                    {
                        if(sync_type == "REQUEST_JSON")
                        {
                            string file_contents = System.IO.File.ReadAllText(response_str);
                            TextAsset text_asset = (TextAsset)creatureJSON.objectReferenceValue;
                            File.WriteAllText(AssetDatabase.GetAssetPath(text_asset), file_contents);
                            EditorUtility.SetDirty(text_asset);
                            creature_asset.creature_manager = null;
                            UpdateData();
                            creature_asset.SetIsDirty(true);
                        }
                        else if(sync_type == "REQUEST_BYTES")
                        {
                            byte[] file_contents = System.IO.File.ReadAllBytes(response_str);
                            TextAsset flat_text_asset = (TextAsset)flatCreatureData.objectReferenceValue;
                            File.WriteAllBytes(AssetDatabase.GetAssetPath(flat_text_asset), file_contents);
                            EditorUtility.SetDirty(flat_text_asset);
                            creature_asset.creature_manager = null;
                            UpdateData();
                            creature_asset.SetIsDirty(true);
                        }

                        AssetDatabase.Refresh();
                    }
                }
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog("Creature Live Sync Error", "Make sure Creature Pro is running in Animate Mode to Live Sync!", "Ok");
            }
        }
    }

	public void SaveCompressedFile()
	{
		var path = EditorUtility.SaveFilePanel(
			"Save data as compressed file",
			"",
			"characterCompressed.bytes",
			"bytes");

		if (path.Length != 0) {
			CreatureAsset creature_asset = (CreatureAsset)target;
			TextAsset text_asset = (TextAsset)creatureJSON.objectReferenceValue;
			string save_text = text_asset.text;
			creature_asset.SaveCompressedText(path, save_text);
		}

	}

	public void CreateStateMachine()
	{
		CreatureAsset creature_asset = (CreatureAsset)target;
		string create_name = "Assets/" + creature_asset.name +  "_StateMachineTransitions.controller";
		// Creates the controller
		var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath (create_name);
		var rootStateMachine = controller.layers[0].stateMachine;

		// Add states
		CreatureManager creature_manager = creature_asset.GetCreatureManager ();
		Dictionary<string, CreatureModule.CreatureAnimation> all_animations
			= creature_manager.animations;
		
		animation_names.Clear();
		foreach (string cur_name in all_animations.Keys) {
			var new_state = rootStateMachine.AddState(cur_name);

			new_state.AddStateMachineBehaviour<CreatureStateMachineBehavior>();
			CreatureStateMachineBehavior cur_behavior = (CreatureStateMachineBehavior)new_state.behaviours[0];
			cur_behavior.play_animation_name = cur_name;
		}

	}
}
