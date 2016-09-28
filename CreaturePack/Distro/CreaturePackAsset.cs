using UnityEngine;
using CreaturePackModule;
using System;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreaturePackAsset : MonoBehaviour {
    public TextAsset creaturePackBytes = null;
    private bool is_dirty = false;
    private CreaturePackLoader packData = null;

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
