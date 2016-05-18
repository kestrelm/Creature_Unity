// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationMeshOpacityTimeSample : Table {
  public static animationMeshOpacityTimeSample GetRootAsanimationMeshOpacityTimeSample(ByteBuffer _bb) { return GetRootAsanimationMeshOpacityTimeSample(_bb, new animationMeshOpacityTimeSample()); }
  public static animationMeshOpacityTimeSample GetRootAsanimationMeshOpacityTimeSample(ByteBuffer _bb, animationMeshOpacityTimeSample obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationMeshOpacityTimeSample __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationMeshOpacity GetMeshOpacities(int j) { return GetMeshOpacities(new animationMeshOpacity(), j); }
  public animationMeshOpacity GetMeshOpacities(animationMeshOpacity obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MeshOpacitiesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public int Time { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<animationMeshOpacityTimeSample> CreateanimationMeshOpacityTimeSample(FlatBufferBuilder builder,
      VectorOffset meshOpacities = default(VectorOffset),
      int time = 0) {
    builder.StartObject(2);
    animationMeshOpacityTimeSample.AddTime(builder, time);
    animationMeshOpacityTimeSample.AddMeshOpacities(builder, meshOpacities);
    return animationMeshOpacityTimeSample.EndanimationMeshOpacityTimeSample(builder);
  }

  public static void StartanimationMeshOpacityTimeSample(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddMeshOpacities(FlatBufferBuilder builder, VectorOffset meshOpacitiesOffset) { builder.AddOffset(0, meshOpacitiesOffset.Value, 0); }
  public static VectorOffset CreateMeshOpacitiesVector(FlatBufferBuilder builder, Offset<animationMeshOpacity>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMeshOpacitiesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTime(FlatBufferBuilder builder, int time) { builder.AddInt(1, time, 0); }
  public static Offset<animationMeshOpacityTimeSample> EndanimationMeshOpacityTimeSample(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationMeshOpacityTimeSample>(o);
  }
};


}
