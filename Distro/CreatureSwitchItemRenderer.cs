using System;
using System.IO;
using System.Collections.Generic;
using CreatureModule;
using MeshBoneUtil;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreatureSwitchItemData {
	public Vector2 uv;
	public float u_width, v_height;

	public CreatureSwitchItemData(Vector2 pos_in, float width_in, float height_in, float canvas_width, float canvas_height)
	{
		uv = new Vector2 (pos_in.x / canvas_width, pos_in.y / canvas_height);
		u_width = width_in / canvas_width;
		v_height = height_in / canvas_height;
	}
}

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CreatureSwitchItemRenderer : MonoBehaviour {
	private MeshFilter meshFilter;
	private Mesh active_mesh, mesh1, mesh2;
	private Vector3[] vertices;
	private Color32[] colors;
	private Vector2[] uvs;
	private Vector2 switch_min_uv, switch_max_uv;
	private int[] triangles;
	private bool swap_mesh;
	private CreatureManager creature_manager;
	public Dictionary<string, CreatureSwitchItemData> switch_items;
	public List<CreatureSwitchItemPacket> switch_item_packets;

	public CreatureRenderer creature_renderer;
	public String switch_region;
	private String target_switch_name;
	private int min_indice;

	private Mesh createMesh () {
		Mesh new_mesh = new Mesh();
		new_mesh.name = "CreatureSwitchItemMeshObject";
		new_mesh.hideFlags = HideFlags.HideAndDontSave;
		new_mesh.MarkDynamic();
		return new_mesh;
	}

	public void AddSwitchItem(string name_in, Vector2 pos_in, float width_in, float height_in, float canvas_width, float canvas_height) {
		switch_items [name_in] = new CreatureSwitchItemData (pos_in, width_in, height_in, canvas_width, canvas_height);
	}

	public void SetTargetSwitchItem(string name_in) {
		target_switch_name = name_in;
	}

#if UNITY_EDITOR
	[MenuItem("GameObject/Creature/CreatureSwitchItemRenderer")]
	static CreatureSwitchItemRenderer CreateRenderer()
	{
		GameObject newObj = new GameObject();
		newObj.name = "New Creature Switch Item Renderer";
		CreatureSwitchItemRenderer new_renderer;
		new_renderer = newObj.AddComponent<CreatureSwitchItemRenderer> () as CreatureSwitchItemRenderer;
		
		return new_renderer;
	}
#endif

	public CreatureSwitchItemRenderer()
	{
		switch_items = new Dictionary<string, CreatureSwitchItemData> ();
	}

	public virtual void Reset()
	{
		meshFilter = GetComponent<MeshFilter>();
		active_mesh = null;
		mesh1 = createMesh ();
		mesh2 = createMesh ();
		vertices = null;
		colors = null;
		uvs = null;
		swap_mesh = false;
	}

	// Use this for initialization
	void Start () {
		Reset ();

		InitData ();
	}

	public void InitData()
	{
		if (creature_renderer) {
			// init the render data
			CreatureManager ref_manager = creature_renderer.creature_asset.GetCreatureManager();
			creature_manager = new CreatureManager(ref_manager.target_creature);
			creature_manager.animations = ref_manager.animations;

			var target_creature = creature_manager.target_creature;
			MeshBoneUtil.MeshRenderBoneComposition render_composition =
				target_creature.render_composition;
			var regions_map = render_composition.getRegionsMap ();
			if (regions_map.ContainsKey (switch_region) == false) {
				return;
			}
			
			var cur_region = regions_map [switch_region];
			int uv_index = cur_region.getStartPtIndex() * 2;
			var global_uvs = target_creature.global_uvs;

			switch_min_uv = new Vector2(global_uvs[uv_index], global_uvs[uv_index + 1]);
			switch_max_uv = new Vector2(switch_min_uv.x, switch_min_uv.y);

			for(int i = 0; i < cur_region.getNumPts(); i++) {
				float cur_u = global_uvs[uv_index];
				float cur_v = global_uvs[uv_index + 1];

				if(cur_u < switch_min_uv.x) {
					switch_min_uv.x = cur_u;
				}
				else if(cur_u > switch_max_uv.x) {
					switch_max_uv.x = cur_u;
				}

				if(cur_v < switch_min_uv.y) {
					switch_min_uv.y = cur_v;
				}
				else if(cur_v > switch_max_uv.y) {
					switch_max_uv.y = cur_v;
				}

				uv_index += 2;
			}

			List<int> render_indices = creature_manager.target_creature.global_indices;
			min_indice = render_indices[cur_region.getStartIndex()];
			for(int i = 0; i < cur_region.getNumIndices(); i++) {
				int cur_indice = render_indices[cur_region.getStartIndex() + i];
				if(cur_indice < min_indice)
				{
					min_indice = cur_indice;
				}
			}
		}
	}

	public void CreateRenderingData()
	{
		if (switch_region.Length <= 0) {
			return;
		}

		var target_creature = creature_manager.target_creature;
		MeshBoneUtil.MeshRenderBoneComposition render_composition =
			target_creature.render_composition;
		var regions_map = render_composition.getRegionsMap ();
		if (regions_map.ContainsKey (switch_region) == false) {
			return;
		}

		var cur_region = regions_map [switch_region];

		vertices = new Vector3[cur_region.getNumPts()];
		colors = new Color32[cur_region.getNumPts()];
		uvs = new Vector2[cur_region.getNumPts()];
		triangles = new int[cur_region.getNumIndices()];
	}

	public void InitSwitchPackets()
	{
		// init any required item packets
		for(int i = 0; i < switch_item_packets.Count; i++) {
			var cur_packet = switch_item_packets[i];
			AddSwitchItem(cur_packet.packet_name, 
			              new Vector2(cur_packet.posX, cur_packet.posY), 
			              cur_packet.item_width,
			              cur_packet.item_height,
			              cur_packet.canvas_width, 
			              cur_packet.canvas_height);
		}
	}

	public void UpdateRenderingData()
	{
		if (switch_region.Length <= 0) {
			return;
		}

		if (target_switch_name == null) {
			return;
		}

		if (switch_items.ContainsKey (target_switch_name) == false) {
			return;
		}
		
		var target_creature = creature_manager.target_creature;
		MeshBoneUtil.MeshRenderBoneComposition render_composition =
			target_creature.render_composition;
		var regions_map = render_composition.getRegionsMap ();
		if (regions_map.ContainsKey (switch_region) == false) {
			return;
		}

		var cur_region = regions_map [switch_region];
		List<float> render_pts = creature_manager.target_creature.render_pts;
		List<float> render_uvs = creature_manager.target_creature.global_uvs;
		//List<byte> render_colors = creature_manager.target_creature.render_colours;

		int pt_index = cur_region.getStartPtIndex() * 3;
		int uv_index = cur_region.getStartPtIndex() * 2;
		int color_index = cur_region.getStartPtIndex() * 4;

		var cur_switch_item = switch_items [target_switch_name];
		float rel_u_width = switch_max_uv.x - switch_min_uv.x;
		float rel_v_height = switch_max_uv.y - switch_min_uv.y;

		for (int i = 0; i < vertices.Length; i++) {
			vertices[i].x = render_pts[pt_index + 0];
			vertices[i].y = render_pts[pt_index + 1];
			vertices[i].z = render_pts[pt_index + 2];

			float read_u = render_uvs[uv_index + 0];
			float read_v = render_uvs[uv_index + 1];

			// transform to new texture space
			float rel_u = (read_u - switch_min_uv.x) / rel_u_width;
			float rel_v = (read_v - switch_min_uv.y) / rel_v_height;
			rel_u = rel_u * cur_switch_item.u_width + cur_switch_item.uv.x;
			rel_v = rel_v * cur_switch_item.v_height + cur_switch_item.uv.y;

			uvs[i].x = rel_u;
			uvs[i].y = 1.0f - rel_v;
			
			colors[i].r = 255;
			colors[i].g = 255;
			colors[i].b = 255;
			colors[i].a = 255;

			pt_index += 3;
			uv_index += 2;
			color_index += 4;
		}

		List<int> render_indices = creature_manager.target_creature.global_indices;
		//int start_lookup = render_indices [cur_region.getStartIndex ()];

		for(int i = 0; i < cur_region.getNumIndices(); i++)
		{
			int lookup = render_indices[cur_region.getStartIndex() + i];
			triangles[i] = lookup - min_indice;
		}
		
		active_mesh.vertices = vertices;
		active_mesh.colors32 = colors;
		active_mesh.triangles = triangles;
		active_mesh.uv = uvs;

	}

	private void doSwapMesh()
	{
		active_mesh = swap_mesh ? mesh1 : mesh2;
		swap_mesh = !swap_mesh;
	}

	public void Awake () 
	{
		Reset ();
	}
	
	// Update is called once per frame
	public virtual void LateUpdate () 
	{
		if (creature_manager != null) {
			doSwapMesh ();
			
			if (vertices == null)
			{
				CreateRenderingData ();
				InitSwitchPackets();
			}

			UpdateRenderingData();

			meshFilter.sharedMesh = active_mesh;
		}

	}
}
