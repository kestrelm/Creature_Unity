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
        }

        if(key_cooldown > 0)
        {
            key_cooldown--;
        }
    }

    public override void initState()
    {
        game_controller.CreateBendPhysics("run");
    }
}
