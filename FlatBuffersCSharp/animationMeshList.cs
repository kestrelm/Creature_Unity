// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationMeshList : Table {
  public static animationMeshList GetRootAsanimationMeshList(ByteBuffer _bb) { return GetRootAsanimationMeshList(_bb, new animationMeshList()); }
  public static animationMeshList GetRootAsanimationMeshList(ByteBuffer _bb, animationMeshList obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationMeshList __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationMeshTimeSample GetTimeSamples(int j) { return GetTimeSamples(new animationMeshTimeSample(), j); }
  public animationMeshTimeSample GetTimeSamples(animationMeshTimeSample obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TimeSamplesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<animationMeshList> CreateanimationMeshList(FlatBufferBuilder builder,
      VectorOffset timeSamples = default(VectorOffset)) {
    builder.StartObject(1);
    animationMeshList.AddTimeSamples(builder, timeSamples);
    return animationMeshList.EndanimationMeshList(builder);
  }

  public static void StartanimationMeshList(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddTimeSamples(FlatBufferBuilder builder, VectorOffset timeSamplesOffset) { builder.AddOffset(0, timeSamplesOffset.Value, 0); }
  public static VectorOffset CreateTimeSamplesVector(FlatBufferBuilder builder, Offset<animationMeshTimeSample>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTimeSamplesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<animationMeshList> EndanimationMeshList(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationMeshList>(o);
  }
};


}
