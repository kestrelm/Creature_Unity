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

public class CreatureFrameCallback
{
	public CreatureFrameCallback()
	{
		resetCallback();
	}

	public void resetCallback()
	{
		triggered = false;
	}

	public bool tryTrigger(float frameIn)
	{
		if(triggered)
		{
			return false;
		}

        if ((int)Math.Round(frameIn) >= frame)
        {
			triggered = true;
			return true;
		}

		return false;
	}

	public string name;
	public string animClipName;
	public int frame = 0;
	public bool triggered = false;
}

public class CreatureIKPacket : MonoBehaviour
{
    public Transform ik_target;
    public bool ik_pos_angle = false;
    public String ik_bone1, ik_bone2;
    public List<MeshBone> carry_bones;
    public List<MeshBoneUtil.CTuple<XnaGeometry.Vector2, XnaGeometry.Vector2>> bones_basis;

#if UNITY_EDITOR
    [MenuItem("GameObject/Creature/CreatureIKPacket")]
    static CreatureIKPacket CreatePacket()
    {
        GameObject newObj = new GameObject();
        newObj.name = "New Creature IK Packet";
        CreatureIKPacket new_packet;
        new_packet = newObj.AddComponent<CreatureIKPacket>() as CreatureIKPacket;

        return new_packet;
    }
#endif

    XnaGeometry.Vector2 getFromBasis(
        XnaGeometry.Vector4 pt_in,
         XnaGeometry.Vector4 base_vec_u,
         XnaGeometry.Vector4 base_vec_v)
    {
        XnaGeometry.Vector2 basis = new XnaGeometry.Vector2(0, 0);
        basis.X = XnaGeometry.Vector4.Dot(pt_in, base_vec_u);
        basis.Y = XnaGeometry.Vector4.Dot(pt_in, base_vec_v);

        return basis;
    }

    public void poseCarryBones(MeshBone endeffector_bone)
    {
        int i = 0;
        foreach(var cur_bone in carry_bones)
        {
            var basis_pair = bones_basis[i];
            var tmp_vec = endeffector_bone.getWorldEndPt() - endeffector_bone.getWorldStartPt();
            var base_vec_u = new XnaGeometry.Vector2(tmp_vec.X, tmp_vec.Y);
            base_vec_u.Normalize();

            var base_vec_v = new XnaGeometry.Vector2(0, 0);
            base_vec_v.X = -base_vec_u.Y;
            base_vec_v.Y = base_vec_u.X;

            var set_startpt = new XnaGeometry.Vector4(0, 0, 0, 1);
            var set_endpt = new XnaGeometry.Vector4(0, 0, 0, 1);

            var calcVec = new XnaGeometry.Vector2(0, 0);
            calcVec = basis_pair.Item1.X * base_vec_u + basis_pair.Item1.Y * base_vec_v;

            set_startpt.X = calcVec.X;
            set_startpt.Y = calcVec.Y;
            set_startpt += endeffector_bone.getWorldStartPt();
            set_startpt.W = 1;

            calcVec = basis_pair.Item2.X * base_vec_u + basis_pair.Item2.Y * base_vec_v;
            set_endpt.X = calcVec.X;
            set_endpt.Y = calcVec.Y;
            set_endpt += endeffector_bone.getWorldStartPt();
            set_endpt.W = 1;

            cur_bone.setWorldStartPt(set_startpt);
            cur_bone.setWorldEndPt(set_endpt);

            i++;
        }
    }

    public void initCarryBones(MeshBone endeffector_bone)
    {
        if(bones_basis != null)
        {
            // Already bound
            return;
        }

        bones_basis = new List<MeshBoneUtil.CTuple<XnaGeometry.Vector2, XnaGeometry.Vector2>>();
        carry_bones = endeffector_bone.getAllChildren();
        carry_bones.RemoveAt(0); // Remove first end_effector bone, we do not want to carry that

        var base_vec_u = endeffector_bone.getWorldEndPt() - endeffector_bone.getWorldStartPt();
        base_vec_u.Normalize();

        var base_vec_v = new XnaGeometry.Vector4(0, 0, 0, 0);
        base_vec_v.X = -base_vec_u.Y;
        base_vec_v.Y = base_vec_u.X;

        foreach (var cur_bone in carry_bones)
        {
            var basis_start = getFromBasis(
                cur_bone.getWorldStartPt() - endeffector_bone.getWorldStartPt(),
                base_vec_u,
                base_vec_v);

            var basis_end = getFromBasis(
                cur_bone.getWorldEndPt() - endeffector_bone.getWorldStartPt(),
                base_vec_u,
                base_vec_v);

            var basis_pair =
                new MeshBoneUtil.CTuple<XnaGeometry.Vector2, XnaGeometry.Vector2>(basis_start, basis_end);

            bones_basis.Add(basis_pair);
        }
    }
}

public class CreatureGameController : MonoBehaviour
{
    public CreatureRenderer creature_renderer;
    public List<CreatureSwitchItemRenderer> switch_renderer_list;
    public delegate void eventTrigger(string event_name);
    public event eventTrigger OnEventTrigger;
    public List<CreatureFrameCallback> event_callbacks = new List<CreatureFrameCallback>();
    public CreaturePhysicsData bend_physics_data = null;
    public float colliderHeight;
    public float simColliderWidth, simColliderHeight;
    public bool noCollisions = false;
    public bool noGravity = false;
    public Rigidbody2D parentRBD;
    public GameObject physics_container = null;
    public CreatureAnimBonesBlend anim_bones_blend = null;
    List<Rigidbody2D> child_bodies;
    List<MeshBone> runtime_bones;
    public bool ik_on = false;
    public List<CreatureIKPacket> ik_packets;
    public bool morph_targets_active = false;
    public Transform morph_target_xform, morph_base_xform;
    public float morph_radius = 1.0f;

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
        new_controller = newObj.AddComponent<CreatureGameController>() as CreatureGameController;
        new_controller.colliderHeight = 1.0f;
        new_controller.simColliderWidth = 10.0f;
        new_controller.simColliderHeight = 10.0f;
        new_controller.switch_renderer_list = new List<CreatureSwitchItemRenderer>();
        new_controller.ik_packets = new List<CreatureIKPacket>();

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

    public UnityEngine.Vector3 TransformWorldPtToCreatureSpace(UnityEngine.Vector3 pt_in)
    {
        return creature_renderer.transform.InverseTransformPoint(pt_in);
    }

    // Returns the vertex attachment point transformed into world space
    // Make sure you have the meta data attached to the CreatureAsset
    public UnityEngine.Vector3 GetVertexAttachmentPoint(string name_in)
    {
        var cur_meta_data = creature_renderer.creature_asset.creature_meta_data;
        if(cur_meta_data == null)
        {
            return new UnityEngine.Vector3(0, 0, 0);
        }

        var char_space_pt = cur_meta_data.getVertexAttachment(name_in, creature_renderer.creature_manager.GetCreature());
        return TransformToCreaturePt(char_space_pt);
    }

    void Awake()
    {
        // Find a reference to the Animator component in Awake since it exists in the scene.
        animator = GetComponent<Animator>();
        if (animator) {
            CreatureStateMachineBehavior[] all_behaviors = animator.GetBehaviours<CreatureStateMachineBehavior>();
            for (int i = 0; i < all_behaviors.Length; i++)
            {
                all_behaviors[i].game_controller = this;
            }
        }
    }

    public UnityEngine.Vector3 GetBoneStartPt(string bone_name)
    {
        var bones_map = creature_renderer.feedback_bones;
        if (bones_map.ContainsKey(bone_name) == false)
        {
            Debug.LogWarning("Invalid Bone Name Requested for position: " + bone_name);
            return new UnityEngine.Vector3(0, 0, 0);
        }

        var read_pt = bones_map[bone_name].start_pt;
        return TransformToCreaturePt(new UnityEngine.Vector3((float)read_pt.X, (float)read_pt.Y, 0));
    }

    public UnityEngine.Vector3 GetBoneEndPt(string bone_name)
    {
        var bones_map = creature_renderer.feedback_bones;
        if (bones_map.ContainsKey(bone_name) == false)
        {
            Debug.LogWarning("Invalid Bone Name Requested for position: " + bone_name);
            return new UnityEngine.Vector3(0, 0, 0);
        }

        var read_pt = bones_map[bone_name].end_pt;

        return TransformToCreaturePt(new UnityEngine.Vector3((float)read_pt.X, (float)read_pt.Y, 0));
    }

    public void AnimationReachedEnd(string anim_name)
    {
        if(customAgent != null)
        {
            customAgent.AnimationReachedEnd(anim_name);
        }
    }

    public void CreateBendPhysics(string anim_clip)
    {
        if (bend_physics_data != null)
        {
            bend_physics_data.clearPhysicsChain();
            bend_physics_data = null;
        }

        CreatureManager cur_manager = creature_renderer.creature_manager;
        if(physics_container != null)
        {
            Destroy(physics_container);
        }

        cur_manager.RunAtTime(cur_manager.GetActiveAnimationStartTime());

        physics_container = new GameObject("CreaturePhysicsContainer");
        bend_physics_data =
            CreatureModule.CreaturePhysicsData.PhysicsUtil.CreateBendPhysicsChain(
                physics_container,
                creature_renderer.gameObject,
                cur_manager.GetCreature().render_composition,
                anim_clip,
                creature_renderer.creature_asset.physics_assets);
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
        for (int i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        var parent_obj = creature_renderer.gameObject;
        Rigidbody2D parent_rbd = parent_obj.GetComponent<Rigidbody2D>();
        Collider2D parent_collider = parent_obj.GetComponent<Collider2D>();

        if (parent_rbd) {
            DestroyImmediate(parent_rbd);
        }

        if (parent_collider) {
            DestroyImmediate(parent_collider);
        }
    }

    public void CreateGameColliders()
    {
        if (creature_renderer == null) {
            return;
        }

        if (transform.childCount > 0)
        {
            Debug.Log("Please remove all children game colliders before running CreateGameColliders!");
            return;
        }

        creature_renderer.InitData();
        MeshBone root_bone = GetRootBone();

        // Create a base rigid body that actually participates in simulation for the creature_renderer
        var parent_obj = creature_renderer.gameObject;
        Rigidbody2D parent_rbd = parent_obj.AddComponent<Rigidbody2D>();
        parentRBD = parent_rbd;
        parent_rbd.isKinematic = false;

        BoxCollider2D parent_collider = parent_obj.AddComponent<BoxCollider2D>();
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

            new_obj.transform.rotation = UnityEngine.Quaternion.AngleAxis(startAngle, new UnityEngine.Vector3(0, 0, 1));

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
        child_bodies = new List<Rigidbody2D>();
        List<Collider2D> bone_colliders = new List<Collider2D>();
        var parent_obj = creature_renderer.gameObject;
        Collider2D parent_collider = parent_obj.GetComponent<Collider2D>();

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
                if (cur_collider1 != cur_collider2) {
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

        if (creature_renderer)
        {
            creature_renderer.SetGameController(this);
        }

        BuildFrameCallbacks();
        ResetFrameCallbacks();

        // Call a custom agent if available
        if (customAgent)
        {
            customAgent.initState();
        }
    }

    private void BuildFrameCallbacks()
    {
        if (creature_renderer)
        {
            var meta_asset = creature_renderer.creature_asset.creature_meta_data;
            if (meta_asset != null)
            {
                foreach (var cur_data in meta_asset.anim_events_map)
                {
                    foreach (var cur_event in cur_data.Value)
                    {
                        var new_callback = new CreatureFrameCallback();
                        new_callback.animClipName = cur_data.Key;
                        new_callback.name = cur_event.Value;
                        new_callback.frame = cur_event.Key;

                        event_callbacks.Add(new_callback);
                    }
                }
            }
        }
    }

    private void ResetFrameCallbacks()
    {
        foreach (var frame_callback in event_callbacks)
        {
            frame_callback.resetCallback();
        }
    }

    public void AnimClipChangeEvent()
    {
        var meta_asset = creature_renderer.creature_asset.creature_meta_data;
        if (meta_asset != null)
        {
            ResetFrameCallbacks();
        }
    }

    public void AnimClipFrameResetEvent()
    {
        var meta_asset = creature_renderer.creature_asset.creature_meta_data;
        if (meta_asset != null)
        {
            ResetFrameCallbacks();
        }
    }

    private void ProcessFrameCallbacks()
    {
        var cur_runtime = creature_renderer.creature_manager.getActualRuntime();
        foreach (var frame_callback in event_callbacks)
        {
            if (frame_callback.animClipName == creature_renderer.creature_manager.active_animation_name)
            {
                var should_trigger = frame_callback.tryTrigger(cur_runtime);
                //Debug.Log(frame_callback.name + " " + frame_callback.frame.ToString());
                if (should_trigger && (OnEventTrigger != null))
                {
                    OnEventTrigger(frame_callback.name);
                }
            }
        }
    }

    private void UpdateGameColliders()
    {
        int i = 0;
        bool is_dir_flipped = false;
        const double flip_cutoff = 10.0;
        if (Math.Abs(creature_renderer.transform.rotation.eulerAngles.y) > flip_cutoff) {
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

            if (cur_body != null)
            {
                cur_body.MoveRotation(setAngle);
                UnityEngine.Vector3 local_pos = new UnityEngine.Vector3((float)spawn_center.X, (float)spawn_center.Y, (float)0);
                cur_body.MovePosition(TransformToCreaturePt(local_pos));
            }

            i++;
        }
    }

    public void computeMorphTargetsPt()
    {
        if((morph_targets_active == false) 
            || (morph_target_xform == null)
            || (morph_base_xform == null))
        {
            return;
        }

        var meta_asset = creature_renderer.creature_asset.creature_meta_data;
        if (meta_asset == null)
        {
            return;
        }

        var base_pt = morph_base_xform.position;
        var parent_xform = creature_renderer.transform;
        var char_base_pos = parent_xform.InverseTransformPoint(base_pt);
        var char_pt_pos = parent_xform.InverseTransformPoint(morph_target_xform.position);
        var radius = 1.0f / ((parent_xform.localScale.x + parent_xform.localScale.y) * 0.5f) * morph_radius;

        meta_asset.computeMorphWeightsWorld(
                new UnityEngine.Vector2(char_pt_pos.x, -char_pt_pos.y),
                new UnityEngine.Vector2(char_base_pos.x, -char_base_pos.y),
                radius
            );
    }

    private void MainUpdateBonesToCustomPositions(Dictionary<string, MeshBoneUtil.MeshBone> bones_map)
    {
        // Modify with available Bend Physics
        if(bend_physics_data != null)
        {
            var parent_xform = creature_renderer.transform;
            bend_physics_data.updateAllKinematicBones(parent_xform);
            bend_physics_data.updateBonePositions(parent_xform);
        }

        // Perform any special bones mixing blending if required
        if(anim_bones_blend != null)
        {
            anim_bones_blend.update(bones_map, creature_renderer.creature_manager);
        }

        // Perform 2 bone ik if required
        if(ik_on && (ik_packets.Count > 0))
        {
            foreach(var ik_packet in ik_packets)
            {
                UpdateBonesIKToCustomPositions(bones_map, ik_packet);
            }
        }

        // Run your own custom override
        UpdateBonesToCustomPositions(bones_map);
    }

    private void UpdateBonesIKToCustomPositions(
        Dictionary<string, MeshBoneUtil.MeshBone> bones_map,
        CreatureIKPacket ik_packet)
    {
        if(!bones_map.ContainsKey(ik_packet.ik_bone1) || !bones_map.ContainsKey(ik_packet.ik_bone2))
        {
            Debug.LogError("Error! IK Bones not set properly.");
            return;
        }

        // Transform from world space to character space
        var bone1 = bones_map[ik_packet.ik_bone1];
        var bone2 = bones_map[ik_packet.ik_bone2];
        var target_worldpos = ik_packet.ik_target.position;
        var target_charpos = TransformWorldPtToCreatureSpace(target_worldpos);
        var length1 = (bone1.getWorldStartPt() - bone1.getWorldEndPt()).Length();
        var length2 = (bone2.getWorldStartPt() - bone2.getWorldEndPt()).Length();
        var base_pt = bone1.getWorldStartPt();

        ik_packet.initCarryBones(bone2);

        double angle1 = 0, angle2 = 0;
        CalcIK_2D_TwoBoneAnalytic(
            out angle1, 
            out angle2, 
            ik_packet.ik_pos_angle, 
            length1, 
            length2, 
            target_charpos.x - base_pt.X, 
            target_charpos.y - base_pt.Y);
        var rotVec1 = RotateVec2D(new XnaGeometry.Vector4(length1, 0, 0, 1), angle1);
        var rotVec2 = RotateVec2D(new XnaGeometry.Vector4(length2, 0, 0, 1), angle2);
        rotVec2 = RotateVec2D(rotVec2, angle1);

        var bone1_startpt = base_pt;
        var bone1_endpt = bone1_startpt + rotVec1;
        var bone2_startpt = bone1_endpt;
        var bone2_endpt = base_pt + rotVec2;

        bone1.setWorldEndPt(bone1_endpt);

        bone2.setWorldStartPt(bone2_startpt);
        bone2.setWorldEndPt(bone2_endpt);

        // Pose carry bones
        ik_packet.poseCarryBones(bone2);
    }

    XnaGeometry.Vector4 RotateVec2D(XnaGeometry.Vector4 vec_in, double angle)
    {
        var ret_vec = new XnaGeometry.Vector4(0, 0, 0, 1);
        ret_vec.X = vec_in.X * Math.Cos(angle) - vec_in.Y * Math.Sin(angle);
        ret_vec.Y = vec_in.Y * Math.Cos(angle) + vec_in.X * Math.Sin(angle);

        return ret_vec;
    }

    // Put your gameplay code here
    void FixedUpdate() {
		// Call this to make the bone colliders follow the animation in a kinematic fashion
        if(creature_renderer)
        {
            UpdateGameColliders();
        }
        else
        {
            Debug.LogError("Creature Renderer not set for GameController!");
        }

        // Call a custom agent if available
        if (customAgent) 
		{
			customAgent.updateStep();
		}

		// Call event triggers if required
		if(creature_renderer)
		{
			var meta_asset = creature_renderer.creature_asset.creature_meta_data;
			if(meta_asset != null)
			{
				ProcessFrameCallbacks();
			}

            // Set main bone override callback
            if (creature_renderer.creature_manager.bones_override_callback == null)
            {
                creature_renderer.creature_manager.bones_override_callback =
                    MainUpdateBonesToCustomPositions;
            }
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
	void UpdateBonesToCustomPositions(Dictionary<string, MeshBoneUtil.MeshBone> bones_map)
	{
        /*
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
        */
    }

    /******************************************************************************
      Copyright (c) 2008-2009 Ryan Juckett
      http://www.ryanjuckett.com/
  
      This software is provided 'as-is', without any express or implied
      warranty. In no event will the authors be held liable for any damages
      arising from the use of this software.
  
      Permission is granted to anyone to use this software for any purpose,
      including commercial applications, and to alter it and redistribute it
      freely, subject to the following restrictions:
  
      1. The origin of this software must not be misrepresented; you must not
         claim that you wrote the original software. If you use this software
         in a product, an acknowledgment in the product documentation would be
         appreciated but is not required.
  
      2. Altered source versions must be plainly marked as such, and must not be
         misrepresented as being the original software.
  
      3. This notice may not be removed or altered from any source
         distribution.
    ******************************************************************************/
    ///***************************************************************************************
    /// CalcIK_2D_TwoBoneAnalytic
    /// Given a two bone chain located at the origin (bone1 is the parent of bone2), this
    /// function will compute the bone angles needed for the end of the chain to line up
    /// with a target position. If there is no valid solution, the angles will be set to
    /// get as close to the target as possible.
    ///  
    /// returns: True when a valid solution was found.
    ///***************************************************************************************
    public static bool CalcIK_2D_TwoBoneAnalytic
    (
        out double angle1,   // Angle of bone 1
        out double angle2,   // Angle of bone 2
        bool solvePosAngle2, // Solve for positive angle 2 instead of negative angle 2
        double length1,      // Length of bone 1. Assumed to be >= zero
        double length2,      // Length of bone 2. Assumed to be >= zero
        double targetX,      // Target x position for the bones to reach
        double targetY       // Target y position for the bones to reach
    )
    {
        Debug.Assert(length1 >= 0);
        Debug.Assert(length2 >= 0);

        const double epsilon = 0.0001; // used to prevent division by small numbers

        bool foundValidSolution = true;

        double targetDistSqr = (targetX * targetX + targetY * targetY);

        //===
        // Compute a new value for angle2 along with its cosine
        double sinAngle2;
        double cosAngle2;

        double cosAngle2_denom = 2 * length1 * length2;
        if (cosAngle2_denom > epsilon)
        {
            cosAngle2 = (targetDistSqr - length1 * length1 - length2 * length2)
                        / (cosAngle2_denom);

            // if our result is not in the legal cosine range, we can not find a
            // legal solution for the target
            if ((cosAngle2 < -1.0) || (cosAngle2 > 1.0))
                foundValidSolution = false;

            // clamp our value into range so we can calculate the best
            // solution when there are no valid ones
            cosAngle2 = Math.Max(-1, Math.Min(1, cosAngle2));

            // compute a new value for angle2
            angle2 = Math.Acos(cosAngle2);

            // adjust for the desired bend direction
            if (!solvePosAngle2)
                angle2 = -angle2;

            // compute the sine of our angle
            sinAngle2 = Math.Sin(angle2);
        }
        else
        {
            // At least one of the bones had a zero length. This means our
            // solvable domain is a circle around the origin with a radius
            // equal to the sum of our bone lengths.
            double totalLenSqr = (length1 + length2) * (length1 + length2);
            if (targetDistSqr < (totalLenSqr - epsilon)
                || targetDistSqr > (totalLenSqr + epsilon))
            {
                foundValidSolution = false;
            }

            // Only the value of angle1 matters at this point. We can just
            // set angle2 to zero. 
            angle2 = 0.0;
            cosAngle2 = 1.0;
            sinAngle2 = 0.0;
        }

        //===
        // Compute the value of angle1 based on the sine and cosine of angle2
        double triAdjacent = length1 + length2 * cosAngle2;
        double triOpposite = length2 * sinAngle2;

        double tanY = targetY * triAdjacent - targetX * triOpposite;
        double tanX = targetX * triAdjacent + targetY * triOpposite;

        // Note that it is safe to call Atan2(0,0) which will happen if targetX and
        // targetY are zero
        angle1 = Math.Atan2(tanY, tanX);

        return foundValidSolution;
    }
}