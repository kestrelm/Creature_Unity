using UnityEngine;
using UnityEditor;
using CreaturePackModule;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CreaturePackAsset))]
public class CreaturePackAssetInspector : Editor
{
    private SerializedProperty creaturePackBytes;

    public CreaturePackAssetInspector()
    {

    }

    public void OnEnable()
    {
        creaturePackBytes = serializedObject.FindProperty("creaturePackBytes");
    }

    public void UpdateDate()
    {
        CreaturePackAsset packAsset = (CreaturePackAsset)target;
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

    override public void OnInspectorGUI()
    {
        CreaturePackAsset packAsset = (CreaturePackAsset)target;
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(creaturePackBytes);

        bool did_change = EditorGUI.EndChangeCheck();

        if(creaturePackBytes.objectReferenceValue)
        {
            if(did_change)
            {
                UpdateDate();
            }

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

            if (GUILayout.Button("Build State Machine"))
            {
                CreateStateMachine();
            }
        }

    }

    public void CreateStateMachine()
    {
        CreaturePackAsset pack_asset = (CreaturePackAsset)target;
        string create_name = "Assets/" + pack_asset.name + "_StateMachineTransitions.controller";
        // Creates the controller
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(create_name);
        var rootStateMachine = controller.layers[0].stateMachine;

        // Add states
        var all_animations
            = pack_asset.GetCreaturePackLoader().animClipMap;

        foreach (string cur_name in all_animations.Keys)
        {
            var new_state = rootStateMachine.AddState(cur_name);

            new_state.AddStateMachineBehaviour<CreaturePackStateMachineBehavior>();
            CreaturePackStateMachineBehavior cur_behavior = (CreaturePackStateMachineBehavior)new_state.behaviours[0];
            cur_behavior.play_animation_name = cur_name;
        }

    }
}
