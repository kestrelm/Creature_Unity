// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationBonesTimeSample : Table {
  public static animationBonesTimeSample GetRootAsanimationBonesTimeSample(ByteBuffer _bb) { return GetRootAsanimationBonesTimeSample(_bb, new animationBonesTimeSample()); }
  public static animationBonesTimeSample GetRootAsanimationBonesTimeSample(ByteBuffer _bb, animationBonesTimeSample obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationBonesTimeSample __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationBone GetBones(int j) { return GetBones(new animationBone(), j); }
  public animationBone GetBones(animationBone obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BonesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public int Time { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<animationBonesTimeSample> CreateanimationBonesTimeSample(FlatBufferBuilder builder,
      VectorOffset bones = default(VectorOffset),
      int time = 0) {
    builder.StartObject(2);
    animationBonesTimeSample.AddTime(builder, time);
    animationBonesTimeSample.AddBones(builder, bones);
    return animationBonesTimeSample.EndanimationBonesTimeSample(builder);
  }

  public static void StartanimationBonesTimeSample(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddBones(FlatBufferBuilder builder, VectorOffset bonesOffset) { builder.AddOffset(0, bonesOffset.Value, 0); }
  public static VectorOffset CreateBonesVector(FlatBufferBuilder builder, Offset<animationBone>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBonesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTime(FlatBufferBuilder builder, int time) { builder.AddInt(1, time, 0); }
  public static Offset<animationBonesTimeSample> EndanimationBonesTimeSample(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationBonesTimeSample>(o);
  }
};


}
