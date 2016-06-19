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

[Serializable]
public class CreatureAnimationAssetData {
	[SerializeField]
	public int start_frame;
	[SerializeField]
	public int end_frame;

	[SerializeField]
	public bool make_point_cache;

	[SerializeField]
	public int cache_approximation;

	public CreatureAnimationAssetData(int start_frame_in, int end_frame_in) {
		start_frame = start_frame_in;
		end_frame = end_frame_in;
		make_point_cache = false;
		cache_approximation = 1;
	}
}

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();
	
	[SerializeField]
	private List<TValue> values = new List<TValue>();
	
	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		foreach(KeyValuePair<TKey, TValue> pair in this)
		{
			keys.Add(pair.Key);
			values.Add(pair.Value);
		}
	}
	
	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();
		
		if(keys.Count != values.Count)
			throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
		
		for(int i = 0; i < keys.Count; i++)
			this.Add(keys[i], values[i]);
	}
}

[Serializable] public class DictionaryOfStringAndAnimation : SerializableDictionary<string, CreatureAnimationAssetData> {}

public class CreatureAsset : MonoBehaviour
{
	public TextAsset creatureJSON, compressedCreatureJSON, flatCreatureData, creatureMetaJSON;
	public CreatureManager creature_manager = null;
	public CreatureMetaData creature_meta_data = null;
	private bool is_dirty;

	[SerializeField]
	public DictionaryOfStringAndAnimation animation_clip_overides = new DictionaryOfStringAndAnimation ();

	[SerializeField]
	public bool useCompressedAsset = false;

	[SerializeField]
	public bool useFlatDataAsset = false;

#if UNITY_EDITOR
	[MenuItem("GameObject/Creature/CreatureAsset")]
	static CreatureAsset CreateCreatureAsset()
	{
		GameObject newObj = new GameObject();
		newObj.name = "New Creature Asset";
		CreatureAsset new_asset;
		new_asset = newObj.AddComponent<CreatureAsset> () as CreatureAsset;
		
		return new_asset;
	}
#endif

	public CreatureAsset()
	{

	}

	public void ResetState () {
		creatureJSON = null;
		compressedCreatureJSON = null;
		creature_manager = null;
		is_dirty = false;
	}

	public bool GetIsDirty()
	{
		return is_dirty;
	}

	public void SetIsDirty(bool flag_in)
	{
		is_dirty = flag_in;
	}

	public void SaveCompressedText(string filename, string text_in)
	{
	/*
		byte[] text1 =   System.Text.Encoding.ASCII.GetBytes(text_in);
		byte[] compressed = LZMAtools.CompressByteArrayToLZMAByteArray(text1);
		File.WriteAllBytes (filename, compressed);
	*/
		Debug.LogWarning("This function is deprecated. Please use FlatBuffers Binary Format instead.");
	}

	public string DecodeCompressedBytes(byte[] bytes)
	{
	/*
		byte[] decompressed = LZMAtools.DecompressLZMAByteArrayToByteArray (bytes);
		return System.Text.Encoding.Default.GetString(decompressed);
	*/

		Debug.LogWarning("This function is deprecated. Please use FlatBuffers Binary Format instead.");
		return null;
	}

	public bool HasNoValidAsset()
	{
		bool regularCheck = !useCompressedAsset && !useFlatDataAsset && (creatureJSON == null);
		bool compressedCheck = useCompressedAsset && (creatureJSON == null);
		bool flatCheck = useFlatDataAsset && (flatCreatureData == null);

		return regularCheck || compressedCheck || flatCheck;
	}

	public string GetAssetString()
	{
		if (HasNoValidAsset ()) {
			return null;
		}

		string readString = null;
		if (useCompressedAsset) {
			readString = DecodeCompressedBytes (compressedCreatureJSON.bytes);
		} 
		else {
			readString = creatureJSON.text;
		}

		return readString;
	}

	public Dictionary<string, object> LoadCreatureJsonData()
	{
		Dictionary<string, object> load_data = null;

		if(useFlatDataAsset)
		{
			byte[] readBytes = flatCreatureData.bytes;
			load_data = CreatureModule.Utils.LoadCreatureFlatDataFromBytes(readBytes);
		}
		else {
			string readString = GetAssetString ();
			load_data = CreatureModule.Utils.LoadCreatureJSONDataFromString (readString);
		}

		return load_data;
	}

	public CreatureManager GetCreatureManager()
	{
		if (HasNoValidAsset())
		{
			Debug.LogError("Input Creature JSON file not set for CreatureAsset: " + name, this);
			ResetState ();
			return null;
		}

		if (creature_manager != null) 
		{
			return creature_manager;
		}
			
		Dictionary<string, object> load_data = LoadCreatureJsonData ();

		CreatureModule.Creature new_creature = new CreatureModule.Creature(ref load_data);
		creature_manager = new CreatureModule.CreatureManager (new_creature);
		creature_manager.CreateAllAnimations (ref load_data);

		var all_animations = creature_manager.animations;
		foreach (KeyValuePair<string, CreatureAnimationAssetData> entry in animation_clip_overides) 
		{
			var cur_name = entry.Key;
			var cur_animation_data = entry.Value;

			if(all_animations.ContainsKey(cur_name)) 
			{
				// Set Animation Frame Ranges
				all_animations[cur_name].start_time = cur_animation_data.start_frame;
				all_animations[cur_name].end_time = cur_animation_data.end_frame;

				// Decide if we need to make point caches
				if(cur_animation_data.make_point_cache)
				{
					var stopWatch = new System.Diagnostics.Stopwatch();
					stopWatch.Start();

					creature_manager.MakePointCache(cur_name, cur_animation_data.cache_approximation);

					stopWatch.Stop ();
					Debug.Log ("Creature Point Cache generation took: " + stopWatch.ElapsedMilliseconds);
				}
			}
		}



		is_dirty = true;

		// Load meta data if available
		creature_meta_data = null;
		if(creatureMetaJSON != null)
		{
			creature_meta_data = new CreatureMetaData();
		 	CreatureModule.Utils.BuildCreatureMetaData(creature_meta_data, creatureMetaJSON.text);
		}

		return creature_manager;
	}
}

