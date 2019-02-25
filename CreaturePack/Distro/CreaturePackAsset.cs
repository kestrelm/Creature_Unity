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
using SimpleJSON;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CreaturePackUtils
{
    public sealed class Tuple<T1, T2>
    {
        private readonly T1 item1;
        private readonly T2 item2;

        /// <summary>
        /// Retyurns the first element of the tuple
        /// </summary>
        public T1 Item1
        {
            get { return item1; }
        }

        /// <summary>
        /// Returns the second element of the tuple
        /// </summary>
        public T2 Item2
        {
            get { return item2; }
        }

        /// <summary>
        /// Create a new tuple value
        /// </summary>
        /// <param name="item1">First element of the tuple</param>
        /// <param name="second">Second element of the tuple</param>
        public Tuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public override string ToString()
        {
            return string.Format("Tuple({0}, {1})", Item1, Item2);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + item1.GetHashCode();
            hash = hash * 23 + item2.GetHashCode();
            return hash;
        }

        public override bool Equals(object o)
        {
            if (o.GetType() != typeof(Tuple<T1, T2>))
            {
                return false;
            }

            var other = (Tuple<T1, T2>)o;

            return this == other;
        }

        public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return
              a.item1.Equals(b.item1) &&
              a.item2.Equals(b.item2);
        }

        public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return !(a == b);
        }

        public void Unpack(Action<T1, T2> unpackerDelegate)
        {
            unpackerDelegate(Item1, Item2);
        }
    }
}

public class CreatureCompositeClip
{
    public string name;
    public int start_frame = 0;
    public int end_frame = 0;
    public CreatureCompositeClip(string name_in, int start_frame_in, int end_frame_in)
    {
        name = name_in;
        start_frame = start_frame_in;
        end_frame = end_frame_in;
    }
}

public class CreatureCompositePlayer
{
    public string active_name;
    public int active_idx = 0;
    public Dictionary<String, List<CreatureCompositeClip>> composite_clips = null;

    void setupForSubClip(CreaturePackPlayer pack_player, int idx, string anim_name)
    {
        if (composite_clips.ContainsKey(active_name))
        {
            var sublist = composite_clips[active_name];
            pack_player.setActiveAnimation(sublist[idx].name);
            pack_player.setRunTime(sublist[idx].start_frame, "");
        }
    }

    // Returns whether the current composite clip is available
    public bool hasCompositeName(string name_in)
    {
        return composite_clips.ContainsKey(name_in);
    }

    // Switches the animation to a new composite clip given its name
    public void setActiveName(string name_in, bool force_reset, CreaturePackPlayer pack_player)
    {
        if((active_name == name_in) && (force_reset == false))
        {
            return;
        }

        active_name = name_in;
        active_idx = 0;
        
        if(composite_clips.ContainsKey(active_name))
        {
            setupForSubClip(pack_player, active_idx, active_name);
        }        
    }

    // Update the composite clip animation step
    public void update(float delta_step, CreaturePackPlayer pack_player, bool should_loop)
    {
        var sublist = composite_clips[active_name];
        var pre_time = pack_player.getRunTime("");

        if(!should_loop)
        {
            if ((pre_time >= sublist[active_idx].end_frame)
                && (active_idx >= (sublist.Count -1 )))
            {
                return;
            }
        }

        pack_player.isLooping = true;
        pack_player.isPlaying = true;
        pack_player.stepTime(delta_step);

        var post_time = pack_player.getRunTime("");

        if ((post_time < pre_time) || (post_time > sublist[active_idx].end_frame))
        {
            var old_idx = active_idx;
            var new_idx = (active_idx + 1) % sublist.Count;
            bool advance_to_next = true;
            if(!should_loop && (new_idx <= old_idx))
            {
                advance_to_next = false;
            }

            if(advance_to_next)
            {
                active_idx = new_idx;
                setupForSubClip(pack_player, active_idx, active_name);
            }
            else
            {
                pack_player.setRunTime(sublist[active_idx].end_frame, "");
            }
        }
    }
}

// Meta Data
public class CreaturePackMetaData
{
    public Dictionary<String,  CreaturePackUtils.Tuple<int, int>> mesh_map;
    public List<String> mesh_sorted_names;
    public Dictionary<String, Dictionary<int, List<int>>> anim_order_map;
    public Dictionary<String, Dictionary<int, String>> anim_events_map;
    public Dictionary<String, HashSet<String>> skin_swaps;
    public HashSet<String> active_skin_swap_names;

    public CreaturePackMetaData()
    {
        mesh_map = new Dictionary<String, CreaturePackUtils.Tuple<int, int>>();
        mesh_sorted_names = new List<string>();
        anim_order_map = new Dictionary<String, Dictionary<int, List<int>>>();
        anim_events_map = new Dictionary<String, Dictionary<int, String>>();
        skin_swaps = new Dictionary<string, HashSet<string>>();
        active_skin_swap_names = new HashSet<String>();
    }

    public void clear()
    {
        mesh_map.Clear();
        mesh_sorted_names.Clear();
        anim_order_map.Clear();
        skin_swaps.Clear();
    }

    int getNumMeshIndices(String name_in)
    {
        var cur_data = mesh_map[name_in];
        return cur_data.Item2 - cur_data.Item1 + 1;
    }

    private void genSortedMeshNames(CreaturePackPlayer pack_player)
    {
        mesh_sorted_names.Clear();
        foreach (var meshData in pack_player.data.meshRegionsList)
        {
            foreach (var cmpMeshData in mesh_map)
            {
                uint cmpMinIdx = (uint)pack_player.data.points.Length, cmpMaxIdx = 0;
                for(int k = cmpMeshData.Value.Item1; k <= cmpMeshData.Value.Item2; k++)
                {
                    var cur_idx = pack_player.data.indices[k];
                    cmpMinIdx = Math.Min(cmpMinIdx, cur_idx);
                    cmpMaxIdx = Math.Max(cmpMaxIdx, cur_idx);
                }

                if ((meshData.first == cmpMinIdx) 
                    && (meshData.second == cmpMaxIdx))
                {
                    mesh_sorted_names.Add(cmpMeshData.Key);
                }
            }
        }
    }

    public bool buildSkinSwapIndices(
        String swap_name,
        uint[] src_indices,
        List<int> skin_swap_indices,
        CreaturePackPlayer pack_player
    )
    {
        // Generate sorted names in mesh drawing order
        if (mesh_sorted_names.Count == 0)
        {
            genSortedMeshNames(pack_player);
        }

        // Now Generate Skin Swap indices
        if (!skin_swaps.ContainsKey(swap_name))
        {
            skin_swap_indices.Clear();
            return false;
        }

        var swap_set = skin_swaps[swap_name];
        active_skin_swap_names.Clear();
        int total_size = 0;
        foreach (var cur_data in mesh_map)
        {
            var cur_name = cur_data.Key;
            if (swap_set.Contains(cur_name))
            {
                total_size += getNumMeshIndices(cur_name);
                active_skin_swap_names.Add(cur_name);
            }
        }

        skin_swap_indices.Clear();

        int offset = 0;
        pack_player.mesh_ranges.Clear();
        foreach (var region_name in mesh_sorted_names)
        {
            if (swap_set.Contains(region_name))
            {
                var num_indices = getNumMeshIndices(region_name);
                var cur_range = mesh_map[region_name];
                int min_idx = -1;
                int max_idx = -1;
                for (int j = 0; j < getNumMeshIndices(region_name); j++)
                {
                    var local_idx = cur_range.Item1 + j;
                    var pt_indice = (int)src_indices[local_idx];
                    skin_swap_indices.Add(pt_indice);

                    if(min_idx < 0)
                    {
                        min_idx = pt_indice;
                        max_idx = min_idx;
                    }
                    else
                    {
                        min_idx = Math.Min(min_idx, pt_indice);
                        max_idx = Math.Max(max_idx, pt_indice);
                    }
                }

                offset += num_indices;
                pack_player.mesh_ranges.Add(new CreaturePackPlayer.CreaturePackMeshRange(min_idx, max_idx));
            }
        }

        return true;
    }
}

public class CreaturePackSplitAnimAsset : MonoBehaviour
{
    public TextAsset creaturePackSplitAnimBytes = null;

#if UNITY_EDITOR
    [MenuItem("GameObject/CreaturePack/CreaturePackSplitAnimAsset")]
    static CreaturePackSplitAnimAsset CreateCreaturePackSplitAnimAsset()
    {
        GameObject newObj = new GameObject();
        newObj.name = "New CreaturePack Split Anim Asset";
        CreaturePackSplitAnimAsset new_asset;
        new_asset = newObj.AddComponent<CreaturePackSplitAnimAsset>() as CreaturePackSplitAnimAsset;
        return new_asset;
    }
#endif

}

public class CreaturePackAsset : MonoBehaviour {
    public TextAsset creaturePackBytes = null, creatureMetaJSON = null;
    private bool is_dirty = false;
    private CreaturePackLoader packData = null;
    public Dictionary<String, Vector2> anchor_points = null;
    public CreatureCompositePlayer composite_player = null;
    public CreaturePackMetaData meta_data = null;
    public bool load_multicore = true; // Enable or Disable this to load data with multiple cores
    public CreaturePackAsset()
    {
        ResetState();
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/CreaturePack/CreaturePackAsset")]
    static CreaturePackAsset CreateCreaturePackAsset()
    {
        GameObject newObj = new GameObject();
        newObj.name = "New Creature Asset";
        CreaturePackAsset new_asset;
        new_asset = newObj.AddComponent<CreaturePackAsset>() as CreaturePackAsset;

        return new_asset;
    }
#endif

    public void ResetState()
    {
        creaturePackBytes = null;
        creatureMetaJSON = null;
        packData = null;
        is_dirty = false;
    }

    public bool HasNoValidAsset()
    {
        return !creaturePackBytes;
    }

    public bool GetIsDirty()
    {
        return is_dirty;
    }

    public void SetIsDirty(bool flagIn)
    {
        is_dirty = flagIn;
    }

    public void LoadMetaData()
    {
        if (!creatureMetaJSON)
        {
            return;
        }

        var readJSON = JSON.Parse(creatureMetaJSON.text);
        // Load Anchor Points
        if (anchor_points != null)
        {
            return;
        }

        anchor_points = new Dictionary<String, Vector2>();
        if (readJSON.HasKey("AnchorPoints"))
        {
            var readAnchors = readJSON["AnchorPoints"]["anchors"];
            for (int i = 0; i < readAnchors.Count; i++)
            {
                var readData = readAnchors[i];
                var readName = readData["name"];
                var readPt = readData["point"];
                anchor_points[readName] = new Vector2(readPt[0].AsFloat, readPt[1].AsFloat);
            }
        }

        // Load Composite Clips
        if (readJSON.HasKey("CompositeClips"))
        {
            composite_player = new CreatureCompositePlayer();
            composite_player.composite_clips = new Dictionary<string, List<CreatureCompositeClip>>();
            var readCompClips = readJSON["CompositeClips"];
            foreach (var subkey in readCompClips.Keys)
            {
                List<CreatureCompositeClip> write_list = new List<CreatureCompositeClip>();
                var subname = subkey.Value;
                var sublist = readCompClips[subname];
                for (int j = 0; j < sublist.Count; j++)
                {
                    var cur_name = sublist[j]["clip"].Value;
                    var start_frame = sublist[j]["start"].AsInt;
                    var end_frame = sublist[j]["end"].AsInt;

                    write_list.Add(new CreatureCompositeClip(cur_name, start_frame, end_frame));
                }

                composite_player.composite_clips[subname] = write_list;
            }
        }

        // MetaData object
        meta_data = new CreaturePackMetaData();

        // Mesh Regions
        if (readJSON.HasKey("meshes"))
        {
            var all_meshes = readJSON["meshes"];
            foreach (var region_key in all_meshes.Keys)
            {
                var region_name = region_key.Value;
                var cur_obj = all_meshes[region_name];
                int start_index = cur_obj["startIndex"].AsInt;
                int end_index = cur_obj["endIndex"].AsInt;

                meta_data.mesh_map[region_name] = new CreaturePackUtils.Tuple<int, int>(start_index, end_index);
            }
        }

        // Skin Swaps
        if (readJSON.HasKey("skinSwapList"))
        {
            var skin_swap_obj = readJSON["skinSwapList"];
            foreach (var cur_key in skin_swap_obj.Keys)
            {
                var swap_name = cur_key.Value;
                var swap_data = skin_swap_obj[swap_name]["swap"];
                var swap_items = swap_data["swap_items"];
                HashSet<String> swap_set = new HashSet<string>();
                for(int j = 0; j < swap_items.Count; j++)
                {
                    var cur_item = swap_items[j].Value;
                    swap_set.Add(cur_item);
                }

                meta_data.skin_swaps[swap_name] = swap_set;
            }
        }

    }

    public CreaturePackLoader GetCreaturePackLoader()
    {
        if(HasNoValidAsset())
        {
            Debug.LogError("Input CreaturePack file not set for CreaturePackAsset: " + name, this);
            ResetState();
            return null;
        }

        if(packData != null)
        {
            return packData;
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();

        Stream readStream = new MemoryStream(creaturePackBytes.bytes);
        packData = new CreaturePackLoader(readStream, load_multicore);

        sw.Stop();
        Debug.Log("Loading time for: " + name + " took: " + sw.ElapsedMilliseconds.ToString() + " ms");

        is_dirty = true;

        return packData;
    }

     // Use this for initialization
     void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
