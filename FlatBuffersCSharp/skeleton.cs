// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class skeleton : Table {
  public static skeleton GetRootAsskeleton(ByteBuffer _bb) { return GetRootAsskeleton(_bb, new skeleton()); }
  public static skeleton GetRootAsskeleton(ByteBuffer _bb, skeleton obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public skeleton __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public skeletonBone GetBones(int j) { return GetBones(new skeletonBone(), j); }
  public skeletonBone GetBones(skeletonBone obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BonesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<skeleton> Createskeleton(FlatBufferBuilder builder,
      VectorOffset bones = default(VectorOffset)) {
    builder.StartObject(1);
    skeleton.AddBones(builder, bones);
    return skeleton.Endskeleton(builder);
  }

  public static void Startskeleton(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddBones(FlatBufferBuilder builder, VectorOffset bonesOffset) { builder.AddOffset(0, bonesOffset.Value, 0); }
  public static VectorOffset CreateBonesVector(FlatBufferBuilder builder, Offset<skeletonBone>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBonesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<skeleton> Endskeleton(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<skeleton>(o);
  }
};


}
