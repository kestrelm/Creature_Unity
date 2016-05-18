// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class uvSwapItemMesh : Table {
  public static uvSwapItemMesh GetRootAsuvSwapItemMesh(ByteBuffer _bb) { return GetRootAsuvSwapItemMesh(_bb, new uvSwapItemMesh()); }
  public static uvSwapItemMesh GetRootAsuvSwapItemMesh(ByteBuffer _bb, uvSwapItemMesh obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public uvSwapItemMesh __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public uvSwapItemData GetItems(int j) { return GetItems(new uvSwapItemData(), j); }
  public uvSwapItemData GetItems(uvSwapItemData obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ItemsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<uvSwapItemMesh> CreateuvSwapItemMesh(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      VectorOffset items = default(VectorOffset)) {
    builder.StartObject(2);
    uvSwapItemMesh.AddItems(builder, items);
    uvSwapItemMesh.AddName(builder, name);
    return uvSwapItemMesh.EnduvSwapItemMesh(builder);
  }

  public static void StartuvSwapItemMesh(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddItems(FlatBufferBuilder builder, VectorOffset itemsOffset) { builder.AddOffset(1, itemsOffset.Value, 0); }
  public static VectorOffset CreateItemsVector(FlatBufferBuilder builder, Offset<uvSwapItemData>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartItemsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<uvSwapItemMesh> EnduvSwapItemMesh(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<uvSwapItemMesh>(o);
  }
};


}
