using System;
using System.IO;
using System.Collections.Generic;
using CreatureModule;
using MeshBoneUtil;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasRenderer)), ExecuteInEditMode]
public class CreatureCanvasRenderer : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh active_mesh, mesh1, mesh2;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector4[] tangents;
    private Color32[] colors;
    private Vector2[] uvs;
    private int[] triangles, skin_swap_triangles;
    private List<int> final_indices, final_skin_swap_indices;
    bool skin_swap_active = false;
    String skin_swap_name = "";
    private bool swap_mesh;
    private float local_time;
    private bool use_custom_time_range;
    private float custom_start_time, custom_end_time;
    public float local_time_scale;
    public CreatureAsset creature_asset = null;
    public CreatureManager creature_manager;
    private CreatureGameController game_controller = null;
    public int animation_choice_index;
    public string active_animation_name;
    public float blend_rate = 0.1f;
    public float region_offsets_z = 0.01f;
    public bool should_loop = true;
    public bool counter_clockwise = false;
    public Material material;
    public Dictionary<string, CreatureBoneData> feedback_bones;

#if UNITY_EDITOR
    [MenuItem("GameObject/Creature/CreatureCanvasRenderer")]
    static CreatureCanvasRenderer CreateCanvasRenderer()
    {
        GameObject newObj = new GameObject();
        newObj.name = "New Creature Canvas Renderer";
        CreatureCanvasRenderer new_renderer;
        new_renderer = newObj.AddComponent<CreatureCanvasRenderer>() as CreatureCanvasRenderer;

        return new_renderer;
    }
#endif

    public CreatureCanvasRenderer()
    {
        local_time_scale = 2.0f;
        region_offsets_z = 0.01f;
        feedback_bones = new Dictionary<string, CreatureBoneData>();
    }

    public virtual void Reset()
    {
        meshFilter = GetComponent<MeshFilter>();
        active_mesh = null;
        mesh1 = CreatureRenderModule.createMesh();
        mesh2 = CreatureRenderModule.createMesh();
        vertices = null;
        normals = null;
        colors = null;
        uvs = null;
        swap_mesh = false;
        local_time = 0;
    }

    public void SetGameController(CreatureGameController controller_in)
    {
        game_controller = controller_in;
    }

    public void InitData()
    {
        if (creature_asset)
        {
            CreatureManager ref_manager = creature_asset.GetCreatureManager();
            creature_manager = new CreatureManager(ref_manager.target_creature);
            creature_manager.animations = ref_manager.animations;
            creature_manager.active_blend_run_times = new Dictionary<string, float>(ref_manager.active_blend_run_times);
            creature_manager.active_blend_animation_names = new List<string>(ref_manager.active_blend_animation_names);
            creature_manager.auto_blend_names = new List<string>(ref_manager.auto_blend_names);
            creature_manager.feedback_bones_map = feedback_bones;

            SetActiveAnimation(active_animation_name);
            creature_manager.SetIsPlaying(true);
        }

        var renderer = GetComponent<CanvasRenderer>();
        renderer.materialCount = 1;
        renderer.SetMaterial(material, 0);
    }

    void Start()
    {
        Reset();
        InitData();
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
    }

    // Sets the animation to the specified animation clip name
    public void SetActiveAnimation(string animation_name, bool already_active_check = false)
    {
        if (already_active_check)
        {
            if (active_animation_name == animation_name)
            {
                return;
            }
        }

        active_animation_name = animation_name;
        creature_manager.SetAutoBlending(false);

        bool can_set = creature_manager.SetActiveAnimationName(active_animation_name);
        if (!can_set)
        {
            creature_manager.SetActiveAnimationName(creature_manager.GetAnimationNames()[0]);
            active_animation_name = creature_manager.GetActiveAnimationName();
            animation_choice_index = 0;
        }


        local_time = creature_manager.animations[creature_manager.GetActiveAnimationName()].start_time;

        if (game_controller)
        {
            game_controller.AnimClipChangeEvent();
        }
    }

    public void BlendToAnimation(string animation_name)
    {
        if (active_animation_name == animation_name)
        {
            return;
        }

        active_animation_name = animation_name;
        creature_manager.SetAutoBlending(true);
        creature_manager.AutoBlendTo(animation_name, blend_rate);

        if (game_controller)
        {
            game_controller.AnimClipChangeEvent();
        }
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

    public void EnableSkinSwap(String swap_name_in, bool active)
    {
        CreatureRenderModule.EnableSkinSwap(
            swap_name_in,
            active,
            creature_manager,
            creature_asset,
            ref skin_swap_active,
            ref skin_swap_name,
            ref skin_swap_triangles,
            ref final_skin_swap_indices);
    }

    public void DisableSkinSwap()
    {
        EnableSkinSwap("", false);
    }

    // Add your own custom Skin Swap into the object
    public bool AddSkinSwap(String swap_name, HashSet<String> swap_set)
    {
        return CreatureRenderModule.AddSkinSwap(creature_asset, swap_name, swap_set);
    }

    public void CreateRenderingData()
    {
        CreatureRenderModule.CreateRenderingData(
            creature_manager,
            ref vertices,
            ref normals,
            ref tangents,
            ref colors,
            ref uvs,
            ref triangles,
            ref final_indices);
    }

    public void UpdateRenderingData()
    {
        CreatureRenderModule.UpdateRenderingData(
            creature_manager,
            counter_clockwise,
            ref vertices,
            ref normals,
            ref tangents,
            ref colors,
            ref uvs,
            creature_asset,
            skin_swap_active,
            active_animation_name,
            ref final_indices,
            ref final_skin_swap_indices,
            ref triangles,
            ref skin_swap_triangles);

        bool should_skin_swap = CreatureRenderModule.shouldSkinSwap(creature_asset, skin_swap_active, ref skin_swap_triangles);

        active_mesh.vertices = vertices;
        active_mesh.colors32 = colors;
        active_mesh.triangles = should_skin_swap ? skin_swap_triangles : triangles;
        active_mesh.normals = normals;
        active_mesh.tangents = tangents;
        active_mesh.uv = uvs;

        //CreatureRenderModule.debugDrawBones(creature_manager.target_creature.render_composition.getRootBone ());
    }

    public void UpdateTime()
    {
        CreatureRenderModule.UpdateTime(
            creature_manager,
            game_controller,
            creature_asset.creature_meta_data,
            active_animation_name,
            local_time_scale,
            region_offsets_z,
            should_loop,
            ref local_time);
    }

    /*
    public List<UIVertex> ConvertMesh()
    {
        Vector3[] vertices = active_mesh.vertices;
        int[] triangles = active_mesh.triangles;
        Vector3[] normals = active_mesh.normals;
        Vector2[] uv = active_mesh.uv;

        List<UIVertex> vertexList = new List<UIVertex>(triangles.Length);

        UIVertex vertex;
        for (int i = 0; i < triangles.Length; i++)
        {
            vertex = new UIVertex();
            int triangle = triangles[i];

            vertex.position = (vertices[triangle] - active_mesh.bounds.center);
            vertex.uv0 = uv[triangle];
            vertex.normal = normals[triangle];

            vertexList.Add(vertex);

            if (i % 3 == 0)
                vertexList.Add(vertex);
        }

        return vertexList;
    }
    */

    // Update is called once per frame
    void LateUpdate()
    {
        if (creature_manager != null)
        {
            doSwapMesh();

            if (creature_asset.GetIsDirty() || vertices == null)
            {
                CreateRenderingData();
                creature_asset.SetIsDirty(false);
            }

            UpdateTime();
            UpdateRenderingData();
        }

        var renderer = GetComponent<CanvasRenderer>();
        renderer.SetMesh(active_mesh);
        renderer.materialCount = 1;
        renderer.SetMaterial(material, 0);
    }

    void OnDisable()
    {
        var renderer = GetComponent<CanvasRenderer>();
        renderer.SetMesh(null);
    }
}
