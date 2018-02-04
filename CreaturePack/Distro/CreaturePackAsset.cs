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
            pack_player.setRunTime(sublist[idx].start_frame);
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
    public void update(float delta_step, CreaturePackPlayer pack_player)
    {
        var pre_time = pack_player.getRunTime();
        pack_player.isLooping = true;
        pack_player.isPlaying = true;
        pack_player.stepTime(delta_step);

        var post_time = pack_player.getRunTime();
        var sublist = composite_clips[active_name];

        if ((post_time < pre_time) || (post_time > sublist[active_idx].end_frame))
        {
            active_idx = (active_idx + 1) % sublist.Count;
            setupForSubClip(pack_player, active_idx, active_name);
        }
    }
}

public class CreaturePackAsset : MonoBehaviour {
    public TextAsset creaturePackBytes = null, creatureMetaJSON = null;
    private bool is_dirty = false;
    private CreaturePackLoader packData = null;
    public Dictionary<String, Vector2> anchor_points = null;
    public CreatureCompositePlayer composite_player = null;
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
        packData = new CreaturePackLoader(readStream);

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
