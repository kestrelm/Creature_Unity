using System;
using CreatureModule;
using UnityEngine;


public class PhysicsTestAgent : CreatureGameAgent
{
    public int key_cooldown = 0;

    PhysicsTestAgent()
        : base()
    {

    }

    public override void AnimationReachedEnd(string anim_name)
    {
        Debug.Log("Reached end of Animation Clip: " + anim_name);
    }

    public override void updateStep()
    {
        /*
        game_controller.creature_renderer.transform.position =
            game_controller.creature_renderer.transform.position + new Vector3(-0.1f, 0, 0);
        */

        if(Input.GetKey("f") && (key_cooldown == 0))
        {
            var cur_scale = game_controller.creature_renderer.transform.localScale;
            var new_scale = new Vector3(-cur_scale.x, cur_scale.y, cur_scale.z);
            game_controller.creature_renderer.transform.localScale = new_scale;

            game_controller.CreateBendPhysics("run");
            key_cooldown = 10;

            game_controller.creature_renderer.blend_rate = 0.1f;
            game_controller.creature_renderer.BlendToAnimation("default");
        }

        if(key_cooldown > 0)
        {
            key_cooldown--;
        }

        var bone_start = game_controller.GetBoneStartPt("Bone_6");
        var bone_end = game_controller.GetBoneEndPt("Bone_6");
        Debug.DrawLine(bone_start, bone_end, Color.yellow);
    }

    public override void initState()
    {
        game_controller.CreateBendPhysics("run");
    }
}
