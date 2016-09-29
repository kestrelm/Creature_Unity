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
    private bool swap_mesh;
    public float local_time_scale;
    public CreaturePackAsset pack_asset;
    public CreaturePackPlayer pack_player;
    public string active_animation_name;
    public float blend_rate = 0.1f;
    public float region_offsets_z = 0.01f;
    public bool should_loop = true, should_play = true;
    public bool counter_clockwise = false;

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
        if(pack_asset)
        {
            var pack_loader = pack_asset.GetCreaturePackLoader();
            pack_player = new CreaturePackPlayer(pack_loader);
            pack_player.setActiveAnimation(active_animation_name);
        }
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

    public void Awake()
    {
        Reset();
        InitData();

        var cur_animator = GetComponent<Animator>();
        if(cur_animator)
        {
            CreaturePackStateMachineBehavior[] all_behaviors = cur_animator.GetBehaviours<CreaturePackStateMachineBehavior>();
            for (int i = 0; i < all_behaviors.Length; i++)
            {
                all_behaviors[i].pack_renderer = this;
            }
        }
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

        var cur_z = 0.0f;
        var render_indices = pack_player.data.indices;
        var total_num_indices = pack_player.data.getNumIndices();

        foreach (var meshData in pack_player.data.meshRegionsList)
        {
            for(int idx = (int)meshData.first; idx <= (int)meshData.second; idx++)
            {
                var pt_idx = render_indices[idx];
                vertices[pt_idx].z = cur_z;
                vertices[pt_idx + 1].z = cur_z;
                vertices[pt_idx + 2].z = cur_z;
            }

            cur_z += region_offsets_z;
        }


        if (!counter_clockwise)
        {
            for (int i = 0; i < total_num_indices; i++)
            {
                triangles[i] = (int)render_indices[i];
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


        active_mesh.vertices = vertices;
        active_mesh.colors32 = colors;
        active_mesh.triangles = triangles;
        active_mesh.normals = normals;
        active_mesh.tangents = tangents;
        active_mesh.uv = uvs;
    }

    public void UpdateTime()
    {
        if (active_animation_name == null || active_animation_name.Length == 0)
        {
            active_animation_name = pack_player.data.GetFirstAnimClipName();
        }


        float time_delta = (Time.deltaTime * local_time_scale);
        if(!should_play)
        {
            time_delta = 0.0f;
        }

        pack_player.isLooping = should_loop;
        pack_player.isPlaying = true;
        pack_player.stepTime(time_delta);
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
