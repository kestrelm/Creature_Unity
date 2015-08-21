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
using System.Collections.Generic;
using XnaGeometry;

namespace MeshBoneUtil
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
      if (o.GetType() != typeof(Tuple<T1, T2>)) {
        return false;
      }

      var other = (Tuple<T1, T2>) o;

      return this == other;
    }

    public static bool operator==(Tuple<T1, T2> a, Tuple<T1, T2> b)
    {
      return 
        a.item1.Equals(b.item1) && 
        a.item2.Equals(b.item2);            
    }

    public static bool operator!=(Tuple<T1, T2> a, Tuple<T1, T2> b)
    {
      return !(a == b);
    }

    public void Unpack(Action<T1, T2> unpackerDelegate)
    {
      unpackerDelegate(Item1, Item2);
    }
  }

	public class dualQuat {
		public XnaGeometry.Quaternion real, imaginary;

		public dualQuat()
		{
			real.W = 0;
			real.X = 0;
			real.Y = 0;
			real.Z = 0;

			imaginary = real;
		}

		public static void zeroOut(ref dualQuat dq)
		{
			dq.real.W = 0;
			dq.real.X = 0;
			dq.real.Y = 0;
			dq.real.Z = 0;

			dq.imaginary = dq.real;
		}
		
		public dualQuat(XnaGeometry.Quaternion q0, XnaGeometry.Vector3 t)
		{
			real = q0;
			imaginary.W = -0.5f * ( t.X * q0.X + t.Y * q0.Y + t.Z * q0.Z);
			imaginary.X =  0.5f * ( t.X * q0.W + t.Y * q0.Z - t.Z * q0.Y);
			imaginary.Y =  0.5f * (-t.X * q0.Z + t.Y * q0.W + t.Z * q0.X);
			imaginary.Z =  0.5f * ( t.X * q0.Y - t.Y * q0.X + t.Z * q0.W);
		}
		
		public void add(ref dualQuat quat_in, float real_factor, float imaginary_factor)
		{
			real = real + (quat_in.real * real_factor);
			imaginary = imaginary + (quat_in.imaginary * imaginary_factor);

		}
		
		public void convertToMat(ref XnaGeometry.Matrix m)
		{
			float cur_length = (float)XnaGeometry.Quaternion.Dot(real, real);
			float w = (float)real.W , x = (float)real.X, y = (float)real.Y, z = (float)real.Z;
			float t0 = (float)imaginary.W, t1 = (float)imaginary.X, t2 = (float)imaginary.Y, t3 = (float)imaginary.Z;

			m.M11 = w*w + x*x - y*y - z*z;
			m.M12 = 2 * x * y - 2 * w * z;
			m.M13 = 2 * x * z + 2 * w * y;

			m.M21 = 2 * x * y + 2 * w * z;
			m.M22 = w * w + y * y - x * x - z * z;
			m.M23 = 2 * y * z - 2 * w * x;

			m.M31 = 2 * x * z - 2 * w * y;
			m.M32 = 2 * y * z + 2 * w * x;
			m.M33 = w * w + z * z - x * x - y * y;
			
			m.M41 = -2 * t0 * x + 2 * w * t1 - 2 * t2 * z + 2 * y * t3;
			m.M42 = -2 * t0 * y + 2 * t1 * z - 2 * x * t3 + 2 * w * t2;
			m.M43 = -2 * t0 * z + 2 * x * t2 + 2 * w * t3 - 2 * t1 * y;

			// ??
			m.M14 = 0;
			m.M24 = 0;
			m.M34 = 0;
			m.M44 = cur_length;
			m /= cur_length;
		}

		public void normalize()
		{
			float norm = (float)Math.Sqrt(real.W * real.W + real.X * real.X + real.Y * real.Y + real.Z * real.Z);
			
			real = real * (1.0 / norm);
			imaginary = imaginary  * (1.0 / norm);
		}
		
		public XnaGeometry.Vector3 transform(ref XnaGeometry.Vector3 p)
		{
			XnaGeometry.Vector3 v0;
			v0.X = real.X; v0.Y = real.Y; v0.Z = real.Z;

			XnaGeometry.Vector3 ve;
			ve.X = imaginary.X; ve.Y = imaginary.Y; ve.Z = imaginary.Z;

			XnaGeometry.Vector3 trans;
			trans = (ve*real.W - v0*imaginary.W + XnaGeometry.Vector3.Cross(v0, ve)) * 2.0f;

			return (XnaGeometry.Vector3.Transform(p, real)) + trans;
		}
		

	};

	public class Utils
	{
		public static XnaGeometry.Vector4 rotateVec4_90(XnaGeometry.Vector4 vec_in)
		{
			return new XnaGeometry.Vector4(-vec_in.Y, vec_in.X, vec_in.Z, vec_in.W);
		}
		
		public static XnaGeometry.Matrix calcRotateMat(XnaGeometry.Vector4 vec_in)
		{
			XnaGeometry.Vector4 dir = vec_in;
			dir.Normalize();
			
			XnaGeometry.Vector4 pep_dir = rotateVec4_90(dir);
			
			XnaGeometry.Vector3 cur_tangent = new XnaGeometry.Vector3(dir.X, dir.Y, 0);
			XnaGeometry.Vector3 cur_normal = new XnaGeometry.Vector3(pep_dir.X, pep_dir.Y, 0);
			XnaGeometry.Vector3 cur_binormal = new XnaGeometry.Vector3(0, 0, 1);
			
			//XnaGeometry.Matrix cur_rotate(cur_tangent, cur_normal, cur_binormal, glm::vec4(0,0,0,1));
			XnaGeometry.Matrix cur_rotate = new XnaGeometry.Matrix();
			cur_rotate = XnaGeometry.Matrix.Identity;

			cur_rotate.Right = cur_tangent;
			cur_rotate.Up = cur_normal;
			cur_rotate.Backward = cur_binormal;

			return cur_rotate;
		}
		
		public static float angleVec4(ref XnaGeometry.Vector4 vec_in)
		{
			float theta = (float)Math.Atan2(vec_in.Y, vec_in.X);
			if(theta < 0) {
				theta += 2.0f * (float)Math.PI;
			}
			
			return theta;
		}

	}

	// MeshBone
	public class MeshBone
	{
		public XnaGeometry.Matrix rest_parent_mat, rest_parent_inv_mat;
		public XnaGeometry.Matrix rest_world_mat, rest_world_inv_mat;
		public XnaGeometry.Matrix bind_world_mat, bind_world_inv_mat, parent_world_mat, parent_world_inv_mat;
		public XnaGeometry.Vector4 local_rest_start_pt, local_rest_end_pt;
		public XnaGeometry.Vector4 local_rest_dir, local_rest_normal_dir, local_binormal_dir;
		public XnaGeometry.Vector4 world_rest_start_pt, world_rest_end_pt;
		public XnaGeometry.Vector4 world_rest_pos;
		public float world_rest_angle;
		public float rest_length;
		public string key;
		public int tag_id;
		
		public XnaGeometry.Vector4 world_start_pt, world_end_pt;
		public XnaGeometry.Matrix world_delta_mat;
		public dualQuat world_dq;
		
		public List<MeshBone> children;

		public MeshBone(
			string key_in,
			XnaGeometry.Vector4 start_pt_in,
			XnaGeometry.Vector4 end_pt_in,
			XnaGeometry.Matrix parent_transform)
		{
			key = key_in;
			world_rest_angle = 0;
			setRestParentMat(parent_transform, null);
			setLocalRestStartPt(start_pt_in);
			setLocalRestEndPt(end_pt_in);
			setParentWorldInvMat(XnaGeometry.Matrix.Identity);
			setParentWorldMat(XnaGeometry.Matrix.Identity);
			local_binormal_dir = new XnaGeometry.Vector4(0,0,1,1);
			tag_id = 0;
			children = new List<MeshBone>();
		}

		public void setRestParentMat(XnaGeometry.Matrix transform_in,
		                      		 XnaGeometry.Matrix? inverse_in)
		{
			rest_parent_mat = transform_in;
			
			if(inverse_in == null) {
				rest_parent_inv_mat = new XnaGeometry.Matrix();
				XnaGeometry.Matrix.Invert(ref rest_parent_mat, out rest_parent_inv_mat);
			}
			else {
				rest_parent_inv_mat = inverse_in.Value;
			}
		}

		public void setParentWorldMat(XnaGeometry.Matrix transform_in)
		{
			parent_world_mat = transform_in;
		}
		
		public void setParentWorldInvMat(XnaGeometry.Matrix transform_in)
		{
			parent_world_inv_mat = transform_in;
		}
		
		public XnaGeometry.Vector4 getLocalRestStartPt()
		{
			return local_rest_start_pt;
		}
		
		public XnaGeometry.Vector4 getLocalRestEndPt()
		{
			return local_rest_end_pt;
		}

		public void setLocalRestStartPt(XnaGeometry.Vector4 world_pt_in)
		{
			local_rest_start_pt = XnaGeometry.Vector4.Transform(world_pt_in, rest_parent_inv_mat);
			calcRestData();
		}
		
		public void setLocalRestEndPt(XnaGeometry.Vector4 world_pt_in)
		{
			local_rest_end_pt = XnaGeometry.Vector4.Transform(world_pt_in, rest_parent_inv_mat);
			calcRestData();
		}
		
		public void calcRestData()
		{
			Tuple<XnaGeometry.Vector4, XnaGeometry.Vector4> 
			calc = computeDirs(local_rest_start_pt, local_rest_end_pt);

			local_rest_dir = calc.Item1;
			local_rest_normal_dir = calc.Item2;
			
			computeRestLength();
		}
		
		public void setWorldStartPt(XnaGeometry.Vector4 world_pt_in)
		{
			world_start_pt = world_pt_in;
		}
		
		public void setWorldEndPt(XnaGeometry.Vector4 world_pt_in)
		{
			world_end_pt = world_pt_in;
		}
		
		public void fixDQs(dualQuat ref_dq)
		{
			if( XnaGeometry.Quaternion.Dot(world_dq.real, ref_dq.real) < 0) {
				world_dq.real = -world_dq.real;
				world_dq.imaginary = -world_dq.imaginary;
			}
			
			for(int i = 0; i < children.Count; i++) {
				MeshBone cur_child = children[i];
				cur_child.fixDQs(world_dq);
			}
		}
		
		public void initWorldPts()
		{
			setWorldStartPt(getWorldRestStartPt());
			setWorldEndPt(getWorldRestEndPt());
			
			for(int i = 0; i < children.Count; i++) {
				children[i].initWorldPts();
			}
		}
		
		public XnaGeometry.Vector4 getWorldRestStartPt()
		{
			XnaGeometry.Vector4 ret_vec = XnaGeometry.Vector4.Transform(local_rest_start_pt, rest_parent_mat);
			return ret_vec;
		}
		
		public XnaGeometry.Vector4 getWorldRestEndPt()
		{
			XnaGeometry.Vector4 ret_vec = XnaGeometry.Vector4.Transform(local_rest_end_pt, rest_parent_mat);
			return ret_vec;
		}
		
		public float getWorldRestAngle()
		{
			return world_rest_angle;
		}
		
		public XnaGeometry.Vector4 getWorldRestPos()
		{
			return world_rest_pos;
		}
		
		public XnaGeometry.Vector4 getWorldStartPt()
		{
			return world_start_pt;
		}
		
		public XnaGeometry.Vector4 getWorldEndPt()
		{
			return world_end_pt;
		}
		
		public XnaGeometry.Matrix getRestParentMat()
		{
			return rest_parent_mat;
		}
		
		public XnaGeometry.Matrix getRestWorldMat()
		{
			return rest_world_mat;
		}
		
		public XnaGeometry.Matrix getWorldDeltaMat()
		{
			return world_delta_mat;
		}
		
		public XnaGeometry.Matrix getParentWorldMat()
		{
			return parent_world_mat;
		}
		
		public XnaGeometry.Matrix getParentWorldInvMat()
		{
			return parent_world_inv_mat;
		}
		
		public dualQuat getWorldDq()
		{
			return world_dq;
		}
		
		public void computeRestParentTransforms()
		{
			XnaGeometry.Vector3 cur_tangent = new XnaGeometry.Vector3(local_rest_dir.X, local_rest_dir.Y, 0);
			XnaGeometry.Vector3 cur_binormal = new XnaGeometry.Vector3(local_binormal_dir.X, local_binormal_dir.Y, local_binormal_dir.Z);
			XnaGeometry.Vector3 cur_normal = new XnaGeometry.Vector3(local_rest_normal_dir.X, local_rest_normal_dir.Y, 0);
			
			XnaGeometry.Matrix cur_translate = XnaGeometry.Matrix.Identity;
			cur_translate.Translation = new XnaGeometry.Vector3(local_rest_end_pt.X, local_rest_end_pt.Y, 0);

			XnaGeometry.Matrix cur_rotate = XnaGeometry.Matrix.Identity;
			cur_rotate.Right = cur_tangent;
			cur_rotate.Up = cur_normal;
			cur_rotate.Backward = cur_binormal;

			//XnaGeometry.Matrix cur_final = cur_translate * cur_rotate;
			XnaGeometry.Matrix cur_final = cur_rotate * cur_translate;

			//rest_world_mat = rest_parent_mat * cur_final;
			rest_world_mat =  cur_final * rest_parent_mat;

			XnaGeometry.Matrix.Invert(ref rest_world_mat, out rest_world_inv_mat);

			XnaGeometry.Vector4 world_rest_dir = getWorldRestEndPt() - getWorldRestStartPt();
			world_rest_dir.Normalize();
			world_rest_angle = Utils.angleVec4(ref world_rest_dir);
			world_rest_pos = getWorldRestStartPt();
			
			
			XnaGeometry.Matrix bind_translate = XnaGeometry.Matrix.Identity;
			bind_translate.Translation = new XnaGeometry.Vector3(getWorldRestStartPt().X, getWorldRestStartPt().Y, 0);

			XnaGeometry.Matrix bind_rotate = Utils.calcRotateMat(getWorldRestEndPt() - getWorldRestStartPt());
			//XnaGeometry.Matrix cur_bind_final = bind_translate * bind_rotate;
			XnaGeometry.Matrix cur_bind_final = bind_rotate * bind_translate;

			bind_world_mat = cur_bind_final;
			XnaGeometry.Matrix.Invert(ref bind_world_mat, out bind_world_inv_mat);
			
			for(int i = 0; i < children.Count; i++) {
				MeshBone cur_bone = children[i];
				cur_bone.setRestParentMat(rest_world_mat, rest_world_inv_mat);
				cur_bone.computeRestParentTransforms();
			}
		}
		
		public void computeParentTransforms()
		{
			XnaGeometry.Matrix translate_parent = XnaGeometry.Matrix.Identity;
			translate_parent.Translation = new XnaGeometry.Vector3(getWorldEndPt().X, getWorldEndPt().Y, 0);

			XnaGeometry.Matrix rotate_parent = Utils.calcRotateMat(getWorldEndPt() - getWorldStartPt());
			
//			XnaGeometry.Matrix final_transform = translate_parent * rotate_parent;
			XnaGeometry.Matrix final_transform = rotate_parent * translate_parent;
			XnaGeometry.Matrix final_inv_transform = new XnaGeometry.Matrix();
			XnaGeometry.Matrix.Invert(ref final_transform, out final_inv_transform);
			
			for(int i = 0; i < children.Count; i++) {
				MeshBone cur_bone = children[i];
				cur_bone.setParentWorldMat(final_transform);
				cur_bone.setParentWorldInvMat(final_inv_transform);
				cur_bone.computeParentTransforms();
			}
		}
		
		public void computeWorldDeltaTransforms()
		{
			Tuple<XnaGeometry.Vector4, XnaGeometry.Vector4> calc = computeDirs(world_start_pt, world_end_pt);
			XnaGeometry.Vector3 cur_tangent = new XnaGeometry.Vector3(calc.Item1.X, calc.Item1.Y, 0);
			XnaGeometry.Vector3 cur_normal = new XnaGeometry.Vector3(calc.Item2.X, calc.Item2.Y, 0);
			XnaGeometry.Vector3 cur_binormal = new XnaGeometry.Vector3(local_binormal_dir.X, local_binormal_dir.Y, local_binormal_dir.Z);

			XnaGeometry.Matrix cur_rotate = XnaGeometry.Matrix.Identity;
			cur_rotate.Right = cur_tangent;
			cur_rotate.Up = cur_normal;
			cur_rotate.Backward = cur_binormal;

			XnaGeometry.Matrix cur_translate = XnaGeometry.Matrix.Identity;
			cur_translate.Translation = new XnaGeometry.Vector3(world_start_pt.X, world_start_pt.Y, 0);

			/*
			world_delta_mat = (cur_translate * cur_rotate)
				* bind_world_inv_mat;
		   */

			world_delta_mat = bind_world_inv_mat * (cur_rotate * cur_translate);

			
			XnaGeometry.Quaternion cur_quat = XnaGeometry.Quaternion.CreateFromRotationMatrix(world_delta_mat);
			if(cur_quat.Z < 0) {
				//cur_quat = -cur_quat;
			}
			
			
			world_dq = new dualQuat(cur_quat, world_delta_mat.Translation);

			for(int i = 0; i < children.Count; i++) {
				MeshBone cur_bone = children[i];
				cur_bone.computeWorldDeltaTransforms();
			}		
		}

		public void addChild(MeshBone bone_in)
		{
			bone_in.setRestParentMat(rest_world_mat, rest_world_inv_mat);
			children.Add(bone_in);
		}
		
		public List<MeshBone> getChildren() {
			return children;
		}
		
		public bool hasBone(MeshBone bone_in)
		{
			for(int i = 0; i < children.Count; i++) {
				MeshBone cur_bone = children[i];
				if(cur_bone == bone_in) {
					return true;
				}
			}
			
			return false;
		}
		
		public MeshBone getChildByKey(string search_key)
		{
			if(String.Equals(key, search_key)) {
				return this;
			}
			
			MeshBone ret_data = null;
			for(int i = 0; i < children.Count; i++) {
				MeshBone cur_bone = children[i];
				
				MeshBone result = cur_bone.getChildByKey(search_key);
				if(result != null) {
					ret_data = result;
					break;
				}
			}
			
			return ret_data;
		}
		
		public string getKey()
		{
			return key;
		}
		
		public List<string> getAllBoneKeys()
		{
			List<string> ret_data = new List<string>();
			ret_data.Add(getKey());
			
			for(int i = 0; i < children.Count; i++) {
				List<string> append_data = children[i].getAllBoneKeys();
				ret_data.AddRange( append_data);
			}
			
			return ret_data;
		}
		
		public List<MeshBone> getAllChildren()
		{
			List<MeshBone> ret_data = new List<MeshBone>();
			ret_data.Add(this);
			for(int i = 0; i < children.Count; i++) {
				List<MeshBone> append_data = children[i].getAllChildren();
				ret_data.AddRange(append_data);
			}
			
			return ret_data;
		}
		
		public int getBoneDepth(MeshBone bone_in, int depth=0)
		{
			if(bone_in == this) {
				return depth;
			}
			
			for(int i = 0; i < children.Count; i++) {
				MeshBone cur_bone = children[i];
				int ret_val = cur_bone.getBoneDepth(bone_in, depth + 1);
				if(ret_val != -1) {
					return ret_val;
				}
			}
			
			return -1;
		}
		
		public bool isLeaf() {
			return children.Count == 0;
		}
		
		public void deleteChildren()
		{
			for(int i = 0; i < children.Count; i++) {
				MeshBone cur_bone = children[i];
				cur_bone.deleteChildren();
			}
			
			children.Clear();
		}

		public void setTagId(int value_in)
		{
			tag_id = value_in;
		}
		
		public int getTagId()
		{
			return tag_id;
		}

		public Tuple<XnaGeometry.Vector4, XnaGeometry.Vector4> 
			computeDirs(XnaGeometry.Vector4 start_pt, XnaGeometry.Vector4 end_pt)
		{
			XnaGeometry.Vector4 tangent = end_pt - start_pt;
			tangent.Normalize();

			XnaGeometry.Vector4 normal = Utils.rotateVec4_90(tangent);
			
			return new Tuple<XnaGeometry.Vector4, XnaGeometry.Vector4> (tangent, normal);
		}
		
		public void computeRestLength()
		{
			XnaGeometry.Vector4 tmp_dir = local_rest_end_pt - local_rest_start_pt;
			rest_length = (float)tmp_dir.Length();
		}

	}

	// MeshRenderRegion
	public class MeshRenderRegion {
		public int start_pt_index, end_pt_index;
		public int start_index, end_index;
		public List<int> store_indices;
		public List<float> store_rest_pts,
						store_uvs;
		public List<XnaGeometry.Vector2> local_displacements;
		public bool use_local_displacements;
		public List<XnaGeometry.Vector2> post_displacements;
		public bool use_post_displacements;
		public bool use_uv_warp;
		public XnaGeometry.Vector2 uv_warp_local_offset, uv_warp_global_offset, uv_warp_scale;
		public List<XnaGeometry.Vector2> uv_warp_ref_uvs;
		public float opacity;
		public Dictionary<string, List<float> > normal_weight_map;
		public List<List<float> > fast_normal_weight_map;
		public List<MeshBone> fast_bones_map;
		public List<List<int> > relevant_bones_indices;
		public string main_bone_key;
		public MeshBone main_bone;
		public bool use_dq;
		public string name;
		public int tag_id;

		public MeshRenderRegion(List<int> indices_in,
		                 List<float> rest_pts_in,
		                 List<float> uvs_in,
		                 int start_pt_index_in,
		                 int end_pt_index_in,
		                 int start_index_in,
		                 int end_index_in)
		{
			store_indices = indices_in;
			store_rest_pts = rest_pts_in;
			store_uvs = uvs_in;
			
			use_local_displacements = false;
			use_post_displacements = false;
			use_uv_warp = false;
			uv_warp_local_offset = new XnaGeometry.Vector2(0,0);
			uv_warp_global_offset = new XnaGeometry.Vector2(0,0);
			uv_warp_scale = new XnaGeometry.Vector2(1,1);
			opacity = 100.0f;
			start_pt_index = start_pt_index_in;
			end_pt_index = end_pt_index_in;
			start_index = start_index_in;
			end_index = end_index_in;
			main_bone = null;
			local_displacements = new List<XnaGeometry.Vector2>();
			post_displacements = new List<XnaGeometry.Vector2>();
			uv_warp_ref_uvs = new List<XnaGeometry.Vector2>();
			normal_weight_map = new Dictionary<string, List<float> >();
			fast_normal_weight_map = new List<List<float> >();
			fast_bones_map = new List<MeshBone> ();
			relevant_bones_indices = new List<List<int> > ();
			use_dq = true;
			tag_id = -1;
			
			initUvWarp();
		}

		public int getIndicesIndex()
		{
			// return store_indices + (start_index);
			return start_index;
		}
		
		public int getRestPtsIndex()
		{
			// return store_rest_pts + (3 * start_pt_index);
			return 3 * start_pt_index;
		}

		public int getUVsIndex()
		{
			// return store_uvs + (2  * start_pt_index);
			return 2  * start_pt_index;
		}

		public int getNumPts()
		{
			return end_pt_index - start_pt_index + 1;
		}
		
		public int getStartPtIndex()
		{
			return start_pt_index;
		}
		
		public int getEndPtIndex()
		{
			return end_pt_index;
		}
		
		public int getNumIndices()
		{
			return end_index - start_index + 1;
		}
		
		public int getStartIndex()
		{
			return start_index;
		}
		
		public int getEndIndex()
		{
			return end_index;
		}
		
		public void poseFinalPts(ref List<float> output_pts,
		                         int output_start_index,
		                  		ref Dictionary<string, MeshBone> bones_map)
		{
			int read_pt_index = getRestPtsIndex();
			int write_pt_index = output_start_index;
			
			// point posing
			int cur_num_pts = getNumPts ();
			dualQuat accum_dq = new dualQuat();

			for(int i = 0; i < cur_num_pts; i++) {
				XnaGeometry.Vector4 cur_rest_pt =
						new XnaGeometry.Vector4(store_rest_pts[0 + read_pt_index], 
				        			            store_rest_pts[1 + read_pt_index], 
				               					store_rest_pts[2 + read_pt_index], 1);
				
				if(use_local_displacements) {
					cur_rest_pt.X += local_displacements[i].X;
					cur_rest_pt.Y += local_displacements[i].Y;
				}
				
				dualQuat.zeroOut(ref accum_dq);

				var bone_indices = relevant_bones_indices[i];
				for(int k = 0; k < bone_indices.Count; k++)
				{
					int j = bone_indices[k];
					MeshBone cur_bone = fast_bones_map[j];
					float cur_weight_val = fast_normal_weight_map[j][i];
					float cur_im_weight_val = cur_weight_val;
		
					dualQuat world_dq = cur_bone.getWorldDq();
					accum_dq.add(ref world_dq, cur_weight_val, cur_im_weight_val);
				}
				
				XnaGeometry.Vector3 final_pt = new XnaGeometry.Vector3(0,0,0);
				accum_dq.normalize();
				XnaGeometry.Vector3 tmp_pt = new XnaGeometry.Vector3(cur_rest_pt.X, cur_rest_pt.Y, cur_rest_pt.Z);
				final_pt = accum_dq.transform(ref tmp_pt);

				// debug start

				// debug end
				
				output_pts[0 + write_pt_index] = (float)final_pt.X;
				output_pts[1 + write_pt_index] = (float)final_pt.Y;
				output_pts[2 + write_pt_index] = (float)final_pt.Z;
				
				if(use_post_displacements) {
					output_pts[0 + write_pt_index] += (float)post_displacements[i].X;
					output_pts[1 + write_pt_index] += (float)post_displacements[i].Y;
				}
				
				read_pt_index += 3;
				write_pt_index += 3;
			}
			
			// uv warping
			if(use_uv_warp) {
				runUvWarp();
			}
		}
		
		public void setMainBoneKey(string key_in)
		{
			main_bone_key = key_in;
		}

		public void determineMainBone(MeshBone root_bone_in)
		{
			main_bone = root_bone_in.getChildByKey(main_bone_key);
		}
		
		public void setUseDq(bool flag_in)
		{
			use_dq = flag_in;
		}
		
		public void setName(string name_in)
		{
			name = name_in;
		}
		
		public string getName()
		{
			return name;
		}
		
		public void setUseLocalDisplacements(bool flag_in)
		{
			use_local_displacements = flag_in;
			if((local_displacements.Count != getNumPts())
			   && use_local_displacements)
			{
				local_displacements.Clear();
				for(int i = 0; i < getNumPts(); i++) {
					local_displacements.Add (new XnaGeometry.Vector2(0,0));
				}
			}
		}
		
		public bool getUseLocalDisplacements()
		{
			return use_local_displacements;
		}

		public void setUsePostDisplacements(bool flag_in)
		{
			use_post_displacements = flag_in;
			if((post_displacements.Count != getNumPts())
			   && use_post_displacements)
			{
				post_displacements.Clear();
				for(int i = 0; i < getNumPts(); i++) {
					post_displacements.Add (new XnaGeometry.Vector2(0,0));
				}
			}
		}
		
		public bool getUsePostDisplacements()
		{
			return use_post_displacements;
		}

		public XnaGeometry.Vector2 getRestLocalPt(int index_in)
		{
			int read_pt_index = getRestPtsIndex() + (3 * index_in);
			XnaGeometry.Vector2 return_pt = new XnaGeometry.Vector2(store_rest_pts[0 + read_pt_index], 
			                                                        store_rest_pts[1 + read_pt_index]);
			return return_pt;
		}
		
		public int getLocalIndex(int index_in)
		{
			int read_index = getIndicesIndex() + index_in;
			return store_indices[read_index];
		}
		
		public void clearLocalDisplacements()
		{
			for(int i = 0; i < local_displacements.Count; i++) {
				local_displacements[i] = new XnaGeometry.Vector2(0,0);
			}
		}
		
		public void clearPostDisplacements()
		{
			for(int i = 0; i < post_displacements.Count; i++) {
				post_displacements[i] = new XnaGeometry.Vector2(0,0);
			}
		}
		
		public void setUseUvWarp(bool flag_in)
		{
			use_uv_warp = flag_in;
			if(use_uv_warp == false) {
				restoreRefUv();
			}
		}
		
		public bool getUseUvWarp()
		{
			return use_uv_warp;
		}
		
		public void setUvWarpLocalOffset(XnaGeometry.Vector2 vec_in)
		{
			uv_warp_local_offset = vec_in;
		}
		
		public void setUvWarpGlobalOffset(XnaGeometry.Vector2 vec_in)
		{
			uv_warp_global_offset = vec_in;
		}
		
		public void setUvWarpScale(XnaGeometry.Vector2 vec_in)
		{
			uv_warp_scale = vec_in;
		}
		
		public XnaGeometry.Vector2 getUvWarpLocalOffset()
		{
			return uv_warp_local_offset;
		}
		
		public XnaGeometry.Vector2 getUvWarpGlobalOffset()
		{
			return uv_warp_global_offset;
		}
		
		public XnaGeometry.Vector2 getUvWarpScale()
		{
			return uv_warp_scale;
		}
		
		public void runUvWarp()
		{
			int cur_uvs_index = getUVsIndex();
			for(int i = 0; i < uv_warp_ref_uvs.Count; i++) {
				XnaGeometry.Vector2 set_uv = uv_warp_ref_uvs[i];
				set_uv -= uv_warp_local_offset;
				set_uv *= uv_warp_scale;
				set_uv += uv_warp_global_offset;
				
				store_uvs[0 + cur_uvs_index] = (float)set_uv.X;
				store_uvs[1 + cur_uvs_index] = (float)set_uv.Y;
				
				cur_uvs_index += 2;
			}
		}
		
		public void restoreRefUv()
		{
			int cur_uvs_index = getUVsIndex();
			for(int i = 0; i < uv_warp_ref_uvs.Count; i++) {
				XnaGeometry.Vector2 set_uv = uv_warp_ref_uvs[i];
				store_uvs[0 + cur_uvs_index] = (float)set_uv.X;
				store_uvs[1 + cur_uvs_index] = (float)set_uv.Y;
				
				cur_uvs_index += 2;
			}
		}
		
		public int getTagId()
		{
			return tag_id;
		}
		
		public void setTagId(int value_in)
		{
			tag_id = value_in;
		}
		
		public void initFastNormalWeightMap(ref Dictionary<string, MeshBone> bones_map)
		{
			fast_normal_weight_map.Clear ();
			fast_bones_map.Clear ();
			relevant_bones_indices.Clear ();

			foreach (var cur_iter in bones_map) {
				string cur_key = cur_iter.Key;
				List<float> values = normal_weight_map[cur_key];
				fast_normal_weight_map.Add(values);

				fast_bones_map.Add (bones_map[cur_key]);
			}

			for(int i = 0; i < getNumPts(); i++)
			{
				List<int> relevant_array = new List<int>();
				float cutoff_val = 0.05f;
				for(int j = 0; j < fast_normal_weight_map.Count; j++)
				{
					float sample_val = fast_normal_weight_map[j][i];
					if(sample_val > cutoff_val)
					{
						relevant_array.Add(j);
					}
				}

				relevant_bones_indices.Add(relevant_array);
			}
		}

		public void initUvWarp()
		{
			int cur_uvs_index = getUVsIndex();
			uv_warp_ref_uvs = new List<XnaGeometry.Vector2>(new XnaGeometry.Vector2[getNumPts()]);
			for(int i = 0; i < getNumPts(); i++) {
				uv_warp_ref_uvs[i] = new XnaGeometry.Vector2(store_uvs[cur_uvs_index],
				                                             store_uvs[cur_uvs_index + 1]);

				cur_uvs_index += 2;
			}
		}

	}

	// MeshRenderBoneComposition
	public class MeshRenderBoneComposition {
		public MeshBone root_bone;
		public Dictionary<string, MeshBone> bones_map;
		public List<MeshRenderRegion> regions;
		public Dictionary<string, MeshRenderRegion> regions_map;

		public MeshRenderBoneComposition()
		{
			root_bone = null;
			bones_map = new Dictionary<string, MeshBone>();
			regions = new List<MeshRenderRegion>();
			regions_map = new Dictionary<string, MeshRenderRegion>();
		}
		
		public void addRegion(MeshRenderRegion region_in)
		{
			regions.Add(region_in);
		}
		
		public void setRootBone(MeshBone root_bone_in)
		{
			root_bone = root_bone_in;
		}
		
		public MeshBone getRootBone()
		{
			return root_bone;
		}
		
		public void initBoneMap()
		{
			bones_map = MeshRenderBoneComposition.genBoneMap(root_bone);
		}

		public void initRegionsMap()
		{
			regions_map.Clear();
			for(int i = 0; i < regions.Count; i++) {
				string cur_key = regions[i].getName();
				regions_map.Add(cur_key, regions[i]);
			}
		}
		
		public static Dictionary<string, MeshBone> genBoneMap(MeshBone input_bone)
		{
			Dictionary<string, MeshBone> ret_map = new Dictionary<string, MeshBone>();
			List<string> all_keys = input_bone.getAllBoneKeys();
			for(int i = 0; i < all_keys.Count; i++) {
				string cur_key = all_keys[i];
				ret_map.Add(cur_key, input_bone.getChildByKey(cur_key));
			}
			
			return ret_map;
		}
		
		public Dictionary<string, MeshBone> getBonesMap()
		{
			return bones_map;
		}
		
		public Dictionary<string, MeshRenderRegion> getRegionsMap()
		{
			return regions_map;
		}
		
		public List<MeshRenderRegion> getRegions()
		{
			return regions;
		}
		
		public MeshRenderRegion getRegionWithId(int id_in)
		{
			for(int i = 0; i < regions.Count; i++) {
				MeshRenderRegion cur_region = regions[i];
				if(cur_region.getTagId() == id_in) {
					return cur_region;
				}
			}
			
			return null;
		}
		
		public void resetToWorldRestPts()
		{
			getRootBone().initWorldPts();
		}
		
		public void updateAllTransforms(bool update_parent_xf)
		{
			if(update_parent_xf) {
				getRootBone().computeParentTransforms();
			}
			
			getRootBone().computeWorldDeltaTransforms();
			getRootBone().fixDQs(getRootBone().getWorldDq());
		}
	}


	// MeshBoneCache
	public class MeshBoneCache {
		public string key;
		public XnaGeometry.Vector4 world_start_pt, world_end_pt;

		public MeshBoneCache(string key_in)
		{
			key = key_in;
		}

		public void setWorldStartPt(XnaGeometry.Vector4 pt_in) {
			world_start_pt = pt_in;
		}
		
		public void setWorldEndPt(XnaGeometry.Vector4 pt_in) {
			world_end_pt = pt_in;
		}
		
		public XnaGeometry.Vector4 getWorldStartPt() {
			return world_start_pt;
		}
		
		public XnaGeometry.Vector4 getWorldEndPt() {
			return world_end_pt;
		}
		
		public string getKey() {
			return key;
		}
		
	}

	// MeshDisplacementCache
	public class MeshDisplacementCache {
		public string key;
		public List<XnaGeometry.Vector2> local_displacements;
		public List<XnaGeometry.Vector2> post_displacements;

		public MeshDisplacementCache(string key_in)
		{
			key = key_in;
			local_displacements = new List<XnaGeometry.Vector2> ();
			post_displacements = new List<XnaGeometry.Vector2> ();
		}

		public void setLocalDisplacements(List<XnaGeometry.Vector2> displacements_in)
		{
			local_displacements = displacements_in;
		}
		
		public void setPostDisplacements(List<XnaGeometry.Vector2> displacements_in)
		{
			post_displacements = displacements_in;
		}
		
		public string getKey() {
			return key;
		}
		
		public List<XnaGeometry.Vector2> getLocalDisplacements() 
		{
			return local_displacements;
		}
		
		public List<XnaGeometry.Vector2> getPostDisplacements() 
		{
			return post_displacements;
		}	
	}

	// MeshUVWarpCache
	public class MeshUVWarpCache {
		public string key;
		public XnaGeometry.Vector2 uv_warp_local_offset, uv_warp_global_offset, uv_warp_scale;
		public bool enabled;

		public MeshUVWarpCache(string key_in)
		{
			uv_warp_global_offset = new XnaGeometry.Vector2(0, 0);
			uv_warp_local_offset = new XnaGeometry.Vector2(0, 0);
			uv_warp_scale = new XnaGeometry.Vector2(-1,-1);
			key = key_in;
			enabled = false;
		}

		public void setUvWarpLocalOffset(XnaGeometry.Vector2 vec_in)
		{
			uv_warp_local_offset = vec_in;
		}
		
		public void setUvWarpGlobalOffset(XnaGeometry.Vector2 vec_in)
		{
			uv_warp_global_offset = vec_in;
		}
		
		public void setUvWarpScale(XnaGeometry.Vector2 vec_in)
		{
			uv_warp_scale = vec_in;
		}
		
		public XnaGeometry.Vector2 getUvWarpLocalOffset()
		{
			return uv_warp_local_offset;
		}
		
		public XnaGeometry.Vector2 getUvWarpGlobalOffset()
		{
			return uv_warp_global_offset;
		}
		
		public XnaGeometry.Vector2 getUvWarpScale()
		{
			return uv_warp_scale;
		}
		
		public string getKey() {
			return key;
		}
		
		public void setEnabled(bool flag_in)
		{
			enabled = flag_in;
		}
		
		public bool getEnabled() {
			return enabled;
		}
	}

	// MeshOpacityCache
	public class MeshOpacityCache {
		public string key;
		public float opacity;

		public MeshOpacityCache(string key_in)
		{
			key = key_in;
			opacity = 100.0f;
		}
		
		public void setOpacity(float value_in)
		{
			opacity = value_in;
		}
		
		public float getOpacity()
		{
			return opacity;
		}
		
		public string getKey() {
			return key;
		}
		

	}

	// MeshBoneCacheManager
	public class MeshBoneCacheManager {
		public List<List<MeshBoneCache> > bone_cache_table;
		public List<bool> bone_cache_data_ready;
		public int start_time, end_time;
		public bool is_ready;


		public MeshBoneCacheManager()
		{
			is_ready = false;
			bone_cache_table = null;
			bone_cache_data_ready = null;
			bone_cache_table = new List<List<MeshBoneCache> > ();
			bone_cache_data_ready = new List<bool> ();
		}

		public void init(int start_time_in, int end_time_in)
		{
			start_time = start_time_in;
			end_time = end_time_in;
			
			int num_frames = end_time - start_time + 1;
			bone_cache_table.Clear();

			bone_cache_data_ready.Clear();
			for(int i = 0; i < num_frames; i++) {
				bone_cache_table.Add(new List<MeshBoneCache>());
				bone_cache_data_ready.Add(false);
			}
			
			is_ready = false;
		}
		
		public int getStartTime()
		{
			return start_time;
		}
		
		public int getEndime()
		{
			return end_time;
		}
		
		public int getIndexByTime(int time_in)
		{
			int retval = time_in - start_time;
			retval = (int)XnaGeometry.MathHelper.Clamp((double)retval,
			                                           (double)0,
			                                           (double)(bone_cache_table.Count) - 1);

			
			return retval;
		}

		public void retrieveValuesAtTime(float time_in,
		                          		 ref Dictionary<string, MeshBone> bone_map)
		{
			int base_time = getIndexByTime((int)Math.Floor((double)time_in));
			int end_time = getIndexByTime((int)Math.Ceiling((double)time_in));
			
			float ratio = (time_in - (float)Math.Floor((double)time_in));
			
			if(bone_cache_data_ready.Count == 0) {
				return;
			}
			
			if((bone_cache_data_ready[base_time] == false)
			   || (bone_cache_data_ready[end_time] == false))
			{
				return;
			}
			
			List<MeshBoneCache> base_cache = bone_cache_table[base_time];
			List<MeshBoneCache> end_cache = bone_cache_table[end_time];
			
			for(int i = 0; i < base_cache.Count; i++) {
				MeshBoneCache base_data = base_cache[i];
				MeshBoneCache end_data = end_cache[i];
				string cur_key = base_data.getKey();
				
				XnaGeometry.Vector4 final_world_start_pt = ((1.0f - ratio) * base_data.getWorldStartPt()) +
					(ratio * end_data.getWorldStartPt());
				
				XnaGeometry.Vector4 final_world_end_pt = ((1.0f - ratio) * base_data.getWorldEndPt()) +
					(ratio * end_data.getWorldEndPt());
				
				bone_map[cur_key].setWorldStartPt(final_world_start_pt);
				bone_map[cur_key].setWorldEndPt(final_world_end_pt);
			}
		}

		public bool allReady()
		{
			if(is_ready) {
				return true;
			}
			else {
				int num_frames = end_time - start_time + 1;
				int ready_cnt = 0;
				for(int i = 0; i < bone_cache_data_ready.Count; i++) {
					if(bone_cache_data_ready[i]) {
						ready_cnt++;
					}
				}
				
				if(ready_cnt == num_frames) {
					is_ready = true;
				}
			}
			
			return is_ready;
		}
		
		public void makeAllReady()
		{
			for(int i = 0; i < bone_cache_data_ready.Count; i++) {
				bone_cache_data_ready[i] = true;
			}
		}
	}

	// MeshDisplacementCacheManager
	public class MeshDisplacementCacheManager {
		public List<List<MeshDisplacementCache> > displacement_cache_table;
		public List<bool> displacement_cache_data_ready;
		public int start_time, end_time;
		public bool is_ready;

		public MeshDisplacementCacheManager()
		{
			is_ready = false;
			displacement_cache_table = null;
			displacement_cache_data_ready = null;
			displacement_cache_table = new List<List<MeshDisplacementCache> > ();
			displacement_cache_data_ready = new List<bool> ();
		}
		
		public void init(int start_time_in, int end_time_in)
		{
			start_time = start_time_in;
			end_time = end_time_in;
			
			int num_frames = end_time - start_time + 1;
			displacement_cache_table.Clear();
			
			displacement_cache_data_ready.Clear();
			for(int i = 0; i < num_frames; i++) {
				displacement_cache_table.Add(new List<MeshDisplacementCache>());
				displacement_cache_data_ready.Add(false);
			}
			
			is_ready = false;
		}
		
		public int getStartTime()
		{
			return start_time;
		}
		
		public int getEndime()
		{
			return end_time;
		}
		
		public int getIndexByTime(int time_in)
		{
			int retval = time_in - start_time;
			retval = (int)XnaGeometry.MathHelper.Clamp((double)retval,
			                                           (double)0,
			                                           (double)(displacement_cache_table.Count) - 1);
			
			
			return retval;
		}

		public void retrieveValuesAtTime(float time_in,
		                          		 Dictionary<string, MeshRenderRegion> regions_map)
		{
			int base_time = getIndexByTime((int)Math.Floor((double)time_in));
			int end_time = getIndexByTime((int)Math.Ceiling((double)time_in));
			
			float ratio = (time_in - (float)Math.Floor((double)time_in));
			
			if(displacement_cache_data_ready.Count == 0) {
				return;
			}
			
			if((displacement_cache_data_ready[base_time] == false)
			   || (displacement_cache_data_ready[end_time] == false))
			{
				return;
			}
			
			List<MeshDisplacementCache> base_cache = displacement_cache_table[base_time];
			List<MeshDisplacementCache> end_cache = displacement_cache_table[end_time];
			
			for(int i = 0; i < base_cache.Count; i++) {
				MeshDisplacementCache base_data = base_cache[i];
				MeshDisplacementCache end_data = end_cache[i];
				string cur_key = base_data.getKey();
				
				MeshRenderRegion set_region = regions_map[cur_key];
				
				if(set_region.getUseLocalDisplacements()) {
					List<XnaGeometry.Vector2> displacements =
						set_region.local_displacements;
					if((base_data.getLocalDisplacements().Count == displacements.Count)
					   && (end_data.getLocalDisplacements().Count == displacements.Count))
					{
						for(int j = 0; j < displacements.Count; j++) {
							XnaGeometry.Vector2 interp_val =
								((1.0f - ratio) * base_data.getLocalDisplacements()[j]) +
									(ratio * end_data.getLocalDisplacements()[j]);
							displacements[j] = interp_val;
						}
					}
					else {
						for(int j = 0; j < displacements.Count; j++) {
							displacements[j] = new XnaGeometry.Vector2(0,0);
						}
					}
				}

				if(set_region.getUsePostDisplacements()) {
					List<XnaGeometry.Vector2> displacements =
						set_region.post_displacements;
					if((base_data.getPostDisplacements().Count == displacements.Count)
					   && (end_data.getPostDisplacements().Count == displacements.Count))
					{
						
						for(int j = 0; j < displacements.Count; j++) {
							XnaGeometry.Vector2 interp_val =
								((1.0f - ratio) * base_data.getPostDisplacements()[j]) +
									(ratio * end_data.getPostDisplacements()[j]);
							displacements[j] = interp_val;
						}
					}
					else {
						for(int j = 0; j < displacements.Count; j++) {
							displacements[j] = new XnaGeometry.Vector2(0,0);
						}					
					}
				}
			}
		}

		public bool allReady()
		{
			if(is_ready) {
				return true;
			}
			else {
				int num_frames = end_time - start_time + 1;
				int ready_cnt = 0;
				for(int i = 0; i < displacement_cache_data_ready.Count; i++) {
					if(displacement_cache_data_ready[i]) {
						ready_cnt++;
					}
				}
				
				if(ready_cnt == num_frames) {
					is_ready = true;
				}
			}
			
			return is_ready;
		}
		
		public void makeAllReady()
		{
			for(int i = 0; i < displacement_cache_data_ready.Count; i++) {
				displacement_cache_data_ready[i] = true;
			}
		}
	}

	// MeshUVWarpCacheManager
	public class MeshUVWarpCacheManager {
		public List<List<MeshUVWarpCache> > uv_cache_table;
		public List<bool> uv_cache_data_ready;
		public int start_time, end_time;
		public bool is_ready;

		public MeshUVWarpCacheManager()
		{
			is_ready = false;
			uv_cache_table = null;
			uv_cache_data_ready = null;
			uv_cache_table = new List<List<MeshUVWarpCache> > ();
			uv_cache_data_ready = new List<bool> ();
		}
		
		public void init(int start_time_in, int end_time_in)
		{
			start_time = start_time_in;
			end_time = end_time_in;
			
			int num_frames = end_time - start_time + 1;
			uv_cache_table.Clear();
			
			uv_cache_data_ready.Clear();
			for(int i = 0; i < num_frames; i++) {
				uv_cache_table.Add(new List<MeshUVWarpCache>());
				uv_cache_data_ready.Add(false);
			}
			
			is_ready = false;
		}
		
		public int getStartTime()
		{
			return start_time;
		}
		
		public int getEndime()
		{
			return end_time;
		}
		
		public int getIndexByTime(int time_in)
		{
			int retval = time_in - start_time;
			retval = (int)XnaGeometry.MathHelper.Clamp((double)retval,
			                                           (double)0,
			                                           (double)(uv_cache_table.Count) - 1);
			
			
			return retval;
		}
		
		public void retrieveValuesAtTime(float time_in,
		                         		 Dictionary<string, MeshRenderRegion> regions_map)
		{
			int base_time = getIndexByTime((int)Math.Floor(time_in));
			int end_time = getIndexByTime((int)Math.Ceiling(time_in));
			
			if(uv_cache_data_ready.Count == 0) {
				return;
			}
			
			if((uv_cache_data_ready[base_time] == false)
			   || (uv_cache_data_ready[end_time] == false))
			{
				return;
			}
			
			List<MeshUVWarpCache> base_cache = uv_cache_table[base_time];

			for(int i = 0; i < base_cache.Count; i++) {
				MeshUVWarpCache base_data = base_cache[i];
				string cur_key = base_data.getKey();
				
				MeshRenderRegion set_region = regions_map[cur_key];
				if(set_region.getUseUvWarp()) {
					XnaGeometry.Vector2 final_local_offset = base_data.getUvWarpLocalOffset();
					
					XnaGeometry.Vector2 final_global_offset = base_data.getUvWarpGlobalOffset();
					
					XnaGeometry.Vector2 final_scale = base_data.getUvWarpScale();

					set_region.setUvWarpLocalOffset(final_local_offset);
					set_region.setUvWarpGlobalOffset(final_global_offset);
					set_region.setUvWarpScale(final_scale);
				}
			}
		}

		public bool allReady()
		{
			if(is_ready) {
				return true;
			}
			else {
				int num_frames = end_time - start_time + 1;
				int ready_cnt = 0;
				for(int i = 0; i < uv_cache_data_ready.Count; i++) {
					if(uv_cache_data_ready[i]) {
						ready_cnt++;
					}
				}
				
				if(ready_cnt == num_frames) {
					is_ready = true;
				}
			}
			
			return is_ready;
		}
		
		public void makeAllReady()
		{
			for(int i = 0; i < uv_cache_data_ready.Count; i++) {
				uv_cache_data_ready[i] = true;
			}
		}
		
	}

	// MeshOpacityCacheManager
	public class MeshOpacityCacheManager {
		public List<List<MeshOpacityCache> > opacity_cache_table;
		public List<bool> opacity_cache_data_ready;
		public int start_time, end_time;
		public bool is_ready;
		
		public MeshOpacityCacheManager()
		{
			is_ready = false;
			opacity_cache_table = null;
			opacity_cache_data_ready = null;
			opacity_cache_table = new List<List<MeshOpacityCache> > ();
			opacity_cache_data_ready = new List<bool> ();
		}
		
		public void init(int start_time_in, int end_time_in)
		{
			start_time = start_time_in;
			end_time = end_time_in;
			
			int num_frames = end_time - start_time + 1;
			opacity_cache_table.Clear();
			
			opacity_cache_data_ready.Clear();
			for(int i = 0; i < num_frames; i++) {
				opacity_cache_table.Add(new List<MeshOpacityCache>());
				opacity_cache_data_ready.Add(false);
			}
			
			is_ready = false;
		}
		
		public int getStartTime()
		{
			return start_time;
		}
		
		public int getEndime()
		{
			return end_time;
		}
		
		public int getIndexByTime(int time_in)
		{
			int retval = time_in - start_time;
			retval = (int)XnaGeometry.MathHelper.Clamp((double)retval,
			                                           (double)0,
			                                           (double)(opacity_cache_table.Count) - 1);
			
			
			return retval;
		}
		
		public void retrieveValuesAtTime(float time_in,
		                                 Dictionary<string, MeshRenderRegion> regions_map)
		{
			int base_time = getIndexByTime((int)Math.Floor(time_in));
			int end_time = getIndexByTime((int)Math.Ceiling(time_in));
			
			if(opacity_cache_data_ready.Count == 0) {
				return;
			}
			
			if((opacity_cache_data_ready[base_time] == false)
			   || (opacity_cache_data_ready[end_time] == false))
			{
				return;
			}
			
			List<MeshOpacityCache> base_cache = opacity_cache_table[base_time];
			
			for(int i = 0; i < base_cache.Count; i++) {
				MeshOpacityCache base_data = base_cache[i];
				string cur_key = base_data.getKey();
				
				MeshRenderRegion set_region = regions_map[cur_key];
				set_region.opacity = base_data.getOpacity();
			}
		}
		
		public bool allReady()
		{
			if(is_ready) {
				return true;
			}
			else {
				int num_frames = end_time - start_time + 1;
				int ready_cnt = 0;
				for(int i = 0; i < opacity_cache_data_ready.Count; i++) {
					if(opacity_cache_data_ready[i]) {
						ready_cnt++;
					}
				}
				
				if(ready_cnt == num_frames) {
					is_ready = true;
				}
			}
			
			return is_ready;
		}
		
		public void makeAllReady()
		{
			for(int i = 0; i < opacity_cache_data_ready.Count; i++) {
				opacity_cache_data_ready[i] = true;
			}
		}
		
	}
}

