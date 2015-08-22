/******************************************************************************
 * Creature Runtimes License
 * 
 * Copyright (c) 2015, Kestrel Moon Studios
 * All rights reserved.
 * 
 * Preamble: This Agreement governs the relationship between Licensee and Kestrel Moon Studios(Hereinafter: Licensor).
 * This Agreement sets the terms, rights, restrictions and obligations on using [Creature Runtimes] (hereinafter: The Software) created and owned by Licensor,
 * as detailed herein:
 * License Grant: Licensor hereby grants Licensee a Sublicensable, Non-assignable & non-transferable, Commercial, Royalty free,
 * Including the rights to create but not distribute derivative works, Non-exclusive license, all with accordance with the terms set forth and
 * other legal restrictions set forth in 3rd party software used while running Software.
 * Limited: Licensee may use Software for the purpose of:
 * Running Software on Licensee’s Website[s] and Server[s];
 * Allowing 3rd Parties to run Software on Licensee’s Website[s] and Server[s];
 * Publishing Software’s output to Licensee and 3rd Parties;
 * Distribute verbatim copies of Software’s output (including compiled binaries);
 * Modify Software to suit Licensee’s needs and specifications.
 * Binary Restricted: Licensee may sublicense Software as a part of a larger work containing more than Software,
 * distributed solely in Object or Binary form under a personal, non-sublicensable, limited license. Such redistribution shall be limited to unlimited codebases.
 * Non Assignable & Non-Transferable: Licensee may not assign or transfer his rights and duties under this license.
 * Commercial, Royalty Free: Licensee may use Software for any purpose, including paid-services, without any royalties
 * Including the Right to Create Derivative Works: Licensee may create derivative works based on Software, 
 * including amending Software’s source code, modifying it, integrating it into a larger work or removing portions of Software, 
 * as long as no distribution of the derivative works is made
 * 
 * THE RUNTIMES IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE RUNTIMES OR THE USE OR OTHER DEALINGS IN THE
 * RUNTIMES.
 *****************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using CreatureModule;
using MeshBoneUtil;
using XnaGeometry;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreatureGameController : MonoBehaviour
{
	public CreatureRenderer creature_renderer;
	public List<CreatureSwitchItemRenderer> switch_renderer_list;
	public float colliderHeight;
	public float simColliderWidth, simColliderHeight;
	public bool noCollisions = false;
	public bool noGravity = false;
	public Rigidbody2D parentRBD;
	List<Rigidbody2D> child_bodies;
	List<MeshBone> runtime_bones;

	public CreatureGameAgent customAgent;

	private Animator animator; 

	// Put your own private gameplay variables here.
	/*
	private bool is_moving = false;
	private bool is_facing_left = false;
	*/

#if UNITY_EDITOR
	// Controller creation
	[MenuItem("GameObject/Creature/CreatureGameController")]
	static CreatureGameController CreateCreatureGameController()
	{
		GameObject newObj = new GameObject();
		newObj.name = "New Creature Game Controller";
		CreatureGameController new_controller;
		new_controller = newObj.AddComponent<CreatureGameController> () as CreatureGameController;
		new_controller.colliderHeight = 1.0f;
		new_controller.simColliderWidth = 10.0f;
		new_controller.simColliderHeight = 10.0f;
		new_controller.switch_renderer_list = new List<CreatureSwitchItemRenderer>();

		return new_controller;
	}
#endif

	public UnityEngine.Vector3 TransformToCreatureDir(UnityEngine.Vector3 dir_in)
	{
		return creature_renderer.transform.TransformDirection(dir_in);
	}

	public UnityEngine.Vector3 TransformToCreaturePt(UnityEngine.Vector3 pt_in)
	{
		return creature_renderer.transform.TransformPoint(pt_in);
	}

	void Awake ()
	{
		// Find a reference to the Animator component in Awake since it exists in the scene.
		animator = GetComponent <Animator> ();
		if (animator) {
			CreatureStateMachineBehavior[] all_behaviors = animator.GetBehaviours<CreatureStateMachineBehavior>();
			for(int i=0;i<all_behaviors.Length;i++)
			{
				all_behaviors[i].game_controller = this;
			}
		}
	}

	public MeshBone GetRootBone()
	{
		CreatureManager cur_manager = creature_renderer.creature_manager;
		
		CreatureModule.Creature cur_creature = cur_manager.target_creature;
		MeshRenderBoneComposition bone_composition = cur_creature.render_composition;
		MeshBone root_bone = bone_composition.root_bone;

		return root_bone;
	}

	public void RemoveGameColliders()
	{
		for(int i=transform.childCount-1;i>=0;i--){
			DestroyImmediate (transform.GetChild(i).gameObject);
		}

		var parent_obj = creature_renderer.gameObject;
		Rigidbody2D parent_rbd = parent_obj.GetComponent<Rigidbody2D> ();
		Collider2D parent_collider = parent_obj.GetComponent<Collider2D> ();

		if (parent_rbd) {
			DestroyImmediate (parent_rbd);
		}
		
		if (parent_collider) {
			DestroyImmediate (parent_collider);
		}
	}

	public void CreateGameColliders()
	{
		if (creature_renderer == null) {
			return;
		}

		if (transform.childCount > 0) 
		{
			Debug.Log ("Please remove all children game colliders before running CreateGameColliders!");
			return;
		}

		creature_renderer.InitData ();
		MeshBone root_bone = GetRootBone();

		// Create a base rigid body that actually participates in simulation for the creature_renderer
		var parent_obj = creature_renderer.gameObject;
		Rigidbody2D parent_rbd = parent_obj.AddComponent<Rigidbody2D> ();
		parentRBD = parent_rbd;
		parent_rbd.isKinematic = false;

		BoxCollider2D parent_collider = parent_obj.AddComponent<BoxCollider2D> ();
		parent_collider.size = new UnityEngine.Vector2(simColliderWidth, simColliderHeight);

		// Create rigid bodies for all the bones
		List<MeshBone> all_children = root_bone.getAllChildren();

		foreach (MeshBone cur_child in all_children) {
			// Create new GameObject
			GameObject new_obj = new GameObject(cur_child.key);

			// Add components
			XnaGeometry.Vector4 spawn_center = (cur_child.getWorldStartPt() + cur_child.getWorldEndPt()) * 0.5f;
			float colliderWidth = (float)(cur_child.getWorldRestEndPt() - cur_child.getWorldRestStartPt()).Length();
			XnaGeometry.Vector4 spawn_dir = (cur_child.getWorldRestEndPt() - cur_child.getWorldRestStartPt());
			spawn_dir.Normalize();

			// Get the direction in the renderer's space
			UnityEngine.Vector3 u_spawn_dir = new UnityEngine.Vector3((float)spawn_dir.X, (float)spawn_dir.Y, (float)0);
			u_spawn_dir = TransformToCreatureDir(u_spawn_dir);

			float startAngle = (float)Math.Atan2((double)u_spawn_dir.y, (double)u_spawn_dir.x) * Mathf.Rad2Deg;
			UnityEngine.Vector3 local_pos = new UnityEngine.Vector3((float)spawn_center.X, (float)spawn_center.Y, (float)0);

			Rigidbody2D new_rbd = new_obj.AddComponent<Rigidbody2D>();
			new_rbd.isKinematic = true;
			BoxCollider2D new_collider = new_obj.AddComponent<BoxCollider2D>();
			new_collider.size = new UnityEngine.Vector2(colliderWidth, colliderHeight);

			// set the position using the renderer's parent transform
			new_obj.transform.position = TransformToCreaturePt(local_pos);

			new_obj.transform.rotation = UnityEngine.Quaternion.AngleAxis(startAngle, new UnityEngine.Vector3(0,0,1));

			// Make new object child of the parent
			new_obj.transform.parent = gameObject.transform;
		}
		
	}
	private void InitControllers()
	{
		if (creature_renderer == null) {
			return;
		}
		
		MeshBone root_bone = GetRootBone();
		
		runtime_bones = root_bone.getAllChildren();
		child_bodies = new List<Rigidbody2D> ();
		List<Collider2D> bone_colliders = new List<Collider2D> ();
		var parent_obj = creature_renderer.gameObject;
		Collider2D parent_collider = parent_obj.GetComponent<Collider2D> ();

		if (noCollisions) {
			parent_collider.enabled = false;
		}

		foreach (Transform child in transform) {
			var cur_body = child.GetComponent<Rigidbody2D>();
			child_bodies.Add(cur_body);
			
			var cur_collider = child.GetComponent<Collider2D>();
			bone_colliders.Add(cur_collider);
			
			// Turn off collision between bone collider and parent collider
			Physics2D.IgnoreCollision(cur_collider, parent_collider);

			if (noCollisions) {
				cur_collider.enabled = false;
			}
		}
		
		// Turn off collisions between bone colliders
		foreach (Collider2D cur_collider1 in bone_colliders) {
			foreach (Collider2D cur_collider2 in bone_colliders) {
				if(cur_collider1 != cur_collider2) {
					Physics2D.IgnoreCollision(cur_collider1, cur_collider2);
				}
			}
		}
	}

	void Start() {
		// Call this to initialize the controller
		InitControllers();

		// Put your custom gameplay code here
		//creature_renderer.SetActiveAnimation ("standing");

		// The following code demonstrates how to do item switching
		// First we prepare the list of items from the switch atlas and add it into the switcher object
		// If you have multiple items, you will add them all in i.e AddSwitchItem("Item1"...); ... AddSwitchItem("ItemX"...);
		// var cur_switcher = switch_renderer_list [0];
		// cur_switcher.AddSwitchItem ("torsoSwitch1", new UnityEngine.Vector2 (136, 4), 481, 569, 2048, 2048);

		// Call a custom agent if available
		if (customAgent) 
		{
			customAgent.initState();
		}
	}

	private void UpdateGameColliders()
	{
		int i = 0;
		bool is_dir_flipped = false;
		const double flip_cutoff = 10.0;
		if (Math.Abs (creature_renderer.transform.rotation.eulerAngles.y) > flip_cutoff) {
			is_dir_flipped = true;
		}

		if (parentRBD) {
			parentRBD.isKinematic = noGravity;
		}
		
		foreach (var cur_body in child_bodies) {
			var cur_bone = runtime_bones[i];
			XnaGeometry.Vector4 spawn_center = (cur_bone.getWorldStartPt() + cur_bone.getWorldEndPt()) * 0.5f;
			XnaGeometry.Vector4 spawn_dir = (cur_bone.getWorldEndPt() - cur_bone.getWorldStartPt());
			spawn_dir.Normalize();
			
			// Get the direction in the renderer's space
			UnityEngine.Vector3 u_spawn_dir = new UnityEngine.Vector3((float)spawn_dir.X, (float)spawn_dir.Y, (float)0);
			
			//u_spawn_dir = TransformToCreatureDir(u_spawn_dir);
			float setAngle = -(float)Math.Atan2((double)u_spawn_dir.y, (double)u_spawn_dir.x) * Mathf.Rad2Deg;

			/*
			if(is_dir_flipped)
			{
				setAngle = -setAngle;
			}
			else {
				setAngle = -setAngle;
			}
			*/

			cur_body.MoveRotation(setAngle);
			
			UnityEngine.Vector3 local_pos = new UnityEngine.Vector3((float)spawn_center.X, (float)spawn_center.Y, (float)0);
			
			cur_body.MovePosition(TransformToCreaturePt(local_pos));
			
			i++;
		}
	}

	// Put your gameplay code here
	void FixedUpdate() {
		// Call this to make the bone colliders follow the animation in a kinematic fashion
		UpdateGameColliders();

		// Call a custom agent if available
		if (customAgent) 
		{
			customAgent.updateStep();
		}

		// The following code demonstrates how to do item switching
		/*
		CreatureManager cur_manager = creature_renderer.creature_manager;
		// First hide the region we want to override
		cur_manager.SetOverrideRegionAlpha ("torso", 0.0f);
		// Now switch
		var cur_switcher = switch_renderer_list [0];
		cur_switcher.SetTargetSwitchItem ("torsoSwitch1");
		*/

		// The following code sets an optional bone override callback for modifying bone positions
		/*
		CreatureManager cur_manager = creature_renderer.creature_manager;
		cur_manager.bones_override_callback = UpdateBonesToCustomPositions;
		*/

		// The following code is an example of how you could handle the 
		// control/behaviour of a character using this runtime
		/*
		bool left_down = Input.GetKeyDown (KeyCode.A);
		bool right_down = Input.GetKeyDown (KeyCode.D);
		bool left_up = Input.GetKeyUp (KeyCode.A);
		bool right_up = Input.GetKeyUp (KeyCode.D);
		bool attack_down = Input.GetKeyDown (KeyCode.R);
		bool attack_up = Input.GetKeyUp (KeyCode.R);
		var parent_obj = creature_renderer.gameObject;
		Rigidbody2D parent_rbd = parent_obj.GetComponent<Rigidbody2D> ();

		if (is_moving) {
			if(is_moving)
			{
				float move_vel_x = 0;
				if(is_facing_left)
				{
					move_vel_x = -5;
				}
				else {
					move_vel_x = 5;
				}
				parent_rbd.velocity = new UnityEngine.Vector2(move_vel_x, 0);
				creature_renderer.SetActiveAnimation ("default", true);
			}

			if(left_up || right_up) {
				is_moving = false;
				var cur_vel = parent_rbd.velocity;
				cur_vel.x = 0;
				parent_rbd.velocity = cur_vel;
				creature_renderer.SetActiveAnimation ("standing");
			}
		} 
		else {
			if (left_down) {
				is_moving = true;
				is_facing_left = true;
			} 
			else if (right_down) {
				is_moving = true;
				is_facing_left = false;
			}

			float facing_angle = 0;
			if(is_facing_left) {
				facing_angle = 180;
			}

			parent_obj.transform.rotation = UnityEngine.Quaternion.AngleAxis(facing_angle, new UnityEngine.Vector3(0, 1, 0));
		}


		if (!is_moving) {
			if(attack_down)
			{
				creature_renderer.SetActiveAnimation ("attack");
			}
			else if(attack_up)
			{
				creature_renderer.SetActiveAnimation ("standing");
			}
		} 
		*/
	}


	// This demonstrates the ability to override/modify bone positions 
	/*
	void UpdateBonesToCustomPositions(Dictionary<string, MeshBoneUtil.MeshBone> bones_map)
	{
		foreach(KeyValuePair<string, MeshBoneUtil.MeshBone> entry in bones_map)
		{
			var cur_bone = entry.Value;
			// displace each bone upwards by y as an example
			var cur_world_start_pos = cur_bone.getWorldStartPt();
			var cur_world_end_pos = cur_bone.getWorldEndPt();

			float displace_y = -20.0f;
			cur_world_start_pos.Y += displace_y;
			cur_world_end_pos.Y += displace_y;

			cur_bone.setWorldStartPt(cur_world_start_pos);
			cur_bone.setWorldEndPt(cur_world_end_pos);
		}

		Debug.Log ("Custom Update");
	}
	*/
}