// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class mesh : Table {
  public static mesh GetRootAsmesh(ByteBuffer _bb) { return GetRootAsmesh(_bb, new mesh()); }
  public static mesh GetRootAsmesh(ByteBuffer _bb, mesh obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public mesh __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float GetPoints(int j) { int o = __offset(4); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int PointsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public float GetUvs(int j) { int o = __offset(6); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int UvsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }
  public int GetIndices(int j) { int o = __offset(8); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int IndicesLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public meshRegion GetRegions(int j) { return GetRegions(new meshRegion(), j); }
  public meshRegion GetRegions(meshRegion obj, int j) { int o = __offset(10); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int RegionsLength { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<mesh> Createmesh(FlatBufferBuilder builder,
      VectorOffset points = default(VectorOffset),
      VectorOffset uvs = default(VectorOffset),
      VectorOffset indices = default(VectorOffset),
      VectorOffset regions = default(VectorOffset)) {
    builder.StartObject(4);
    mesh.AddRegions(builder, regions);
    mesh.AddIndices(builder, indices);
    mesh.AddUvs(builder, uvs);
    mesh.AddPoints(builder, points);
    return mesh.Endmesh(builder);
  }

  public static void Startmesh(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddPoints(FlatBufferBuilder builder, VectorOffset pointsOffset) { builder.AddOffset(0, pointsOffset.Value, 0); }
  public static VectorOffset CreatePointsVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartPointsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUvs(FlatBufferBuilder builder, VectorOffset uvsOffset) { builder.AddOffset(1, uvsOffset.Value, 0); }
  public static VectorOffset CreateUvsVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartUvsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddIndices(FlatBufferBuilder builder, VectorOffset indicesOffset) { builder.AddOffset(2, indicesOffset.Value, 0); }
  public static VectorOffset CreateIndicesVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartIndicesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddRegions(FlatBufferBuilder builder, VectorOffset regionsOffset) { builder.AddOffset(3, regionsOffset.Value, 0); }
  public static VectorOffset CreateRegionsVector(FlatBufferBuilder builder, Offset<meshRegion>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartRegionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<mesh> Endmesh(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<mesh>(o);
  }
};


}
