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
using MeshBoneUtil;
using XnaGeometry;
using JsonFx;
using UnityEngine;

namespace CreatureModule
{
    // Meta Data
    public class CreatureMetaData
    {
        public Dictionary<int, MeshBoneUtil.CTuple<int, int>> mesh_map;
        public Dictionary<String, Dictionary<int, List<int>>> anim_order_map;
        public Dictionary<String, Dictionary<int, String>> anim_events_map;
        public Dictionary<String, HashSet<String>> skin_swaps;
        public HashSet<int> active_skin_swap_ids;
        public Dictionary<String, int> vertex_attachments;

        public class MorphData
        {
            public List<byte[]> morph_spaces = new List<byte[]>();
            public String center_clip;
            public List<MeshBoneUtil.CTuple<String, UnityEngine.Vector2>> morph_clips = new List<MeshBoneUtil.CTuple<string, UnityEngine.Vector2>>();
            public float[] weights;
            public UnityEngine.Vector2 bounds_min, bounds_max;
            public int morph_res;
            public List<MeshBoneUtil.CTuple<String, float>> play_anims_data = new List<MeshBoneUtil.CTuple<string, float>>();
            public MeshBoneUtil.CTuple<String, float> play_center_anim_data;
            public List<float> play_pts = new List<float>();
            public UnityEngine.Vector2 play_img_pt;

            public bool isValid()
            {
                return (morph_spaces.Count > 0);
            }
        }

        public MorphData morph_data = new MorphData();

        // Anim Color Data
        public class AnimColorData
        {
            public int frame;
            public byte r, g, b;
        }

        public Dictionary<String, Dictionary<String, List<AnimColorData>>> anim_region_colors = 
            new Dictionary<string, Dictionary<string, List<AnimColorData>>>();
        public CreatureMetaData()
        {
            mesh_map = new Dictionary<int, MeshBoneUtil.CTuple<int, int>>();
            anim_order_map = new Dictionary<String, Dictionary<int, List<int>>>();
            anim_events_map = new Dictionary<String, Dictionary<int, String>>();
            skin_swaps = new Dictionary<string, HashSet<string>>();
            active_skin_swap_ids = new HashSet<int>();
            vertex_attachments = new Dictionary<string, int>();
        }

        public void clear()
        {
            mesh_map.Clear();
            anim_order_map.Clear();
            skin_swaps.Clear();
            vertex_attachments.Clear();
        }

        // Returns vertex attachment point in character space
        public UnityEngine.Vector3 getVertexAttachment(string name_in, Creature creature_in)
        {
            if(vertex_attachments.ContainsKey(name_in))
            {
                var cur_idx = vertex_attachments[name_in];
                var render_pts = creature_in.render_pts;
                return new UnityEngine.Vector3(
                    render_pts[cur_idx * 3],
                    render_pts[cur_idx * 3 + 1],
                    render_pts[cur_idx * 3 + 2]
                    );
            }

            return new UnityEngine.Vector3(0, 0, 0);
        }

        public void updateRegionColors(Dictionary<String, CreatureAnimation> animations)
        {
            if(anim_region_colors.Count == 0)
            {
                return;
            }

            foreach(var cur_pair in animations)
            {
                var clip_name = cur_pair.Key;
                var clip_anim = cur_pair.Value;
                if (anim_region_colors.ContainsKey(clip_name))
                {
                    var clip_regions_data = anim_region_colors[clip_name];
                    var opacity_cache = clip_anim.opacity_cache;
                    var opacity_table = opacity_cache.opacity_cache_table;
                    for (int m = opacity_cache.getStartTime(); m <= opacity_cache.getEndime(); m++)
                    {
                        var idx = opacity_cache.getIndexByTime(m);
                        var regions_data = opacity_table[idx];
                        foreach (var cur_region in regions_data)
                        {
                            if (clip_regions_data.ContainsKey(cur_region.getKey()))
                            {
                                var read_anim_data = clip_regions_data[cur_region.getKey()];
                                var read_colors_data = read_anim_data[idx];
                                if (read_colors_data.frame == m)
                                {
                                    cur_region.red = (float)(read_colors_data.r) / 255.0f * 100.0f;
                                    cur_region.green = (float)(read_colors_data.g) / 255.0f * 100.0f;
                                    cur_region.blue = (float)(read_colors_data.b) / 255.0f * 100.0f;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void buildSkinSwapIndices(
            String swap_name,
            MeshRenderBoneComposition bone_composition,
            List<int> skin_swap_indices
        )
        {
            if (!skin_swaps.ContainsKey(swap_name))
            {
                skin_swap_indices.Clear();
                return;
            }

            var swap_set = skin_swaps[swap_name];
            active_skin_swap_ids.Clear();
            int total_size = 0;
            var regions_map = bone_composition.getRegionsMap();
            foreach (var cur_data in regions_map)
            {
                if (swap_set.Contains(cur_data.Key))
                {
                    var cur_region = cur_data.Value;
                    total_size += cur_region.getNumIndices();
                    active_skin_swap_ids.Add(cur_region.getTagId());
                }
            }

            skin_swap_indices.Clear();

            int offset = 0;
            foreach(var cur_data in regions_map)
            {
                if (swap_set.Contains(cur_data.Key))
                {
                    var cur_region = cur_data.Value;
                    for(int j = 0; j < cur_region.getNumIndices(); j++)
                    {
                        skin_swap_indices.Add(cur_region.getLocalIndex(j));
                    }

                    offset += cur_region.getNumIndices();
                }
            }
        }

        public bool hasAnimatedOrder(String anim_name, int time_in)
        {
            return (sampleOrder(anim_name, time_in) != null);
        }

        public void updateIndicesAndPoints(
            List<int> dst_indices,
            List<int> src_indices,
            List<float> dst_pts,
            float delta_z,
            int num_indices,
            int num_pts,
            String anim_name,
            bool skin_swap_active,
            int time_in)
        {
            bool has_data = false;
            var cur_order = sampleOrder(anim_name, time_in);
            if (cur_order != null)
            {
                has_data = (cur_order.Count > 0);
            }

            if (has_data)
            {
                float cur_z = 0;
                // Copy new ordering to destination
                List<int> write_ptr = dst_indices;
                int total_num_write_indices = 0;
                foreach (var region_id in cur_order)
                {
                    if (mesh_map.ContainsKey(region_id) == false)
                    {
                        // region not found, just copy and return
                        for (int i = 0; i < dst_indices.Count; i++)
                        {
                            dst_indices[i] = src_indices[i];
                        }
                        return;
                    }

                    // Write indices
                    var mesh_data = mesh_map[region_id];
                    int num_write_indices = mesh_data.Item2 - mesh_data.Item1 + 1;
                    var region_src_ptr = src_indices;

                    if ((total_num_write_indices + num_write_indices) > num_indices)
                    {
                        // overwriting boundaries of array, regions do not match so copy and return
                        for (int i = 0; i < dst_indices.Count; i++)
                        {
                            dst_indices[i] = src_indices[i];
                        }
                        return;
                    }

                    bool valid_region = true;
                    if(skin_swap_active)
                    {
                        valid_region = active_skin_swap_ids.Contains(region_id);
                    }

                    if (valid_region)
                    {
                        for (int i = 0; i < num_write_indices; i++)
                        {
                            write_ptr[total_num_write_indices + i] = region_src_ptr[mesh_data.Item1 + i];
                        }

                        total_num_write_indices += num_write_indices;
                    }

                    // Write points
                    int start_idx = mesh_data.Item1;
                    int end_idx = mesh_data.Item2;

                    if (src_indices[end_idx] < num_pts)
                    {
                        for (int i = start_idx; i <= end_idx; i++)
                        {
                            int cur_pt_idx = src_indices[i] * 3;
                            dst_pts[cur_pt_idx + 2] = cur_z;
                        }
                    }

                    cur_z += delta_z;
                }
            }
            else
            {
                // Nothing changed, just copy from source
                for (int i = 0; i < dst_indices.Count; i++)
                {
                    dst_indices[i] = src_indices[i];
                }
            }
        }

        public List<int> sampleOrder(String anim_name, int time_in)
        {
            if (anim_order_map.ContainsKey(anim_name))
            {
                var order_table = anim_order_map[anim_name];
                if (order_table.Count == 0)
                {
                    return null;
                }

                var keys_sorted = new List<int>(order_table.Keys);
                keys_sorted.Sort();

                int sample_time = keys_sorted[0];

                foreach (var curKey in keys_sorted)
                {
                    if (time_in >= curKey)
                    {
                        sample_time = curKey;
                    }
                }

                return order_table[sample_time];
            }

            return null;
        }

        public float morphSampleFilterPt(
            float q11, // (x1, y1)
            float q12, // (x1, y2)
            float q21, // (x2, y1)
            float q22, // (x2, y2)
            float x1,
            float y1,
            float x2,
            float y2,
            float x,
            float y)
        {
            float x2x1, y2y1, x2x, y2y, yy1, xx1;
            x2x1 = x2 - x1;
            y2y1 = y2 - y1;
            x2x = x2 - x;
            y2y = y2 - y;
            yy1 = y - y1;
            xx1 = x - x1;

            float denom = (x2x1 * y2y1);
            float numerator = (
                q11 * x2x * y2y +
                q21 * xx1 * y2y +
                q12 * x2x * yy1 +
                q22 * xx1 * yy1
                );

            return (denom == 0) ? q11 : (1.0f / denom * numerator);
        }

        public float morphLookupVal(int x_in, int y_in, int idx)
        {
            var cur_space = morph_data.morph_spaces[idx];
            return (float)(cur_space[y_in * morph_data.morph_res + x_in]) / 255.0f;
        }

        public void computeMorphWeights(UnityEngine.Vector2 img_pt)
        {
            morph_data.play_img_pt = img_pt;
            float x1 = (float)Math.Floor(img_pt.x);
            float y1 = (float)Math.Floor(img_pt.y);
            float x2 = (float)Math.Ceiling(img_pt.x);
            float y2 = (float)Math.Ceiling(img_pt.y);

            for (int i = 0; i < morph_data.morph_spaces.Count; i++)
            {
                float q11 = (float)morphLookupVal((int)x1, (int)y1, i); // (x1, y1)
                float q12 = (float)morphLookupVal((int)x1, (int)y2, i); // (x1, y2)
                float q21 = (float)morphLookupVal((int)x2, (int)y1, i); // (x2, y1)
                float q22 = (float)morphLookupVal((int)x2, (int)y2, i); // (x2, y2)

                float sample_val = morphSampleFilterPt(
                    q11, q12, q21, q22, x1, y1, x2, y2, img_pt.x, img_pt.y);
                morph_data.weights[i] = sample_val;
            }
        }

        public void computeMorphWeightsNormalised(UnityEngine.Vector2 normal_pt)
	    {
            var img_pt = normal_pt;
            img_pt.x *= morph_data.morph_res - 1;
            img_pt.y *= morph_data.morph_res - 1;

            img_pt.x = Math.Max(Math.Min((float)morph_data.morph_res - 1.0f, img_pt.x), 0.0f);
		    img_pt.y = Math.Max(Math.Min((float)morph_data.morph_res - 1.0f, img_pt.y), 0.0f);

            computeMorphWeights(img_pt);
        }

        public void computeMorphWeightsWorld(UnityEngine.Vector2 world_pt, UnityEngine.Vector2 base_pt, float radius)
        {
            var rel_pt = world_pt - base_pt;
            var cur_length = rel_pt.magnitude;
            if (cur_length > radius)
            {
                rel_pt.Normalize();
                rel_pt *= radius;
            }

            var normal_pt = (rel_pt + (new UnityEngine.Vector2(radius, radius))) / (radius * 2.0f);
            computeMorphWeightsNormalised(normal_pt);
        }

        public void updateMorphPoints(Creature creature_in, float ratio_in)
        {
            var render_pts = morph_data.play_pts;
            for (int j = 0; j < creature_in.total_num_pts * 3; j += 3)
            {
                render_pts[j] +=
                    (creature_in.render_pts[j] * ratio_in);
                render_pts[j + 1] +=
                    (creature_in.render_pts[j + 1] * ratio_in);
            }
        }

        public void updateMorphStep(CreatureManager manager_in, float delta_step)
        {
            var creature_in = manager_in.GetCreature();
            if (morph_data.play_anims_data.Count == 0)
            {
                var all_clips = manager_in.GetAllAnimations();
                morph_data.play_anims_data = new List<MeshBoneUtil.CTuple<string, float>>();
                for (int i = 0; i < morph_data.morph_clips.Count; i++)
                {
                    var cur_clip_name = morph_data.morph_clips[i].Item1;
                    var cur_start_time = all_clips[cur_clip_name].start_time;
                    var cur_play_data = new MeshBoneUtil.CTuple<string, float>(cur_clip_name, cur_start_time);
                    morph_data.play_anims_data.Add(cur_play_data);
                }

                if (morph_data.center_clip.Length > 0)
                {
                    var center_clip_name = morph_data.center_clip;
                    var center_start_time = all_clips[center_clip_name].start_time;
                    morph_data.play_center_anim_data = new MeshBoneUtil.CTuple<string, float>(center_clip_name, center_start_time);
                }

                morph_data.play_pts = new List<float>(new float[creature_in.total_num_pts * 3]);
            }

            for(int i = 0; i < morph_data.play_pts.Count; i++)
            {
                morph_data.play_pts[i] = 0;
            }

            float center_ratio = 0;
            bool has_center = (morph_data.center_clip.Length > 0);
            if (has_center)
            {
                var radius = (float)morph_data.morph_res * 0.5f;
                var test_pt = morph_data.play_img_pt - (new UnityEngine.Vector2(morph_data.morph_res / 2, morph_data.morph_res / 2));
                center_ratio = (test_pt / ((float)morph_data.morph_res * 0.5f)).magnitude;

                var clip_name = morph_data.play_center_anim_data.Item1;
                manager_in.SetActiveAnimationName(clip_name);
                manager_in.setRunTime(morph_data.play_center_anim_data.Item2);
                manager_in.Update(delta_step);

                float inv_center_ratio = 1.0f - center_ratio;
                updateMorphPoints(creature_in, inv_center_ratio);
                morph_data.play_center_anim_data = 
                    new MeshBoneUtil.CTuple<string, float>(clip_name, manager_in.getRunTime());
            }

            for (int i = 0; i < morph_data.play_anims_data.Count; i++)
            {
                var cur_data = morph_data.play_anims_data[i];
                var clip_name = cur_data.Item1;
                manager_in.SetActiveAnimationName(clip_name);
                manager_in.setRunTime(cur_data.Item2);
                manager_in.Update(delta_step);

                morph_data.play_anims_data[i] = 
                    new MeshBoneUtil.CTuple<string, float>(clip_name, manager_in.getRunTime());
                updateMorphPoints(
                    creature_in,
                    (center_ratio > 0) ? (morph_data.weights[i] * center_ratio) : morph_data.weights[i]);
            }

            // Copy to current render points
            for(int i = 0; i < morph_data.play_pts.Count; i++)
            {
                creature_in.render_pts[i] = morph_data.play_pts[i];
            }
        }
    }

    public class CreaturePhysicsData
    {
        public class boxAndBone
        {
            public boxAndBone(
                GameObject box_in,
                MeshBone bone_in,
                MeshBone next_bone_in,
                XnaGeometry.Vector2 basis_in)
            {
                box = box_in;
                bone = bone_in;
                next_bone = next_bone_in;
                basis = basis_in;
                end_box = null;
            }

            public GameObject box, end_box;
            public MeshBone bone, next_bone;
            public XnaGeometry.Vector2 basis;
        }

        Dictionary<String, boxAndBone> bodies;
        Dictionary<String, List<HingeJoint>> constraints;
        List<MeshBone> kinematic_bones;
        String anim_clip_name;

        CreaturePhysicsData()
        {
            bodies = new Dictionary<string, boxAndBone>();
            constraints = new Dictionary<string, List<HingeJoint>>();
            kinematic_bones = new List<MeshBone>();
        }

        static String getConstraintsKey(String bone1, String bone2)
        {
            return bone1 + "_" + bone2;
        }

        public List<HingeJoint> getConstraint(String bone1, String bone2)
        {
            var test_name = getConstraintsKey(bone1, bone2);
            if (constraints.ContainsKey(test_name))
            {
                return constraints[test_name];
            }

            test_name = getConstraintsKey(bone2, bone1);
            if (constraints.ContainsKey(test_name))
            {
                return constraints[test_name];
            }

            return null;
        }

        private void setupBoxSettings(Rigidbody box_in, UnityEngine.Vector3 pt_in, bool sim_physics)
        {
            box_in.isKinematic = !sim_physics;
            //box_in.MovePosition(pt_in);
            box_in.detectCollisions = false;
            box_in.useGravity = false;
        }

        private HingeJoint makeChainConstraints(
            GameObject parent_body,
            Rigidbody body2,
            float stiffness,
            float damping)
        {
            var constraint_inst = parent_body.AddComponent<HingeJoint>();
            constraint_inst.enableCollision = false;
            constraint_inst.useLimits = true;
            constraint_inst.useSpring = true;
            JointLimits limits = constraint_inst.limits;
            limits.min = 0;
            limits.max = 0;
            limits.bounciness = 0;
            limits.bounceMinVelocity = 0;

            constraint_inst.limits = limits;

            JointSpring hingeSpring = constraint_inst.spring;
            hingeSpring.spring = stiffness;
            hingeSpring.damper = damping;
            hingeSpring.targetPosition = 0;
            constraint_inst.spring = hingeSpring;

            constraint_inst.connectedAnchor = new UnityEngine.Vector3(0, 0, 0);
            constraint_inst.connectedBody = body2;

            return constraint_inst;
        }

        public void createPhysicsChain(
            GameObject container,
            GameObject parent,
            List<MeshBone> bones_in,
            float stiffness,
            float damping,
            String anim_clip_name_in)
        {
            if (bones_in.Count < 2)
            {
                return;
            }

            anim_clip_name = anim_clip_name_in;
            // Create bodies
            for (int i = 0; i < bones_in.Count; i++)
            {
                var cur_bone = bones_in[i];
                MeshBone next_bone = null;
                XnaGeometry.Vector2 cur_basis = new XnaGeometry.Vector2(0, 0);

                if (i < (bones_in.Count - 1))
                {
                    next_bone = bones_in[i + 1];
                    var cur_end_pt = new UnityEngine.Vector3(
                        (float)cur_bone.getWorldEndPt().X,
                        (float)cur_bone.getWorldEndPt().Y,
                        (float)cur_bone.getWorldEndPt().Z);

                    var next_start_pt = new UnityEngine.Vector3(
                        (float)next_bone.getWorldStartPt().X,
                        (float)next_bone.getWorldStartPt().Y,
                        (float)next_bone.getWorldStartPt().Z);

                    var next_end_pt = new UnityEngine.Vector3(
                        (float)next_bone.getWorldEndPt().X,
                        (float)next_bone.getWorldEndPt().Y,
                        (float)next_bone.getWorldEndPt().Z);

                    var cur_dir = cur_end_pt - next_start_pt;
                    var next_dir = next_end_pt - next_start_pt;
                    var next_unit_dir = next_dir.normalized;
                    var next_unit_norm = new UnityEngine.Vector3(-next_unit_dir.y, next_unit_dir.x, next_unit_dir.z);

                    var cur_u = UnityEngine.Vector3.Dot(cur_dir, next_unit_dir);
                    var cur_v = UnityEngine.Vector3.Dot(cur_dir, next_unit_norm);
                    cur_basis.X = cur_u;
                    cur_basis.Y = cur_v;
                }

                var new_body_obj = new GameObject("Creature_PhysicsBody_" + i.ToString());
                new_body_obj.transform.parent = container.transform;
                var new_body = new_body_obj.AddComponent<Rigidbody>();

                bool sim_physics = false;
                if (i > 0)
                {
                    sim_physics = true;
                }

                var bone_start_pt = new UnityEngine.Vector3(
                    (float)cur_bone.getWorldStartPt().X,
                    (float)cur_bone.getWorldStartPt().Y,
                    (float)cur_bone.getWorldStartPt().Z);
                var body_pt = parent.transform.TransformPoint(bone_start_pt);
                setupBoxSettings(new_body, body_pt, sim_physics);
                new_body_obj.transform.position = body_pt;

                bodies.Add(cur_bone.getKey(), new boxAndBone(new_body_obj, cur_bone, next_bone, cur_basis));

                // Last bone, add to tip
                if (i == (bones_in.Count - 1))
                {
                    new_body_obj = new GameObject("Creature_PhysicsBody_Last");
                    new_body_obj.transform.parent = container.transform;
                    new_body = new_body_obj.AddComponent<Rigidbody>();
                    new_body.isKinematic = !sim_physics;

                    // Flip y and z
                    var bone_end_pt = new UnityEngine.Vector3(
                        (float)cur_bone.getWorldEndPt().X,
                        (float)cur_bone.getWorldEndPt().Y,
                        (float)cur_bone.getWorldEndPt().Z);
                    body_pt = parent.transform.TransformPoint(bone_end_pt);
                    setupBoxSettings(new_body, body_pt, sim_physics);
                    new_body_obj.transform.position = body_pt;

                    bodies[cur_bone.getKey()].end_box = new_body_obj;
                }
            }


            // Create constraints
            for (int i = 0; i < bones_in.Count - 1; i++)
            {
                var bone_name_1 = bones_in[i].getKey();
                var bone_name_2 = bones_in[i + 1].getKey();
                var body1 = bodies[bone_name_1].box;
                var body2 = bodies[bone_name_2].box;
                var main_constraint =
                    makeChainConstraints(body1, body2.GetComponent<Rigidbody>(), stiffness, damping);
                if (getConstraint(bone_name_1, bone_name_2) == null)
                {
                    constraints.Add(
                        getConstraintsKey(bone_name_1, bone_name_2),
                        new List<HingeJoint>());
                }

                var cur_list = getConstraint(bone_name_1, bone_name_2);
                cur_list.Add(main_constraint);
            }

            {
                var last_bone_name = bones_in[bones_in.Count - 1].getKey();
                var last_base_body = bodies[last_bone_name].box;
                var last_end_body = bodies[last_bone_name].end_box;
                var last_constraint =
                    makeChainConstraints(
                        last_base_body,
                        last_end_body.GetComponent<Rigidbody>(),
                        stiffness,
                        damping);

                constraints.Add(
                    getConstraintsKey(last_bone_name, last_bone_name),
                    new List<HingeJoint>());
                var cur_list = getConstraint(last_bone_name, last_bone_name);
                cur_list.Add(last_constraint);
            }
        }

        public void clearPhysicsChain()
        {
            foreach (var cur_data in bodies)
            {
                var box_data = cur_data.Value;
                if (box_data.box)
                {
                    UnityEngine.Object.Destroy(box_data.box);
                }

                if (box_data.end_box)
                {
                    UnityEngine.Object.Destroy(box_data.end_box);
                }
            }

            constraints.Clear();
            bodies.Clear();
        }

        public void updateKinematicPos(Transform base_xform, MeshBone bone_in)
        {
            if (!bodies.ContainsKey(bone_in.getKey()))
            {
                return;
            }

            var set_body = bodies[bone_in.getKey()].box;
            var new_pos = new UnityEngine.Vector3(
                (float)bone_in.getWorldStartPt().X,
                (float)bone_in.getWorldStartPt().Y,
                (float)bone_in.getWorldStartPt().Z
            );

            set_body.transform.position = base_xform.TransformPoint(new_pos);
        }

        public void updateAllKinematicBones(Transform base_xform)
        {
            foreach (var cur_bone in kinematic_bones)
            {
                updateKinematicPos(base_xform, cur_bone);
            }
        }

        public void updateBonePositions(Transform base_xform)
        {
            foreach (var body_data in bodies)
            {
                var cur_box = body_data.Value.box;
                var set_bone = body_data.Value.bone;
                {
                    var char_pos = base_xform.InverseTransformPoint(cur_box.transform.position);
                    // Flip y and z
                    var set_bone_start_pt = new XnaGeometry.Vector4(char_pos.x, char_pos.y, char_pos.z, 1.0f);
                    set_bone.setWorldStartPt(set_bone_start_pt);
                }

                // Compute end pt from basis
                var bone_dir = new XnaGeometry.Vector2(0, 0);
                GameObject next_base_body = null, next_end_body = null;

                if (body_data.Value.next_bone != null)
                {
                    var next_data = bodies[body_data.Value.next_bone.getKey()];
                    next_base_body = next_data.box;

                    if (next_data.next_bone != null)
                    {
                        next_end_body = bodies[next_data.next_bone.getKey()].box;
                    }
                    else if (next_data.end_box != null)
                    {
                        next_end_body = next_data.end_box;
                    }
                }
                else if (body_data.Value.end_box != null)
                {
                    next_base_body = cur_box;
                    next_end_body = body_data.Value.end_box;
                }

                if (next_base_body && next_end_body)
                {
                    var next_base_pos =
                    base_xform.InverseTransformPoint(next_base_body.transform.position);
                    var next_end_pos =
                    base_xform.InverseTransformPoint(next_end_body.transform.position);

                    if (!body_data.Value.end_box)
                    {
                        set_bone.setWorldEndPt(
                            new XnaGeometry.Vector4(next_base_pos.x, next_base_pos.y, 0.0f, 1.0f));
                    }
                    else
                    {
                        set_bone.setWorldEndPt(
                           new XnaGeometry.Vector4(next_end_pos.x, next_end_pos.y, 0.0f, 1.0f));
                    }
                }

            }
        }

        [Serializable]
        public class BendPhysicsChain
        {
            public string motor_name = "";

            public string anim_clip_name = "";

            [SerializeField]
            public bool active = false;

            public int num_bones = 0;

            [SerializeField]
            public float stiffness = 10.0f;

            [SerializeField]
            public float damping = 0.1f;

            public List<string> bone_names = new List<string>();
        }

        public class PhysicsUtil
        {
            public static CreaturePhysicsData CreateBendPhysicsChain(
            GameObject container,
            GameObject parent,
            MeshRenderBoneComposition bone_composition,
            String anim_clip,
            List<BendPhysicsChain> bend_physics_chains)
            {
                var physics_data = new CreaturePhysicsData();

                foreach (var chain_data in bend_physics_chains)
                {
                    if ((chain_data.anim_clip_name == anim_clip) &&
                        (chain_data.active))
                    {
                        List<MeshBone> chain_bones = new List<MeshBone>();
                        foreach (var bone_name in chain_data.bone_names)
                        {
                            if (!bone_composition.getBonesMap().ContainsKey(bone_name))
                            {
                                return null;
                            }

                            var cur_bone = bone_composition.getBonesMap()[bone_name];
                            chain_bones.Add(cur_bone);
                        }

                        physics_data.createPhysicsChain(
                            container,
                            parent,
                            chain_bones,
                            chain_data.stiffness,
                            chain_data.damping,
                            anim_clip);

                        physics_data.kinematic_bones.Add(chain_bones[0]);
                    }
                }

                return physics_data;
            }
        }
    }

    // Utils
    class Utils
    {
        public static Dictionary<string, object> LoadCreatureJSONData(string filename_in)
        {
            string text = System.IO.File.ReadAllText(filename_in);
            //var raw_obj = new System.Text.Json.JsonParser().Parse(text);

            Dictionary<string, object> ret_dict = null;
            ret_dict = JsonFx.Json.JsonReader.Deserialize(text, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            //ret_dict = (Dictionary<string, object>)raw_obj;

            return ret_dict;
        }

        public static Dictionary<string, object> LoadCreatureJSONDataFromString(string text_in)
        {
            //var raw_obj = new System.Text.Json.JsonParser().Parse(text_in);

            Dictionary<string, object> ret_dict = null;
            ret_dict = JsonFx.Json.JsonReader.Deserialize(text_in, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            //ret_dict = (Dictionary<string, object>)raw_obj;

            return ret_dict;
        }

        public static Dictionary<string, object> LoadCreatureFlatDataFromBytes(byte[] flat_bytes)
        {
            return CreatureFlatDataReader.Utils.LoadCreatureFlatDataFromBytes(flat_bytes);
        }

        public static List<string> GetAllAnimationNames(Dictionary<string, object> json_data)
        {
            Dictionary<string, object> json_animations = (Dictionary<string, object>)json_data["animation"];
            List<string> keyList = new List<string>(json_animations.Keys);

            return keyList;
        }

        public static void BuildCreatureMetaData(
            CreatureMetaData meta_data,
            string json_text_in,
            List<CreaturePhysicsData.BendPhysicsChain> physics_assets,
            List<String> skin_swap_names,
            List<String> morph_poses,
            List<String> vertex_attachments)
        {
            meta_data.clear();
            var json_dict = JsonFx.Json.JsonReader.Deserialize(json_text_in, typeof(Dictionary<string, object>)) as Dictionary<string, object>;

            // Build Region Ordering
            if (json_dict.ContainsKey("meshes"))
            {
                var all_meshes = (Dictionary<string, object>)json_dict["meshes"];
                foreach (var cur_data in all_meshes)
                {
                    var cur_obj = (Dictionary<string, object>)cur_data.Value;
                    int region_id = Convert.ToInt32(cur_obj["id"]);
                    int start_index = Convert.ToInt32(cur_obj["startIndex"]);
                    int end_index = Convert.ToInt32(cur_obj["endIndex"]);

                    meta_data.mesh_map[region_id] = new MeshBoneUtil.CTuple<int, int>(start_index, end_index);
                }
            }

            if (json_dict.ContainsKey("regionOrders"))
            {
                var all_anim_data = (Dictionary<string, object>)json_dict["regionOrders"];
                foreach (var cur_anim_data in all_anim_data)
                {
                    var anim_name = cur_anim_data.Key;
                    var order_data = (System.Object[])cur_anim_data.Value;
                    Dictionary<int, List<int>> write_order_dict = new Dictionary<int, List<int>>();

                    foreach (var switch_data in order_data)
                    {
                        var switch_dict = (Dictionary<string, object>)switch_data;
                        int switch_time = Convert.ToInt32(switch_dict["switch_time"]);
                        List<int> switch_order = ReadIntArrayJSON(switch_dict, "switch_order");

                        write_order_dict[switch_time] = switch_order;
                    }

                    meta_data.anim_order_map[anim_name] = write_order_dict;
                }
            }

            // Build Event Triggers
            if (json_dict.ContainsKey("eventTriggers"))
            {
                var events_obj = (Dictionary<string, object>)json_dict["eventTriggers"];
                foreach (var cur_data in events_obj)
                {
                    var cur_anim_name = cur_data.Key;
                    var cur_events_map = new Dictionary<int, string>();
                    var cur_obj_array = (System.Object[])cur_data.Value;

                    foreach (var cur_events_json in cur_obj_array)
                    {
                        var cur_events_obj = (Dictionary<string, object>)cur_events_json;
                        var cur_event_name = (string)cur_events_obj["event_name"];
                        var switch_time = Convert.ToInt32(cur_events_obj["switch_time"]);

                        cur_events_map[switch_time] = cur_event_name;
                    }

                    meta_data.anim_events_map[cur_anim_name] = cur_events_map;
                }
            }

            // Skin Swaps
            skin_swap_names.Clear();
            if (json_dict.ContainsKey("skinSwapList"))
            {
                var skin_swap_obj = (Dictionary<string, object>)json_dict["skinSwapList"];
                foreach (var cur_data in skin_swap_obj)
                {
                    var swap_name = cur_data.Key;
                    var swap_data = (Dictionary<string, object>)((Dictionary<string, object>)cur_data.Value)["swap"];
                    var swap_items = (System.Object[])swap_data["swap_items"];
                    HashSet<String> swap_set = new HashSet<string>();
                    foreach (var cur_item in swap_items)
                    {
                        swap_set.Add((String)cur_item);
                    }

                    meta_data.skin_swaps[swap_name] = swap_set;
                    skin_swap_names.Add(swap_name);
                }
            }

            // Physics Data
            if (json_dict.ContainsKey("physicsData"))
            {
                CreaturePhysicsData.BendPhysicsChain[] old_physics_assets =
                    new CreaturePhysicsData.BendPhysicsChain[physics_assets.Count];
                physics_assets.CopyTo(old_physics_assets);
                physics_assets.Clear();

                var physics_obj = (Dictionary<string, object>)json_dict["physicsData"];
                foreach (var cur_data in physics_obj)
                {
                    var cur_anim_name = cur_data.Key;
                    var motor_objs = (Dictionary<string, object>)cur_data.Value;
                    foreach (var cur_motor in motor_objs)
                    {
                        var motor_name = cur_motor.Key;
                        var bone_objs = (System.Object[])cur_motor.Value;
                        CreaturePhysicsData.BendPhysicsChain new_chain = new CreaturePhysicsData.BendPhysicsChain();
                        new_chain.motor_name = motor_name;
                        new_chain.anim_clip_name = cur_anim_name;
                        new_chain.num_bones = bone_objs.Length;

                        foreach (var cur_bone in bone_objs)
                        {
                            var bone_name = (string)cur_bone;
                            new_chain.bone_names.Add(bone_name);
                        }

                        foreach (var old_chain in old_physics_assets)
                        {
                            if ((old_chain.anim_clip_name == cur_anim_name)
                                && (old_chain.motor_name == motor_name))
                            {
                                new_chain.active = old_chain.active;
                                new_chain.stiffness = old_chain.stiffness;
                                new_chain.damping = old_chain.damping;
                            }
                        }

                        physics_assets.Add(new_chain);
                    }
                }
            }

            // Morph Spaces
            if (json_dict.ContainsKey("MorphTargets")
                && json_dict.ContainsKey("MorphRes") 
                && json_dict.ContainsKey("MorphSpace"))
            {

                morph_poses.Clear();
                var morph_obj = (Dictionary<string, object>)json_dict["MorphTargets"];

                var morph_center_array = (System.Object[])morph_obj["CenterData"];
                meta_data.morph_data.center_clip = (string)morph_center_array[1];
                morph_poses.Add(meta_data.morph_data.center_clip);

                var morph_shapes_array = (System.Object[])morph_obj["MorphShape"];
                meta_data.morph_data.bounds_min = new UnityEngine.Vector2(Single.MaxValue, Single.MaxValue);
                meta_data.morph_data.bounds_max = new UnityEngine.Vector2(Single.MinValue, Single.MinValue);
                foreach(var cur_shape_data in morph_shapes_array)
                {
                    var cur_array = (System.Object[])cur_shape_data;
                    var cur_clip = (String)cur_array[0];
                    var pts_array = (Double[])cur_array[1];
                    var cur_pt = new UnityEngine.Vector2(Convert.ToSingle(pts_array[0]), Convert.ToSingle(pts_array[1]));
                    meta_data.morph_data.bounds_min.x = Math.Min(meta_data.morph_data.bounds_min.x, cur_pt.y);
                    meta_data.morph_data.bounds_max.x = Math.Max(meta_data.morph_data.bounds_max.x, cur_pt.y);
                    meta_data.morph_data.bounds_min.y = Math.Min(meta_data.morph_data.bounds_min.x, cur_pt.y);
                    meta_data.morph_data.bounds_max.y = Math.Max(meta_data.morph_data.bounds_max.x, cur_pt.y);

                    meta_data.morph_data.morph_clips.Add(new MeshBoneUtil.CTuple<String, UnityEngine.Vector2>(cur_clip, cur_pt));
                    morph_poses.Add(cur_clip);
                }

                meta_data.morph_data.morph_res = Convert.ToInt32(json_dict["MorphRes"]);
                {
                    // Transform to Image space
                    for(int j = 0; j < meta_data.morph_data.morph_clips.Count; j++)
                    {
                        var cur_clip = meta_data.morph_data.morph_clips[j];
                        var cur_name = cur_clip.Item1;
                        var bounds_size = meta_data.morph_data.bounds_max - meta_data.morph_data.bounds_min;
                        var cur_pt = cur_clip.Item2;
                        cur_pt = (cur_pt - meta_data.morph_data.bounds_min);
                        cur_pt.x /= bounds_size.x; cur_pt.y /= bounds_size.y;
                        cur_pt *= (float)(meta_data.morph_data.morph_res - 1);

                        cur_pt.x = Math.Min(Math.Max(0.0f, cur_pt.x), (float)(meta_data.morph_data.morph_res - 1));
                        cur_pt.y = Math.Min(Math.Max(0.0f, cur_pt.y), (float)(meta_data.morph_data.morph_res - 1));

                        meta_data.morph_data.morph_clips[j] =
                            new MeshBoneUtil.CTuple<string, UnityEngine.Vector2>(cur_name, cur_pt);
                    }
                }


                var raw_str = (String)json_dict["MorphSpace"];
                var raw_bytes = Convert.FromBase64String(raw_str);
                for (int j = 0; j < morph_shapes_array.Length; j++)
                {
                    int space_size = meta_data.morph_data.morph_res * meta_data.morph_data.morph_res;
                    int byte_idx = j * space_size;
                    byte[] space_data = new byte[space_size];

                    Array.Copy(raw_bytes, byte_idx, space_data, 0, space_size);
                    meta_data.morph_data.morph_spaces.Add(space_data);
                }

                meta_data.morph_data.weights = new float[morph_shapes_array.Length];
            }

            // Vertex Attachments
            if(json_dict.ContainsKey("VertAttachments"))
            {
                vertex_attachments.Clear();
                var attachments_obj = (Dictionary<string, object>)json_dict["VertAttachments"];
                var attachments_list = (System.Object[])attachments_obj["attachments"];
                for(int j = 0; j < attachments_list.Length; j++)
                {
                    var cur_attachment = (Dictionary<string, object>)attachments_list[j];
                    var cur_name = (string)cur_attachment["attach_name"];
                    var cur_idx = (int)cur_attachment["idx"];

                    meta_data.vertex_attachments[cur_name] = cur_idx;
                    vertex_attachments.Add(cur_name);
                }
            }

            // Animated Region Colors
            meta_data.anim_region_colors.Clear();
            if(json_dict.ContainsKey("AnimRegionColors"))
            {
                var region_colors_node = (Dictionary<string, object>)json_dict["AnimRegionColors"];
                foreach(var cur_data in region_colors_node)
                {
                    var anim_name = cur_data.Key;
                    var regions_node = (Dictionary<string, object>)cur_data.Value;
                    Dictionary<String, List<CreatureMetaData.AnimColorData>> regions_anim = 
                        new Dictionary<string, List<CreatureMetaData.AnimColorData>>();
                    foreach(var r_data in regions_node)
                    {
                        var r_name = r_data.Key;
                        var b64_data = (String)r_data.Value;
                        byte[] bytes_data = Convert.FromBase64String(b64_data);
                        int chunk_size = sizeof(int) + (sizeof(byte) * 3);
                        int chunk_num = (int)bytes_data.Length / chunk_size;
                        List<CreatureMetaData.AnimColorData> colors_anim = new List<CreatureMetaData.AnimColorData>();
                        for (int m = 0; m < chunk_num; m++)
                        {
                            int base_ptr = m * chunk_size;
                            int read_ptr = base_ptr;

                            int frame_val = 0;
                            byte r_val = 0, g_val = 0, b_val = 0;

                            frame_val = BitConverter.ToInt32(bytes_data, read_ptr);
                            read_ptr += sizeof(int);

                            r_val = bytes_data[read_ptr];
                            read_ptr += sizeof(byte);

                            g_val = bytes_data[read_ptr];
                            read_ptr += sizeof(byte);

                            b_val = bytes_data[read_ptr];
                            read_ptr += sizeof(byte);

                            CreatureMetaData.AnimColorData color_data = new CreatureMetaData.AnimColorData();
                            color_data.frame = frame_val;
                            color_data.r = r_val;
                            color_data.g = g_val;
                            color_data.b = b_val;

                            colors_anim.Add(color_data);
                        }

                        regions_anim.Add(r_name, colors_anim);
                    }
                    meta_data.anim_region_colors.Add(anim_name, regions_anim);
                }
            }
        }

        public static float[] getFloatArray(System.Object raw_data)
        {
            if (raw_data.GetType() == typeof(double[]))
            {
                double[] float_obj = (double[])raw_data;
                float[] float_array = Array.ConvertAll(float_obj, item => (float)Convert.ToSingle(item));

                return float_array;
            }

            System.Object[] cur_obj = (System.Object[])raw_data;

            float[] ret_array = Array.ConvertAll(cur_obj, item => (float)Convert.ToSingle(item));

            return ret_array;
        }

        public static int[] getIntArray(System.Object raw_data)
        {
            System.Object[] cur_obj = (System.Object[])raw_data;

            int[] raw_array = Array.ConvertAll(cur_obj, item => (int)Convert.ToInt32(item));

            return raw_array;
        }


        public static List<XnaGeometry.Vector2> ReadPointsArray2DJSON(Dictionary<string, object> data,
                                                       string key)
        {
            if (data[key].GetType() == typeof(double[]))
            {
                double[] double_obj = (double[])data[key];

                List<XnaGeometry.Vector2> ret_double_list = new List<XnaGeometry.Vector2>(double_obj.Length);
                int num_dbl_points = double_obj.Length / 2;
                for (int i = 0; i < num_dbl_points; i++)
                {
                    int cur_index = i * 2;
                    ret_double_list.Add(
                                new XnaGeometry.Vector2(Convert.ToDouble(double_obj[0 + cur_index]),
                                                        Convert.ToDouble(double_obj[1 + cur_index])));

                }

                return ret_double_list;
            }

            System.Object[] cur_obj = (System.Object[])data[key];

            List<XnaGeometry.Vector2> ret_list = new List<XnaGeometry.Vector2>(cur_obj.Length);
            int num_points = cur_obj.Length / 2;
            for (int i = 0; i < num_points; i++)
            {
                int cur_index = i * 2;
                ret_list.Add(
                    new XnaGeometry.Vector2(Convert.ToDouble(cur_obj[0 + cur_index]),
                                        Convert.ToDouble(cur_obj[1 + cur_index])));

            }

            return ret_list;
        }

        public static List<float> ReadFloatArray3DJSON(Dictionary<string, object> data,
                                                     string key)
        {
            if (data[key].GetType() == typeof(double[]))
            {
                double[] cur_double_obj = (double[])data[key];

                List<float> ret_double_list = new List<float>(cur_double_obj.Length);
                int num_double_points = cur_double_obj.Length / 2;

                for (int i = 0; i < num_double_points; i++)
                {
                    int cur_index = i * 2;
                    ret_double_list.Add((float)Convert.ToSingle(cur_double_obj[0 + cur_index]));
                    ret_double_list.Add((float)Convert.ToSingle(cur_double_obj[1 + cur_index]));
                    ret_double_list.Add(0);
                }

                return ret_double_list;
            }

            System.Object[] cur_obj = (System.Object[])data[key];

            List<float> ret_list = new List<float>(cur_obj.Length);
            int num_points = cur_obj.Length / 2;

            for (int i = 0; i < num_points; i++)
            {
                int cur_index = i * 2;
                ret_list.Add((float)Convert.ToSingle(cur_obj[0 + cur_index]));
                ret_list.Add((float)Convert.ToSingle(cur_obj[1 + cur_index]));
                ret_list.Add(0);
            }

            return ret_list;
        }

        public static bool ReadBoolJSON(Dictionary<string, object> data,
                                        string key)
        {
            bool val = (bool)data[key];
            return val;
        }

        public static List<float> ReadFloatArrayJSON(Dictionary<string, object> data,
                                              string key)
        {
            if (data[key].GetType() == typeof(double[]))
            {
                double[] cur_double_obj = (double[])data[key];
                List<float> ret_double_list = new List<float>(cur_double_obj.Length);
                for (int i = 0; i < cur_double_obj.Length; i++)
                {
                    ret_double_list.Add(Convert.ToSingle(cur_double_obj[i]));
                }

                return ret_double_list;
            }

            System.Object[] cur_obj = (System.Object[])data[key];
            List<float> ret_list = new List<float>(cur_obj.Length);
            for (int i = 0; i < cur_obj.Length; i++)
            {
                ret_list.Add(Convert.ToSingle(cur_obj[i]));
            }

            return ret_list;
        }

        public static List<int> ReadIntArrayJSON(Dictionary<string, object> data,
                                            string key)
        {
            if (data[key].GetType() == typeof(int[]))
            {
                int[] cur_int_data = (int[])data[key];
                List<int> ret_int_list = new List<int>(cur_int_data.Length);
                for (int i = 0; i < cur_int_data.Length; i++)
                {
                    ret_int_list.Add(Convert.ToInt32(cur_int_data[i]));
                }

                return ret_int_list;
            }
            else if (data[key].GetType() == typeof(System.Object[]))
            {
                System.Object[] cur_obj = (System.Object[])data[key];
                List<int> ret_list = new List<int>(cur_obj.Length);
                for (int i = 0; i < cur_obj.Length; i++)
                {
                    ret_list.Add(Convert.ToInt32(cur_obj[i]));
                }

                return ret_list;
            }

            return new List<int>(0);
        }

        public static XnaGeometry.Matrix ReadMatrixJSON(Dictionary<string, object> data,
                                                        string key)
        {
            float[] raw_array = getFloatArray(data[key]);
            return new XnaGeometry.Matrix(raw_array[0], raw_array[1], raw_array[2], raw_array[3],
                                          raw_array[4], raw_array[5], raw_array[6], raw_array[7],
                                          raw_array[8], raw_array[9], raw_array[10], raw_array[11],
                                          raw_array[12], raw_array[13], raw_array[14], raw_array[15]);
        }

        public static XnaGeometry.Vector4 ReadVector4JSON(Dictionary<string, object> data,
                                                          string key)
        {
            float[] raw_array = getFloatArray(data[key]);
            return new XnaGeometry.Vector4(raw_array[0], raw_array[1], 0, 1);
        }

        public static XnaGeometry.Vector2 ReadVector2JSON(Dictionary<string, object> data,
                                                          string key)
        {
            float[] raw_array = getFloatArray(data[key]);
            return new XnaGeometry.Vector2(raw_array[0], raw_array[1]);
        }

        public static MeshBone CreateBones(Dictionary<string, object> json_obj,
                                           string key)
        {
            MeshBone root_bone = null;
            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
            Dictionary<int, MeshBoneUtil.CTuple<MeshBone, List<int>>> bone_data = new Dictionary<int, CTuple<MeshBone, List<int>>>();
            Dictionary<int, int> child_set = new Dictionary<int, int>();

            // layout bones
            foreach (KeyValuePair<string, object> cur_node in base_obj)
            {

                string cur_name = cur_node.Key;
                Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;

                int cur_id = Convert.ToInt32(node_dict["id"]); //GetJSONNodeFromKey(*cur_node, "id")->value.toNumber();
                XnaGeometry.Matrix cur_parent_mat = Utils.ReadMatrixJSON(node_dict, "restParentMat");

                XnaGeometry.Vector4 cur_local_rest_start_pt = Utils.ReadVector4JSON(node_dict, "localRestStartPt");
                XnaGeometry.Vector4 cur_local_rest_end_pt = Utils.ReadVector4JSON(node_dict, "localRestEndPt");
                List<int> cur_children_ids = Utils.ReadIntArrayJSON(node_dict, "children");

                MeshBone new_bone = new MeshBone(cur_name,
                                                 new XnaGeometry.Vector4(0, 0, 0, 0),
                                                 new XnaGeometry.Vector4(0, 0, 0, 0),
                                                 cur_parent_mat);
                new_bone.local_rest_start_pt = cur_local_rest_start_pt;
                new_bone.local_rest_end_pt = cur_local_rest_end_pt;
                new_bone.calcRestData();
                new_bone.setTagId(cur_id);

                bone_data[cur_id] = new MeshBoneUtil.CTuple<MeshBone, List<int>>(new_bone, cur_children_ids);

                foreach (int cur_child_id in cur_children_ids)
                {
                    child_set.Add(cur_child_id, cur_child_id);
                }
            }

            // Find root
            foreach (KeyValuePair<int, MeshBoneUtil.CTuple<MeshBone, List<int>>> cur_data in bone_data)
            {
                int cur_id = cur_data.Key;
                if (child_set.ContainsKey(cur_id) == false)
                {
                    // not a child, so is root
                    root_bone = cur_data.Value.Item1;
                    break;
                }
            }

            // construct hierarchy
            foreach (KeyValuePair<int, MeshBoneUtil.CTuple<MeshBone, List<int>>> cur_data in bone_data)
            {
                MeshBone cur_bone = cur_data.Value.Item1;
                List<int> children_ids = cur_data.Value.Item2;
                foreach (int cur_child_id in children_ids)
                {
                    MeshBone child_bone = bone_data[cur_child_id].Item1;
                    cur_bone.addChild(child_bone);
                }

            }


            return root_bone;
        }

        public static List<MeshRenderRegion> CreateRegions(Dictionary<string, object> json_obj,
                                                           string key,
                                                           List<int> indices_in,
                                                           List<float> rest_pts_in,
                                                           List<float> uvs_in)
        {
            List<MeshRenderRegion> ret_regions = new List<MeshRenderRegion>();
            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

            foreach (KeyValuePair<string, object> cur_node in base_obj)
            {
                string cur_name = cur_node.Key;
                Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;

                int cur_id = Convert.ToInt32(node_dict["id"]); //(int)GetJSONNodeFromKey(*cur_node, "id")->value.toNumber();
                int cur_start_pt_index = Convert.ToInt32(node_dict["start_pt_index"]); //(int)GetJSONNodeFromKey(*cur_node, "start_pt_index")->value.toNumber();
                int cur_end_pt_index = Convert.ToInt32(node_dict["end_pt_index"]); //(int)GetJSONNodeFromKey(*cur_node, "end_pt_index")->value.toNumber();
                int cur_start_index = Convert.ToInt32(node_dict["start_index"]); //(int)GetJSONNodeFromKey(*cur_node, "start_index")->value.toNumber();
                int cur_end_index = Convert.ToInt32(node_dict["end_index"]); //(int)GetJSONNodeFromKey(*cur_node, "end_index")->value.toNumber();

                MeshRenderRegion new_region = new MeshRenderRegion(indices_in,
                                                                   rest_pts_in,
                                                                   uvs_in,
                                                                   cur_start_pt_index,
                                                                   cur_end_pt_index,
                                                                   cur_start_index,
                                                                   cur_end_index);

                new_region.setName(cur_name);
                new_region.setTagId(cur_id);

                // Read in weights
                Dictionary<string, List<float>> weight_map =
                    new_region.normal_weight_map;
                Dictionary<string, object> weight_obj = (Dictionary<string, object>)node_dict["weights"];

                foreach (KeyValuePair<string, object> w_node in weight_obj)
                {

                    string w_key = w_node.Key;
                    List<float> values = ReadFloatArrayJSON(weight_obj, w_key);
                    weight_map.Add(w_key, values);
                }

                ret_regions.Add(new_region);
            }

            return ret_regions;
        }

        public static MeshBoneUtil.CTuple<int, int> GetStartEndTimes(Dictionary<string, object> json_obj,
                                                          string key)
        {
            int start_time = 0;
            int end_time = 0;
            bool first = true;
            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

            foreach (KeyValuePair<string, object> cur_node in base_obj)
            {

                int cur_val = Convert.ToInt32(cur_node.Key);
                if (first)
                {
                    start_time = cur_val;
                    end_time = cur_val;
                    first = false;
                }
                else
                {
                    if (cur_val > end_time)
                    {
                        end_time = cur_val;
                    }

                    if (cur_val < start_time)
                    {
                        start_time = cur_val;
                    }
                }
            }

            MeshBoneUtil.CTuple<int, int> ret_times = new MeshBoneUtil.CTuple<int, int>(start_time, end_time);
            return ret_times;
        }

        static public XnaGeometry.Vector4 ptInterp(XnaGeometry.Vector4 src_pt, XnaGeometry.Vector4 target_pt, float fraction)
	    {
            return ((1.0f - fraction) * src_pt) + (fraction* target_pt);
        }

        static public List<XnaGeometry.Vector2> ptsInterp(
            List<XnaGeometry.Vector2> src_pts, 
            List<XnaGeometry.Vector2> target_pts,
            float fraction)
		{
            List<XnaGeometry.Vector2> ret_pts = new List<XnaGeometry.Vector2>();
            for (var i = 0; i < src_pts.Count; i++)
			{
				ret_pts.Add(((1.0f - fraction) * src_pts[i]) + (fraction* target_pts[i]));
            }

			return ret_pts;
		}
        static public float scalarInterp(float src_val, float target_val, float fraction)
        {
            return ((1.0f - fraction) * src_val) + (fraction * target_val);
        }

        static public XnaGeometry.Vector4 vec4Interp(XnaGeometry.Vector4 src_vec, XnaGeometry.Vector4 target_vec, float fraction)
        {
            return ((1.0f - fraction) * src_vec) + (fraction * target_vec);
        }

        static public void FillBoneCache(Dictionary<string, object> json_obj,
                                           string key,
                                           int start_time,
                                           int end_time,
                                           ref MeshBoneCacheManager cache_manager)
        {
            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

            cache_manager.init(start_time, end_time);

            int prev_time = start_time;
            foreach (KeyValuePair<string, object> cur_node in base_obj)
            {
                int cur_time = Convert.ToInt32(cur_node.Key);
                List<MeshBoneCache> cache_list = new List<MeshBoneCache>();

                Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;
                foreach (KeyValuePair<string, object> bone_node in node_dict)
                {
                    string cur_name = bone_node.Key;
                    Dictionary<string, object> bone_dict = (Dictionary<string, object>)bone_node.Value;

                    XnaGeometry.Vector4 cur_start_pt = Utils.ReadVector4JSON(bone_dict, "start_pt"); //ReadJSONVec4_2(*bone_node, "start_pt");
                    XnaGeometry.Vector4 cur_end_pt = Utils.ReadVector4JSON(bone_dict, "end_pt"); //ReadJSONVec4_2(*bone_node, "end_pt");

                    MeshBoneCache cache_data = new MeshBoneCache(cur_name);
                    cache_data.setWorldStartPt(cur_start_pt);
                    cache_data.setWorldEndPt(cur_end_pt);

                    cache_list.Add(cache_data);
                }

                int set_index = cache_manager.getIndexByTime(cur_time);
                cache_manager.bone_cache_table[set_index] = cache_list;

                var gap_diff = cur_time - prev_time;
                if (gap_diff > 1)
                {
                    // Gap Step
                    var prev_index = cache_manager.getIndexByTime(prev_time);
                    for (int j = 1; j < gap_diff; j++)
                    {
                        var gap_fraction = (float)j / (float)gap_diff;
                        List<MeshBoneCache> gap_cache_list = new List<MeshBoneCache>();
                        
                        for (int k = 0; k < cache_list.Count; k++)
                        {
                            var cur_data = cache_manager.bone_cache_table[set_index][k];
                            var prev_data = cache_manager.bone_cache_table[prev_index][k];
                            MeshBoneCache gap_cache_data = new MeshBoneCache(cur_data.getKey());
                            gap_cache_data.setWorldStartPt(
                                ptInterp(prev_data.getWorldStartPt(), cur_data.getWorldStartPt(), gap_fraction));
                            gap_cache_data.setWorldEndPt(
                                ptInterp(prev_data.getWorldEndPt(), cur_data.getWorldEndPt(), gap_fraction));

                            gap_cache_list.Add(gap_cache_data);
                        }

                        cache_manager.bone_cache_table[prev_index + j] = gap_cache_list;
                    }
                }

                prev_time = cur_time;
            }

            cache_manager.makeAllReady();
        }

        public static void FillDeformationCache(Dictionary<string, object> json_obj,
                                                 string key,
                                                 int start_time,
                                                 int end_time,
                                                 ref MeshDisplacementCacheManager cache_manager)
        {
            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

            cache_manager.init(start_time, end_time);

            int prev_time = start_time;
            foreach (KeyValuePair<string, object> cur_node in base_obj)
            {
                int cur_time = Convert.ToInt32(cur_node.Key);

                List<MeshDisplacementCache> cache_list = new List<MeshDisplacementCache>();

                Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;
                foreach (KeyValuePair<string, object> mesh_node in node_dict)
                {
                    string cur_name = mesh_node.Key;
                    Dictionary<string, object> mesh_dict = (Dictionary<string, object>)mesh_node.Value;

                    MeshDisplacementCache cache_data = new MeshDisplacementCache(cur_name);

                    bool use_local_displacement = Utils.ReadBoolJSON(mesh_dict, "use_local_displacements"); //GetJSONNodeFromKey(*mesh_node, "use_local_displacements")->value.toBool();
                    bool use_post_displacement = Utils.ReadBoolJSON(mesh_dict, "use_post_displacements"); //GetJSONNodeFromKey(*mesh_node, "use_post_displacements")->value.toBool();

                    if (use_local_displacement)
                    {
                        List<XnaGeometry.Vector2> read_pts = Utils.ReadPointsArray2DJSON(mesh_dict,
                                                                                  "local_displacements"); //ReadJSONPoints2DVector(*mesh_node, "local_displacements");
                        cache_data.setLocalDisplacements(read_pts);
                    }

                    if (use_post_displacement)
                    {
                        List<XnaGeometry.Vector2> read_pts = Utils.ReadPointsArray2DJSON(mesh_dict,
                                                                                  "post_displacements"); //ReadJSONPoints2DVector(*mesh_node, "post_displacements");
                        cache_data.setPostDisplacements(read_pts);
                    }

                    cache_list.Add(cache_data);
                }

                int set_index = cache_manager.getIndexByTime(cur_time);
                cache_manager.displacement_cache_table[set_index] = cache_list;

                var gap_diff = cur_time - prev_time;
                if (gap_diff > 1)
                {
                    // Gap Step
                    var prev_index = cache_manager.getIndexByTime(prev_time);
                    for (int j = 1; j < gap_diff; j++)
                    {
                        var gap_fraction = (float)j / (float)gap_diff;
                        List<MeshDisplacementCache> gap_cache_list = new List<MeshDisplacementCache>();

                        for (int k = 0; k < cache_list.Count; k++)
                        {
                            var cur_data = cache_manager.displacement_cache_table[set_index][k];
                            var prev_data = cache_manager.displacement_cache_table[prev_index][k];
                            MeshDisplacementCache gap_cache_data = new MeshDisplacementCache(cur_data.getKey());
                            if (cur_data.getLocalDisplacements().Count > 0)
                            {
                                gap_cache_data.setLocalDisplacements(
                                    ptsInterp(prev_data.getLocalDisplacements(), cur_data.getLocalDisplacements(), gap_fraction));
                            }
                            else
                            {
                                gap_cache_data.setPostDisplacements(
                                    ptsInterp(prev_data.getPostDisplacements(), cur_data.getPostDisplacements(), gap_fraction));
                            }

                            gap_cache_list.Add(gap_cache_data);
                        }

                        cache_manager.displacement_cache_table[prev_index + j] = gap_cache_list;
                    }
                }

                prev_time = cur_time;
            }

            cache_manager.makeAllReady();
        }

        public static void FillUVSwapCache(Dictionary<string, object> json_obj,
                                           string key,
                                           int start_time,
                                           int end_time,
                                           ref MeshUVWarpCacheManager cache_manager)
        {
            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

            cache_manager.init(start_time, end_time);

            foreach (KeyValuePair<string, object> cur_node in base_obj)
            {
                int cur_time = Convert.ToInt32(cur_node.Key);

                List<MeshUVWarpCache> cache_list = new List<MeshUVWarpCache>();

                Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;
                foreach (KeyValuePair<string, object> uv_node in node_dict)
                {
                    string cur_name = uv_node.Key;
                    Dictionary<string, object> uv_dict = (Dictionary<string, object>)uv_node.Value;

                    MeshUVWarpCache cache_data = new MeshUVWarpCache(cur_name);
                    bool use_uv = Utils.ReadBoolJSON(uv_dict, "enabled"); //GetJSONNodeFromKey(*uv_node, "enabled")->value.toBool();
                    cache_data.setEnabled(use_uv);
                    if (use_uv)
                    {
                        XnaGeometry.Vector2 local_offset = Utils.ReadVector2JSON(uv_dict, "local_offset"); //ReadJSONVec2(*uv_node, "local_offset");
                        XnaGeometry.Vector2 global_offset = Utils.ReadVector2JSON(uv_dict, "global_offset"); //ReadJSONVec2(*uv_node, "global_offset");
                        XnaGeometry.Vector2 scale = Utils.ReadVector2JSON(uv_dict, "scale"); //ReadJSONVec2(*uv_node, "scale");
                        cache_data.setUvWarpLocalOffset(local_offset);
                        cache_data.setUvWarpGlobalOffset(global_offset);
                        cache_data.setUvWarpScale(scale);
                    }

                    cache_list.Add(cache_data);
                }

                int set_index = cache_manager.getIndexByTime(cur_time);
                cache_manager.uv_cache_table[set_index] = cache_list;
            }

            cache_manager.makeAllReady();
        }

        public static void FillOpacityCache(Dictionary<string, object> json_obj,
                                           string key,
                                           int start_time,
                                           int end_time,
                                           ref MeshOpacityCacheManager cache_manager)
        {
            cache_manager.init(start_time, end_time);

            if (json_obj.ContainsKey(key) == false)
            {
                return;
            }

            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

            int prev_time = start_time;
            foreach (KeyValuePair<string, object> cur_node in base_obj)
            {
                int cur_time = Convert.ToInt32(cur_node.Key);

                List<MeshOpacityCache> cache_list = new List<MeshOpacityCache>();

                Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;
                foreach (KeyValuePair<string, object> opacity_node in node_dict)
                {
                    string cur_name = opacity_node.Key;
                    Dictionary<string, object> opacity_dict = (Dictionary<string, object>)opacity_node.Value;

                    MeshOpacityCache cache_data = new MeshOpacityCache(cur_name);
                    double cur_opacity = Convert.ToDouble(opacity_dict["opacity"]);
                    cache_data.setOpacity((float)cur_opacity);

                    cache_list.Add(cache_data);
                }

                int set_index = cache_manager.getIndexByTime(cur_time);
                cache_manager.opacity_cache_table[set_index] = cache_list;

                var gap_diff = cur_time - prev_time;
                if (gap_diff > 1)
                {
                    // Gap Step
                    var prev_index = cache_manager.getIndexByTime(prev_time);
                    for (int j = 1; j < gap_diff; j++)
                    {
                        var gap_fraction = (float)j / (float)gap_diff;
                        List<MeshOpacityCache> gap_cache_list = new List<MeshOpacityCache>();
                        for (int k = 0; k < cache_list.Count; k++)
                        {
                            var cur_data = cache_manager.opacity_cache_table[set_index][k];
                            var prev_data = cache_manager.opacity_cache_table[prev_index][k];
                            MeshOpacityCache gap_cache_data = new MeshOpacityCache(cur_data.getKey());
                            gap_cache_data.setOpacity(scalarInterp(prev_data.getOpacity(), cur_data.getOpacity(), gap_fraction));

                            gap_cache_list.Add(gap_cache_data);
                        }

                        cache_manager.opacity_cache_table[prev_index + j] = gap_cache_list;
                    }
                }

                prev_time = cur_time;
            }

            cache_manager.makeAllReady();
        }

        public static Dictionary<String, List<CreatureUVSwapPacket>>
        FillSwapUvPacketMap(Dictionary<string, object> json_obj, string key)
        {
            Dictionary<String, List<CreatureUVSwapPacket>> ret_map = new Dictionary<String, List<CreatureUVSwapPacket>>();

            if (json_obj.ContainsKey(key) == false)
            {
                return ret_map;
            }

            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

            foreach (var cur_node in base_obj)
            {
                var cur_name = cur_node.Key;
                List<CreatureUVSwapPacket> cur_packets = new List<CreatureUVSwapPacket>();

                object[] node_list = (object[])cur_node.Value;

                foreach (var packet_node in node_list)
                {
                    Dictionary<string, object> packet_dict = (Dictionary<string, object>)packet_node;
                    var local_offset = ReadVector2JSON(packet_dict, "local_offset");
                    var global_offset = ReadVector2JSON(packet_dict, "global_offset");
                    var scale = ReadVector2JSON(packet_dict, "scale");
                    int tag = Convert.ToInt32(packet_dict["tag"]);

                    var new_packet = new CreatureUVSwapPacket(local_offset, global_offset, scale, tag);
                    cur_packets.Add(new_packet);
                }

                ret_map[cur_name] = cur_packets;
            }

            return ret_map;
        }

        public static Dictionary<String, XnaGeometry.Vector2>
        FillAnchorPointMap(Dictionary<string, object> json_obj, string key)
        {
            Dictionary<String, XnaGeometry.Vector2> ret_map = new Dictionary<String, XnaGeometry.Vector2>();
            if (json_obj.ContainsKey(key) == false)
            {
                return ret_map;
            }

            Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
            object[] anchor_obj = (object[])base_obj["AnchorPoints"];

            foreach (var cur_node in anchor_obj)
            {
                Dictionary<string, object> anchor_dict = (Dictionary<string, object>)cur_node;
                var cur_pt = ReadVector2JSON(anchor_dict, "point");
                var cur_name = (String)anchor_dict["anim_clip_name"];

                ret_map[cur_name] = cur_pt;
            }

            return ret_map;
        }

    }

    public class CreatureUVSwapPacket
    {
        public XnaGeometry.Vector2 local_offset;
        public XnaGeometry.Vector2 global_offset;
        public XnaGeometry.Vector2 scale;
        public int tag;

        public CreatureUVSwapPacket(XnaGeometry.Vector2 local_offset_in,
                    XnaGeometry.Vector2 global_offset_in,
                    XnaGeometry.Vector2 scale_in,
                    int tag_in)
        {
            local_offset = local_offset_in;
            global_offset = global_offset_in;
            scale = scale_in;
            tag = tag_in;
        }


    };

    // Class for the creature character
    public class Creature
    {
        // mesh and skeleton data
        public List<int> global_indices;
        public List<float> global_pts, global_uvs;
        public List<float> render_pts;
        public List<byte> render_colours;
        public int total_num_pts, total_num_indices;
        public MeshRenderBoneComposition render_composition;
        public Dictionary<String, List<CreatureUVSwapPacket>> uv_swap_packets;
        public Dictionary<String, int> active_uv_swap_actions;
        public Dictionary<String, XnaGeometry.Vector2> anchor_point_map;
        public bool anchor_points_active;

        public Creature(ref Dictionary<string, object> load_data)
        {
            total_num_pts = 0;
            total_num_indices = 0;
            global_indices = null;
            global_pts = null;
            global_uvs = null;
            render_pts = null;
            render_colours = null;
            render_composition = null;
            uv_swap_packets = new Dictionary<String, List<CreatureUVSwapPacket>>();
            active_uv_swap_actions = new Dictionary<String, int>();
            anchor_point_map = new Dictionary<String, XnaGeometry.Vector2>();
            anchor_points_active = false;

            LoadFromData(ref load_data);
        }

        // Fills entire mesh with (r,g,b,a) colours
        public void FillRenderColours(byte r, byte g, byte b, byte a)
        {
            for (int i = 0; i < total_num_pts; i++)
            {
                int cur_colour_index = i * 4;
                render_colours[0 + cur_colour_index] = r;
                render_colours[1 + cur_colour_index] = g;
                render_colours[2 + cur_colour_index] = b;
                render_colours[3 + cur_colour_index] = a;
            }
        }

        public void LoadFromData(ref Dictionary<string, object> load_data)
        {
            // Load points and topology
            Dictionary<string, object> json_mesh = (Dictionary<string, object>)load_data["mesh"];

            global_pts = Utils.ReadFloatArray3DJSON(json_mesh, "points");
            total_num_pts = global_pts.Count / 3;

            global_indices = Utils.ReadIntArrayJSON(json_mesh, "indices");
            total_num_indices = global_indices.Count;

            global_uvs = Utils.ReadFloatArrayJSON(json_mesh, "uvs");

            render_colours = new List<byte>(new byte[total_num_pts * 4]);
            FillRenderColours(255, 255, 255, 255);

            render_pts = new List<float>(new float[global_pts.Count]);

            // Load bones
            MeshBone root_bone = Utils.CreateBones(load_data,
                                                   "skeleton");


            // Load regions
            List<MeshRenderRegion> regions = Utils.CreateRegions(json_mesh,
                                                                 "regions",
                                                                 global_indices,
                                                                 global_pts,
                                                                 global_uvs);

            // Add into composition
            render_composition = new MeshRenderBoneComposition();
            render_composition.setRootBone(root_bone);
            render_composition.getRootBone().computeRestParentTransforms();

            foreach (MeshRenderRegion cur_region in regions)
            {
                cur_region.setMainBoneKey(root_bone.getKey());
                cur_region.determineMainBone(root_bone);
                render_composition.addRegion(cur_region);
            }

            render_composition.initBoneMap();
            render_composition.initRegionsMap();

            foreach (MeshRenderRegion cur_region in regions)
            {
                cur_region.initFastNormalWeightMap(ref render_composition.bones_map);
            }

            render_composition.resetToWorldRestPts();

            // Fill up uv swap packets
            uv_swap_packets = Utils.FillSwapUvPacketMap(load_data, "uv_swap_items");

            // Load Anchor Points
            anchor_point_map = Utils.FillAnchorPointMap(load_data, "anchor_points_items");
        }

        public void SetActiveItemSwap(String region_name, int swap_idx)
        {
            active_uv_swap_actions[region_name] = swap_idx;
        }

        public void RemoveActiveItemSwap(String region_name)
        {
            active_uv_swap_actions.Remove(region_name);
        }

        public XnaGeometry.Vector2 GetAnchorPoint(String anim_clip_name_in)
        {
            if (anchor_point_map.ContainsKey(anim_clip_name_in))
            {
                return anchor_point_map[anim_clip_name_in];
            }

            return new XnaGeometry.Vector2(0, 0);
        }
    }

    // High-level Class to store data on bones for feedback/retrieval layers
    public class CreatureBoneData
    {
        public CreatureBoneData()
        {
            start_pt = new XnaGeometry.Vector3(0, 0, 0);
            end_pt = new XnaGeometry.Vector3(0, 0, 0);
        }

        public CreatureBoneData(XnaGeometry.Vector3 start_pt_in, XnaGeometry.Vector3 end_pt_in)
        {
            start_pt = start_pt_in;
            end_pt = end_pt_in;
        }

        public CreatureBoneData(XnaGeometry.Vector4 start_pt_in, XnaGeometry.Vector4 end_pt_in)
        {
            start_pt = new XnaGeometry.Vector3(start_pt_in.X, start_pt_in.Y, start_pt_in.Z);
            end_pt = new XnaGeometry.Vector3(end_pt_in.X, end_pt_in.Y, end_pt_in.Z);
        }

        public XnaGeometry.Vector3 start_pt, end_pt;
    }

    // Class for animating the creature character
    public class CreatureAnimation
    {
        public string name;
        public float start_time, end_time;
        public MeshBoneCacheManager bones_cache;
        public MeshDisplacementCacheManager displacement_cache;
        public MeshUVWarpCacheManager uv_warp_cache;
        public MeshOpacityCacheManager opacity_cache;
        public List<List<float>> cache_pts;

        public CreatureAnimation(ref Dictionary<string, object> load_data,
                                   string name_in)
        {
            name = name_in;
            bones_cache = new MeshBoneCacheManager();
            displacement_cache = new MeshDisplacementCacheManager();
            uv_warp_cache = new MeshUVWarpCacheManager();
            opacity_cache = new MeshOpacityCacheManager();
            cache_pts = new List<List<float>>();

            LoadFromData(name_in, ref load_data);
        }

        public void LoadFromData(string name_in,
                                 ref Dictionary<string, object> load_data)
        {
            Dictionary<string, object> json_anim_base = (Dictionary<string, object>)load_data["animation"];
            Dictionary<string, object> json_clip = (Dictionary<string, object>)json_anim_base[name_in];

            MeshBoneUtil.CTuple<int, int> start_end_times = Utils.GetStartEndTimes(json_clip, "bones");
            start_time = start_end_times.Item1;
            end_time = start_end_times.Item2;

            // bone animation
            Utils.FillBoneCache(json_clip,
                                "bones",
                                (int)start_time,
                                (int)end_time,
                                ref bones_cache);

            // mesh deformation animation
            Utils.FillDeformationCache(json_clip,
                                       "meshes",
                                       (int)start_time,
                                       (int)end_time,
                                       ref displacement_cache);

            // uv swapping animation
            Utils.FillUVSwapCache(json_clip,
                                  "uv_swaps",
                                  (int)start_time,
                                  (int)end_time,
                                  ref uv_warp_cache);

            // opacity animation
            Utils.FillOpacityCache(json_clip,
                                   "mesh_opacities",
                                   (int)start_time,
                                   (int)end_time,
                                   ref opacity_cache);
        }

        int clipNum(int n, int lower, int upper)
        {
            return Math.Max(lower, Math.Min(n, upper));
        }

        int getIndexByTime(int time_in)
        {
            int retval = time_in - (int)start_time;
            retval = clipNum(retval, 0, (int)cache_pts.Count - 1);

            return retval;
        }

        public bool hasCachePts()
        {
            return cache_pts.Count != 0;
        }

        public float correctTime(float run_time, bool should_loop)
        {
            float ret_time = run_time;
            if (ret_time > end_time)
            {
                if(should_loop)
                {
                    ret_time = start_time;
                }
                else
                {
                    ret_time = end_time;
                }
            }
            else if (ret_time < start_time)
            {
                ret_time = end_time;
            }

            return ret_time;
        }

        public void poseFromCachePts(float time_in, List<float> target_pts, int num_pts)
        {
            int cur_floor_time = getIndexByTime((int)Math.Floor(time_in));
            int cur_ceil_time = getIndexByTime((int)Math.Ceiling(time_in));
            float cur_ratio = (time_in - start_time) - (float)cur_floor_time;

            List<float> set_pt = target_pts;
            List<float> floor_pts = cache_pts[cur_floor_time];
            List<float> ceil_pts = cache_pts[cur_ceil_time];

            int ref_idx = 0;
            for (int i = 0; i < num_pts; i++)
            {


                set_pt[0 + ref_idx] = ((1.0f - cur_ratio) * floor_pts[0 + ref_idx]) + (cur_ratio * ceil_pts[0 + ref_idx]);
                set_pt[1 + ref_idx] = ((1.0f - cur_ratio) * floor_pts[1 + ref_idx]) + (cur_ratio * ceil_pts[1 + ref_idx]);
                set_pt[2 + ref_idx] = ((1.0f - cur_ratio) * floor_pts[2 + ref_idx]) + (cur_ratio * ceil_pts[2 + ref_idx]);

                ref_idx += 3;
            }
        }
    }

    // Classes for mixing/blending animation bones from different clips into the current clip
    public class CreatureBonesBlend
    {
        public string anim_clip_name;
        public List<string> bone_names = new List<string>();
        public float blend_factor = 0.5f;
        public float run_time = 0.0f;
        public float delta_run_time = 1.0f;

        public CreatureBonesBlend(string anim_clip_name_in, List<string> bone_names_in, float blend_factor_in)
        {
            anim_clip_name = anim_clip_name_in;
            bone_names = bone_names_in;
            blend_factor = blend_factor_in;
        }
    }

    // Actual class to control mixing/blending animation bones from different clips into the current clip
    // This allows you to actually blend parts of an animation ( certain bones ) from one clip to the current clip
    // So you can blend in say some running motion of the arms into a walk cycle
    /* Usage: First create a class that inherits from CreatureGameAgent.
     * In the example below, we add a list of bones we intend to do do mixing on.
     * We instantiate a CreatureBonesBlend object with the appropriate parameters ( animation clip we want to mix with plus the blending factor and bone list )
     * We then add into into a CreatureModule.CreatureAnimBonesBlend object and assign it to the CreatureGameController
     * See implementation details on how the actual blending is performed in the CreatureAnimBonesBlend update() call.
        public class testHeroAgent : CreatureGameAgent
        {
            public override void initState()
            {
                var creature_renderer = game_controller.creature_renderer;

                var bones_list = new List<string>();
                bones_list.Add("BArm");
                bones_list.Add("BElbow");
                bones_list.Add("BHand");
                bones_list.Add("BWeapon");
     
                var bones_mix = new CreatureModule.CreatureBonesBlend("Run", bones_list, 0.5f);
                var bones_mix_manager = new CreatureModule.CreatureAnimBonesBlend();
                bones_mix_manager.bones_blend.Add(bones_mix);
                game_controller.anim_bones_blend = bones_mix_manager;
            }

            public override void updateStep()
            {

            }
        }
     * 
     */
    public class CreatureAnimBonesBlend
    {
        public List<CreatureBonesBlend> bones_blend = new List<CreatureBonesBlend>();
        Dictionary<string, MeshBoneUtil.CTuple<XnaGeometry.Vector4, XnaGeometry.Vector4>> ref_bone_positions =
            new Dictionary<string, MeshBoneUtil.CTuple<XnaGeometry.Vector4, XnaGeometry.Vector4>>();
        Dictionary<string, MeshBoneUtil.CTuple<XnaGeometry.Vector4, XnaGeometry.Vector4>> final_bone_positions =
            new Dictionary<string, MeshBoneUtil.CTuple<XnaGeometry.Vector4, XnaGeometry.Vector4>>();

        public void update(
            Dictionary<string, MeshBoneUtil.MeshBone> bones_map,
            CreatureModule.CreatureManager creature_manager
            )
        {
            var animations = creature_manager.GetAllAnimations();

            // Save out current positions
            foreach (var cur_data in bones_map)
            {
                ref_bone_positions[cur_data.Key] =
                    new MeshBoneUtil.CTuple<XnaGeometry.Vector4, XnaGeometry.Vector4>(
                        cur_data.Value.getWorldStartPt(),
                        cur_data.Value.getWorldEndPt());

                final_bone_positions[cur_data.Key] =
                    new MeshBoneUtil.CTuple<XnaGeometry.Vector4, XnaGeometry.Vector4>(
                        cur_data.Value.getWorldStartPt(),
                        cur_data.Value.getWorldEndPt());
            }

            // Now do blending
            foreach (var cur_blend in bones_blend)
            {
                var cur_anim = animations[cur_blend.anim_clip_name];
                creature_manager.PoseJustBones(cur_blend.anim_clip_name, cur_blend.run_time, false);

                cur_blend.run_time += cur_blend.delta_run_time;
                cur_blend.run_time = cur_anim.correctTime(cur_blend.run_time, true);

                foreach (var bone_name in cur_blend.bone_names)
                {
                    var set_bone = final_bone_positions[bone_name];
                    var new_start =
                        Utils.vec4Interp(
                            ref_bone_positions[bone_name].Item1,
                            bones_map[bone_name].getWorldStartPt(),
                            cur_blend.blend_factor);

                    var new_end =
                        Utils.vec4Interp(
                            ref_bone_positions[bone_name].Item2,
                            bones_map[bone_name].getWorldEndPt(),
                            cur_blend.blend_factor);

                    final_bone_positions[bone_name] =
                        new MeshBoneUtil.CTuple<XnaGeometry.Vector4, XnaGeometry.Vector4>(new_start, new_end);
                }
            }

            // Set actual bone positions
            foreach (var cur_data in bones_map)
            {
                cur_data.Value.setWorldStartPt(final_bone_positions[cur_data.Key].Item1);
                cur_data.Value.setWorldEndPt(final_bone_positions[cur_data.Key].Item2);
            }
        }
    }

    // Class for managing a collection of animations and a creature character
    public class CreatureManager
    {
        public Dictionary<string, CreatureModule.CreatureAnimation> animations;
        public CreatureModule.Creature target_creature;
        public string active_animation_name;
        public bool is_playing;
        public float run_time;
        public float time_scale;
        public List<List<float>> blend_render_pts;
        public bool do_blending;
        public float blending_factor;
        public Dictionary<string, float> region_override_alphas;
        public List<string> active_blend_animation_names;
        public List<string> auto_blend_names;
        public Dictionary<string, float> active_blend_run_times;
        public bool do_auto_blending;
        public float auto_blend_delta;
        public bool should_loop;
        public float region_offsets_z;
        public Action<Dictionary<string, MeshBone>> bones_override_callback;
        public Dictionary<string, CreatureBoneData> feedback_bones_map;

        public CreatureManager(CreatureModule.Creature target_creature_in)
        {
            target_creature = target_creature_in;
            is_playing = false;
            run_time = 0;
            time_scale = 30.0f;
            blending_factor = 0;
            region_offsets_z = 0.01f;
            animations = new Dictionary<string, CreatureAnimation>();
            bones_override_callback = null;
            region_override_alphas = new Dictionary<string, float>();

            blend_render_pts = new List<List<float>>();
            blend_render_pts.Add(new List<float>());
            blend_render_pts.Add(new List<float>());

            active_blend_animation_names = new List<string>();
            active_blend_animation_names.Add("");
            active_blend_animation_names.Add("");

            do_auto_blending = false;
            auto_blend_delta = 0.0f;

            auto_blend_names = new List<string>();
            auto_blend_names.Add("");
            auto_blend_names.Add("");

            active_blend_run_times = new Dictionary<string, float>();

            should_loop = true;

            feedback_bones_map = null;
        }

        // Create a point cache for a specific animation
        // This speeds up playback but you will lose the ability to directly
        // manipulate the bones.
        public void
        MakePointCache(String animation_name_in, int gapStep)
        {
            if (gapStep < 1)
            {
                gapStep = 1;
            }

            float store_run_time = getRunTime();
            CreatureAnimation cur_animation = animations[animation_name_in];
            if (cur_animation.hasCachePts())
            {
                // cache already generated, just exit
                return;
            }

            List<List<float>> cache_pts_list = cur_animation.cache_pts;

            //for(int i = (int)cur_animation.start_time; i <= (int)cur_animation.end_time; i++)
            int i = (int)cur_animation.start_time;
            while (true)
            {
                run_time = (float)i;
                List<float> new_pts = new List<float>(new float[target_creature.total_num_pts * 3]);
                UpdateRegionSwitches(animation_name_in);
                PoseCreature(animation_name_in, new_pts, getRunTime());

                int realStep = gapStep;
                if (i + realStep > cur_animation.end_time)
                {
                    realStep = (int)cur_animation.end_time - i;
                }

                bool firstCase = realStep > 1;
                bool secondCase = cache_pts_list.Count >= 1;
                if (firstCase && secondCase)
                {
                    // fill in the gaps
                    var prev_pts = cache_pts_list[cache_pts_list.Count - 1];
                    for (int j = 0; j < realStep; j++)
                    {
                        float factor = (float)j / (float)realStep;
                        var gap_pts = InterpFloatList(prev_pts, new_pts, factor);
                        cache_pts_list.Add(gap_pts);
                    }
                }

                cache_pts_list.Add(new_pts);
                i += realStep;

                if (i > cur_animation.end_time || realStep == 0)
                {
                    break;
                }
            }

            setRunTime(store_run_time);
        }

        public List<float> InterpFloatList(List<float> firstList, List<float> secondList, float factor)
        {
            List<float> ret_float_list = new List<float>(firstList.Count);
            for (int i = 0; i < firstList.Count; i++)
            {
                float new_val = ((1.0f - factor) * firstList[i]) + (factor * secondList[i]);
                ret_float_list.Add(new_val);
            }

            return ret_float_list;
        }

        // Create an animation
        public void CreateAnimation(ref Dictionary<string, object> load_data,
                                     string name_in)
        {
            CreatureModule.CreatureAnimation new_animation = new CreatureModule.CreatureAnimation(ref load_data,
                                                                                                   name_in);
            AddAnimation(new_animation);
        }

        // Create all animations
        public void CreateAllAnimations(ref Dictionary<string, object> load_data)
        {
            List<string> all_animation_names = Utils.GetAllAnimationNames(load_data);
            foreach (string cur_name in all_animation_names)
            {
                CreateAnimation(ref load_data, cur_name);
            }

            SetActiveAnimationName(all_animation_names[0]);
        }

        // Add an animation
        public void AddAnimation(CreatureModule.CreatureAnimation animation_in)
        {
            animations[animation_in.name] = animation_in;
            active_blend_run_times[animation_in.name] = animation_in.start_time;
        }

        // Return an animation
        public CreatureModule.CreatureAnimation
            GetAnimation(string name_in)
        {
            return animations[name_in];
        }

        // Return the creature
        public CreatureModule.Creature
            GetCreature()
        {
            return target_creature;
        }

        // Returns all the animation names
        public List<string> GetAnimationNames()
        {
            List<string> ret_names = new List<string>();
            foreach (string cur_name in animations.Keys)
            {
                ret_names.Add(cur_name);
            }

            return ret_names;
        }

        // Sets the current animation to be active by name
        public bool SetActiveAnimationName(string name_in)
        {
            if (name_in == null || animations.ContainsKey(name_in) == false)
            {
                return false;
            }

            active_animation_name = name_in;
            CreatureAnimation cur_animation = animations[active_animation_name];
            run_time = cur_animation.start_time;

            UpdateRegionSwitches(name_in);

            return true;
        }

        // Update the region switching properties
        private void UpdateRegionSwitches(string animation_name_in)
        {
            if (animations.ContainsKey(animation_name_in))
            {
                var cur_animation = animations[animation_name_in];

                var displacement_cache_manager = cur_animation.displacement_cache;
                var displacement_table =
                    displacement_cache_manager.displacement_cache_table[0];

                var uv_warp_cache_manager = cur_animation.uv_warp_cache;
                var uv_swap_table =
                    uv_warp_cache_manager.uv_cache_table[0];

                MeshBoneUtil.MeshRenderBoneComposition render_composition =
                    target_creature.render_composition;
                List<MeshBoneUtil.MeshRenderRegion> all_regions = render_composition.getRegions();

                int index = 0;
                foreach (MeshBoneUtil.MeshRenderRegion cur_region in all_regions)
                {
                    // Setup active or inactive displacements
                    bool use_local_displacements = !(displacement_table[index].getLocalDisplacements().Count == 0);
                    bool use_post_displacements = !(displacement_table[index].getPostDisplacements().Count == 0);
                    cur_region.setUseLocalDisplacements(use_local_displacements);
                    cur_region.setUsePostDisplacements(use_post_displacements);

                    // Setup active or inactive uv swaps
                    cur_region.setUseUvWarp(uv_swap_table[index].getEnabled());

                    index++;
                }
            }
        }

        // Returns the name of the currently active animation
        public string GetActiveAnimationName()
        {
            return active_animation_name;
        }

        // Returns the table of all animations
        public Dictionary<string, CreatureModule.CreatureAnimation>
            GetAllAnimations()
        {
            return animations;
        }

        // Returns if animation is playing
        bool GetIsPlaying()
        {
            return is_playing;
        }

        // Sets whether the animation is playing
        public void SetIsPlaying(bool flag_in)
        {
            is_playing = flag_in;
        }

        // Sets the run time of the animation
        public void setRunTime(float time_in)
        {
            run_time = time_in;
            correctTime();
        }

        // Increments the run time of the animation by a delta value
        public void increRunTime(float delta_in)
        {
            run_time += delta_in;
            correctTime();
        }

        public void correctTime()
        {
            CreatureAnimation cur_animation = animations[active_animation_name];
            run_time = cur_animation.correctTime(run_time, should_loop);
        }

        // Returns the current run time of the animation
        public float getRunTime()
        {
            return run_time;
        }

        // Returns current run time in consideration of blending
        public float getActualRuntime()
        {
            if (do_auto_blending)
            {
                if (active_blend_run_times.ContainsKey(active_animation_name))
                {
                    return active_blend_run_times[active_animation_name];
                }
            }

            return run_time;
        }

        // Runs a single step of the animation for a given delta timestep
        public void Update(float delta)
        {
            if (!is_playing)
            {
                return;
            }

            increRunTime(delta * time_scale);

            if (do_auto_blending)
            {
                ProcessAutoBlending();
                // process run times for blends
                IncreAutoBlendRunTimes(delta * time_scale);
            }

            RunCreature();
        }

        public void RunAtTime(float time_in)
        {
            if (!is_playing)
            {
                return;
            }

            setRunTime(time_in);
            RunCreature();
        }

        public void RunCreature()
        {
            if (do_blending)
            {
                for (int i = 0; i < 2; i++)
                {
                    string cur_animation_name = active_blend_animation_names[i];
                    CreatureAnimation cur_animation = animations[cur_animation_name];
                    float cur_animation_run_time = active_blend_run_times[cur_animation_name];

                    if (cur_animation.hasCachePts())
                    {
                        cur_animation.poseFromCachePts(cur_animation_run_time, blend_render_pts[i], target_creature.total_num_pts);
                        ApplyUVSwapsAndColorChanges(cur_animation_name, blend_render_pts[i], cur_animation_run_time);
                        PoseJustBones(cur_animation_name, cur_animation_run_time);
                    }
                    else
                    {
                        UpdateRegionSwitches(active_blend_animation_names[i]);
                        PoseCreature(active_blend_animation_names[i], blend_render_pts[i], cur_animation_run_time);
                    }

                    // Set feedback bones map for easy retrieval if available
                    if (feedback_bones_map != null)
                    {
                        Dictionary<string, MeshBoneUtil.MeshBone> bones_map =
                            target_creature.render_composition.getBonesMap();
                        foreach (var cur_bone_packet in bones_map)
                        {
                            XnaGeometry.Vector4 cur_bone_start_pt = new XnaGeometry.Vector4(0, 0, 0, 0);
                            XnaGeometry.Vector4 cur_bone_end_pt = new XnaGeometry.Vector4(0, 0, 0, 0);

                            if ((i != 0) && (feedback_bones_map.ContainsKey(cur_bone_packet.Key)))
                            {
                                var read_start = feedback_bones_map[cur_bone_packet.Key].start_pt;
                                var read_end = feedback_bones_map[cur_bone_packet.Key].end_pt;
                                cur_bone_start_pt = new XnaGeometry.Vector4(read_start.X, read_start.Y, read_start.Z, 0);
                                cur_bone_end_pt = new XnaGeometry.Vector4(read_end.X, read_end.Y, read_end.Z, 0);
                            }

                            var set_blend_factor = (i == 0) ? (1.0f - blending_factor) : blending_factor;

                            var set_start_pt = set_blend_factor * cur_bone_packet.Value.getWorldStartPt() + cur_bone_start_pt;
                            var set_end_pt = set_blend_factor * cur_bone_packet.Value.getWorldEndPt() + cur_bone_end_pt;

                            feedback_bones_map[cur_bone_packet.Key] = new CreatureBoneData(set_start_pt, set_end_pt);
                        }
                    }
                }

                for (int j = 0; j < target_creature.total_num_pts * 3; j++)
                {
                    int set_data_index = j;
                    float read_data_1 = blend_render_pts[0][j];
                    float read_data_2 = blend_render_pts[1][j];

                    target_creature.render_pts[set_data_index] =
                        ((1.0f - blending_factor) * (read_data_1)) +
                            (blending_factor * (read_data_2));
                }
            }
            else
            {
                CreatureAnimation cur_animation = animations[active_animation_name];
                if (cur_animation.hasCachePts())
                {
                    cur_animation.poseFromCachePts(getRunTime(), target_creature.render_pts, target_creature.total_num_pts);
                    ApplyUVSwapsAndColorChanges(active_animation_name, target_creature.render_pts, getRunTime());
                    PoseJustBones(cur_animation.name, getRunTime());
                }
                else
                {
                    PoseCreature(active_animation_name, target_creature.render_pts, getRunTime());
                }

                // Set feedback bones map for easy retrieval if available
                if(feedback_bones_map != null)
                {
                    Dictionary<string, MeshBoneUtil.MeshBone> bones_map = 
                        target_creature.render_composition.getBonesMap();
                    foreach(var cur_bone_packet in bones_map)
                    {
                        feedback_bones_map[cur_bone_packet.Key] =
                            new CreatureBoneData(
                                cur_bone_packet.Value.getWorldStartPt(),
                                cur_bone_packet.Value.getWorldEndPt());
                    }
                }
            }

            RunUVItemSwap();
        }

        // Sets scaling for time
        public void SetTimeScale(float scale_in)
        {
            time_scale = scale_in;
        }

        // Enables/Disables blending
        public void SetBlending(bool flag_in)
        {
            do_blending = flag_in;

            if (do_blending)
            {
                if (blend_render_pts[0].Count == 0)
                {
                    blend_render_pts[0] = new List<float>(new float[target_creature.total_num_pts * 3]);
                }

                if (blend_render_pts[1].Count == 0)
                {
                    blend_render_pts[1] = new List<float>(new float[target_creature.total_num_pts * 3]);
                }

            }
        }

        // Sets auto blending
        public void SetAutoBlending(bool flag_in)
        {
            do_auto_blending = flag_in;
            SetBlending(flag_in);

            if (do_auto_blending)
            {
                AutoBlendTo(active_animation_name, 0.1f);
            }
        }

        // Use auto blending to blend to the next animation
        public void AutoBlendTo(string animation_name_in, float blend_delta)
        {
            if (animation_name_in == auto_blend_names[1])
            {
                // already blending to that so just return
                return;
            }

            ResetBlendTime(animation_name_in);
            active_blend_run_times[active_animation_name] = getActualRuntime();

            auto_blend_delta = blend_delta;
            auto_blend_names[0] = active_animation_name;
            auto_blend_names[1] = animation_name_in;
            blending_factor = 0;

            active_animation_name = animation_name_in;

            SetBlendingAnimations(auto_blend_names[0], auto_blend_names[1]);
        }

        public void ResetBlendTime(string name_in)
        {
            CreatureAnimation cur_animation = animations[name_in];
            active_blend_run_times[name_in] = cur_animation.start_time;
        }

        public float GetActiveAnimationStartTime()
        {
            if (animations.ContainsKey(active_animation_name) == false)
            {
                return 0;
            }

            CreatureAnimation cur_animation = animations[active_animation_name];
            return cur_animation.start_time;
        }

        public float GetActiveAnimationEndTime()
        {
            if (animations.ContainsKey(active_animation_name) == false)
            {
                return 0;
            }

            CreatureAnimation cur_animation = animations[active_animation_name];
            return cur_animation.end_time;
        }

        // Resets animation to start time
        public void ResetToStartTimes()
        {
            if (animations.ContainsKey(active_animation_name) == false)
            {
                return;
            }

            // reset non blend time
            CreatureAnimation cur_animation = animations[active_animation_name];
            run_time = cur_animation.start_time;

            // reset blend times too
            foreach (KeyValuePair<string, float> blend_time_data in active_blend_run_times)
            {
                ResetBlendTime(blend_time_data.Key);
            }
        }

        private void ProcessAutoBlending()
        {
            // process blending factor
            blending_factor += auto_blend_delta;
            if (blending_factor > 1)
            {
                blending_factor = 1;
            }
        }

        private void IncreAutoBlendRunTimes(float delta_in)
        {
            string set_animation_name = "";
            foreach (string cur_animation_name in auto_blend_names)
            {
                if ((animations.ContainsKey(cur_animation_name))
                    && (set_animation_name.Equals(cur_animation_name) == false))
                {
                    float cur_run_time = active_blend_run_times[cur_animation_name];
                    cur_run_time += delta_in;
                    cur_run_time = correctRunTime(cur_run_time, cur_animation_name);

                    active_blend_run_times[cur_animation_name] = cur_run_time;

                    set_animation_name = cur_animation_name;
                }
            }
        }

        private float correctRunTime(float time_in, string animation_name)
        {
            float ret_time = time_in;
            CreatureAnimation cur_animation = animations[animation_name];
            float anim_start_time = cur_animation.start_time;
            float anim_end_time = cur_animation.end_time;

            if (ret_time > anim_end_time)
            {
                if (should_loop)
                {
                    ret_time = anim_start_time;
                }
                else
                {
                    ret_time = anim_end_time;
                }
            }
            else if (ret_time < anim_start_time)
            {
                if (should_loop)
                {
                    ret_time = anim_end_time;
                }
                else
                {
                    ret_time = anim_start_time;
                }
            }

            return ret_time;
        }

        // Sets blending animation names
        public void SetBlendingAnimations(string name_1, string name_2)
        {
            active_blend_animation_names[0] = name_1;
            active_blend_animation_names[1] = name_2;
        }

        // Sets the blending factor
        public void SetBlendingFactor(float value_in)
        {
            blending_factor = value_in;
        }

        // Given a set of coordinates in local creature space,
        // see if any bone is in contact
        public string IsContactBone(XnaGeometry.Vector2 pt_in,
                                  float radius)
        {
            MeshBoneUtil.MeshBone cur_bone = target_creature.render_composition.getRootBone();
            return ProcessContactBone(pt_in, radius, cur_bone);
        }

        public string ProcessContactBone(XnaGeometry.Vector2 pt_in,
                                         float radius,
                                         MeshBoneUtil.MeshBone bone_in)
        {
            string ret_name = "";
            XnaGeometry.Vector4 diff_vec = bone_in.getWorldEndPt() - bone_in.getWorldStartPt();

            XnaGeometry.Vector2 cur_vec = new XnaGeometry.Vector2(diff_vec.X, diff_vec.Y);
            float cur_length = (float)cur_vec.Length();

            XnaGeometry.Vector2 unit_vec = cur_vec;
            unit_vec.Normalize();

            XnaGeometry.Vector2 norm_vec = new XnaGeometry.Vector2(unit_vec.Y, unit_vec.X);

            XnaGeometry.Vector2 src_pt = new XnaGeometry.Vector2(bone_in.getWorldStartPt().X, bone_in.getWorldStartPt().Y);
            XnaGeometry.Vector2 rel_vec = pt_in - src_pt;
            float proj = (float)XnaGeometry.Vector2.Dot(rel_vec, unit_vec);

            if ((proj >= 0) && (proj <= cur_length))
            {
                float norm_proj = (float)XnaGeometry.Vector2.Dot(rel_vec, norm_vec);
                if (norm_proj <= radius)
                {
                    return bone_in.getKey();
                }
            }

            List<MeshBone> cur_children = bone_in.getChildren();
            foreach (MeshBone cur_child in cur_children)
            {
                ret_name = ProcessContactBone(pt_in, radius, cur_child);
                if (!(ret_name.Equals("")))
                {
                    break;
                }
            }

            return ret_name;
        }

        public void UpdateRegionColours()
        {
            MeshBoneUtil.MeshRenderBoneComposition render_composition =
                target_creature.render_composition;
            List<MeshBoneUtil.MeshRenderRegion> cur_regions =
                render_composition.getRegions();

            for (int i = 0; i < cur_regions.Count; i++)
            {
                MeshBoneUtil.MeshRenderRegion cur_region = cur_regions[i];
                float read_val = cur_region.opacity;

                // see if there is an override alpha as well
                if (region_override_alphas.ContainsKey(cur_region.name))
                {
                    read_val = region_override_alphas[cur_region.name];
                }

                if (read_val < 0.0f)
                {
                    read_val = 0.0f;
                }
                else if (read_val > 100.0f)
                {
                    read_val = 100.0f;
                }

                float opacity_factor = read_val / 100.0f;
                byte set_opacity = (byte)(opacity_factor * 255.0f);
                byte set_r = (byte)(cur_region.red / 100.0f * opacity_factor * 255.0f);
                byte set_g = (byte)(cur_region.green / 100.0f * opacity_factor * 255.0f);
                byte set_b = (byte)(cur_region.blue / 100.0f * opacity_factor * 255.0f);

                int cur_rgba_index = cur_region.getStartPtIndex() * 4;
                for (int j = 0; j < cur_region.getNumPts(); j++)
                {
                    target_creature.render_colours[cur_rgba_index] = set_r;
                    target_creature.render_colours[cur_rgba_index + 1] = set_g;
                    target_creature.render_colours[cur_rgba_index + 2] = set_b;
                    target_creature.render_colours[cur_rgba_index + 3] = set_opacity;

                    cur_rgba_index += 4;
                }

            }
        }

        // Sets an override opacity/alpha value for a region
        public void SetOverrideRegionAlpha(string region_name_in, float value_in)
        {
            region_override_alphas[region_name_in] = value_in;
        }

        private void ApplyUVSwapsAndColorChanges(string animation_name_in,
                                                 List<float> target_pts,
                                                 float input_run_time)
        {
            CreatureAnimation cur_animation = animations[animation_name_in];

            MeshBoneUtil.MeshUVWarpCacheManager uv_warp_cache_manager = cur_animation.uv_warp_cache;
            MeshBoneUtil.MeshOpacityCacheManager opacity_cache_manager = cur_animation.opacity_cache;

            MeshBoneUtil.MeshRenderBoneComposition render_composition =
                target_creature.render_composition;

            Dictionary<string, MeshBoneUtil.MeshRenderRegion> regions_map =
                render_composition.getRegionsMap();

            uv_warp_cache_manager.retrieveValuesAtTime(input_run_time,
                                                       regions_map);

            opacity_cache_manager.retrieveValuesAtTime(input_run_time,
                                                       regions_map);

            UpdateRegionColours();

            List<MeshBoneUtil.MeshRenderRegion> cur_regions =
                render_composition.getRegions();
            for (int j = 0; j < cur_regions.Count; j++)
            {
                MeshBoneUtil.MeshRenderRegion cur_region = cur_regions[j];

                if (cur_region.use_uv_warp)
                {
                    cur_region.runUvWarp();
                }

                // add in z offsets for different regions
                for (int k = cur_region.getStartPtIndex() * 3;
                    k <= cur_region.getEndPtIndex() * 3;
                    k += 3)
                {
                    target_pts[k + 2] = j * region_offsets_z;
                }
            }
        }

        public void PoseJustBones(string animation_name_in,
                                  float input_run_time,
                                  bool run_callback=true)
        {
            CreatureAnimation cur_animation = animations[animation_name_in];
            MeshBoneUtil.MeshRenderBoneComposition render_composition =
                target_creature.render_composition;

            MeshBoneUtil.MeshBoneCacheManager bone_cache_manager = cur_animation.bones_cache;
            Dictionary<string, MeshBoneUtil.MeshBone> bones_map =
                render_composition.getBonesMap();

            bone_cache_manager.retrieveValuesAtTime(input_run_time,
                                                    ref bones_map);

            AlterBonesByAnchor(bones_map, animation_name_in);

            if ((bones_override_callback != null) && run_callback)
            {
                bones_override_callback(bones_map);
            }
        }

        public void RunUVItemSwap()
        {
            MeshBoneUtil.MeshRenderBoneComposition render_composition =
                target_creature.render_composition;
            Dictionary<string, MeshBoneUtil.MeshRenderRegion> regions_map =
                render_composition.getRegionsMap();

            var swap_packets = target_creature.uv_swap_packets;
            var active_swap_actions = target_creature.active_uv_swap_actions;

            if ((swap_packets.Count == 0) || (active_swap_actions.Count == 0))
            {
                return;
            }

            foreach (var cur_action in active_swap_actions)
            {
                if (regions_map.ContainsKey(cur_action.Key))
                {
                    var swap_tag = cur_action.Value;
                    var swap_list = swap_packets[cur_action.Key];
                    foreach (var cur_item in swap_list)
                    {
                        if (cur_item.tag == swap_tag)
                        {
                            // Perform UV Item Swap
                            var cur_region = regions_map[cur_action.Key];
                            cur_region.setUvWarpLocalOffset(cur_item.local_offset);
                            cur_region.setUvWarpGlobalOffset(cur_item.global_offset);
                            cur_region.setUvWarpScale(cur_item.scale);
                            cur_region.runUvWarp();

                            break;
                        }
                    }
                }
            }
        }

        public void AlterBonesByAnchor(Dictionary<string, MeshBoneUtil.MeshBone> bones_map, String animation_name_in)
        {
            if (target_creature.anchor_points_active == false)
            {
                return;
            }

            var anchor_point = target_creature.GetAnchorPoint(animation_name_in);
            var anchor_vector = new XnaGeometry.Vector4(anchor_point.X, anchor_point.Y, 0, 0);
            foreach (var cur_data in bones_map)
            {
                var cur_bone = cur_data.Value;
                var start_pt = cur_bone.getWorldStartPt();
                var end_pt = cur_bone.getWorldEndPt();

                start_pt -= anchor_vector;
                end_pt -= anchor_vector;

                cur_bone.setWorldStartPt(start_pt);
                cur_bone.setWorldEndPt(end_pt);
            }
        }

        public void PoseCreature(string animation_name_in,
                                   List<float> target_pts,
                                 float input_run_time)
        {
            CreatureAnimation cur_animation = animations[animation_name_in];

            MeshBoneUtil.MeshBoneCacheManager bone_cache_manager = cur_animation.bones_cache;
            MeshBoneUtil.MeshDisplacementCacheManager displacement_cache_manager = cur_animation.displacement_cache;
            MeshBoneUtil.MeshUVWarpCacheManager uv_warp_cache_manager = cur_animation.uv_warp_cache;
            MeshBoneUtil.MeshOpacityCacheManager opacity_cache_manager = cur_animation.opacity_cache;

            MeshBoneUtil.MeshRenderBoneComposition render_composition =
                target_creature.render_composition;

            // Extract values from caches
            Dictionary<string, MeshBoneUtil.MeshBone> bones_map =
                render_composition.getBonesMap();
            Dictionary<string, MeshBoneUtil.MeshRenderRegion> regions_map =
                render_composition.getRegionsMap();

            bone_cache_manager.retrieveValuesAtTime(input_run_time,
                                                    ref bones_map);

            AlterBonesByAnchor(bones_map, animation_name_in);

            if (bones_override_callback != null)
            {
                bones_override_callback(bones_map);
            }

            displacement_cache_manager.retrieveValuesAtTime(input_run_time,
                                                            regions_map);
            uv_warp_cache_manager.retrieveValuesAtTime(input_run_time,
                                                       regions_map);

            opacity_cache_manager.retrieveValuesAtTime(input_run_time,
                                                       regions_map);

            UpdateRegionColours();

            // Do posing, decide if we are blending or not
            List<MeshBoneUtil.MeshRenderRegion> cur_regions =
                render_composition.getRegions();
            Dictionary<string, MeshBoneUtil.MeshBone> cur_bones =
                render_composition.getBonesMap();

            render_composition.updateAllTransforms(false);
            for (int j = 0; j < cur_regions.Count; j++)
            {
                MeshBoneUtil.MeshRenderRegion cur_region = cur_regions[j];

                int cur_pt_index = cur_region.getStartPtIndex();

                cur_region.poseFinalPts(ref target_pts,
                                        cur_pt_index * 3,
                                         ref cur_bones);

                // add in z offsets for different regions
                for (int k = cur_region.getStartPtIndex() * 3;
                    k <= cur_region.getEndPtIndex() * 3;
                    k += 3)
                {
                    target_pts[k + 2] = j * region_offsets_z;
                }
            }
        }


    }

}

