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
        creature_asset.ResetState();

        TextAsset text_asset = (TextAsset)creatureJSON.objectReferenceValue;
        creature_asset.creatureJSON = text_asset;
        if (text_asset) {
			if (text_asset.text.Length > 0) {
				FillAnimationNames ();
			}
		}

		TextAsset compressed_text_asset = (TextAsset)compressedCreatureJSON.objectReferenceValue;
        creature_asset.compressedCreatureJSON = compressed_text_asset;
        if (compressed_text_asset) {
			if (compressed_text_asset.text.Length > 0) {
				FillAnimationNames ();
			} 
		}

		TextAsset flat_text_asset = (TextAsset)flatCreatureData.objectReferenceValue;
        creature_asset.flatCreatureData = flat_text_asset;
        if (flat_text_asset) {
			if (flat_text_asset.text.Length > 0) {
				FillAnimationNames ();
			} 
		}

		TextAsset meta_text_asset = (TextAsset)creatureMetaJSON.objectReferenceValue;
        creature_asset.creatureMetaJSON = meta_text_asset;
        if (meta_text_asset) {
			if (meta_text_asset.text.Length > 0) {
                creature_asset.creature_manager = null;
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
                EditorGUILayout.Space();
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
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Live Sync Options", EditorStyles.boldLabel);
            if (GUILayout.Button("Live Sync"))
            {
                RunLiveSync();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation State Options", EditorStyles.boldLabel);
        if (GUILayout.Button ("Build State Machine")) 
		{
			CreateStateMachine();
		}

        if(creature_asset.physics_assets.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bend Physics Chains", EditorStyles.boldLabel);

            foreach (var cur_chain in creature_asset.physics_assets)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                EditorGUILayout.LabelField("Motor Name:", GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField(cur_chain.motor_name, GUILayout.MaxWidth(100));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                EditorGUILayout.LabelField("Anim Clip:", GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField(cur_chain.anim_clip_name, GUILayout.MaxWidth(100));
                EditorGUILayout.EndHorizontal();

                bool chain_active = cur_chain.active;
                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                chain_active = EditorGUILayout.Toggle("Active:", chain_active);
                EditorGUILayout.EndHorizontal();
                cur_chain.active = chain_active;

                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                EditorGUILayout.LabelField("Num Bones:", GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField(cur_chain.num_bones.ToString(), GUILayout.MaxWidth(100));
                EditorGUILayout.EndHorizontal();

                float chain_stiffness = cur_chain.stiffness;
                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                EditorGUILayout.LabelField("Stiffness:", GUILayout.MaxWidth(100));
                chain_stiffness = EditorGUILayout.FloatField(chain_stiffness, GUILayout.MaxWidth(50));
                EditorGUILayout.EndHorizontal();
                cur_chain.stiffness = chain_stiffness;

                float chain_damping = cur_chain.damping;
                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                EditorGUILayout.LabelField("Damping:", GUILayout.MaxWidth(100));
                chain_damping = EditorGUILayout.FloatField(chain_damping, GUILayout.MaxWidth(50));
                EditorGUILayout.EndHorizontal();
                cur_chain.damping = chain_damping;
            }
        }

        if (creature_asset.skin_swap_names.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Skin Swaps", EditorStyles.boldLabel);
            foreach (var cur_swap_name in creature_asset.skin_swap_names)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                EditorGUILayout.LabelField(cur_swap_name, GUILayout.MaxWidth(100));
                EditorGUILayout.EndHorizontal();
            }
        }

        if(creature_asset.morph_poses.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Morph Target Poses", EditorStyles.boldLabel);
            foreach (var cur_pose_name in creature_asset.morph_poses)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                EditorGUILayout.LabelField(cur_pose_name, GUILayout.MaxWidth(100));
                EditorGUILayout.EndHorizontal();
            }
        }

        if(creature_asset.vertex_attachments.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Vertex Attachments", EditorStyles.boldLabel);
            foreach (var cur_attachment_name in creature_asset.vertex_attachments)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
                EditorGUILayout.LabelField(cur_attachment_name, GUILayout.MaxWidth(100));
                EditorGUILayout.EndHorizontal();
            }
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
