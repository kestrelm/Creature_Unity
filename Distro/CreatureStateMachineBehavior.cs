using UnityEngine;
using System.Collections;

public class CreatureStateMachineBehavior : StateMachineBehaviour {
	public CreatureGameController game_controller;
	public string play_animation_name;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		var creature_renderer = game_controller.creature_renderer;
		creature_renderer.BlendToAnimation(play_animation_name);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
