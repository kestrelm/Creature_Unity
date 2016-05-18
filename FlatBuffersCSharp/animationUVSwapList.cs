// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationUVSwapList : Table {
  public static animationUVSwapList GetRootAsanimationUVSwapList(ByteBuffer _bb) { return GetRootAsanimationUVSwapList(_bb, new animationUVSwapList()); }
  public static animationUVSwapList GetRootAsanimationUVSwapList(ByteBuffer _bb, animationUVSwapList obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationUVSwapList __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationUVSwapTimeSample GetTimeSamples(int j) { return GetTimeSamples(new animationUVSwapTimeSample(), j); }
  public animationUVSwapTimeSample GetTimeSamples(animationUVSwapTimeSample obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TimeSamplesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<animationUVSwapList> CreateanimationUVSwapList(FlatBufferBuilder builder,
      VectorOffset timeSamples = default(VectorOffset)) {
    builder.StartObject(1);
    animationUVSwapList.AddTimeSamples(builder, timeSamples);
    return animationUVSwapList.EndanimationUVSwapList(builder);
  }

  public static void StartanimationUVSwapList(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddTimeSamples(FlatBufferBuilder builder, VectorOffset timeSamplesOffset) { builder.AddOffset(0, timeSamplesOffset.Value, 0); }
  public static VectorOffset CreateTimeSamplesVector(FlatBufferBuilder builder, Offset<animationUVSwapTimeSample>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTimeSamplesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<animationUVSwapList> EndanimationUVSwapList(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationUVSwapList>(o);
  }
};


}
