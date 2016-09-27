using UnityEngine;
using System.Collections;

public class CreaturePackStateMachineBehavior : StateMachineBehaviour
{
    public CreaturePackRenderer pack_renderer;
    public string play_animation_name;
    public bool do_blending = false;
    public float blend_delta = 0.1f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var creature_renderer = pack_renderer;

        if (!do_blending)
        {
            creature_renderer.pack_player.setActiveAnimation(play_animation_name);
        }
        else
        {
            creature_renderer.pack_player.blendToAnimation(play_animation_name, blend_delta);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
