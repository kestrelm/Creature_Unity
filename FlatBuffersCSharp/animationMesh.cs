// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationMesh : Table {
  public static animationMesh GetRootAsanimationMesh(ByteBuffer _bb) { return GetRootAsanimationMesh(_bb, new animationMesh()); }
  public static animationMesh GetRootAsanimationMesh(ByteBuffer _bb, animationMesh obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationMesh __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public bool UseDq { get { int o = __offset(6); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool UseLocalDisplacements { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool UsePostDisplacements { get { int o = __offset(10); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float GetLocalDisplacements(int j) { int o = __offset(12); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int LocalDisplacementsLength { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }
  public float GetPostDisplacements(int j) { int o = __offset(14); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int PostDisplacementsLength { get { int o = __offset(14); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<animationMesh> CreateanimationMesh(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      bool use_dq = false,
      bool use_local_displacements = false,
      bool use_post_displacements = false,
      VectorOffset local_displacements = default(VectorOffset),
      VectorOffset post_displacements = default(VectorOffset)) {
    builder.StartObject(6);
    animationMesh.AddPostDisplacements(builder, post_displacements);
    animationMesh.AddLocalDisplacements(builder, local_displacements);
    animationMesh.AddName(builder, name);
    animationMesh.AddUsePostDisplacements(builder, use_post_displacements);
    animationMesh.AddUseLocalDisplacements(builder, use_local_displacements);
    animationMesh.AddUseDq(builder, use_dq);
    return animationMesh.EndanimationMesh(builder);
  }

  public static void StartanimationMesh(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddUseDq(FlatBufferBuilder builder, bool useDq) { builder.AddBool(1, useDq, false); }
  public static void AddUseLocalDisplacements(FlatBufferBuilder builder, bool useLocalDisplacements) { builder.AddBool(2, useLocalDisplacements, false); }
  public static void AddUsePostDisplacements(FlatBufferBuilder builder, bool usePostDisplacements) { builder.AddBool(3, usePostDisplacements, false); }
  public static void AddLocalDisplacements(FlatBufferBuilder builder, VectorOffset localDisplacementsOffset) { builder.AddOffset(4, localDisplacementsOffset.Value, 0); }
  public static VectorOffset CreateLocalDisplacementsVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartLocalDisplacementsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPostDisplacements(FlatBufferBuilder builder, VectorOffset postDisplacementsOffset) { builder.AddOffset(5, postDisplacementsOffset.Value, 0); }
  public static VectorOffset CreatePostDisplacementsVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartPostDisplacementsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<animationMesh> EndanimationMesh(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationMesh>(o);
  }
};


}
