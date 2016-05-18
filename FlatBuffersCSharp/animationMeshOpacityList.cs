// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationMeshOpacityList : Table {
  public static animationMeshOpacityList GetRootAsanimationMeshOpacityList(ByteBuffer _bb) { return GetRootAsanimationMeshOpacityList(_bb, new animationMeshOpacityList()); }
  public static animationMeshOpacityList GetRootAsanimationMeshOpacityList(ByteBuffer _bb, animationMeshOpacityList obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationMeshOpacityList __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationMeshOpacityTimeSample GetTimeSamples(int j) { return GetTimeSamples(new animationMeshOpacityTimeSample(), j); }
  public animationMeshOpacityTimeSample GetTimeSamples(animationMeshOpacityTimeSample obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TimeSamplesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<animationMeshOpacityList> CreateanimationMeshOpacityList(FlatBufferBuilder builder,
      VectorOffset timeSamples = default(VectorOffset)) {
    builder.StartObject(1);
    animationMeshOpacityList.AddTimeSamples(builder, timeSamples);
    return animationMeshOpacityList.EndanimationMeshOpacityList(builder);
  }

  public static void StartanimationMeshOpacityList(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddTimeSamples(FlatBufferBuilder builder, VectorOffset timeSamplesOffset) { builder.AddOffset(0, timeSamplesOffset.Value, 0); }
  public static VectorOffset CreateTimeSamplesVector(FlatBufferBuilder builder, Offset<animationMeshOpacityTimeSample>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTimeSamplesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<animationMeshOpacityList> EndanimationMeshOpacityList(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationMeshOpacityList>(o);
  }
};


}
