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

using UnityEngine;
using CreaturePackModule;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreaturePackRenderer : MonoBehaviour {
    private MeshFilter meshFilter;
    private Mesh active_mesh, mesh1, mesh2;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector4[] tangents;
    private Color32[] colors;
    private Vector2[] uvs;

    private int[] triangles;
    private int[] skin_swap_triangles;
    private List<int> skin_swap_indices = new List<int>();
    private bool swap_mesh;
    public float local_time_scale;
    public CreaturePackAsset pack_asset;
    public CreaturePackPlayer pack_player;
    public string active_animation_name;
    public float blend_rate = 0.1f;
    public float region_offsets_z = 0.01f;
    public bool should_loop = true, should_play = true;
    public bool counter_clockwise = false;
    public bool use_anchor_pts = false;
    public string anchor_pts_anim = "";
    public bool use_composite_clips = false;
    public string composite_anim = "";
    public Dictionary<String, byte> alpha_region_overrides = new Dictionary<string, byte>();

    public bool use_skin_swap = false;
    private string prev_skin_swap = "";
    public bool use_meta_data = false;
    private bool skin_swap_valid = false;
    public string skin_swap = "";

    private Mesh createMesh()
    {
        Mesh new_mesh = new Mesh();
        new_mesh.name = "CreaturePack Mesh Object";
        new_mesh.hideFlags = HideFlags.HideAndDontSave;
        new_mesh.MarkDynamic();
        return new_mesh;
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/CreaturePack/CreaturePackRenderer")]
    static CreaturePackRenderer CreateRenderer()
    {
        GameObject newObj = new GameObject();
        newObj.name = "New Creature Pack Renderer";
        CreaturePackRenderer new_renderer;
        new_renderer = newObj.AddComponent<CreaturePackRenderer>() as CreaturePackRenderer;

        return new_renderer;
    }
#endif

    public CreaturePackRenderer()
    {
        local_time_scale = 60.0f;
        region_offsets_z = 0.01f;
    }

    public virtual void Reset()
    {
        meshFilter = GetComponent<MeshFilter>();
        active_mesh = null;
        mesh1 = createMesh();
        mesh2 = createMesh();
        vertices = null;
        normals = null;
        colors = null;
        uvs = null;
        swap_mesh = false;
    }

    public void InitData()
    {
        if (pack_asset && (pack_player == null))
        {
            var pack_loader = pack_asset.GetCreaturePackLoader();
            pack_player = new CreaturePackPlayer(pack_loader);
            pack_player.setActiveAnimation(active_animation_name);

        }

        if(pack_asset != null && pack_player != null)
        {
            if (use_anchor_pts || use_composite_clips || use_skin_swap || use_meta_data)
            {
                pack_asset.LoadMetaData();
                if (pack_asset.composite_player != null)
                {
                    pack_asset.composite_player.setActiveName(composite_anim, true, pack_player);
                }
            }
        }
    }

    // Adds an alpha region override, make sure your MetaData slot is connected
    public void setAlphaRegionOverride(String region_name, byte val_in)
    {
        alpha_region_overrides[region_name] = val_in;
    }

    // Removes an alpha region override
    public void removeAlphaRegionOverride(String region_name)
    {
        if(alpha_region_overrides.ContainsKey(region_name))
        {
            alpha_region_overrides.Remove(region_name);
        }
    }

    // Clears all alpha region overrides
    public void clearAlphaRegionsOverride()
    {
        alpha_region_overrides.Clear();
    }

    public void setCompositeActiveClip(String name_in)
    {
        if (pack_asset.composite_player != null)
        {
            composite_anim = name_in;
            pack_asset.composite_player.setActiveName(composite_anim, true, pack_player);
        }
    }

    private bool reloadSkinSwap()
    {
        if(pack_asset.meta_data == null)
        {
            return false;
        }

        var retval = pack_asset.meta_data.buildSkinSwapIndices(
            skin_swap,
            pack_player.data.indices, 
            skin_swap_indices,
            pack_player);

        if(!retval)
        {
            skin_swap_indices.Clear();
        }

        prev_skin_swap = skin_swap;
        skin_swap_valid = true;
        return true;
    }

    // Use this for initialization
    void Start () {
        Awake();
	}

    private void doSwapMesh()
    {
        active_mesh = swap_mesh ? mesh1 : mesh2;
        swap_mesh = !swap_mesh;
    }

    public void InitRenderer()
    {
        Reset();
        InitData();

        var cur_animator = GetComponent<Animator>();
        if (cur_animator)
        {
            CreaturePackStateMachineBehavior[] all_behaviors = cur_animator.GetBehaviours<CreaturePackStateMachineBehavior>();
            for (int i = 0; i < all_behaviors.Length; i++)
            {
                all_behaviors[i].pack_renderer = this;
            }
        }
    }

    public void Awake()
    {
        InitRenderer();
    }

    public void OnEnable()
    {
        InitRenderer();
    }

    public void CreateRenderingData()
    {
        var total_num_pts = pack_player.getRenderPointsLength() / 3;
        var total_num_indices = pack_player.data.getNumIndices();

        vertices = new Vector3[total_num_pts];
        normals = new Vector3[total_num_pts];
        tangents = new Vector4[total_num_pts];
        colors = new Color32[total_num_pts];
        uvs = new Vector2[total_num_pts];
        triangles = new int[total_num_indices];
    }

    public void processRegionAlphaOverrides()
    {
        if((pack_asset.meta_data == null) || (alpha_region_overrides.Count == 0))
        {
            return;
        }
        var render_colors = pack_player.render_colors;
        var render_indices = pack_player.data.indices;

        foreach (var cur_override in alpha_region_overrides)
        {
            var cur_name = cur_override.Key;
            var cur_alpha = cur_override.Value;

            if(pack_asset.meta_data.mesh_map.ContainsKey(cur_name))
            {
                var region_range = pack_asset.meta_data.mesh_map[cur_name];
                for(int j = region_range.Item1; j <= region_range.Item2; j++)
                {
                    var cur_idx = render_indices[j];
                    colors[cur_idx].r = cur_alpha;
                    colors[cur_idx].g = cur_alpha;
                    colors[cur_idx].b = cur_alpha;
                    colors[cur_idx].a = cur_alpha;
                }
            }
        }
    }

    public void UpdateRenderingData()
    {
        int pt_index = 0;
        int uv_index = 0;
        int color_index = 0;

        var render_pts = pack_player.render_points;
        var render_uvs = pack_player.render_uvs;
        var render_colors = pack_player.render_colors;
        float normal_z = 1.0f;
        if (counter_clockwise)
        {
            normal_z = -1.0f;
        }

        var total_num_pts = pack_player.getRenderPointsLength() / 3;
        for (int i = 0; i < total_num_pts; i++)
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

        processRegionAlphaOverrides();

        var cur_z = 0.0f;
        var render_indices = pack_player.data.indices;
        var total_num_indices = pack_player.data.getNumIndices();

        foreach (var meshData in pack_player.data.meshRegionsList)
        {
            for(int idx = (int)meshData.first; idx <= (int)meshData.second; idx++)
            {
                vertices[idx].z = cur_z;
            }

            cur_z += region_offsets_z;
        }

        if (use_anchor_pts)
        {
            if (pack_asset.anchor_points != null)
            {
                string cur_anchor_anim = pack_player.activeAnimationName;
                if(anchor_pts_anim.Length > 0)
                {
                    cur_anchor_anim = anchor_pts_anim;
                }

                if (pack_asset.anchor_points.ContainsKey(cur_anchor_anim))
                {
                    var curAnchorPts = pack_asset.anchor_points[cur_anchor_anim];
                    for (int i = 0; i < total_num_pts; i++)
                    {
                        vertices[i].x -= curAnchorPts.x;
                        vertices[i].y -= curAnchorPts.y;
                    }
                }
            }
        }

        // Process indices
        if((skin_swap != prev_skin_swap) && use_skin_swap)
        {
            reloadSkinSwap();
            skin_swap_triangles = new int[skin_swap_indices.Count];
        }
        bool perform_skin_swap = skin_swap_valid && use_skin_swap;

        if (!counter_clockwise)
        {
            if (perform_skin_swap)
            {
                skin_swap_indices.CopyTo(skin_swap_triangles, 0);
            }
            else
            {
                for (int i = 0; i < total_num_indices; i++)
                {
                    triangles[i] = (int)render_indices[i];
                }
            }
        }
        else
        {
            if (perform_skin_swap)
            {
                for(int i = 0; i < skin_swap_indices.Count; i+=3)
                {
                    skin_swap_triangles[i] = skin_swap_indices[i];
                    skin_swap_triangles[i + 1] = skin_swap_indices[i + 2];
                    skin_swap_triangles[i + 2] = skin_swap_indices[i + 1];
                }
            }
            else
            {
                for (int i = 0; i < total_num_indices; i += 3)
                {
                    triangles[i] = (int)render_indices[i];
                    triangles[i + 1] = (int)render_indices[i + 2];
                    triangles[i + 2] = (int)render_indices[i + 1];
                }
            }
        }


        active_mesh.vertices = vertices;
        active_mesh.colors32 = colors;
        active_mesh.triangles = perform_skin_swap ? skin_swap_triangles : triangles;
        active_mesh.normals = normals;
        active_mesh.tangents = tangents;
        active_mesh.uv = uvs;
    }

    public bool compositeClipActive()
    {
        return use_composite_clips && (pack_asset.composite_player != null);
    }

    public void UpdateTime()
    {
        if (active_animation_name == null || active_animation_name.Length == 0)
        {
            active_animation_name = pack_player.data.GetFirstAnimClipName();
        }

        float real_time_scale = Application.isPlaying ? local_time_scale : 0.0f;
        float time_delta = (Time.deltaTime * real_time_scale);
        if(!should_play)
        {
            time_delta = 0.0f;
        }

        if(compositeClipActive())
        {
            var comp_data = pack_asset.composite_player.composite_clips;
            if(comp_data.ContainsKey(composite_anim))
            {
                pack_asset.composite_player.update(time_delta, pack_player, should_loop);
            }
        }
        else
        {
            pack_player.isLooping = should_loop;
            pack_player.isPlaying = true;
            pack_player.stepTime(time_delta);
        }

        pack_player.syncRenderData();
    }

    // Update is called once per frame
    public virtual void LateUpdate()
    {
        if(pack_player == null)
        {
            Awake();
        }

        if (pack_player != null)
        {
            doSwapMesh();

            if (pack_asset.GetIsDirty() || vertices == null)
            {
                CreateRenderingData();
                pack_asset.SetIsDirty(false);
            }

            UpdateTime();
            UpdateRenderingData();

            meshFilter.sharedMesh = active_mesh;
        }
    }
}
