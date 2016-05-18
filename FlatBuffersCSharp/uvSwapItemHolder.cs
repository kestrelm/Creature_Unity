// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class uvSwapItemHolder : Table {
  public static uvSwapItemHolder GetRootAsuvSwapItemHolder(ByteBuffer _bb) { return GetRootAsuvSwapItemHolder(_bb, new uvSwapItemHolder()); }
  public static uvSwapItemHolder GetRootAsuvSwapItemHolder(ByteBuffer _bb, uvSwapItemHolder obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public uvSwapItemHolder __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public uvSwapItemMesh GetMeshes(int j) { return GetMeshes(new uvSwapItemMesh(), j); }
  public uvSwapItemMesh GetMeshes(uvSwapItemMesh obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MeshesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<uvSwapItemHolder> CreateuvSwapItemHolder(FlatBufferBuilder builder,
      VectorOffset meshes = default(VectorOffset)) {
    builder.StartObject(1);
    uvSwapItemHolder.AddMeshes(builder, meshes);
    return uvSwapItemHolder.EnduvSwapItemHolder(builder);
  }

  public static void StartuvSwapItemHolder(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddMeshes(FlatBufferBuilder builder, VectorOffset meshesOffset) { builder.AddOffset(0, meshesOffset.Value, 0); }
  public static VectorOffset CreateMeshesVector(FlatBufferBuilder builder, Offset<uvSwapItemMesh>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMeshesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<uvSwapItemHolder> EnduvSwapItemHolder(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<uvSwapItemHolder>(o);
  }
};


}
