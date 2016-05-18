// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationUVSwapTimeSample : Table {
  public static animationUVSwapTimeSample GetRootAsanimationUVSwapTimeSample(ByteBuffer _bb) { return GetRootAsanimationUVSwapTimeSample(_bb, new animationUVSwapTimeSample()); }
  public static animationUVSwapTimeSample GetRootAsanimationUVSwapTimeSample(ByteBuffer _bb, animationUVSwapTimeSample obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationUVSwapTimeSample __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationUVSwap GetUvSwaps(int j) { return GetUvSwaps(new animationUVSwap(), j); }
  public animationUVSwap GetUvSwaps(animationUVSwap obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int UvSwapsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public int Time { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<animationUVSwapTimeSample> CreateanimationUVSwapTimeSample(FlatBufferBuilder builder,
      VectorOffset uvSwaps = default(VectorOffset),
      int time = 0) {
    builder.StartObject(2);
    animationUVSwapTimeSample.AddTime(builder, time);
    animationUVSwapTimeSample.AddUvSwaps(builder, uvSwaps);
    return animationUVSwapTimeSample.EndanimationUVSwapTimeSample(builder);
  }

  public static void StartanimationUVSwapTimeSample(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddUvSwaps(FlatBufferBuilder builder, VectorOffset uvSwapsOffset) { builder.AddOffset(0, uvSwapsOffset.Value, 0); }
  public static VectorOffset CreateUvSwapsVector(FlatBufferBuilder builder, Offset<animationUVSwap>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUvSwapsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTime(FlatBufferBuilder builder, int time) { builder.AddInt(1, time, 0); }
  public static Offset<animationUVSwapTimeSample> EndanimationUVSwapTimeSample(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationUVSwapTimeSample>(o);
  }
};


}
