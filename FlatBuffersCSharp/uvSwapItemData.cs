// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class uvSwapItemData : Table {
  public static uvSwapItemData GetRootAsuvSwapItemData(ByteBuffer _bb) { return GetRootAsuvSwapItemData(_bb, new uvSwapItemData()); }
  public static uvSwapItemData GetRootAsuvSwapItemData(ByteBuffer _bb, uvSwapItemData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public uvSwapItemData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float GetLocalOffset(int j) { int o = __offset(4); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int LocalOffsetLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public float GetGlobalOffset(int j) { int o = __offset(6); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int GlobalOffsetLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }
  public float GetScale(int j) { int o = __offset(8); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int ScaleLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public int Tag { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<uvSwapItemData> CreateuvSwapItemData(FlatBufferBuilder builder,
      VectorOffset local_offset = default(VectorOffset),
      VectorOffset global_offset = default(VectorOffset),
      VectorOffset scale = default(VectorOffset),
      int tag = 0) {
    builder.StartObject(4);
    uvSwapItemData.AddTag(builder, tag);
    uvSwapItemData.AddScale(builder, scale);
    uvSwapItemData.AddGlobalOffset(builder, global_offset);
    uvSwapItemData.AddLocalOffset(builder, local_offset);
    return uvSwapItemData.EnduvSwapItemData(builder);
  }

  public static void StartuvSwapItemData(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddLocalOffset(FlatBufferBuilder builder, VectorOffset localOffsetOffset) { builder.AddOffset(0, localOffsetOffset.Value, 0); }
  public static VectorOffset CreateLocalOffsetVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartLocalOffsetVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGlobalOffset(FlatBufferBuilder builder, VectorOffset globalOffsetOffset) { builder.AddOffset(1, globalOffsetOffset.Value, 0); }
  public static VectorOffset CreateGlobalOffsetVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartGlobalOffsetVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddScale(FlatBufferBuilder builder, VectorOffset scaleOffset) { builder.AddOffset(2, scaleOffset.Value, 0); }
  public static VectorOffset CreateScaleVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartScaleVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTag(FlatBufferBuilder builder, int tag) { builder.AddInt(3, tag, 0); }
  public static Offset<uvSwapItemData> EnduvSwapItemData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<uvSwapItemData>(o);
  }
};


}
