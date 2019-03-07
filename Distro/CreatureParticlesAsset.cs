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
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreatureParticlesAsset : MonoBehaviour
{
    public TextAsset particlesData;
    private List<String> animation_clipnames = new List<string>();

#if UNITY_EDITOR
    [MenuItem("GameObject/Creature/CreatureParticlesAsset")]
    static CreatureParticlesAsset CreateParticlesAsset()
    {
        GameObject newObj = new GameObject();
        newObj.name = "New Creature Particles Asset";
        return newObj.AddComponent<CreatureParticlesAsset>() as CreatureParticlesAsset;
    }
#endif

    public struct particlesOffsetsData
    {
        public int offset;
        public int num_particles;

        public particlesOffsetsData(int offset_in = -1, int num_particles_in = 0)
        {
            offset = offset_in;
            num_particles = num_particles_in;
        }
    }

    public struct clipOffsetsData
    {
        public int base_idx;
        public int num_frames;
        public string name;
        // <Frame, <layer idx, particlesOffsetsData>>
        public Dictionary<int, Dictionary<int, particlesOffsetsData>> particles_lookup;

        public clipOffsetsData(int base_idx_in, int num_frames_in, string name_in)
        {
            base_idx = base_idx_in;
            num_frames = num_frames_in;
            name = name_in;
            particles_lookup = new Dictionary<int, Dictionary<int, particlesOffsetsData>>();
        }
    };

    private object[] m_PackData;
    private Dictionary<string, clipOffsetsData> m_ClipDataOffsets = new Dictionary<string, clipOffsetsData>();
    private int m_maxParticlesNum = 0;
    private int m_maxIndicesNum = 0;

    public object[] getPackData()
    {
        if (particlesData == null)
        {
            Debug.Log("<color=red>Error: </color>Please check to see that particlesData is assigned for Particles to run!");
            return null;
        }

        if (m_PackData == null)
        {
            Stream readStream = new MemoryStream(particlesData.bytes);
            var newReader = new MPacker.MPacker();
            m_PackData = (object[])newReader.Unpack(readStream);
            computeDataInfo(m_PackData);
        }

        return m_PackData;
    }

    public int getVersion(object[] pack_objs)
    {
        return (int)pack_objs[0];
    }

    public int getGapStep(object[] pack_objs)
    {
        return (int)pack_objs[1];
    }

    public int getAnimClipsNum(object[] pack_objs)
    {
        return (int)pack_objs[2];
    }

    public string[] getAnimClipNames(object[] pack_objs)
    {
        string[] ret_names = new string[m_ClipDataOffsets.Count];
        int i = 0;
        foreach (var cur_data in m_ClipDataOffsets)
        {
            ret_names[i] = cur_data.Key;
            i++;
        }

        return ret_names;
    }

    public void computeDataInfo(object[] pack_objs)
    {
        Dictionary<int, int> max_lparticles_num = new Dictionary<int, int>();
        animation_clipnames.Clear();
        m_ClipDataOffsets.Clear();

        var num_clips = getAnimClipsNum(pack_objs);
        int offset = 2;
        for (int i = 0; i < num_clips; i++)
        {
            clipOffsetsData new_offsets_data = new clipOffsetsData(-1, 0, "");

            // name
            offset++;
            new_offsets_data.base_idx = offset;
            new_offsets_data.name = (string)pack_objs[offset];
            animation_clipnames.Add(new_offsets_data.name);
            Debug.Log("Creating Particles Data for: " + animation_clipnames[animation_clipnames.Count - 1]);

            // num frames
            offset++;
            int num_frames = (int)pack_objs[offset];
            new_offsets_data.num_frames = num_frames;

            for (int j = 0; j < num_frames; j++)
            {
                // num layers
                offset++;
                int num_layers = (int)pack_objs[offset];
                Dictionary<int, particlesOffsetsData> particles_layers_map = new Dictionary<int, particlesOffsetsData>();

                for (int k = 0; k < num_layers; k++)
                {
                    // layer idx
                    offset++;
                    int layer_idx = (int)pack_objs[offset];
                    if (!max_lparticles_num.ContainsKey(layer_idx))
                    {
                        max_lparticles_num.Add(layer_idx, 0);
                    }

                    // num particles
                    offset++;
                    int num_particles = (int)pack_objs[offset];

                    int local_max_particles = 0;
                    for (int m = 0; m < num_particles; m++)
                    {
                        local_max_particles += (((object[])pack_objs[offset + (5 * m) + 1]).Length / 2);
                    }
                    max_lparticles_num[layer_idx] = Math.Max(max_lparticles_num[layer_idx], local_max_particles);

                    particles_layers_map.Add(layer_idx, new particlesOffsetsData(offset + 1, num_particles));
                    for (int m = 0; m < num_particles; m++)
                    {
                        // particles
                        offset += 5;
                    }
                }

                new_offsets_data.particles_lookup.Add(j, particles_layers_map);
            }

            m_ClipDataOffsets.Add(new_offsets_data.name, new_offsets_data);
        }

        m_maxParticlesNum = 0;
        foreach (var cur_data in max_lparticles_num)
        {
            m_maxParticlesNum = Math.Max(m_maxParticlesNum, cur_data.Value);
        }

        m_maxIndicesNum = 3 * 2 * m_maxParticlesNum;
        Debug.Log("Total Particles Sequences: " + m_ClipDataOffsets.Count.ToString());
    }

    public class ParticlesMeshModifier : CreatureMeshModifier
    {
        public CreatureParticlesAsset p_asset;
        public ParticlesMeshModifier(int num_indices_in, int num_pts_in, CreatureParticlesAsset p_asset_in)
            : base(num_indices_in, num_pts_in)
        {
            p_asset = p_asset_in;
        }

        public override void initData(CreatureManager creature_manager, CreatureAsset creature_asset)
        {
            var cur_char = creature_manager.GetCreature();
            m_isValid = false;
            m_numIndices = cur_char.total_num_indices;
        }

        public void setVertData(
            Vector2 pt_in,
            float pt_z,
            Vector2 uv_in,
            Vector4 color_in,
            ref int pt_offset,
            ref int uv_offset,
            ref int colors_offset,
            ref Vector3[] vertices,
            ref Vector2[] uvs,
            ref Color32[] colors)
        {
            Vector3[] set_pts = vertices;
            Vector2[] set_uvs = uvs;
            Color32[] set_colors = colors;

            // set points
            int offset_idx = pt_offset / 3;
            set_pts[offset_idx].x = pt_in.x;
            set_pts[offset_idx].y = pt_in.y;
            set_pts[offset_idx].z = pt_z;
            pt_offset += 3;

            // set uvs
            offset_idx = uv_offset / 2;
            set_uvs[offset_idx].x = uv_in.x;
            set_uvs[offset_idx].y = uv_in.y;
            uv_offset += 2;

            // set color
            set_colors[colors_offset] = new Color32((byte)(color_in.x * 255.0f), (byte)(color_in.y * 255.0f), (byte)(color_in.z * 255.0f), (byte)(color_in.w * 255.0f));
            colors_offset += 1;
        }

        public void setIndiceData(ref int offset, int val, ref List<int> final_indices)
        {
            final_indices[offset] = val;
            offset++;
        }

        public Vector2 rotVec2D(Vector2 vec_in, float angle)
        {
            var ret_vec = new Vector2(vec_in.x, vec_in.y);
            ret_vec.x = vec_in.x * (float)Math.Cos((double)angle) - vec_in.y * (float)Math.Sin((double)angle);
            ret_vec.y = vec_in.x * (float)Math.Sin((double)angle) + vec_in.y * (float)Math.Cos((double)angle);
            return ret_vec;
        }

        public Vector2 rotVec2D_90(Vector2 vec_in)
        {
            var ret_vec = new Vector2(vec_in.x, vec_in.y);
            ret_vec.x = -vec_in.y;
            ret_vec.y = vec_in.x;
            return ret_vec;
        }

        public void CopyFloatsToVec3Array(List<float> vals_in, int offset_in, int num_in, ref Vector3[] array_out, int offset_out)
        {
            for (int i = 0; i < num_in; i += 3)
            {
                int base_i = i + offset_in;
                array_out[(i / 3) + offset_out] = new Vector3(vals_in[base_i], vals_in[base_i + 1], vals_in[base_i + 2]);
            }
        }

        public void CopyFloatsToUVArray(List<float> vals_in, int offset_in, int num_in, ref Vector2[] array_out, int offset_out)
        {
            for (int i = 0; i < num_in; i += 2)
            {
                int base_i = i + offset_in;
                array_out[(i / 2) + offset_out] = new Vector2(vals_in[base_i], 1.0f - vals_in[base_i + 1]);
            }
        }

        public void CopyBytesToColorArray(List<byte> vals_in, int offset_in, int num_in, ref Color32[] array_out, int offset_out)
        {
            for (int i = 0; i < num_in; i += 4)
            {
                int base_i = i + offset_in;
                array_out[(i / 4) + offset_out] = new Color32(vals_in[base_i], vals_in[base_i + 1], vals_in[base_i + 2], vals_in[base_i + 3]);
            }
        }

        public void CopyIntsToIntArray(List<int> vals_in, int offset_in, int num_in, ref List<int> array_out, int offset_out)
        {
            for (int i = 0; i < num_in; i++)
            {
                array_out[i + offset_out] = vals_in[i + offset_in];
            }
        }

        public Vector2 GetParticlePos(object[] p_pos_list, int idx)
        {
            return new Vector2((float)p_pos_list[idx * 2], (float)p_pos_list[idx * 2 + 1]);
        }

        public override void update(
            CreatureManager creature_manager,
            bool counter_clockwise,
            ref Vector3[] vertices,
            ref Vector3[] normals,
            ref Vector4[] tangents,
            ref Color32[] colors,
            ref Vector2[] uvs,
            CreatureAsset creature_asset,
            float region_overlap_z_delta,
            bool skin_swap_active,
            string skin_swap_name,
            string active_animation_name,
            ref List<int> final_indices,
            ref List<int> final_skin_swap_indices,
            ref int[] triangles,
            ref int[] skin_swap_triangles)
        {
            m_isValid = false;
            var meta_data = creature_asset.creature_meta_data;
            if (meta_data == null)
            {
                // Meta Data not loaded
                return;
            }

            var cur_anim_name = creature_manager.GetActiveAnimationName();
            if (!p_asset.m_ClipDataOffsets.ContainsKey(cur_anim_name))
            {
                // No animation
                return;
            }

            // Copy over Character data first
            var cur_char = creature_manager.GetCreature();
            int char_num_pts = cur_char.total_num_pts;
            var char_pts = cur_char.render_pts;
            CopyFloatsToVec3Array(char_pts, 0, char_pts.Count, ref vertices, 0);

            var char_uvs = cur_char.global_uvs;
            CopyFloatsToUVArray(char_uvs, 0, char_uvs.Count, ref uvs, 0);
            CopyBytesToColorArray(cur_char.render_colours, 0, cur_char.render_colours.Count, ref colors, 0);

            // Generate Particle data
            var pack_objs = p_asset.m_PackData;
            var clip_data = p_asset.m_ClipDataOffsets[cur_anim_name];
            var cur_anim = creature_manager.GetAnimation(cur_anim_name);
            var delta_frames = (int)(Math.Round(creature_manager.getActualRuntime() - cur_anim.start_time));
            int clip_runtime = Math.Min(Math.Max(0, delta_frames), clip_data.num_frames - 1);

            var frame_data = clip_data.particles_lookup[clip_runtime];

            // Now start mesh generation
            int pt_offset = char_num_pts * 3;
            int uv_offset = char_num_pts * 2;
            int colors_offset = char_num_pts;
            int indices_offset = 0;
            m_numIndices = 0;

            var all_regions = cur_char.render_composition.getRegions();
            // Go through each mesh region and check to see if we need to process it considering SkinSwaps as well
            float region_z = 0.0f;
            foreach (var cur_region in all_regions)
            {
                bool is_valid = true;
                if (skin_swap_active)
                {
                    is_valid = (meta_data.skin_swaps[skin_swap_name].Contains(cur_region.getName()));
                }

                if (is_valid)
                {
                    // Copy over base region indices
                    CopyIntsToIntArray(cur_region.store_indices, cur_region.getStartIndex(), cur_region.getNumIndices(), ref final_indices, indices_offset);
                    indices_offset += cur_region.getNumIndices();

                    // Set base region z
                    for (int i = 0; i < cur_region.getNumIndices(); i++)
                    {
                        vertices[cur_region.store_indices[i + cur_region.getStartIndex()]].z = region_z;
                    }

                    int layer_idx = cur_region.getTagId();
                    if (frame_data.ContainsKey(cur_region.getTagId()))
                    {
                        // Process Particles for layer Start
                        var layer_data = frame_data[cur_region.getTagId()];
                        for (int i = 0; i < layer_data.num_particles; i++)
                        {
                            int rel_offset = layer_data.offset + (i * 5);
                            var p_pos_list = (object[])pack_objs[rel_offset];
                            float p_angle = (float)pack_objs[rel_offset + 1];
                            int p_sprite_idx = -(int)pack_objs[rel_offset + 2];
                            var p_size = (object[])pack_objs[rel_offset + 3];
                            var p_color = (object[])pack_objs[rel_offset + 4];
                            var p_sprite_uvs = meta_data.uvs_data[p_sprite_idx];

                            var b_uv0 = new Vector2(p_sprite_uvs.uv0.x, p_sprite_uvs.uv0.y);
                            var b_uv1 = new Vector2(p_sprite_uvs.uv1.x, p_sprite_uvs.uv1.y);
                            var b_color = new Vector4((float)p_color[0], (float)p_color[1], (float)p_color[2], (float)p_color[3]);
                            region_z += region_overlap_z_delta * 0.1f;

                            if (p_pos_list.Length == 2)
                            {
                                // No Trail
                                int idx_pt_offset = pt_offset / 3;
                                var b_pos = new Vector2((float)p_pos_list[0], (float)p_pos_list[1]);

                                setVertData(
                                    b_pos + rotVec2D(new Vector2(-(float)p_size[0], -(float)p_size[1]), p_angle),
                                    region_z,
                                    new Vector2(b_uv0.x, b_uv0.y),
                                    b_color,
                                    ref pt_offset,
                                    ref uv_offset,
                                    ref colors_offset,
                                    ref vertices,
                                    ref uvs,
                                    ref colors
                                );


                                setVertData(
                                    b_pos + rotVec2D(new Vector2(-(float)p_size[0], (float)p_size[1]), p_angle),
                                    region_z,
                                    new Vector2(b_uv0.x, b_uv1.y),
                                    b_color,
                                    ref pt_offset,
                                    ref uv_offset,
                                    ref colors_offset,
                                    ref vertices,
                                    ref uvs,
                                    ref colors
                                );

                                setVertData(
                                    b_pos + rotVec2D(new Vector2((float)p_size[0], (float)p_size[1]), p_angle),
                                    region_z,
                                    new Vector2(b_uv1.x, b_uv1.y),
                                    b_color,
                                    ref pt_offset,
                                    ref uv_offset,
                                    ref colors_offset,
                                    ref vertices,
                                    ref uvs,
                                    ref colors
                                );

                                setVertData(
                                    b_pos + rotVec2D(new Vector2((float)p_size[0], -(float)p_size[1]), p_angle),
                                    region_z,
                                    new Vector2(b_uv1.x, b_uv0.y),
                                    b_color,
                                    ref pt_offset,
                                    ref uv_offset,
                                    ref colors_offset,
                                    ref vertices,
                                    ref uvs,
                                    ref colors
                                );

                                // Indices
                                setIndiceData(ref indices_offset, idx_pt_offset + 2, ref final_indices);
                                setIndiceData(ref indices_offset, idx_pt_offset + 1, ref final_indices);
                                setIndiceData(ref indices_offset, idx_pt_offset, ref final_indices);
                                setIndiceData(ref indices_offset, idx_pt_offset + 2, ref final_indices);
                                setIndiceData(ref indices_offset, idx_pt_offset, ref final_indices);
                                setIndiceData(ref indices_offset, idx_pt_offset + 3, ref final_indices);
                            }
                            else if (p_pos_list.Length > 2)
                            {
                                // With Trail
                                int idx_pt_offset = pt_offset / 3;
                                int trail_num = p_pos_list.Length / 2;

                                var dir_p = new Vector2(0, 0);
                                var rot_p = new Vector2(0, 0);
                                var diff_p = new Vector2(0, 0);
                                var delta_uvs = b_uv1 - b_uv0;
                                for (int j = 0; j < trail_num; j++)
                                {
                                    var p1 = GetParticlePos(p_pos_list, j);

                                    if (j < (trail_num - 1))
                                    {
                                        diff_p = GetParticlePos(p_pos_list, j + 1) - GetParticlePos(p_pos_list, j);
                                    }
                                    else
                                    {
                                        diff_p = GetParticlePos(p_pos_list, trail_num - 1) - GetParticlePos(p_pos_list, trail_num - 2);
                                    }

                                    if (diff_p.magnitude > 0)
                                    {
                                        dir_p = diff_p.normalized;
                                    }

                                    rot_p = rotVec2D_90(dir_p) * (float)p_size[1];
                                    var cur_uv = new Vector2(0, 0);
                                    float sub_alpha = Math.Min(1.0f / (float)(trail_num) * (float)(j), 1.0f);

                                    if (j < (trail_num - 1))
                                    {
                                        cur_uv.x = delta_uvs.x / (float)(trail_num) * (float)(j) + b_uv0.x;
                                        cur_uv.y = b_uv0.y;

                                        setVertData(
                                            p1 + rot_p,
                                            region_z,
                                            cur_uv,
                                            b_color * sub_alpha,
                                            ref pt_offset,
                                            ref uv_offset,
                                            ref colors_offset,
                                            ref vertices,
                                            ref uvs,
                                            ref colors
                                        );

                                        cur_uv.y = b_uv1.y;
                                        setVertData(
                                            p1 - rot_p,
                                            region_z,
                                            cur_uv,
                                            b_color * sub_alpha,
                                            ref pt_offset,
                                            ref uv_offset,
                                            ref colors_offset,
                                            ref vertices,
                                            ref uvs,
                                            ref colors
                                        );
                                    }
                                    else
                                    {
                                        // Final trail pos
                                        cur_uv.x = b_uv1.x;
                                        cur_uv.y = b_uv0.y;

                                        setVertData(
                                            p1 + diff_p + rot_p,
                                            region_z,
                                            cur_uv,
                                            b_color * sub_alpha,
                                            ref pt_offset,
                                            ref uv_offset,
                                            ref colors_offset,
                                            ref vertices,
                                            ref uvs,
                                            ref colors
                                        );

                                        cur_uv.y = b_uv1.y;
                                        setVertData(
                                            p1 + diff_p + rot_p,
                                            region_z,
                                            cur_uv,
                                            b_color * sub_alpha,
                                            ref pt_offset,
                                            ref uv_offset,
                                            ref colors_offset,
                                            ref vertices,
                                            ref uvs,
                                            ref colors
                                        );
                                    }
                                }

                                // Indices
                                int delta_trail_indices = 0;
                                for (int j = 0; j < trail_num - 1; j++)
                                {
                                    setIndiceData(ref indices_offset, idx_pt_offset + 2 + delta_trail_indices, ref final_indices);
                                    setIndiceData(ref indices_offset, idx_pt_offset + delta_trail_indices, ref final_indices);
                                    setIndiceData(ref indices_offset, idx_pt_offset + 1 + delta_trail_indices, ref final_indices);

                                    setIndiceData(ref indices_offset, idx_pt_offset + 2 + delta_trail_indices, ref final_indices);
                                    setIndiceData(ref indices_offset, idx_pt_offset + 1 + delta_trail_indices, ref final_indices);
                                    setIndiceData(ref indices_offset, idx_pt_offset + 3 + delta_trail_indices, ref final_indices);
                                    
                                    delta_trail_indices += 2;
                                }
                            }

                        }

                        // Process Particles for layer End
                    }
                }

                region_z += region_overlap_z_delta;
            }

            m_numIndices = indices_offset;
            m_isValid = true;
        }        
    }

    public ParticlesMeshModifier mesh_modifier;
    public void setupMeshModifier(CreatureManager creature_manager)
    {
        getPackData();
        var cur_char = creature_manager.GetCreature();
        int max_indices_num = m_maxIndicesNum + cur_char.total_num_indices;
        mesh_modifier = new ParticlesMeshModifier(
            max_indices_num,
            (m_maxParticlesNum * 4) + cur_char.total_num_pts,
            this);
        mesh_modifier.m_maxIndice = max_indices_num;
    }

}