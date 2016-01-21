using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace CreatureFlatDataReader {
	class Utils{
		public static Dictionary<string, object> LoadCreatureFlatDataFromBytes(byte[] flat_bytes)
		{
			FlatBuffers.ByteBuffer flat_bytes_buffer = new FlatBuffers.ByteBuffer (flat_bytes);
			Dictionary<string, object> ret_dict = new Dictionary<string, object>();


			var rootObj = CreatureFlatData.rootData.GetRootAsrootData (flat_bytes_buffer);
			var meshObj = rootObj.DataMesh;
			var skeletonObj = rootObj.DataSkeleton;
			var animationObject = rootObj.DataAnimation;
			var uvSwapItemsObject = rootObj.DataUvSwapItem;
			var anchorPointsObject = rootObj.DataAnchorPoints;

			// ------- Process Mesh --------------------------
			Dictionary<string, object> mesh_dict = new Dictionary<string, object> ();

			// Mesh Points
			Object[] write_points = new Object[meshObj.PointsLength];
			for (int i = 0; i < write_points.Length; i++) {
				write_points[i] = meshObj.GetPoints(i);
			}
			mesh_dict ["points"] = write_points;

			// Mesh Uvs
			Object[] write_uvs = new Object[meshObj.UvsLength];
			for (int i = 0; i < write_uvs.Length; i++) {
				write_uvs[i] = meshObj.GetUvs(i);
			}
			mesh_dict ["uvs"] = write_uvs;

			// Mesh Indices
			Object[] write_indices = new Object[meshObj.IndicesLength];
			for (int i = 0; i < write_indices.Length; i++) {
				write_indices[i] = meshObj.GetIndices(i);
			}
			mesh_dict ["indices"] = write_indices;

			// Mesh Regions
			Dictionary<string, object> mesh_region_list_dict = new Dictionary<string, object> ();
			for (int i = 0; i < meshObj.RegionsLength; i++) {
				var meshRegionObj = meshObj.GetRegions(i);
				Dictionary<string, object> mesh_region_dict = new Dictionary<string, object>();

				mesh_region_dict["start_pt_index"] = meshRegionObj.StartPtIndex;
				mesh_region_dict["end_pt_index"] = meshRegionObj.EndPtIndex;
				mesh_region_dict["start_index"] = meshRegionObj.StartIndex;
				mesh_region_dict["end_index"] = meshRegionObj.EndIndex;
				mesh_region_dict["id"] = meshRegionObj.Id;

				Dictionary<string, object> write_region_weights = new Dictionary<string, object>();
				for(int j = 0; j < meshRegionObj.WeightsLength; j++) {
					var curBoneWeight = meshRegionObj.GetWeights(j);

					Object [] write_bone_weights = new Object[curBoneWeight.WeightsLength];
					for(int m = 0; m < write_bone_weights.Length; m++)
					{
						write_bone_weights[m] = curBoneWeight.GetWeights(m);
					}

					write_region_weights[curBoneWeight.Name] = write_bone_weights;
				}

				mesh_region_dict["weights"] = write_region_weights;

				mesh_region_list_dict[meshRegionObj.Name] = mesh_region_dict;
			}
			mesh_dict ["regions"] = mesh_region_list_dict;

			
			ret_dict ["mesh"] = mesh_dict;

			// ------- Process Skeleton ----------------------
			Dictionary<string, object> skeleton_dict = new Dictionary<string, object> ();

			for (int i = 0; i < skeletonObj.BonesLength; i++) {
				var skeletonBoneObj = skeletonObj.GetBones(i);
				Dictionary<string, object> skeleton_bone_dict = new Dictionary<string, object>();

				skeleton_bone_dict["id"] = skeletonBoneObj.Id;

				Object [] write_skeleton_bone_restParentMat = new Object[skeletonBoneObj.RestParentMatLength];
				for(int j = 0; j < write_skeleton_bone_restParentMat.Length; j++) {
					write_skeleton_bone_restParentMat[j] = skeletonBoneObj.GetRestParentMat(j);
				}
				skeleton_bone_dict["restParentMat"] = write_skeleton_bone_restParentMat;

				Object [] write_skeleton_bone_localRestStartPt = new Object[skeletonBoneObj.LocalRestStartPtLength];
				for(int j = 0; j < write_skeleton_bone_localRestStartPt.Length; j++) {
					write_skeleton_bone_localRestStartPt[j] = skeletonBoneObj.GetLocalRestStartPt(j);
				}
				skeleton_bone_dict["localRestStartPt"] = write_skeleton_bone_localRestStartPt;

				Object [] write_skeleton_bone_localRestEndPt = new Object[skeletonBoneObj.LocalRestEndPtLength];
				for(int j = 0; j < write_skeleton_bone_localRestEndPt.Length; j++) {
					write_skeleton_bone_localRestEndPt[j] = skeletonBoneObj.GetLocalRestEndPt(j);
				}
				skeleton_bone_dict["localRestEndPt"] = write_skeleton_bone_localRestEndPt;

				Object [] write_skeleton_bone_children = new Object[skeletonBoneObj.ChildrenLength];
				for(int j = 0; j < write_skeleton_bone_children.Length; j++) {
					write_skeleton_bone_children[j] = skeletonBoneObj.GetChildren(j);
				}
				skeleton_bone_dict["children"] = write_skeleton_bone_children;

				skeleton_dict[skeletonBoneObj.Name] = skeleton_bone_dict;
			}

			ret_dict ["skeleton"] = skeleton_dict;

			// ------- Process Animations --------------------
			Dictionary<string, object> animation_dict = new Dictionary<string, object> ();

			for (int i = 0; i < animationObject.ClipsLength; i++) {
				var animationClipObj = animationObject.GetClips(i);
				var animationBonesObj = animationClipObj.Bones;
				var animationMeshObj = animationClipObj.Meshes;
				var animationUvSwapObj = animationClipObj.UvSwaps;
				var animationMeshOpacitiesObj = animationClipObj.MeshOpacities;

				Dictionary<string, object> animationClipDict = new Dictionary<string, object>();

				// Animation Bones
				Dictionary<string, object> animationBonesTimeSamplesDict = new Dictionary<string, object>();
				for(int j = 0; j < animationBonesObj.TimeSamplesLength; j++) {
					var animationBonesTimeSample = animationBonesObj.GetTimeSamples(j);
					String curTime = animationBonesTimeSample.Time.ToString();

					Dictionary<string, object> animationBonesDict = new Dictionary<string, object>();
					for(int k = 0; k < animationBonesTimeSample.BonesLength; k++) {
						Dictionary<string, object> animationSingleBoneDict = new Dictionary<string, object> ();
						var animationSingleBone = animationBonesTimeSample.GetBones(k);

						Object [] write_animation_single_bone_start_pt = new Object[animationSingleBone.StartPtLength];
						for(int m = 0; m < write_animation_single_bone_start_pt.Length; m++) {
							write_animation_single_bone_start_pt[m] = animationSingleBone.GetStartPt(m);
						}
						animationSingleBoneDict["start_pt"] = write_animation_single_bone_start_pt;

						Object [] write_animation_single_bone_end_pt = new Object[animationSingleBone.EndPtLength];
						for(int m = 0; m < write_animation_single_bone_end_pt.Length; m++) {
							write_animation_single_bone_end_pt[m] = animationSingleBone.GetEndPt(m);
						}
						animationSingleBoneDict["end_pt"] = write_animation_single_bone_end_pt;

						animationBonesDict[animationSingleBone.Name] = animationSingleBoneDict;
					}

					animationBonesTimeSamplesDict[curTime] = animationBonesDict;
				}

				animationClipDict["bones"] = animationBonesTimeSamplesDict;

				// Animation Meshes
				Dictionary<string, object> animationMeshTimeSamplesDict = new Dictionary<string, object>();
				for(int j = 0; j < animationMeshObj.TimeSamplesLength; j++) {
					var animationMeshTimeSample = animationMeshObj.GetTimeSamples(j);
					String curTime = animationMeshTimeSample.Time.ToString();

					Dictionary<string, object> animationMeshesDict = new Dictionary<string, object>();
					for(int k = 0; k < animationMeshTimeSample.MeshesLength; k++) {
						Dictionary<string, object> animationSingleMeshDict = new Dictionary<string, object>();
						var animationSingleMesh = animationMeshTimeSample.GetMeshes(k);

						animationSingleMeshDict["use_dq"] = animationSingleMesh.UseDq;
						animationSingleMeshDict["use_local_displacements"] = animationSingleMesh.UseLocalDisplacements;
						animationSingleMeshDict["use_post_displacements"] = animationSingleMesh.UsePostDisplacements;

						if(animationSingleMesh.UseLocalDisplacements) {
							Object [] write_animation_single_mesh_localDisplacements = new Object[animationSingleMesh.LocalDisplacementsLength];
							for(int m = 0; m < write_animation_single_mesh_localDisplacements.Length; m++) {
								write_animation_single_mesh_localDisplacements[m] = animationSingleMesh.GetLocalDisplacements(m);
							}
							animationSingleMeshDict["local_displacements"] = write_animation_single_mesh_localDisplacements;
						}

						if(animationSingleMesh.UsePostDisplacements) {
							Object [] write_animation_single_mesh_postDisplacements = new Object[animationSingleMesh.PostDisplacementsLength];
							for(int m = 0; m < write_animation_single_mesh_postDisplacements.Length; m++) {
								write_animation_single_mesh_postDisplacements[m] = animationSingleMesh.GetPostDisplacements(m);
							}
							animationSingleMeshDict["post_displacements"] = write_animation_single_mesh_postDisplacements;
						}

						animationMeshesDict[animationSingleMesh.Name] = animationSingleMeshDict;
					}

					animationMeshTimeSamplesDict[curTime] = animationMeshesDict;
				}

				animationClipDict["meshes"] = animationMeshTimeSamplesDict;

				// Animation UV Swaps
				Dictionary<string, object> animationUvSwapTimeSamplesDict = new Dictionary<string, object>();
				for(int j = 0; j < animationUvSwapObj.TimeSamplesLength; j++) {
					var animationUvSwapTimeSample = animationUvSwapObj.GetTimeSamples(j);
					String curTime = animationUvSwapTimeSample.Time.ToString();

					Dictionary<string, object> animationUVSwapDict = new Dictionary<string, object>();
					for(int k = 0; k < animationUvSwapTimeSample.UvSwapsLength; k++) {
						Dictionary<string, object> animationSingleUvSwapDict = new Dictionary<string, object>();
						var animationSingleUvSwap = animationUvSwapTimeSample.GetUvSwaps(k);

						animationSingleUvSwapDict["enabled"] = animationSingleUvSwap.Enabled;

						Object [] write_animation_single_uvswap_localOffset = new Object[animationSingleUvSwap.LocalOffsetLength];
						for(int m = 0; m < write_animation_single_uvswap_localOffset.Length; m++) {
							write_animation_single_uvswap_localOffset[m] = animationSingleUvSwap.GetLocalOffset(m);
						}
						animationSingleUvSwapDict["local_offset"] = write_animation_single_uvswap_localOffset;

						Object [] write_animation_single_uvswap_globalOffset = new Object[animationSingleUvSwap.GlobalOffsetLength];
						for(int m = 0; m < write_animation_single_uvswap_globalOffset.Length; m++) {
							write_animation_single_uvswap_globalOffset[m] = animationSingleUvSwap.GetGlobalOffset(m);
						}
						animationSingleUvSwapDict["global_offset"] = write_animation_single_uvswap_globalOffset;

						Object [] write_animation_single_uvswap_scale = new Object[animationSingleUvSwap.ScaleLength];
						for(int m = 0; m < write_animation_single_uvswap_scale.Length; m++) {
							write_animation_single_uvswap_scale[m] = animationSingleUvSwap.GetScale(m);
						}
						animationSingleUvSwapDict["scale"] = write_animation_single_uvswap_scale;

						animationUVSwapDict[animationSingleUvSwap.Name] = animationSingleUvSwapDict;
					}

					animationUvSwapTimeSamplesDict[curTime] = animationUVSwapDict;
				}

				animationClipDict["uv_swaps"] = animationUvSwapTimeSamplesDict;

				// Animation Mesh Opacities
				Dictionary<string, object> animationMeshOpacitiesTimeSamplesDict = new Dictionary<string, object>();
				for(int j = 0; j < animationMeshOpacitiesObj.TimeSamplesLength; j++) {
					var animationMeshOpacitiesTimeSample = animationMeshOpacitiesObj.GetTimeSamples(j);
					String curTime = animationMeshOpacitiesTimeSample.Time.ToString();

					Dictionary<string, object> animationMeshOpacitiesDict = new Dictionary<string, object>();
					for(int k = 0; k < animationMeshOpacitiesTimeSample.MeshOpacitiesLength; k++) {
						Dictionary<string, object> animationSingleMeshOpacitiesDict = new Dictionary<string, object>();
						var animationSingleMeshOpacities = animationMeshOpacitiesTimeSample.GetMeshOpacities(k);

						animationSingleMeshOpacitiesDict["opacity"] = animationSingleMeshOpacities.Opacity;

						animationMeshOpacitiesDict[animationSingleMeshOpacities.Name] = animationSingleMeshOpacitiesDict;
					}

					animationMeshOpacitiesTimeSamplesDict[curTime] = animationMeshOpacitiesDict;
				}

				animationClipDict["mesh_opacities"] = animationMeshOpacitiesTimeSamplesDict;

				animation_dict[animationClipObj.Name] = animationClipDict;
			}

			ret_dict ["animation"] = animation_dict;

			// uv item swaps
			Dictionary<string, object> uvItemSwapsDict = new Dictionary<string, object>();
			for(int j = 0; j < uvSwapItemsObject.MeshesLength; j++) {
				var curSwapMesh = uvSwapItemsObject.GetMeshes(j);
				object[] node_list = new object[curSwapMesh.ItemsLength];
				var mesh_name = curSwapMesh.Name;

				for(int k = 0; k < curSwapMesh.ItemsLength; k++) {
					var curItem = curSwapMesh.GetItems(k);
					Dictionary<string, object> packet_dict = new Dictionary<string, object>();

					Object [] write_localOffset = new Object[curItem.LocalOffsetLength];
					for(int m = 0; m < write_localOffset.Length; m++) {
						write_localOffset[m] = curItem.GetLocalOffset(m);
					}
					packet_dict["local_offset"] = write_localOffset;

					Object [] write_globalOffset = new Object[curItem.GlobalOffsetLength];
					for(int m = 0; m < write_globalOffset.Length; m++) {
						write_globalOffset[m] = curItem.GetGlobalOffset(m);
					}
					packet_dict["global_offset"] = write_globalOffset;

					Object [] write_scale = new Object[curItem.ScaleLength];
					for(int m = 0; m < write_scale.Length; m++) {
						write_scale[m] = curItem.GetScale(m);
					}
					packet_dict["scale"] = write_scale;

					packet_dict["tag"] = curItem.Tag;

					node_list[k] = packet_dict;
				}

				uvItemSwapsDict[mesh_name] = node_list;
			}

			ret_dict["uv_swap_items"] = uvItemSwapsDict;

			// anchor points
			Dictionary<string, object> anchorPointsBaseDict = new Dictionary<string, object>();

			Object[] anchorPointsList = new Object[anchorPointsObject.AnchorPointsLength];
			for(int j = 0; j < anchorPointsObject.AnchorPointsLength; j++)
			{
				var curItem = anchorPointsObject.GetAnchorPoints(j);

				Dictionary<string, object> anchor_dict = new Dictionary<string, object>();
				Object [] write_cur_point = new Object[curItem.PointLength];
				for(int m = 0; m < write_cur_point.Length; m++) {
					write_cur_point[m] = curItem.GetPoint(m);
				}

				anchor_dict["point"] = write_cur_point;
				anchor_dict["anim_clip_name"] = curItem.AnimClipName;

				anchorPointsList[j] = anchor_dict;
			}

			anchorPointsBaseDict["AnchorPoints"] = anchorPointsList;
			ret_dict["anchor_points_items"] = anchorPointsBaseDict;

			return ret_dict;
		}
	}
}
