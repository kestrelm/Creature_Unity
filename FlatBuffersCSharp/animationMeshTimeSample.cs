// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationMeshTimeSample : Table {
  public static animationMeshTimeSample GetRootAsanimationMeshTimeSample(ByteBuffer _bb) { return GetRootAsanimationMeshTimeSample(_bb, new animationMeshTimeSample()); }
  public static animationMeshTimeSample GetRootAsanimationMeshTimeSample(ByteBuffer _bb, animationMeshTimeSample obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationMeshTimeSample __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationMesh GetMeshes(int j) { return GetMeshes(new animationMesh(), j); }
  public animationMesh GetMeshes(animationMesh obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MeshesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public int Time { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<animationMeshTimeSample> CreateanimationMeshTimeSample(FlatBufferBuilder builder,
      VectorOffset meshes = default(VectorOffset),
      int time = 0) {
    builder.StartObject(2);
    animationMeshTimeSample.AddTime(builder, time);
    animationMeshTimeSample.AddMeshes(builder, meshes);
    return animationMeshTimeSample.EndanimationMeshTimeSample(builder);
  }

  public static void StartanimationMeshTimeSample(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddMeshes(FlatBufferBuilder builder, VectorOffset meshesOffset) { builder.AddOffset(0, meshesOffset.Value, 0); }
  public static VectorOffset CreateMeshesVector(FlatBufferBuilder builder, Offset<animationMesh>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMeshesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTime(FlatBufferBuilder builder, int time) { builder.AddInt(1, time, 0); }
  public static Offset<animationMeshTimeSample> EndanimationMeshTimeSample(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationMeshTimeSample>(o);
  }
};


}
