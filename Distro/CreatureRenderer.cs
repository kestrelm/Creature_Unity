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

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreatureRenderer : MonoBehaviour 
{
	private MeshFilter meshFilter;
	private Mesh active_mesh, mesh1, mesh2;
	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector4[] tangents;
	private Color32[] colors;
	private Vector2[] uvs;
	private int[] triangles;
	private bool swap_mesh;
	private float local_time;
	private bool use_custom_time_range;
	private float custom_start_time, custom_end_time;
	public float local_time_scale;
	public CreatureAsset creature_asset;
	public CreatureManager creature_manager;
	public int animation_choice_index;
	public string active_animation_name;
	public float blend_rate = 0.1f;
	public float region_offsets_z = 0.01f;
	public bool should_loop;
	public bool counter_clockwise = false;

	private Mesh createMesh () {
		Mesh new_mesh = new Mesh();
		new_mesh.name = "Creature Mesh Object";
		new_mesh.hideFlags = HideFlags.HideAndDontSave;
		new_mesh.MarkDynamic();
		return new_mesh;
	}

#if UNITY_EDITOR
	[MenuItem("GameObject/Creature/CreatureRenderer")]
	static CreatureRenderer CreateRenderer()
	{
		GameObject newObj = new GameObject();
		newObj.name = "New Creature Renderer";
		CreatureRenderer new_renderer;
		new_renderer = newObj.AddComponent<CreatureRenderer> () as CreatureRenderer;

		return new_renderer;
	}
#endif

	public CreatureRenderer()
	{
		local_time_scale = 2.0f;
		region_offsets_z = 0.01f;
	}
	
	public virtual void Reset()
	{
		meshFilter = GetComponent<MeshFilter>();
		active_mesh = null;
		mesh1 = createMesh ();
		mesh2 = createMesh ();
		vertices = null;
		normals = null;
		colors = null;
		uvs = null;
		swap_mesh = false;
		local_time = 0;
	}

	public void InitData()
	{
		if (creature_asset) {
			CreatureManager ref_manager = creature_asset.GetCreatureManager();
			creature_manager = new CreatureManager(ref_manager.target_creature);
			creature_manager.animations = ref_manager.animations;
			creature_manager.active_blend_run_times = new Dictionary<string, float>(ref_manager.active_blend_run_times);
			creature_manager.active_blend_animation_names = new List<string>(ref_manager.active_blend_animation_names);
			creature_manager.auto_blend_names = new List<string>(ref_manager.auto_blend_names);

			SetActiveAnimation(active_animation_name);
			creature_manager.SetIsPlaying(true);
		}
	}
	
	void Start () {
		Reset ();
		
		// Testing
		/*
		string filepath = "/Users/jychong/Projects/EngineAppMedia/Test/default.json";
		Dictionary<string, object> load_data = CreatureModule.Utils.LoadCreatureJSONData (filepath);
		
		CreatureModule.Creature new_creature = new CreatureModule.Creature(ref load_data);
		CreatureModule.CreatureManager new_manager = new CreatureModule.CreatureManager (new_creature);
		new_manager.CreateAnimation (ref load_data, "default");
		//new_manager.CreateAnimation (ref load_data, "second");
		
		new_manager.SetActiveAnimationName ("default");
		new_manager.SetIsPlaying (true);
		
		creature_manager = new_manager;
		*/
		InitData ();
	}
	
	private void doSwapMesh()
	{
		active_mesh = swap_mesh ? mesh1 : mesh2;
		swap_mesh = !swap_mesh;
	}
	
	public void Awake () 
	{
		Reset ();
		InitData ();
	}

	// Sets the animation to the specified animation clip name
	public void SetActiveAnimation(string animation_name, bool already_active_check=false)
	{
		if (already_active_check) {
			if (active_animation_name == animation_name) {
				return;
			}
		}

		active_animation_name = animation_name;

		bool can_set = creature_manager.SetActiveAnimationName (active_animation_name);
		if (!can_set) {
			creature_manager.SetActiveAnimationName(creature_manager.GetAnimationNames()[0]);
			active_animation_name = creature_manager.GetActiveAnimationName();
			animation_choice_index = 0;
		}


		local_time = creature_manager.animations [creature_manager.GetActiveAnimationName()].start_time;
	}

	public void BlendToAnimation(string animation_name)
	{
		if (active_animation_name == animation_name) {
			return;
		}

		active_animation_name = animation_name;
		creature_manager.SetAutoBlending (true);
		creature_manager.AutoBlendTo (animation_name, blend_rate);
	}

	// Returns the local playback time
	public float GetLocalTime()
	{
		return local_time;
	}

	// Set the local playback time
	public void SetLocalTime(float t)
	{
		local_time = t;
	}

	// Returns the currently playing animation clip
	public String GetActiveAnimation()
	{
		return active_animation_name;
	}

	// If true, will use a user defined animation clip range
	public void SetUseCustomTimeRange(bool flag_in)
	{
		use_custom_time_range = flag_in;
	}

	// Sets the user specified animation clip range
	public void SetCustomTimeRange(float start_time, float end_time) 
	{
		custom_start_time = start_time;
		custom_end_time = end_time;
	}
	
	public void CreateRenderingData()
	{
		vertices = new Vector3[creature_manager.target_creature.total_num_pts];
		normals = new Vector3[creature_manager.target_creature.total_num_pts];
		tangents = new Vector4[creature_manager.target_creature.total_num_pts];
		colors = new Color32[creature_manager.target_creature.total_num_pts];
		uvs = new Vector2[creature_manager.target_creature.total_num_pts];
		triangles = new int[creature_manager.target_creature.total_num_indices];
	}
	
	public void UpdateRenderingData()
	{
		int pt_index = 0;
		int uv_index = 0;
		int color_index = 0;
		
		List<float> render_pts = creature_manager.target_creature.render_pts;
		List<float> render_uvs = creature_manager.target_creature.global_uvs;
		List<byte> render_colors = creature_manager.target_creature.render_colours;
		float normal_z = 1.0f;
		if(counter_clockwise)
		{
			normal_z = -1.0f;
		}

		for(int i = 0; i < creature_manager.target_creature.total_num_pts; i++)
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

		if(!counter_clockwise)
		{
			for(int i = 0; i < creature_manager.target_creature.total_num_indices; i++)
			{
				triangles[i] = render_indices[i];
			}
		}
		else {
			for(int i = 0; i < creature_manager.target_creature.total_num_indices; i+=3)
			{
				triangles[i] = render_indices[i];
				triangles[i + 1] = render_indices[i + 2];
				triangles[i + 2] = render_indices[i + 1];
			}
		}

		active_mesh.vertices = vertices;
		active_mesh.colors32 = colors;
		active_mesh.triangles = triangles;
		active_mesh.normals = normals;
		active_mesh.tangents = tangents;
		active_mesh.uv = uvs;

		//debugDrawBones (creature_manager.target_creature.render_composition.getRootBone ());
	}

	public void debugDrawBones(MeshBone bone_in)
	{
		XnaGeometry.Vector4 pt1 = bone_in.world_start_pt;
		XnaGeometry.Vector4 pt2 = bone_in.world_end_pt;
		
		Debug.DrawLine (new Vector3((float)pt1.X, (float)pt1.Y, 0), 
		                new Vector3((float)pt2.X, (float)pt2.Y, 0), Color.white);
		
		foreach(MeshBone cur_child in bone_in.children)
		{
			debugDrawBones(cur_child);
		}
	}

	public void UpdateTime()
	{
		if (active_animation_name == null || active_animation_name.Length == 0) {
			return;
		}


		float time_delta = (Time.deltaTime * local_time_scale);

		creature_manager.region_offsets_z = region_offsets_z;
		creature_manager.should_loop = should_loop;
		creature_manager.Update(time_delta);
		local_time = creature_manager.getRunTime();
	}

	public virtual void LateUpdate () 
	{
		if (creature_manager != null) {
			doSwapMesh ();

			if (creature_asset.GetIsDirty() || vertices == null)
			{
				CreateRenderingData ();
				creature_asset.SetIsDirty(false);
			}

			UpdateTime();
			UpdateRenderingData ();

			meshFilter.sharedMesh = active_mesh;
		}
	}
	
}

