using System;
using System.IO;
using System.Collections.Generic;
using CreatureModule;
using MeshBoneUtil;
using UnityEngine;

namespace CreatureModule
{
    public class CreatureRenderModule
    {
        public static Mesh createMesh()
        {
            Mesh new_mesh = new Mesh();
            new_mesh.name = "Creature Mesh Object";
            new_mesh.hideFlags = HideFlags.HideAndDontSave;
            new_mesh.MarkDynamic();
            return new_mesh;
        }

        public static void SetIndexBuffer(List<int> input_list, int[] output_array, bool do_counterclockwise)
        {
            if (!do_counterclockwise)
            {
                for (int i = 0; i < input_list.Count; i++)
                {
                    output_array[i] = input_list[i];
                }
            }
            else
            {
                for (int i = 0; i < input_list.Count; i += 3)
                {
                    output_array[i] = input_list[i];
                    output_array[i + 1] = input_list[i + 2];
                    output_array[i + 2] = input_list[i + 1];
                }
            }
        }

        public static bool shouldSkinSwap(CreatureAsset creature_asset, bool skin_swap_active, ref int[] skin_swap_triangles)
        {
            return (creature_asset.creature_meta_data != null) &&
                skin_swap_active &&
                (skin_swap_triangles != null);
        }

        public static bool AddSkinSwap(CreatureAsset creature_asset, String swap_name, HashSet<String> swap_set)
        {
            if (creature_asset.creature_meta_data == null)
            {
                return false;
            }

            if (creature_asset.creature_meta_data.skin_swaps.ContainsKey(swap_name))
            {
                return false;
            }

            creature_asset.creature_meta_data.skin_swaps[swap_name] = swap_set;

            return true;
        }

        public static void EnableSkinSwap(
            String swap_name_in,
            bool active,
            CreatureManager creature_manager,
            CreatureAsset creature_asset,
            ref bool skin_swap_active,
            ref string skin_swap_name,
            ref int[] skin_swap_triangles,
            ref List<int> final_skin_swap_indices)
        {
            skin_swap_active = active;
            if (!skin_swap_active)
            {
                skin_swap_name = "";
                skin_swap_triangles = null;
                final_skin_swap_indices = null;
            }
            else
            {
                skin_swap_name = swap_name_in;
                if (creature_asset.creature_meta_data != null)
                {
                    final_skin_swap_indices = new List<int>();
                    creature_asset.creature_meta_data.buildSkinSwapIndices(
                        skin_swap_name,
                        creature_manager.GetCreature().render_composition,
                        final_skin_swap_indices);
                    skin_swap_triangles = new int[final_skin_swap_indices.Count];
                }
            }
        }

        public static void CreateRenderingData(
            CreatureManager creature_manager,
            ref Vector3[] vertices,
            ref Vector3[] normals,
            ref Vector4[] tangents,
            ref Color32[] colors,
            ref Vector2[] uvs,
            ref int[] triangles,
            ref List<int> final_indices)
        {
            vertices = new Vector3[creature_manager.target_creature.total_num_pts];
            normals = new Vector3[creature_manager.target_creature.total_num_pts];
            tangents = new Vector4[creature_manager.target_creature.total_num_pts];
            colors = new Color32[creature_manager.target_creature.total_num_pts];
            uvs = new Vector2[creature_manager.target_creature.total_num_pts];
            triangles = new int[creature_manager.target_creature.total_num_indices];
            final_indices = new List<int>(new int[creature_manager.target_creature.total_num_indices]);
        }

        public static void UpdateRenderingData(
            CreatureManager creature_manager, 
            bool counter_clockwise,
            ref Vector3[] vertices,
            ref Vector3[] normals,
            ref Vector4[] tangents,
            ref Color32[] colors,
            ref Vector2[] uvs,
            CreatureAsset creature_asset,
            bool skin_swap_active,
            string active_animation_name,
            ref List<int> final_indices,
            ref List<int> final_skin_swap_indices,
            ref int[] triangles,
            ref int[] skin_swap_triangles)
        {
            int pt_index = 0;
            int uv_index = 0;
            int color_index = 0;

            List<float> render_pts = creature_manager.target_creature.render_pts;
            List<float> render_uvs = creature_manager.target_creature.global_uvs;
            List<byte> render_colors = creature_manager.target_creature.render_colours;
            float normal_z = 1.0f;
            if (counter_clockwise)
            {
                normal_z = -1.0f;
            }

            for (int i = 0; i < creature_manager.target_creature.total_num_pts; i++)
            {
                vertices[i].x = render_pts[pt_index + 0];
                vertices[i].y = render_pts[pt_index + 1];
                vertices[i].z = render_pts[pt_index + 2];

                normals[i].x = 0;
                normals[i].y = 0;
                normals[i].z = normal_z;

                tangents[i].x = 1.0f;
                tangents[i].y = 0;
                tangents[i].z = 0;

                uvs[i].x = render_uvs[uv_index + 0];
                uvs[i].y = 1.0f - render_uvs[uv_index + 1];

                colors[i].r = render_colors[color_index + 0];
                colors[i].g = render_colors[color_index + 1];
                colors[i].b = render_colors[color_index + 2];
                colors[i].a = render_colors[color_index + 3];

                pt_index += 3;
                uv_index += 2;
                color_index += 4;
            }

            List<int> render_indices = creature_manager.target_creature.global_indices;
            var real_run_time = creature_manager.run_time;
            if (creature_manager.do_blending &&
                creature_manager.active_blend_run_times.ContainsKey(active_animation_name))
            {
                real_run_time = creature_manager.active_blend_run_times[active_animation_name];
            }

            bool is_animate_order = false;
            if(creature_asset.creature_meta_data != null)
            {
                is_animate_order = creature_asset.creature_meta_data.hasAnimatedOrder(active_animation_name, (int)real_run_time);
            }

            // index re-ordering
            if (creature_asset.creature_meta_data != null)
            {
                if (is_animate_order)
                {
                    // do index re-ordering
                    creature_asset.creature_meta_data.updateIndicesAndPoints(
                        final_indices,
                        render_indices,
                        render_pts,
                        0,
                        creature_manager.target_creature.total_num_indices,
                        creature_manager.target_creature.total_num_pts,
                        active_animation_name,
                        skin_swap_active,
                        (int)real_run_time);

                    if(skin_swap_active)
                    {
                        for(int i = 0; i < final_skin_swap_indices.Count; i++)
                        {
                            final_skin_swap_indices[i] = final_indices[i];
                        }
                        SetIndexBuffer(final_skin_swap_indices, skin_swap_triangles, counter_clockwise);
                    }
                    else
                    {
                        SetIndexBuffer(final_indices, triangles, counter_clockwise);
                    }
                }
            }

            if (shouldSkinSwap(creature_asset, skin_swap_active, ref skin_swap_triangles) && (is_animate_order == false))
            {
                // Skin Swap with no Animated Ordering
                SetIndexBuffer(final_skin_swap_indices, skin_swap_triangles, counter_clockwise);
            }
            else if(is_animate_order == false)
            {
                // plain copy
                for (int i = 0; i < render_indices.Count; i++)
                {
                    final_indices[i] = render_indices[i];
                }
                SetIndexBuffer(final_indices, triangles, counter_clockwise);
            }
        }

        public static void UpdateTime(
            CreatureManager creature_manager, 
            CreatureGameController game_controller,
            CreatureMetaData meta_data,
            string active_animation_name,
            float local_time_scale,
            float region_offsets_z,
            bool should_loop,
            ref float local_time)
        {
            if (active_animation_name == null || active_animation_name.Length == 0)
            {
                return;
            }

            bool morph_targets_valid = false;
            if (game_controller != null) {
                if ((meta_data != null) && game_controller.morph_targets_active)
                {
                    morph_targets_valid = meta_data.morph_data.isValid();
                }
            }

            var old_time = creature_manager.getActualRuntime();
            float time_delta = (Time.deltaTime * local_time_scale);

            creature_manager.region_offsets_z = region_offsets_z;
            creature_manager.should_loop = should_loop;

            if(morph_targets_valid)
            {
                game_controller.computeMorphTargetsPt();
                meta_data.updateMorphStep(creature_manager, time_delta);
            }
            else
            {
                creature_manager.Update(time_delta);
            }

            local_time = creature_manager.getActualRuntime();

            bool reached_anim_end = false;
            if ((local_time < old_time) && (game_controller != null))
            {
                game_controller.AnimClipFrameResetEvent();
                reached_anim_end = true;
            }

            if (local_time >= creature_manager.GetAnimation(active_animation_name).end_time)
            {
                reached_anim_end = true;
            }

            if (reached_anim_end && (game_controller != null))
            {
                game_controller.AnimationReachedEnd(active_animation_name);
            }
        }
        public static void debugDrawBones(MeshBone bone_in)
        {
            XnaGeometry.Vector4 pt1 = bone_in.world_start_pt;
            XnaGeometry.Vector4 pt2 = bone_in.world_end_pt;

            Debug.DrawLine(new Vector3((float)pt1.X, (float)pt1.Y, 0),
                            new Vector3((float)pt2.X, (float)pt2.Y, 0), Color.white);

            foreach (MeshBone cur_child in bone_in.children)
            {
                debugDrawBones(cur_child);
            }
        }
    }
}
