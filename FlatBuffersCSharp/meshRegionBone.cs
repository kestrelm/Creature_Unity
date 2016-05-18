// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class meshRegionBone : Table {
  public static meshRegionBone GetRootAsmeshRegionBone(ByteBuffer _bb) { return GetRootAsmeshRegionBone(_bb, new meshRegionBone()); }
  public static meshRegionBone GetRootAsmeshRegionBone(ByteBuffer _bb, meshRegionBone obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public meshRegionBone __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public float GetWeights(int j) { int o = __offset(6); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int WeightsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<meshRegionBone> CreatemeshRegionBone(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      VectorOffset weights = default(VectorOffset)) {
    builder.StartObject(2);
    meshRegionBone.AddWeights(builder, weights);
    meshRegionBone.AddName(builder, name);
    return meshRegionBone.EndmeshRegionBone(builder);
  }

  public static void StartmeshRegionBone(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddWeights(FlatBufferBuilder builder, VectorOffset weightsOffset) { builder.AddOffset(1, weightsOffset.Value, 0); }
  public static VectorOffset CreateWeightsVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartWeightsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<meshRegionBone> EndmeshRegionBone(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<meshRegionBone>(o);
  }
};


}
