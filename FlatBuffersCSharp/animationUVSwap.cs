// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationUVSwap : Table {
  public static animationUVSwap GetRootAsanimationUVSwap(ByteBuffer _bb) { return GetRootAsanimationUVSwap(_bb, new animationUVSwap()); }
  public static animationUVSwap GetRootAsanimationUVSwap(ByteBuffer _bb, animationUVSwap obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationUVSwap __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public float GetLocalOffset(int j) { int o = __offset(6); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int LocalOffsetLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }
  public float GetGlobalOffset(int j) { int o = __offset(8); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int GlobalOffsetLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public float GetScale(int j) { int o = __offset(10); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int ScaleLength { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }
  public bool Enabled { get { int o = __offset(12); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }

  public static Offset<animationUVSwap> CreateanimationUVSwap(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      VectorOffset local_offset = default(VectorOffset),
      VectorOffset global_offset = default(VectorOffset),
      VectorOffset scale = default(VectorOffset),
      bool enabled = false) {
    builder.StartObject(5);
    animationUVSwap.AddScale(builder, scale);
    animationUVSwap.AddGlobalOffset(builder, global_offset);
    animationUVSwap.AddLocalOffset(builder, local_offset);
    animationUVSwap.AddName(builder, name);
    animationUVSwap.AddEnabled(builder, enabled);
    return animationUVSwap.EndanimationUVSwap(builder);
  }

  public static void StartanimationUVSwap(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddLocalOffset(FlatBufferBuilder builder, VectorOffset localOffsetOffset) { builder.AddOffset(1, localOffsetOffset.Value, 0); }
  public static VectorOffset CreateLocalOffsetVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartLocalOffsetVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGlobalOffset(FlatBufferBuilder builder, VectorOffset globalOffsetOffset) { builder.AddOffset(2, globalOffsetOffset.Value, 0); }
  public static VectorOffset CreateGlobalOffsetVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartGlobalOffsetVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddScale(FlatBufferBuilder builder, VectorOffset scaleOffset) { builder.AddOffset(3, scaleOffset.Value, 0); }
  public static VectorOffset CreateScaleVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartScaleVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEnabled(FlatBufferBuilder builder, bool enabled) { builder.AddBool(4, enabled, false); }
  public static Offset<animationUVSwap> EndanimationUVSwap(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationUVSwap>(o);
  }
};


}
