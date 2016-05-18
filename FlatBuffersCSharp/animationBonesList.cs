// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationBonesList : Table {
  public static animationBonesList GetRootAsanimationBonesList(ByteBuffer _bb) { return GetRootAsanimationBonesList(_bb, new animationBonesList()); }
  public static animationBonesList GetRootAsanimationBonesList(ByteBuffer _bb, animationBonesList obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationBonesList __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationBonesTimeSample GetTimeSamples(int j) { return GetTimeSamples(new animationBonesTimeSample(), j); }
  public animationBonesTimeSample GetTimeSamples(animationBonesTimeSample obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TimeSamplesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<animationBonesList> CreateanimationBonesList(FlatBufferBuilder builder,
      VectorOffset timeSamples = default(VectorOffset)) {
    builder.StartObject(1);
    animationBonesList.AddTimeSamples(builder, timeSamples);
    return animationBonesList.EndanimationBonesList(builder);
  }

  public static void StartanimationBonesList(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddTimeSamples(FlatBufferBuilder builder, VectorOffset timeSamplesOffset) { builder.AddOffset(0, timeSamplesOffset.Value, 0); }
  public static VectorOffset CreateTimeSamplesVector(FlatBufferBuilder builder, Offset<animationBonesTimeSample>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTimeSamplesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<animationBonesList> EndanimationBonesList(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationBonesList>(o);
  }
};


}
