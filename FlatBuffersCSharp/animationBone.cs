// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationBone : Table {
  public static animationBone GetRootAsanimationBone(ByteBuffer _bb) { return GetRootAsanimationBone(_bb, new animationBone()); }
  public static animationBone GetRootAsanimationBone(ByteBuffer _bb, animationBone obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationBone __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public float GetStartPt(int j) { int o = __offset(6); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int StartPtLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }
  public float GetEndPt(int j) { int o = __offset(8); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int EndPtLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<animationBone> CreateanimationBone(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      VectorOffset start_pt = default(VectorOffset),
      VectorOffset end_pt = default(VectorOffset)) {
    builder.StartObject(3);
    animationBone.AddEndPt(builder, end_pt);
    animationBone.AddStartPt(builder, start_pt);
    animationBone.AddName(builder, name);
    return animationBone.EndanimationBone(builder);
  }

  public static void StartanimationBone(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartPt(FlatBufferBuilder builder, VectorOffset startPtOffset) { builder.AddOffset(1, startPtOffset.Value, 0); }
  public static VectorOffset CreateStartPtVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartStartPtVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEndPt(FlatBufferBuilder builder, VectorOffset endPtOffset) { builder.AddOffset(2, endPtOffset.Value, 0); }
  public static VectorOffset CreateEndPtVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartEndPtVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<animationBone> EndanimationBone(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationBone>(o);
  }
};


}
