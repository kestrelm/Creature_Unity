// ------------------------------------------------------------------------------
//  Created by Kestrel Moon Studios. C# Engine. 
//  Copyright (c) 2015 Kestrel Moon Studios. All rights reserved.
// ------------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using CreatureModule;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CreatureRenderer))]
public class CreatureRendererInspector : Editor
{
	private SerializedProperty creature_asset;
	private SerializedProperty animation_choice_index;
	private SerializedProperty active_animation_name;
	private SerializedProperty should_loop;
	private SerializedProperty local_time_scale;
	private SerializedProperty region_offsets_z;
	private SerializedProperty counter_clockwise;
	
	void OnEnable () 
	{
		creature_asset = serializedObject.FindProperty("creature_asset");
		animation_choice_index = serializedObject.FindProperty("animation_choice_index");
		active_animation_name = serializedObject.FindProperty("active_animation_name");
		should_loop = serializedObject.FindProperty("should_loop");
		local_time_scale = serializedObject.FindProperty("local_time_scale");
		region_offsets_z = serializedObject.FindProperty ("region_offsets_z");
		counter_clockwise = serializedObject.FindProperty ("counter_clockwise");
	}

	void UpdateData()
	{
		CreatureRenderer creature_renderer = (CreatureRenderer)target;
		creature_renderer.creature_asset = (CreatureAsset)creature_asset.objectReferenceValue;
		creature_renderer.animation_choice_index = animation_choice_index.intValue;
		creature_renderer.should_loop = should_loop.boolValue;
		creature_renderer.local_time_scale = local_time_scale.floatValue;
		creature_renderer.region_offsets_z = region_offsets_z.floatValue;
		creature_renderer.counter_clockwise = counter_clockwise.boolValue;
		creature_renderer.InitData();
		creature_renderer.CreateRenderingData();
	}

	void updateTargetAnimation()
	{
		CreatureRenderer creature_renderer = (CreatureRenderer)target;
		CreatureAsset cur_asset = (CreatureAsset)creature_asset.objectReferenceValue;

		int i = 0;
		string set_name = null;
		foreach (string cur_name in cur_asset.creature_manager.animations.Keys) {
			if(i == animation_choice_index.intValue)
			{
				set_name = cur_name;
				break;
			}
			i++;
		}

		if (creature_renderer.active_animation_name.Equals (set_name) == false) {
			creature_renderer.SetActiveAnimation (set_name);
			active_animation_name.stringValue = creature_renderer.active_animation_name;
		}

	}

	override public void OnInspectorGUI () 
	{
		serializedObject.Update();
		
		EditorGUI.BeginChangeCheck();
		
		EditorGUILayout.PropertyField (creature_asset);
		EditorGUILayout.PropertyField (local_time_scale);
		EditorGUILayout.PropertyField (region_offsets_z);
		EditorGUILayout.PropertyField (counter_clockwise);

		bool did_change = EditorGUI.EndChangeCheck();

		// new assignment
		if ((creature_asset.objectReferenceValue != null) && did_change) 
		{
			UpdateData();
		}

		if(creature_asset.objectReferenceValue != null)
		{
			CreatureAsset cur_asset = (CreatureAsset)creature_asset.objectReferenceValue;
			// asset changed
			if(cur_asset.GetIsDirty()) {
				UpdateData();
				cur_asset.SetIsDirty(false);
			}

			// animations
			if(cur_asset.creature_manager != null)
			{
				string[] animation_names = new string[cur_asset.creature_manager.animations.Keys.Count];
				int i = 0;
				foreach(string cur_name in cur_asset.creature_manager.animations.Keys)
				{
					animation_names[i] = cur_name;
					i++;
				}

				animation_choice_index.intValue = EditorGUILayout.Popup("Animation:",
				                                               animation_choice_index.intValue, animation_names);

				should_loop.boolValue = EditorGUILayout.Toggle("Loop", should_loop.boolValue);

				if(!Application.isPlaying)
				{
					updateTargetAnimation();
				}
				serializedObject.ApplyModifiedProperties();
			}
		}


	}
}


