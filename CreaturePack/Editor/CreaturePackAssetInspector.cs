using UnityEngine;
using UnityEditor;
using CreaturePackModule;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CreaturePackAsset))]
public class CreaturePackAssetInspector : Editor
{
    private SerializedProperty creaturePackBytes;
    private SerializedProperty creatureMetaJSON;

    public CreaturePackAssetInspector()
    {

    }

    public void OnEnable()
    {
        creaturePackBytes = serializedObject.FindProperty("creaturePackBytes");
        creatureMetaJSON = serializedObject.FindProperty("creatureMetaJSON");
    }

    public void UpdateDate()
    {
        CreaturePackAsset packAsset = (CreaturePackAsset)target;

        {
            TextAsset bytesAsset = (TextAsset)creaturePackBytes.objectReferenceValue;
            if (bytesAsset)
            {
                if (bytesAsset.bytes.Length > 0)
                {
                    packAsset.ResetState();
                    packAsset.creaturePackBytes = bytesAsset;
                }
            }
        }

        {
            TextAsset jsonAsset = (TextAsset)creatureMetaJSON.objectReferenceValue;
            if (jsonAsset)
            {
                if (jsonAsset.bytes.Length > 0)
                {
                    packAsset.creatureMetaJSON = jsonAsset;
                    packAsset.LoadMetaData();
                }
            }
        }
    }

    override public void OnInspectorGUI()
    {
        CreaturePackAsset packAsset = (CreaturePackAsset)target;
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(creaturePackBytes);
        EditorGUILayout.PropertyField(creatureMetaJSON);

        bool did_change = EditorGUI.EndChangeCheck();

        if(creaturePackBytes.objectReferenceValue)
        {
            if(did_change)
            {
                UpdateDate();
            }

            EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel, GUILayout.MaxHeight(20));

            var loaderData = packAsset.GetCreaturePackLoader();
            foreach(var curClip in loaderData.animClipMap)
            {
                var curName = curClip.Key;
                var startFrame = curClip.Value.startTime;
                var endFrame = curClip.Value.endTime;
                var showString = curName + " (" + startFrame.ToString() + ", " + endFrame.ToString() + ")";

                EditorGUILayout.BeginVertical(GUILayout.MaxHeight(20));

                EditorGUILayout.LabelField("Animation Clip: ", showString,
                                    GUILayout.MaxHeight(20));


                EditorGUILayout.EndVertical();
            }

            if(packAsset.anchor_points != null)
            {
                EditorGUILayout.LabelField("Anchor Points", EditorStyles.boldLabel, GUILayout.MaxHeight(20));
                foreach (var anchor_data in packAsset.anchor_points)
                {
                    EditorGUILayout.BeginVertical(GUILayout.MaxHeight(20));
                    EditorGUILayout.LabelField(anchor_data.Key,
                                        GUILayout.MaxHeight(20));
                    EditorGUILayout.EndVertical();
                }
            }

            var metaData = packAsset.meta_data;
            if (metaData != null)
            {
                if (metaData.skin_swaps != null)
                {
                    EditorGUILayout.LabelField("Skin Swaps", EditorStyles.boldLabel, GUILayout.MaxHeight(20));
                    foreach (var swap_data in metaData.skin_swaps)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.MaxHeight(20));
                        EditorGUILayout.LabelField(swap_data.Key,
                                            GUILayout.MaxHeight(20));
                        EditorGUILayout.EndVertical();
                    }
                }

                if(packAsset.composite_player != null)
                {
                    EditorGUILayout.LabelField("Composite Clips", EditorStyles.boldLabel, GUILayout.MaxHeight(20));
                    foreach(var comp_data in packAsset.composite_player.composite_clips)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.MaxHeight(20));
                        EditorGUILayout.LabelField(comp_data.Key,
                                            GUILayout.MaxHeight(20));
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            if (GUILayout.Button("Build State Machine"))
            {
                CreateStateMachine();
            }
        }
    }

    private void AddBehaviorState(UnityEditor.Animations.AnimatorStateMachine rootStateMachine, string nameIn)
    {
        var new_state = rootStateMachine.AddState(nameIn);
        new_state.AddStateMachineBehaviour<CreaturePackStateMachineBehavior>();
        CreaturePackStateMachineBehavior cur_behavior = (CreaturePackStateMachineBehavior)new_state.behaviours[0];
        cur_behavior.play_animation_name = nameIn;
    }

    public void CreateStateMachine()
    {
        CreaturePackAsset pack_asset = (CreaturePackAsset)target;
        string create_name = "Assets/" + pack_asset.name + "_StateMachineTransitions.controller";
        // Creates the controller
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(create_name);
        var rootStateMachine = controller.layers[0].stateMachine;

        // Add states
        var all_animations = pack_asset.GetCreaturePackLoader().animClipMap;

        foreach (string cur_name in all_animations.Keys)
        {
            AddBehaviorState(rootStateMachine, cur_name);
        }


        var metaData = pack_asset.meta_data;
        if (metaData != null)
        {
            if (pack_asset.composite_player != null)
            {
                foreach (var cur_name in pack_asset.composite_player.composite_clips.Keys)
                {
                    AddBehaviorState(rootStateMachine, cur_name);
                }
            }
        }
    }
}
