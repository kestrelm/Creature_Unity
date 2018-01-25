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

public class CreaturePackAsset : MonoBehaviour {
    public TextAsset creaturePackBytes = null, creatureMetaJSON = null;
    private bool is_dirty = false;
    private CreaturePackLoader packData = null;
    public Dictionary<String, Vector2> anchor_points = null;

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
        if(!creatureMetaJSON)
        {
            return;
        }

        if(anchor_points != null)
        {
            return;
        }

        anchor_points = new Dictionary<String, Vector2>();
        var readJSON = JSON.Parse(creatureMetaJSON.text);
        if(readJSON.HasKey("AnchorPoints"))
        {
            var readAnchors = readJSON["AnchorPoints"]["anchors"];
            for(int i = 0; i < readAnchors.Count; i++)
            {
                var readData = readAnchors[i.ToString()];
                var readName = readData["name"];
                var readPt = readData["point"];
                anchor_points[readName] = new Vector2(readPt[0].AsFloat, readPt[1].AsFloat);
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
