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
using MeshBoneUtil;
using XnaGeometry;
using JsonFx;

namespace CreatureModule
{
	// Utils
	class Utils
	{
		public static Dictionary<string, object> LoadCreatureJSONData(string filename_in)
		{
			string text = System.IO.File.ReadAllText (filename_in);
			//var raw_obj = new System.Text.Json.JsonParser().Parse(text);

			Dictionary<string, object> ret_dict = null;
			ret_dict = JsonFx.Json.JsonReader.Deserialize(text, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
			//ret_dict = (Dictionary<string, object>)raw_obj;

			return ret_dict;
		}

		public static Dictionary<string, object> LoadCreatureJSONDataFromString(string text_in)
		{
			//var raw_obj = new System.Text.Json.JsonParser().Parse(text_in);

			Dictionary<string, object> ret_dict = null;
			ret_dict = JsonFx.Json.JsonReader.Deserialize(text_in, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
			//ret_dict = (Dictionary<string, object>)raw_obj;

			return ret_dict;
		}

		public static Dictionary<string, object> LoadCreatureFlatDataFromBytes(byte[] flat_bytes)
		{
			return CreatureFlatDataReader.Utils.LoadCreatureFlatDataFromBytes (flat_bytes);
		}
			
			public static List<string> GetAllAnimationNames(Dictionary<string, object> json_data)
		{
			Dictionary<string, object> json_animations = (Dictionary<string, object>)json_data["animation"];
			List<string> keyList = new List<string>(json_animations.Keys);

			return keyList;
		}

		public static float[] getFloatArray(System.Object raw_data)
		{
			if(raw_data.GetType() == typeof(double[]))
			{
				double[] float_obj = (double[])raw_data;
				float[] float_array = Array.ConvertAll (float_obj, item => (float)Convert.ToSingle(item));

				return float_array;
			}

			System.Object[] cur_obj = (System.Object[])raw_data;

			float[] ret_array = Array.ConvertAll (cur_obj, item => (float)Convert.ToSingle(item));

			return ret_array;
		}

		public static int[] getIntArray(System.Object raw_data)
		{
			System.Object[] cur_obj = (System.Object[])raw_data;

			int[] raw_array = Array.ConvertAll(cur_obj, item => (int)Convert.ToInt32(item));

			return raw_array;
		}


		public static List<XnaGeometry.Vector2> ReadPointsArray2DJSON(Dictionary<string, object> data,
		                                               string key)
		{
			if(data[key].GetType() == typeof(double[]))
			{
				double[] double_obj = (double[])data[key];

				List<XnaGeometry.Vector2> ret_double_list = new List<XnaGeometry.Vector2>(double_obj.Length);
				int num_dbl_points = double_obj.Length / 2;
				for (int i = 0; i < num_dbl_points; i++) 
				{
					int cur_index = i * 2;
					ret_double_list.Add(
								new XnaGeometry.Vector2(Convert.ToDouble(double_obj[0 + cur_index]),
														Convert.ToDouble(double_obj[1 + cur_index])) );

				}
				
				return ret_double_list;
			}

			System.Object[] cur_obj = (System.Object[])data[key];

			List<XnaGeometry.Vector2> ret_list = new List<XnaGeometry.Vector2>(cur_obj.Length);
			int num_points = cur_obj.Length / 2;
			for (int i = 0; i < num_points; i++) 
			{
				int cur_index = i * 2;
				ret_list.Add(
					new XnaGeometry.Vector2(Convert.ToDouble(cur_obj[0 + cur_index]),
				                        Convert.ToDouble(cur_obj[1 + cur_index])) );

			}
			
			return ret_list;
		}

		public static List<float> ReadFloatArray3DJSON(Dictionary<string, object> data,
		                                             string key)
		{
			if(data[key].GetType() == typeof(double[]))
			{
				double[] cur_double_obj = (double[])data[key];

				List<float> ret_double_list = new List<float>(cur_double_obj.Length);
				int num_double_points = cur_double_obj.Length / 2;

				for (int i = 0; i < num_double_points; i++) 
				{
					int cur_index = i * 2;
					ret_double_list.Add((float)Convert.ToSingle(cur_double_obj[0 + cur_index]));
					ret_double_list.Add((float)Convert.ToSingle(cur_double_obj[1 + cur_index]));
					ret_double_list.Add(0);
				}
				
				return ret_double_list;
			}

			System.Object[] cur_obj = (System.Object[])data[key];

			List<float> ret_list = new List<float>(cur_obj.Length);
			int num_points = cur_obj.Length / 2;

			for (int i = 0; i < num_points; i++) 
			{
				int cur_index = i * 2;
				ret_list.Add((float)Convert.ToSingle(cur_obj[0 + cur_index]));
				ret_list.Add((float)Convert.ToSingle(cur_obj[1 + cur_index]));
				ret_list.Add(0);
			}
			
			return ret_list;
		}

		public static bool ReadBoolJSON(Dictionary<string, object> data,
		                                string key)
		{
			bool val = (bool)data [key];
			return val;
		}

		public static List<float> ReadFloatArrayJSON(Dictionary<string, object> data,
		                                      string key)
		{
			if(data[key].GetType() == typeof(double[]))
			{
				double[] cur_double_obj = (double[])data[key];
				List<float> ret_double_list = new List<float>(cur_double_obj.Length);
				for (int i = 0; i < cur_double_obj.Length; i++) {
					ret_double_list.Add(Convert.ToSingle (cur_double_obj[i]));
				}
				
				return ret_double_list;
			}

			System.Object[] cur_obj = (System.Object[])data[key];
			List<float> ret_list = new List<float>(cur_obj.Length);
			for (int i = 0; i < cur_obj.Length; i++) {
				ret_list.Add(Convert.ToSingle (cur_obj[i]));
			}
			
			return ret_list;
		}

		public static List<int> ReadIntArrayJSON(Dictionary<string, object> data,
		                                    string key)
		{
			if(data[key].GetType() == typeof(int[]))
			{
				int[] cur_int_data = (int[])data[key];
				List<int> ret_int_list = new List<int>(cur_int_data.Length);
				for (int i = 0; i < cur_int_data.Length; i++) {
					ret_int_list.Add(Convert.ToInt32(cur_int_data[i]));
				}

				return ret_int_list;
			}
			else if(data[key].GetType() == typeof(System.Object[]))
			{
				System.Object[] cur_obj = (System.Object[])data[key];
				List<int> ret_list = new List<int>(cur_obj.Length);
				for (int i = 0; i < cur_obj.Length; i++) {
					ret_list.Add(Convert.ToInt32(cur_obj[i]));
				}
				
				return ret_list;
			}

			return new List<int>(0);
		}

		public static XnaGeometry.Matrix ReadMatrixJSON(Dictionary<string, object> data,
		                                                string key)
		{
			float[] raw_array = getFloatArray(data[key]);
			return new XnaGeometry.Matrix(raw_array[0], raw_array[1], raw_array[2], raw_array[3],
			                              raw_array[4], raw_array[5], raw_array[6], raw_array[7],
			                              raw_array[8], raw_array[9], raw_array[10], raw_array[11],
			                              raw_array[12], raw_array[13], raw_array[14], raw_array[15]);
		}

		public static XnaGeometry.Vector4 ReadVector4JSON(Dictionary<string, object> data,
		                                                  string key)
		{
			float[] raw_array = getFloatArray(data[key]);
			return new XnaGeometry.Vector4(raw_array[0], raw_array[1], 0, 1);
		}

		public static XnaGeometry.Vector2 ReadVector2JSON(Dictionary<string, object> data,
		                                                  string key)
		{
			float[] raw_array = getFloatArray(data[key]);
			return new XnaGeometry.Vector2(raw_array[0], raw_array[1]);
		}

		public static MeshBone CreateBones(Dictionary<string, object> json_obj,
		                                   string key)
		{
			MeshBone root_bone = null;
			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
			Dictionary<int, Tuple<MeshBone, List<int> > > bone_data = new Dictionary<int, Tuple<MeshBone, List<int> > >();
			Dictionary<int, int> child_set = new Dictionary<int, int>();
			
			// layout bones
			foreach(KeyValuePair<string, object> cur_node in base_obj)
			{

				string cur_name = cur_node.Key;
				Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;

				int cur_id =  Convert.ToInt32(node_dict["id"]); //GetJSONNodeFromKey(*cur_node, "id")->value.toNumber();
				XnaGeometry.Matrix cur_parent_mat = Utils.ReadMatrixJSON(node_dict, "restParentMat");

				XnaGeometry.Vector4 cur_local_rest_start_pt = Utils.ReadVector4JSON(node_dict, "localRestStartPt");
				XnaGeometry.Vector4 cur_local_rest_end_pt = Utils.ReadVector4JSON(node_dict, "localRestEndPt");
				List<int> cur_children_ids = Utils.ReadIntArrayJSON(node_dict, "children");
				
				MeshBone new_bone = new MeshBone(cur_name,
				                                 new XnaGeometry.Vector4(0,0,0,0),
				                                 new XnaGeometry.Vector4(0,0,0,0),
				                                 cur_parent_mat);
				new_bone.local_rest_start_pt = cur_local_rest_start_pt;
				new_bone.local_rest_end_pt = cur_local_rest_end_pt;
				new_bone.calcRestData();
				new_bone.setTagId(cur_id);
				
				bone_data[cur_id] = new Tuple<MeshBone, List<int> >(new_bone, cur_children_ids);
				
				foreach(int cur_child_id in cur_children_ids) {
					child_set.Add(cur_child_id, cur_child_id);
				}
			}
			
			// Find root
			foreach(KeyValuePair<int, Tuple<MeshBone, List<int> >> cur_data in bone_data)
			{
				int cur_id = cur_data.Key;
				if(child_set.ContainsKey(cur_id) == false) {
					// not a child, so is root
					root_bone = cur_data.Value.Item1;
					break;
				}
			}
			
			// construct hierarchy
			foreach(KeyValuePair<int, Tuple<MeshBone, List<int> >> cur_data in bone_data)
			{
				MeshBone cur_bone = cur_data.Value.Item1;
				List<int> children_ids = cur_data.Value.Item2;
				foreach(int cur_child_id in children_ids)
				{
					MeshBone child_bone = bone_data[cur_child_id].Item1;
					cur_bone.addChild(child_bone);
				}
				
			}


			return root_bone;
		}

		public static List<MeshRenderRegion> CreateRegions(Dictionary<string, object> json_obj,
		                                                   string key,
		                                                   List<int> indices_in,
		                                                   List<float> rest_pts_in,
		                                                   List<float> uvs_in)
		{
			List<MeshRenderRegion> ret_regions = new List<MeshRenderRegion> ();
			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
			
			foreach (KeyValuePair<string, object> cur_node in base_obj)
			{
				string cur_name = cur_node.Key;
				Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;

				int cur_id = Convert.ToInt32 (node_dict["id"]); //(int)GetJSONNodeFromKey(*cur_node, "id")->value.toNumber();
				int cur_start_pt_index = Convert.ToInt32 (node_dict["start_pt_index"]); //(int)GetJSONNodeFromKey(*cur_node, "start_pt_index")->value.toNumber();
				int cur_end_pt_index = Convert.ToInt32 (node_dict["end_pt_index"]); //(int)GetJSONNodeFromKey(*cur_node, "end_pt_index")->value.toNumber();
				int cur_start_index = Convert.ToInt32 (node_dict["start_index"]); //(int)GetJSONNodeFromKey(*cur_node, "start_index")->value.toNumber();
				int cur_end_index = Convert.ToInt32 (node_dict["end_index"]); //(int)GetJSONNodeFromKey(*cur_node, "end_index")->value.toNumber();
				
				MeshRenderRegion new_region = new MeshRenderRegion(indices_in,
				                                                   rest_pts_in,
				                                                   uvs_in,
				                                                   cur_start_pt_index,
				                                                   cur_end_pt_index,
				                                                   cur_start_index,
				                                                   cur_end_index);

				new_region.setName(cur_name);
				new_region.setTagId(cur_id);
				
				// Read in weights
				Dictionary<string, List<float> > weight_map =
					new_region.normal_weight_map;
				Dictionary<string, object> weight_obj = (Dictionary<string, object>)node_dict["weights"];
				
				foreach(KeyValuePair<string, object> w_node in weight_obj)
				{

					string w_key = w_node.Key;
					List<float> values = ReadFloatArrayJSON(weight_obj, w_key);
					weight_map.Add(w_key, values);
				}
				
				ret_regions.Add(new_region);
			}
			
			return ret_regions;
		}

		public static Tuple<int, int> GetStartEndTimes(Dictionary<string, object> json_obj,
		                                           	   string key)
		{
			int start_time = 0;
			int end_time = 0;
			bool first = true;
			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
			
			foreach (KeyValuePair<string, object> cur_node in base_obj)
			{

				int cur_val = Convert.ToInt32(cur_node.Key);
				if(first) {
					start_time = cur_val;
					end_time = cur_val;
					first = false;
				}
				else {
					if(cur_val > end_time) {
						end_time = cur_val;
					}

					if(cur_val < start_time) {
						start_time = cur_val;
					}
				}
			}
			
			MeshBoneUtil.Tuple<int, int> ret_times = new MeshBoneUtil.Tuple<int,int>(start_time,end_time);
			return ret_times;
		}

		static public void FillBoneCache(Dictionary<string, object> json_obj,
		                          		 string key,
		                          		 int start_time,
		                          		 int end_time,
		                          		 ref MeshBoneCacheManager cache_manager)
		{
			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

			cache_manager.init(start_time, end_time);
			
			foreach (KeyValuePair<string, object> cur_node in base_obj)
			{
				int cur_time = Convert.ToInt32(cur_node.Key);
				List<MeshBoneCache> cache_list = new List<MeshBoneCache>();

				Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;
				foreach (KeyValuePair<string, object> bone_node in node_dict)
				{
					string cur_name = bone_node.Key;
					Dictionary<string, object> bone_dict = (Dictionary<string, object>)bone_node.Value;

					XnaGeometry.Vector4 cur_start_pt = Utils.ReadVector4JSON(bone_dict, "start_pt"); //ReadJSONVec4_2(*bone_node, "start_pt");
					XnaGeometry.Vector4 cur_end_pt = Utils.ReadVector4JSON(bone_dict, "end_pt"); //ReadJSONVec4_2(*bone_node, "end_pt");
					
					MeshBoneCache cache_data = new MeshBoneCache(cur_name);
					cache_data.setWorldStartPt(cur_start_pt);
					cache_data.setWorldEndPt(cur_end_pt);
					
					cache_list.Add(cache_data);
				}
				
				int set_index = cache_manager.getIndexByTime(cur_time);
				cache_manager.bone_cache_table[set_index] = cache_list;
			}
			
			cache_manager.makeAllReady();
		}

		public static void FillDeformationCache(Dictionary<string, object> json_obj,
		                                 		string key,
		                                 		int start_time,
		                                 		int end_time,
		                                 		ref MeshDisplacementCacheManager cache_manager)
		{
			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
			
			cache_manager.init(start_time, end_time);

			foreach (KeyValuePair<string, object> cur_node in base_obj)
			{
				int cur_time = Convert.ToInt32(cur_node.Key);

				List<MeshDisplacementCache> cache_list = new List<MeshDisplacementCache>();
				
				Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;
				foreach (KeyValuePair<string, object> mesh_node in node_dict)
				{
					string cur_name = mesh_node.Key;
					Dictionary<string, object> mesh_dict = (Dictionary<string, object>)mesh_node.Value;

					MeshDisplacementCache cache_data = new MeshDisplacementCache(cur_name);
					
					bool use_local_displacement = Utils.ReadBoolJSON(mesh_dict, "use_local_displacements"); //GetJSONNodeFromKey(*mesh_node, "use_local_displacements")->value.toBool();
					bool use_post_displacement = Utils.ReadBoolJSON(mesh_dict, "use_post_displacements"); //GetJSONNodeFromKey(*mesh_node, "use_post_displacements")->value.toBool();
					
					if(use_local_displacement) {
						List<XnaGeometry.Vector2> read_pts = Utils.ReadPointsArray2DJSON(mesh_dict,
						                                                          "local_displacements"); //ReadJSONPoints2DVector(*mesh_node, "local_displacements");
						cache_data.setLocalDisplacements(read_pts);
					}
					
					if(use_post_displacement) {
						List<XnaGeometry.Vector2> read_pts = Utils.ReadPointsArray2DJSON(mesh_dict,
						                                                          "post_displacements"); //ReadJSONPoints2DVector(*mesh_node, "post_displacements");
						cache_data.setPostDisplacements(read_pts);
					}
					
					cache_list.Add(cache_data);
				}
				
				int set_index = cache_manager.getIndexByTime(cur_time);
				cache_manager.displacement_cache_table[set_index] = cache_list;
			}
			
			cache_manager.makeAllReady();
		}

		public static void FillUVSwapCache(Dictionary<string, object> json_obj,
		                                   string key,
		                                   int start_time,
		                                   int end_time,
		                            	   ref MeshUVWarpCacheManager cache_manager)
		{
			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
			
			cache_manager.init(start_time, end_time);
			
			foreach (KeyValuePair<string, object> cur_node in base_obj)
			{
				int cur_time = Convert.ToInt32(cur_node.Key);

				List<MeshUVWarpCache> cache_list = new List<MeshUVWarpCache>();
				
				Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;
				foreach (KeyValuePair<string, object> uv_node in node_dict)
				{
					string cur_name = uv_node.Key;
					Dictionary<string, object> uv_dict = (Dictionary<string, object>)uv_node.Value;

					MeshUVWarpCache cache_data = new MeshUVWarpCache(cur_name);
					bool use_uv = Utils.ReadBoolJSON(uv_dict, "enabled"); //GetJSONNodeFromKey(*uv_node, "enabled")->value.toBool();
					cache_data.setEnabled(use_uv);
					if(use_uv) {
						XnaGeometry.Vector2 local_offset = Utils.ReadVector2JSON(uv_dict, "local_offset"); //ReadJSONVec2(*uv_node, "local_offset");
						XnaGeometry.Vector2 global_offset = Utils.ReadVector2JSON(uv_dict, "global_offset"); //ReadJSONVec2(*uv_node, "global_offset");
						XnaGeometry.Vector2 scale = Utils.ReadVector2JSON(uv_dict, "scale"); //ReadJSONVec2(*uv_node, "scale");
						cache_data.setUvWarpLocalOffset(local_offset);
						cache_data.setUvWarpGlobalOffset(global_offset);
						cache_data.setUvWarpScale(scale);
					}
					
					cache_list.Add(cache_data);
				}
				
				int set_index = cache_manager.getIndexByTime(cur_time);
				cache_manager.uv_cache_table[set_index] = cache_list;
			}
			
			cache_manager.makeAllReady();
		}

		public static void FillOpacityCache(Dictionary<string, object> json_obj,
		                                   string key,
		                                   int start_time,
		                                   int end_time,
		                                   ref MeshOpacityCacheManager cache_manager)
		{
			cache_manager.init(start_time, end_time);

			if(json_obj.ContainsKey(key) == false)
			{
				return;
			}

			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
			
			
			foreach (KeyValuePair<string, object> cur_node in base_obj)
			{
				int cur_time = Convert.ToInt32(cur_node.Key);
				
				List<MeshOpacityCache> cache_list = new List<MeshOpacityCache>();
				
				Dictionary<string, object> node_dict = (Dictionary<string, object>)cur_node.Value;
				foreach (KeyValuePair<string, object> opacity_node in node_dict)
				{
					string cur_name = opacity_node.Key;
					Dictionary<string, object> opacity_dict = (Dictionary<string, object>)opacity_node.Value;
					
					MeshOpacityCache cache_data = new MeshOpacityCache(cur_name);
					double cur_opacity =  Convert.ToDouble (opacity_dict["opacity"]);
					cache_data.setOpacity((float)cur_opacity);
					
					cache_list.Add(cache_data);
				}
				
				int set_index = cache_manager.getIndexByTime(cur_time);
				cache_manager.opacity_cache_table[set_index] = cache_list;
			}
			
			cache_manager.makeAllReady();
		}

		public static Dictionary<String, List<CreatureUVSwapPacket> >
		FillSwapUvPacketMap(Dictionary<string, object> json_obj, string key)
		{
			Dictionary<String, List<CreatureUVSwapPacket> >ret_map = new Dictionary<String, List<CreatureUVSwapPacket> >();

			if(json_obj.ContainsKey(key) == false)
			{
				return ret_map;
			}

			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];

			foreach(var cur_node in base_obj)
			{
				var cur_name = cur_node.Key;
				List<CreatureUVSwapPacket> cur_packets = new List<CreatureUVSwapPacket>();

				object[] node_list = (object[])cur_node.Value;

				foreach(var packet_node in node_list)
				{
					Dictionary<string, object> packet_dict = (Dictionary<string, object>)packet_node;
					var local_offset = ReadVector2JSON(packet_dict, "local_offset");
					var global_offset = ReadVector2JSON(packet_dict, "global_offset");
					var scale = ReadVector2JSON(packet_dict, "scale");
					int tag = Convert.ToInt32(packet_dict["tag"]);

					var new_packet = new CreatureUVSwapPacket(local_offset, global_offset, scale, tag);
					cur_packets.Add(new_packet);
				}

				ret_map[cur_name] = cur_packets;
			}

			return ret_map;
		}

		public static Dictionary<String, XnaGeometry.Vector2>
		FillAnchorPointMap(Dictionary<string, object> json_obj, string key)
		{
			Dictionary<String, XnaGeometry.Vector2> ret_map = new Dictionary<String, XnaGeometry.Vector2>();
			if(json_obj.ContainsKey(key) == false)
			{
				return ret_map;
			}

			Dictionary<string, object> base_obj = (Dictionary<string, object>)json_obj[key];
			object[] anchor_obj = (object[] )base_obj["AnchorPoints"];

			foreach(var cur_node in anchor_obj)
			{
				Dictionary<string, object> anchor_dict = (Dictionary<string, object>)cur_node;
				var cur_pt = ReadVector2JSON(anchor_dict, "point");
				var cur_name = (String)anchor_dict["anim_clip_name"];

				ret_map[cur_name] = cur_pt;
			}

			return ret_map;
		}

	}

		public class CreatureUVSwapPacket {
			public XnaGeometry.Vector2 local_offset;
			public XnaGeometry.Vector2 global_offset;
			public XnaGeometry.Vector2 scale;
			public int tag;

			public CreatureUVSwapPacket(XnaGeometry.Vector2 local_offset_in,
						XnaGeometry.Vector2 global_offset_in,
						XnaGeometry.Vector2 scale_in,
						int tag_in)
			{
				local_offset = local_offset_in;
				global_offset = global_offset_in;
				scale = scale_in;
				tag = tag_in;
			}


		};

	// Class for the creature character
	public class Creature {
		// mesh and skeleton data
		public List<int> global_indices;
		public List<float> global_pts, global_uvs;
		public List<float> render_pts;
		public List<byte> render_colours;
		public int total_num_pts, total_num_indices;
		public MeshRenderBoneComposition render_composition;
		public Dictionary<String, List<CreatureUVSwapPacket>> uv_swap_packets;
		public Dictionary<String, int> active_uv_swap_actions;
		public Dictionary<String, XnaGeometry.Vector2> anchor_point_map;
		public bool anchor_points_active;
		
		public Creature(ref Dictionary<string, object> load_data)
		{
			total_num_pts = 0;
			total_num_indices = 0;
			global_indices = null;
			global_pts = null;
			global_uvs = null;
			render_pts = null;
			render_colours = null;
			render_composition = null;
			uv_swap_packets = new Dictionary<String, List<CreatureUVSwapPacket>>();
			active_uv_swap_actions = new Dictionary<String, int>();
			anchor_point_map = new Dictionary<String, XnaGeometry.Vector2>();
			anchor_points_active = false;

			LoadFromData(ref load_data);
		}
		
		// Fills entire mesh with (r,g,b,a) colours
		public void FillRenderColours(byte r, byte g, byte b, byte a)
		{
			for(int i = 0; i < total_num_pts; i++)
			{
				int cur_colour_index = i * 4;
				render_colours[0 + cur_colour_index] = r;
				render_colours[1 + cur_colour_index] = g;
				render_colours[2 + cur_colour_index] = b;
				render_colours[3 + cur_colour_index] = a;
			}
		}
		
		public void LoadFromData(ref Dictionary<string, object> load_data)
		{
			// Load points and topology
			Dictionary<string, object> json_mesh = (Dictionary<string, object>)load_data["mesh"];

			global_pts = Utils.ReadFloatArray3DJSON(json_mesh, "points");
			total_num_pts = global_pts.Count / 3;

			global_indices = Utils.ReadIntArrayJSON (json_mesh, "indices");
			total_num_indices = global_indices.Count;

			global_uvs = Utils.ReadFloatArrayJSON (json_mesh, "uvs");

			render_colours = new List<byte>(new byte[total_num_pts * 4]);
			FillRenderColours(255, 255, 255, 255);

			render_pts = new List<float> (new float[global_pts.Count]);

			// Load bones
			MeshBone root_bone = Utils.CreateBones(load_data,
			                                       "skeleton");


			// Load regions
			List<MeshRenderRegion> regions = Utils.CreateRegions(json_mesh,
			                                                     "regions",
			                                                     global_indices,
			                                                     global_pts,
			                                                     global_uvs);

			// Add into composition
			render_composition = new MeshRenderBoneComposition();
			render_composition.setRootBone(root_bone);
			render_composition.getRootBone().computeRestParentTransforms();
			
			foreach(MeshRenderRegion cur_region in regions) {
				cur_region.setMainBoneKey(root_bone.getKey());
				cur_region.determineMainBone(root_bone);
				render_composition.addRegion(cur_region);
			}
			
			render_composition.initBoneMap();
			render_composition.initRegionsMap();
			
			foreach(MeshRenderRegion cur_region in regions) {
				cur_region.initFastNormalWeightMap(ref render_composition.bones_map);
			}
			
			render_composition.resetToWorldRestPts();

			// Fill up uv swap packets
			uv_swap_packets = Utils.FillSwapUvPacketMap(load_data, "uv_swap_items");

			// Load Anchor Points
			anchor_point_map = Utils.FillAnchorPointMap(load_data, "anchor_points_items");
		}

		public void SetActiveItemSwap(String region_name, int swap_idx)
		{
			active_uv_swap_actions[region_name] = swap_idx;
		}

		public void RemoveActiveItemSwap(String region_name)
		{
			active_uv_swap_actions.Remove(region_name);
		}

		public XnaGeometry.Vector2 GetAnchorPoint(String anim_clip_name_in)
		{
			if(anchor_point_map.ContainsKey(anim_clip_name_in))
			{
				return anchor_point_map[anim_clip_name_in];
			}

			return new XnaGeometry.Vector2(0,0);
		}
	}

	// Class for animating the creature character
	public class CreatureAnimation {
		public string name;
		public float start_time, end_time;
		public MeshBoneCacheManager bones_cache;
		public MeshDisplacementCacheManager displacement_cache;
		public MeshUVWarpCacheManager uv_warp_cache;
		public MeshOpacityCacheManager opacity_cache;
		public List<List<float> > cache_pts;

		public CreatureAnimation(ref Dictionary<string, object> load_data,
			                  	 string name_in)
		{
			name = name_in;
			bones_cache = new MeshBoneCacheManager ();
			displacement_cache = new MeshDisplacementCacheManager ();
			uv_warp_cache = new MeshUVWarpCacheManager ();
			opacity_cache = new MeshOpacityCacheManager ();
			cache_pts = new List<List<float> > ();

			LoadFromData(name_in, ref load_data);
		}

		public void LoadFromData(string name_in,
		                         ref Dictionary<string, object> load_data)
		{
			Dictionary<string, object> json_anim_base = (Dictionary<string, object>)load_data["animation"];
			Dictionary<string, object> json_clip = (Dictionary<string, object>)json_anim_base[name_in];

			Tuple<int, int> start_end_times = Utils.GetStartEndTimes(json_clip, "bones"); 
			start_time = start_end_times.Item1;
			end_time = start_end_times.Item2;

			// bone animation
			Utils.FillBoneCache(json_clip,
			                    "bones",
			                    (int)start_time,
			                    (int)end_time,
			                    ref bones_cache);
			
			// mesh deformation animation
			Utils.FillDeformationCache(json_clip,
			                           "meshes",
			                           (int)start_time,
			                           (int)end_time,
			                           ref displacement_cache);

			// uv swapping animation
			Utils.FillUVSwapCache(json_clip,
			                      "uv_swaps",
			                      (int)start_time,
			                      (int)end_time,
			                      ref uv_warp_cache);

			// opacity animation
			Utils.FillOpacityCache(json_clip,
			                       "mesh_opacities",
			                       (int)start_time,
			                       (int)end_time,
			                       ref opacity_cache);
		}

		int clipNum(int n, int lower, int upper) {
			return Math.Max(lower, Math.Min(n, upper));
		}

		int getIndexByTime(int time_in)
		{
			int retval = time_in - (int)start_time;
			retval = clipNum(retval, 0, (int)cache_pts.Count - 1);
			
			return retval;
		}
		
		public bool hasCachePts() {
			return cache_pts.Count != 0;
		}
		
		public void poseFromCachePts(float time_in, List<float> target_pts, int num_pts)
		{
			int cur_floor_time = getIndexByTime((int)Math.Floor(time_in));
			int cur_ceil_time = getIndexByTime((int)Math.Ceiling(time_in));
			float cur_ratio = (time_in - start_time) - (float)cur_floor_time;
			
			List<float> set_pt = target_pts;
			List<float> floor_pts = cache_pts[cur_floor_time];
			List<float> ceil_pts = cache_pts[cur_ceil_time];

			int ref_idx = 0;
			for(int i = 0; i < num_pts; i++)
			{
				
				
				set_pt[0 + ref_idx] = ((1.0f - cur_ratio) * floor_pts[0 + ref_idx]) + (cur_ratio * ceil_pts[0 + ref_idx]);
				set_pt[1 + ref_idx] = ((1.0f - cur_ratio) * floor_pts[1 + ref_idx]) + (cur_ratio * ceil_pts[1 + ref_idx]);
				set_pt[2 + ref_idx] = ((1.0f - cur_ratio) * floor_pts[2 + ref_idx]) + (cur_ratio * ceil_pts[2 + ref_idx]);

				ref_idx += 3;
			}
		}
	}

	// Class for managing a collection of animations and a creature character
	public class CreatureManager {
		public Dictionary<string, CreatureModule.CreatureAnimation> animations;
		public CreatureModule.Creature target_creature;
		public string active_animation_name;
		public bool is_playing;
		public float run_time;
		public float time_scale;
		public List<List<float> > blend_render_pts;
		public bool do_blending;
		public float blending_factor;
		public Dictionary<string, float> region_override_alphas;
		public List<string> active_blend_animation_names;
		public List<string> auto_blend_names;
		public Dictionary<string, float> active_blend_run_times;
		public bool do_auto_blending;
		public float auto_blend_delta;
		public bool should_loop;
		public float region_offsets_z;
		public Action<Dictionary<string, MeshBone> > bones_override_callback;

		public CreatureManager(CreatureModule.Creature target_creature_in)
		{
			target_creature = target_creature_in;
			is_playing = false;
			run_time = 0;
			time_scale = 30.0f;
			blending_factor = 0;
			region_offsets_z = 0.01f;
			animations = new Dictionary<string, CreatureAnimation> ();
			bones_override_callback = null;
			region_override_alphas = new Dictionary<string, float> ();

			blend_render_pts = new List<List<float> >();
			blend_render_pts.Add(new List<float> ());
			blend_render_pts.Add(new List<float> ());

			active_blend_animation_names = new List<string> ();
			active_blend_animation_names.Add ("");
			active_blend_animation_names.Add ("");

			do_auto_blending = false;
			auto_blend_delta = 0.0f;

			auto_blend_names = new List<string>();
			auto_blend_names.Add("");
			auto_blend_names.Add("");

			active_blend_run_times = new Dictionary<string, float>();

			should_loop = true;
		}

		// Create a point cache for a specific animation
		// This speeds up playback but you will lose the ability to directly
		// manipulate the bones.
		public void
		MakePointCache(String animation_name_in, int gapStep)
		{
			if (gapStep < 1) {
				gapStep = 1;
			}

			float store_run_time = getRunTime();
			CreatureAnimation cur_animation = animations[animation_name_in];
			if(cur_animation.hasCachePts())
			{
				// cache already generated, just exit
				return;
			}
			
			List<List<float> > cache_pts_list = cur_animation.cache_pts;
			
			//for(int i = (int)cur_animation.start_time; i <= (int)cur_animation.end_time; i++)
			int i = (int)cur_animation.start_time;
			while(true)
			{
				run_time = (float)i;
				List<float> new_pts = new List<float>(new float[target_creature.total_num_pts * 3]);
				PoseCreature(animation_name_in, new_pts, getRunTime());

				int realStep = gapStep;
				if(i + realStep > cur_animation.end_time)
				{
					realStep = (int)cur_animation.end_time - i;
				}

				bool firstCase = realStep > 1;
				bool secondCase = cache_pts_list.Count >= 1;
				if(firstCase && secondCase)
				{
					// fill in the gaps
					var prev_pts = cache_pts_list[cache_pts_list.Count - 1];
					for(int j = 0; j < realStep; j++)
					{
						float factor = (float)j / (float)realStep;
						var gap_pts = InterpFloatList(prev_pts, new_pts, factor);
						cache_pts_list.Add (gap_pts);
					}
				}

				cache_pts_list.Add(new_pts);
				i += realStep;

				if(i > cur_animation.end_time || realStep == 0)
				{
					break;
				}
			}
			
			setRunTime(store_run_time);
		}

		public List<float> InterpFloatList(List<float> firstList, List<float> secondList, float factor)
		{
			List<float> ret_float_list = new List<float> (firstList.Count);
			for (int i = 0; i < firstList.Count; i++) {
				float new_val = ((1.0f - factor) * firstList[i]) + (factor * secondList[i]);
				ret_float_list.Add (new_val);
			}

			return ret_float_list;
		}
		
		// Create an animation
		public void CreateAnimation(ref Dictionary<string, object> load_data,
		                     		string name_in)
		{
			CreatureModule.CreatureAnimation new_animation = new CreatureModule.CreatureAnimation(ref load_data,
			                                                                                       name_in);
			AddAnimation(new_animation);
		}

		// Create all animations
		public void CreateAllAnimations(ref Dictionary<string, object> load_data)
		{
			List<string> all_animation_names = Utils.GetAllAnimationNames (load_data);
			foreach (string cur_name in all_animation_names) 
			{
				CreateAnimation(ref load_data, cur_name);
			}

			SetActiveAnimationName (all_animation_names [0]);
		}
		
		// Add an animation
		public void AddAnimation(CreatureModule.CreatureAnimation animation_in)
		{
			animations[animation_in.name] = animation_in;
			active_blend_run_times[animation_in.name] = animation_in.start_time;
		}
		
		// Return an animation
		public CreatureModule.CreatureAnimation
			GetAnimation(string name_in)
		{
			return animations[name_in];
		}
		
		// Return the creature
		public CreatureModule.Creature
			GetCreature()
		{
			return target_creature;
		}

		// Returns all the animation names
		public List<string> GetAnimationNames()
		{
			List<string> ret_names = new List<string> ();
			foreach (string cur_name in animations.Keys) {
				ret_names.Add(cur_name);
			}

			return ret_names;
		}

		// Sets the current animation to be active by name
		public bool SetActiveAnimationName(string name_in)
		{
			if (name_in == null || animations.ContainsKey (name_in) == false) {
				return false;
			}

			active_animation_name = name_in;
			CreatureAnimation cur_animation = animations[active_animation_name];
			run_time = cur_animation.start_time;
			
			UpdateRegionSwitches (name_in);

			return true;
		}

		// Update the region switching properties
		private void UpdateRegionSwitches(string animation_name_in)
		{
			if (animations.ContainsKey(animation_name_in)) {
				var cur_animation = animations[animation_name_in];
				
				var displacement_cache_manager = cur_animation.displacement_cache;
				var displacement_table =
					displacement_cache_manager.displacement_cache_table[0];
				
				var uv_warp_cache_manager = cur_animation.uv_warp_cache;
				var uv_swap_table =
					uv_warp_cache_manager.uv_cache_table[0];
				
				MeshBoneUtil.MeshRenderBoneComposition render_composition =
					target_creature.render_composition;
				List<MeshBoneUtil.MeshRenderRegion> all_regions = render_composition.getRegions();

				int index = 0;
				foreach(MeshBoneUtil.MeshRenderRegion cur_region in all_regions) 
				{
					// Setup active or inactive displacements
					bool use_local_displacements = !(displacement_table[index].getLocalDisplacements().Count == 0);
					bool use_post_displacements = !(displacement_table[index].getPostDisplacements().Count == 0);
					cur_region.setUseLocalDisplacements(use_local_displacements);
					cur_region.setUsePostDisplacements(use_post_displacements);
					
					// Setup active or inactive uv swaps
					cur_region.setUseUvWarp(uv_swap_table[index].getEnabled());
					
					index++;
				}
			}
		}
		
		// Returns the name of the currently active animation
		public string GetActiveAnimationName()
		{
			return active_animation_name;
		}

		// Returns the table of all animations
		public Dictionary<string, CreatureModule.CreatureAnimation >
			GetAllAnimations()
		{
			return animations;
		}
		
		// Returns if animation is playing
		bool GetIsPlaying()
		{
			return is_playing;
		}
		
		// Sets whether the animation is playing
		public void SetIsPlaying(bool flag_in)
		{
			is_playing = flag_in;
		}
		
		// Sets the run time of the animation
		public void setRunTime(float time_in)
		{
			run_time = time_in;
			correctTime ();
		}
		
		// Increments the run time of the animation by a delta value
		public void increRunTime(float delta_in)
		{
			run_time += delta_in;
			correctTime ();
		}

		public void correctTime()
		{
			CreatureAnimation cur_animation = animations[active_animation_name];
			if((run_time > cur_animation.end_time) && should_loop)
			{
				run_time = cur_animation.start_time;
			}
			else if(run_time < cur_animation.start_time)
			{
				run_time = cur_animation.end_time;
			}
		}
		
		// Returns the current run time of the animation
		public float getRunTime()
		{
			return run_time;
		}
		
		// Runs a single step of the animation for a given delta timestep
		public void Update(float delta)
		{
			if(!is_playing)
			{
				return;
			}
			
			increRunTime(delta * time_scale);

			if (do_auto_blending) {
				ProcessAutoBlending();
				// process run times for blends
				IncreAutoBlendRunTimes(delta * time_scale);
			}
		
			RunCreature ();
		}

		public void RunAtTime(float time_in)
		{
			if(!is_playing)
			{
				return;
			}

			setRunTime (time_in);
			RunCreature ();
		}

		public void RunCreature()
		{
			if(do_blending)
			{
				for(int i = 0; i < 2; i++) {
					string cur_animation_name = active_blend_animation_names[i];
					CreatureAnimation cur_animation = animations[cur_animation_name];
					float cur_animation_run_time = active_blend_run_times[cur_animation_name];

					if(cur_animation.hasCachePts())
					{
						cur_animation.poseFromCachePts(cur_animation_run_time, blend_render_pts[i], target_creature.total_num_pts);
						ApplyUVSwapsAndColorChanges(cur_animation_name, blend_render_pts[i], cur_animation_run_time);
						PoseJustBones(cur_animation_name, cur_animation_run_time);
					}
					else {
						UpdateRegionSwitches(active_blend_animation_names[i]);
						PoseCreature(active_blend_animation_names[i], blend_render_pts[i], cur_animation_run_time);
					}
				}
				
				for(int j = 0; j < target_creature.total_num_pts * 3; j++)
				{
					int set_data_index = j;
					float read_data_1 = blend_render_pts[0][j];
					float read_data_2 = blend_render_pts[1][j];
					
					target_creature.render_pts[set_data_index] = 
						((1.0f - blending_factor) * (read_data_1)) +
							(blending_factor * (read_data_2));
				}
			}
			else {
				CreatureAnimation cur_animation = animations[active_animation_name];
				if(cur_animation.hasCachePts())
				{
					cur_animation.poseFromCachePts(getRunTime(), target_creature.render_pts, target_creature.total_num_pts);
					ApplyUVSwapsAndColorChanges(active_animation_name, target_creature.render_pts, getRunTime());
					PoseJustBones(cur_animation.name, getRunTime());
				}
				else {
					PoseCreature(active_animation_name, target_creature.render_pts, getRunTime());
				}
			}

			RunUVItemSwap();
		}
		
		// Sets scaling for time
		public void SetTimeScale(float scale_in)
		{
			time_scale = scale_in;
		}
		
		// Enables/Disables blending
		public void SetBlending(bool flag_in)
		{
			do_blending = flag_in;
			
			if (do_blending) {
				if (blend_render_pts [0].Count == 0) {
					blend_render_pts [0] = new List<float> (new float[target_creature.total_num_pts * 3]);
				}
				
				if (blend_render_pts [1].Count == 0) {
					blend_render_pts [1] = new List<float> (new float[target_creature.total_num_pts * 3]);
				}

			}
		}

		// Sets auto blending
		public void SetAutoBlending(bool flag_in)
		{
			do_auto_blending = flag_in;
			SetBlending(flag_in);
			
			if(do_auto_blending)
			{
				AutoBlendTo(active_animation_name, 0.1f);
			}
		}

		// Use auto blending to blend to the next animation
		public void AutoBlendTo(string animation_name_in, float blend_delta)
		{
			if(animation_name_in == auto_blend_names[1])
			{
				// already blending to that so just return
				return;
			}
			
			ResetBlendTime(animation_name_in);
			
			auto_blend_delta = blend_delta;
			auto_blend_names[0] = active_animation_name;
			auto_blend_names[1] = animation_name_in;
			blending_factor = 0;
			
			active_animation_name = animation_name_in;
			
			SetBlendingAnimations(auto_blend_names[0], auto_blend_names[1]);
		}

		public void ResetBendTime(string name_in)
		{
			CreatureAnimation cur_animation = animations[name_in];
			active_blend_run_times[name_in] = cur_animation.start_time;
		}

		// Resets animation to start time
		public void ResetToStartTimes()
		{
			if(animations.ContainsKey(active_animation_name) == false)
			{
				return;
			}
			
			// reset non blend time
			CreatureAnimation cur_animation = animations[active_animation_name];
			run_time = cur_animation.start_time;
			
			// reset blend times too
			foreach (KeyValuePair<string, float> blend_time_data in active_blend_run_times)
			{
				ResetBlendTime(blend_time_data.Key);
			}
		}

		private void ProcessAutoBlending()
		{
			// process blending factor
			blending_factor += auto_blend_delta;
			if(blending_factor > 1)
			{
				blending_factor = 1;
			}
		}

		private void IncreAutoBlendRunTimes(float delta_in)
		{
			string set_animation_name = "";
			foreach (string cur_animation_name in auto_blend_names)
			{
				if ((animations.ContainsKey(cur_animation_name)) 
				    && (set_animation_name.Equals(cur_animation_name) == false))
				{
					float cur_run_time = active_blend_run_times[cur_animation_name];
					cur_run_time += delta_in;
					cur_run_time = correctRunTime(cur_run_time, cur_animation_name);

					active_blend_run_times[cur_animation_name] = cur_run_time;
					
					set_animation_name = cur_animation_name;
				}
			}
		}

		private float correctRunTime(float time_in, string animation_name)
		{
			float ret_time = time_in;
			CreatureAnimation cur_animation = animations[animation_name];
			float anim_start_time = cur_animation.start_time;
			float anim_end_time = cur_animation.end_time;
			
			if (ret_time > anim_end_time)
			{
				if (should_loop)
				{
					ret_time = anim_start_time;
				}
				else {
					ret_time = anim_end_time;
				}
			}
			else if (ret_time < anim_start_time)
			{
				if (should_loop)
				{
					ret_time = anim_end_time;
				}
				else {
					ret_time = anim_start_time;
				}
			}
			
			return ret_time;
		}

		private void ResetBlendTime(string name_in)
		{

		}
		
		// Sets blending animation names
		public void SetBlendingAnimations(string name_1, string name_2)
		{
			active_blend_animation_names[0] = name_1;
			active_blend_animation_names[1] = name_2;
		}
			
		// Sets the blending factor
		public void SetBlendingFactor(float value_in)
		{
			blending_factor = value_in;
		}
		
		// Given a set of coordinates in local creature space,
		// see if any bone is in contact
		public string IsContactBone(XnaGeometry.Vector2 pt_in,
		                          float radius)
		{
			MeshBoneUtil.MeshBone cur_bone = target_creature.render_composition.getRootBone();
			return ProcessContactBone(pt_in, radius, cur_bone);
		}
		
		public string ProcessContactBone(XnaGeometry.Vector2 pt_in,
			                             float radius,
		                                 MeshBoneUtil.MeshBone bone_in)
		{
			string ret_name = "";
			XnaGeometry.Vector4 diff_vec = bone_in.getWorldEndPt() - bone_in.getWorldStartPt();

			XnaGeometry.Vector2 cur_vec = new XnaGeometry.Vector2(diff_vec.X, diff_vec.Y);
			float cur_length = (float)cur_vec.Length();

			XnaGeometry.Vector2 unit_vec = cur_vec;
			unit_vec.Normalize();

			XnaGeometry.Vector2 norm_vec = new XnaGeometry.Vector2(unit_vec.Y, unit_vec.X);

			XnaGeometry.Vector2 src_pt = new XnaGeometry.Vector2(bone_in.getWorldStartPt().X, bone_in.getWorldStartPt().Y);
			XnaGeometry.Vector2 rel_vec = pt_in - src_pt;
			float proj = (float)XnaGeometry.Vector2.Dot(rel_vec, unit_vec);
			
			if( (proj >= 0) && (proj <= cur_length))
			{
				float norm_proj = (float)XnaGeometry.Vector2.Dot(rel_vec, norm_vec);
				if(norm_proj <= radius)
				{
					return bone_in.getKey();
				}
			}
			
			List<MeshBone> cur_children = bone_in.getChildren();
			foreach(MeshBone cur_child in cur_children)
			{
				ret_name = ProcessContactBone(pt_in, radius, cur_child);
				if(!(ret_name.Equals(""))) {
					break;
				}
			}
			
			return ret_name;
		}

		public void UpdateRegionColours()
		{
			MeshBoneUtil.MeshRenderBoneComposition render_composition =
				target_creature.render_composition;
			List<MeshBoneUtil.MeshRenderRegion> cur_regions =
				render_composition.getRegions();

			for (int i = 0; i < cur_regions.Count; i++) {
				MeshBoneUtil.MeshRenderRegion cur_region = cur_regions[i];
				float read_val = cur_region.opacity;

				// see if there is an override alpha as well
				if(region_override_alphas.ContainsKey(cur_region.name))
				{
					read_val = region_override_alphas[cur_region.name];
				}

				if(read_val < 0.0f)
				{
					read_val = 0.0f;
				}
				else if(read_val > 100.0f)
				{
					read_val = 100.0f;
				}

				byte set_opacity =  (byte)(read_val / 100.0f * 255.0f);
				int cur_rgba_index = cur_region.getStartPtIndex() * 4;
				for(int j = 0; j < cur_region.getNumPts(); j++)
				{
					target_creature.render_colours[cur_rgba_index] = set_opacity;
					target_creature.render_colours[cur_rgba_index + 1] = set_opacity;
					target_creature.render_colours[cur_rgba_index + 2] = set_opacity;
					target_creature.render_colours[cur_rgba_index + 3] = set_opacity;

					cur_rgba_index += 4;
				}

			}
		}

		// Sets an override opacity/alpha value for a region
		public void SetOverrideRegionAlpha(string region_name_in, float value_in)
		{
			region_override_alphas [region_name_in] = value_in;
		}

		private void ApplyUVSwapsAndColorChanges(string animation_name_in,
		                                         List<float> target_pts,
		                                         float input_run_time)
		{
			CreatureAnimation cur_animation = animations[animation_name_in];
			
			MeshBoneUtil.MeshUVWarpCacheManager uv_warp_cache_manager = cur_animation.uv_warp_cache;
			MeshBoneUtil.MeshOpacityCacheManager opacity_cache_manager = cur_animation.opacity_cache;
			
			MeshBoneUtil.MeshRenderBoneComposition render_composition =
				target_creature.render_composition;

			Dictionary<string, MeshBoneUtil.MeshRenderRegion> regions_map =
				render_composition.getRegionsMap();

			uv_warp_cache_manager.retrieveValuesAtTime(input_run_time,
			                                           regions_map);
			
			opacity_cache_manager.retrieveValuesAtTime(input_run_time,
			                                           regions_map);
			
			UpdateRegionColours ();

			List<MeshBoneUtil.MeshRenderRegion> cur_regions =
				render_composition.getRegions();
			for(int j = 0; j < cur_regions.Count; j++) {
				MeshBoneUtil.MeshRenderRegion cur_region = cur_regions[j];

				if(cur_region.use_uv_warp)
				{
					cur_region.runUvWarp();
				}
				
				// add in z offsets for different regions
				for(int k = cur_region.getStartPtIndex() * 3;
				    k <= cur_region.getEndPtIndex() * 3; 
				    k+=3)
				{
					target_pts[k + 2] = j * region_offsets_z;
				}
			}
		}

		public void PoseJustBones(string animation_name_in,
		                          float input_run_time)
		{
			CreatureAnimation cur_animation = animations[animation_name_in];
			MeshBoneUtil.MeshRenderBoneComposition render_composition =
				target_creature.render_composition;

			MeshBoneUtil.MeshBoneCacheManager bone_cache_manager = cur_animation.bones_cache;
			Dictionary<string, MeshBoneUtil.MeshBone> bones_map =
				render_composition.getBonesMap();

			bone_cache_manager.retrieveValuesAtTime(input_run_time,
			                                        ref bones_map);

			AlterBonesByAnchor(bones_map, animation_name_in);    
			
			if(bones_override_callback != null) 
			{
				bones_override_callback(bones_map);
			}
		}

		public void RunUVItemSwap()
		{
			MeshBoneUtil.MeshRenderBoneComposition render_composition =
				target_creature.render_composition;			
			Dictionary<string, MeshBoneUtil.MeshRenderRegion> regions_map =
				render_composition.getRegionsMap();

			var swap_packets = target_creature.uv_swap_packets;
			var active_swap_actions = target_creature.active_uv_swap_actions;

			if((swap_packets.Count == 0) || (active_swap_actions.Count == 0))
			{
				return;
			}

			foreach(var cur_action in active_swap_actions)
			{
				if(regions_map.ContainsKey(cur_action.Key))
				{
					var swap_tag = cur_action.Value;
					var swap_list = swap_packets[cur_action.Key];
					foreach(var cur_item in swap_list)
					{
						if(cur_item.tag == swap_tag)
						{
							// Perform UV Item Swap
							var cur_region = regions_map[cur_action.Key];
							cur_region.setUvWarpLocalOffset(cur_item.local_offset);
							cur_region.setUvWarpGlobalOffset(cur_item.global_offset);
							cur_region.setUvWarpScale(cur_item.scale);
							cur_region.runUvWarp();

							break;
						}
					}
				}
			}
		}

		public void AlterBonesByAnchor(Dictionary<string, MeshBoneUtil.MeshBone> bones_map, String animation_name_in)
		{
			if(target_creature.anchor_points_active == false)
			{
				return;
			}

			var anchor_point = target_creature.GetAnchorPoint(animation_name_in);
			var anchor_vector =  new XnaGeometry.Vector4(anchor_point.X, anchor_point.Y, 0, 0);
			foreach(var cur_data in bones_map)
			{
				var cur_bone = cur_data.Value;
				var start_pt = cur_bone.getWorldStartPt();
				var end_pt = cur_bone.getWorldEndPt();

				start_pt -= anchor_vector;
				end_pt -= anchor_vector;

				cur_bone.setWorldStartPt(start_pt);
				cur_bone.setWorldEndPt(end_pt);
			}
		}
		
		public void PoseCreature(string animation_name_in,
		                  		 List<float> target_pts,
		                         float input_run_time)
		{
			CreatureAnimation cur_animation = animations[animation_name_in];
			
			MeshBoneUtil.MeshBoneCacheManager bone_cache_manager = cur_animation.bones_cache;
			MeshBoneUtil.MeshDisplacementCacheManager displacement_cache_manager = cur_animation.displacement_cache;
			MeshBoneUtil.MeshUVWarpCacheManager uv_warp_cache_manager = cur_animation.uv_warp_cache;
			MeshBoneUtil.MeshOpacityCacheManager opacity_cache_manager = cur_animation.opacity_cache;
			
			MeshBoneUtil.MeshRenderBoneComposition render_composition =
				target_creature.render_composition;
			
			// Extract values from caches
			Dictionary<string, MeshBoneUtil.MeshBone> bones_map =
				render_composition.getBonesMap();
			Dictionary<string, MeshBoneUtil.MeshRenderRegion> regions_map =
				render_composition.getRegionsMap();
			
			bone_cache_manager.retrieveValuesAtTime(input_run_time,
			                                        ref bones_map);

			AlterBonesByAnchor(bones_map, animation_name_in);                               

			if(bones_override_callback != null) 
			{
				bones_override_callback(bones_map);
			}

			displacement_cache_manager.retrieveValuesAtTime(input_run_time,
			                                                regions_map);
			uv_warp_cache_manager.retrieveValuesAtTime(input_run_time,
			                                           regions_map);

			opacity_cache_manager.retrieveValuesAtTime(input_run_time,
			                                           regions_map);

			UpdateRegionColours ();

			// Do posing, decide if we are blending or not
			List<MeshBoneUtil.MeshRenderRegion> cur_regions =
				render_composition.getRegions();
			Dictionary<string, MeshBoneUtil.MeshBone> cur_bones =
				render_composition.getBonesMap();
			
			render_composition.updateAllTransforms(false);
			for(int j = 0; j < cur_regions.Count; j++) {
				MeshBoneUtil.MeshRenderRegion cur_region = cur_regions[j];
				
				int cur_pt_index = cur_region.getStartPtIndex();

				cur_region.poseFinalPts(ref target_pts,
				                        cur_pt_index * 3,
				                         ref cur_bones);

				// add in z offsets for different regions
				for(int k = cur_region.getStartPtIndex() * 3;
				    k <= cur_region.getEndPtIndex() * 3; 
				    k+=3)
				{
					target_pts[k + 2] = j * region_offsets_z;
				}
			}
		}
		

	}

}

