// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class anchorPointsHolder : Table {
  public static anchorPointsHolder GetRootAsanchorPointsHolder(ByteBuffer _bb) { return GetRootAsanchorPointsHolder(_bb, new anchorPointsHolder()); }
  public static anchorPointsHolder GetRootAsanchorPointsHolder(ByteBuffer _bb, anchorPointsHolder obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public anchorPointsHolder __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public anchorPointData GetAnchorPoints(int j) { return GetAnchorPoints(new anchorPointData(), j); }
  public anchorPointData GetAnchorPoints(anchorPointData obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AnchorPointsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<anchorPointsHolder> CreateanchorPointsHolder(FlatBufferBuilder builder,
      VectorOffset anchorPoints = default(VectorOffset)) {
    builder.StartObject(1);
    anchorPointsHolder.AddAnchorPoints(builder, anchorPoints);
    return anchorPointsHolder.EndanchorPointsHolder(builder);
  }

  public static void StartanchorPointsHolder(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddAnchorPoints(FlatBufferBuilder builder, VectorOffset anchorPointsOffset) { builder.AddOffset(0, anchorPointsOffset.Value, 0); }
  public static VectorOffset CreateAnchorPointsVector(FlatBufferBuilder builder, Offset<anchorPointData>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAnchorPointsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<anchorPointsHolder> EndanchorPointsHolder(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<anchorPointsHolder>(o);
  }
};


}
