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
using XnaGeometry;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreatureIKPacket : MonoBehaviour
{
    public Transform ik_target;
    public bool ik_pos_angle = false;
    public String ik_bone1, ik_bone2;
    public List<MeshBone> carry_bones;
    public List<MeshBoneUtil.CTuple<XnaGeometry.Vector2, XnaGeometry.Vector2>> bones_basis;

#if UNITY_EDITOR
    [MenuItem("GameObject/Creature/CreatureIKPacket")]
    static CreatureIKPacket CreatePacket()
    {
        GameObject newObj = new GameObject();
        newObj.name = "New Creature IK Packet";
        CreatureIKPacket new_packet;
        new_packet = newObj.AddComponent<CreatureIKPacket>() as CreatureIKPacket;

        return new_packet;
    }
#endif

    XnaGeometry.Vector2 getFromBasis(
        XnaGeometry.Vector4 pt_in,
         XnaGeometry.Vector4 base_vec_u,
         XnaGeometry.Vector4 base_vec_v)
    {
        XnaGeometry.Vector2 basis = new XnaGeometry.Vector2(0, 0);
        basis.X = XnaGeometry.Vector4.Dot(pt_in, base_vec_u);
        basis.Y = XnaGeometry.Vector4.Dot(pt_in, base_vec_v);

        return basis;
    }

    public void poseCarryBones(MeshBone endeffector_bone)
    {
        int i = 0;
        foreach (var cur_bone in carry_bones)
        {
            var basis_pair = bones_basis[i];
            var tmp_vec = endeffector_bone.getWorldEndPt() - endeffector_bone.getWorldStartPt();
            var base_vec_u = new XnaGeometry.Vector2(tmp_vec.X, tmp_vec.Y);
            base_vec_u.Normalize();

            var base_vec_v = new XnaGeometry.Vector2(0, 0);
            base_vec_v.X = -base_vec_u.Y;
            base_vec_v.Y = base_vec_u.X;

            var set_startpt = new XnaGeometry.Vector4(0, 0, 0, 1);
            var set_endpt = new XnaGeometry.Vector4(0, 0, 0, 1);

            var calcVec = new XnaGeometry.Vector2(0, 0);
            calcVec = basis_pair.Item1.X * base_vec_u + basis_pair.Item1.Y * base_vec_v;

            set_startpt.X = calcVec.X;
            set_startpt.Y = calcVec.Y;
            set_startpt += endeffector_bone.getWorldStartPt();
            set_startpt.W = 1;

            calcVec = basis_pair.Item2.X * base_vec_u + basis_pair.Item2.Y * base_vec_v;
            set_endpt.X = calcVec.X;
            set_endpt.Y = calcVec.Y;
            set_endpt += endeffector_bone.getWorldStartPt();
            set_endpt.W = 1;

            cur_bone.setWorldStartPt(set_startpt);
            cur_bone.setWorldEndPt(set_endpt);

            i++;
        }
    }

    public void initCarryBones(MeshBone endeffector_bone)
    {
        if (bones_basis != null)
        {
            // Already bound
            return;
        }

        bones_basis = new List<MeshBoneUtil.CTuple<XnaGeometry.Vector2, XnaGeometry.Vector2>>();
        carry_bones = endeffector_bone.getAllChildren();
        carry_bones.RemoveAt(0); // Remove first end_effector bone, we do not want to carry that

        var base_vec_u = endeffector_bone.getWorldEndPt() - endeffector_bone.getWorldStartPt();
        base_vec_u.Normalize();

        var base_vec_v = new XnaGeometry.Vector4(0, 0, 0, 0);
        base_vec_v.X = -base_vec_u.Y;
        base_vec_v.Y = base_vec_u.X;

        foreach (var cur_bone in carry_bones)
        {
            var basis_start = getFromBasis(
                cur_bone.getWorldStartPt() - endeffector_bone.getWorldStartPt(),
                base_vec_u,
                base_vec_v);

            var basis_end = getFromBasis(
                cur_bone.getWorldEndPt() - endeffector_bone.getWorldStartPt(),
                base_vec_u,
                base_vec_v);

            var basis_pair =
                new MeshBoneUtil.CTuple<XnaGeometry.Vector2, XnaGeometry.Vector2>(basis_start, basis_end);

            bones_basis.Add(basis_pair);
        }
    }
}