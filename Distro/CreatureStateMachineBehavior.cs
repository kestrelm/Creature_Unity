using UnityEngine;
using System.Collections;

public class CreatureStateMachineBehavior : StateMachineBehaviour {
	public CreatureGameController game_controller;
	public string play_animation_name;
	public bool custom_frame_range;
	public bool do_blending = false;
	public int custom_start_frame, custom_end_frame;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
        if(game_controller == null)
        {
            // Try to grab it from parent component
            game_controller = animator.GetComponentInParent<CreatureGameController>();
        }

		var creature_renderer = game_controller.creature_renderer;
		if (custom_frame_range) {
			creature_renderer.creature_manager.GetAnimation(play_animation_name).start_time = custom_start_frame;
			creature_renderer.creature_manager.GetAnimation(play_animation_name).end_time = custom_end_frame;
		}

		if(!do_blending)
		{
			creature_renderer.SetActiveAnimation(play_animation_name);
		}
		else
		{
			creature_renderer.BlendToAnimation(play_animation_name);
		}
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
