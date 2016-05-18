// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class anchorPointData : Table {
  public static anchorPointData GetRootAsanchorPointData(ByteBuffer _bb) { return GetRootAsanchorPointData(_bb, new anchorPointData()); }
  public static anchorPointData GetRootAsanchorPointData(ByteBuffer _bb, anchorPointData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public anchorPointData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float GetPoint(int j) { int o = __offset(4); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int PointLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public string AnimClipName { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }

  public static Offset<anchorPointData> CreateanchorPointData(FlatBufferBuilder builder,
      VectorOffset point = default(VectorOffset),
      StringOffset anim_clip_name = default(StringOffset)) {
    builder.StartObject(2);
    anchorPointData.AddAnimClipName(builder, anim_clip_name);
    anchorPointData.AddPoint(builder, point);
    return anchorPointData.EndanchorPointData(builder);
  }

  public static void StartanchorPointData(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddPoint(FlatBufferBuilder builder, VectorOffset pointOffset) { builder.AddOffset(0, pointOffset.Value, 0); }
  public static VectorOffset CreatePointVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartPointVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAnimClipName(FlatBufferBuilder builder, StringOffset animClipNameOffset) { builder.AddOffset(1, animClipNameOffset.Value, 0); }
  public static Offset<anchorPointData> EndanchorPointData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<anchorPointData>(o);
  }
};


}
