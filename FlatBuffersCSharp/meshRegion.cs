// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class meshRegion : Table {
  public static meshRegion GetRootAsmeshRegion(ByteBuffer _bb) { return GetRootAsmeshRegion(_bb, new meshRegion()); }
  public static meshRegion GetRootAsmeshRegion(ByteBuffer _bb, meshRegion obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public meshRegion __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int StartPtIndex { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EndPtIndex { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int StartIndex { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EndIndex { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Id { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public meshRegionBone GetWeights(int j) { return GetWeights(new meshRegionBone(), j); }
  public meshRegionBone GetWeights(meshRegionBone obj, int j) { int o = __offset(16); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int WeightsLength { get { int o = __offset(16); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<meshRegion> CreatemeshRegion(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int start_pt_index = 0,
      int end_pt_index = 0,
      int start_index = 0,
      int end_index = 0,
      int id = 0,
      VectorOffset weights = default(VectorOffset)) {
    builder.StartObject(7);
    meshRegion.AddWeights(builder, weights);
    meshRegion.AddId(builder, id);
    meshRegion.AddEndIndex(builder, end_index);
    meshRegion.AddStartIndex(builder, start_index);
    meshRegion.AddEndPtIndex(builder, end_pt_index);
    meshRegion.AddStartPtIndex(builder, start_pt_index);
    meshRegion.AddName(builder, name);
    return meshRegion.EndmeshRegion(builder);
  }

  public static void StartmeshRegion(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartPtIndex(FlatBufferBuilder builder, int startPtIndex) { builder.AddInt(1, startPtIndex, 0); }
  public static void AddEndPtIndex(FlatBufferBuilder builder, int endPtIndex) { builder.AddInt(2, endPtIndex, 0); }
  public static void AddStartIndex(FlatBufferBuilder builder, int startIndex) { builder.AddInt(3, startIndex, 0); }
  public static void AddEndIndex(FlatBufferBuilder builder, int endIndex) { builder.AddInt(4, endIndex, 0); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(5, id, 0); }
  public static void AddWeights(FlatBufferBuilder builder, VectorOffset weightsOffset) { builder.AddOffset(6, weightsOffset.Value, 0); }
  public static VectorOffset CreateWeightsVector(FlatBufferBuilder builder, Offset<meshRegionBone>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartWeightsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<meshRegion> EndmeshRegion(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<meshRegion>(o);
  }
};


}
