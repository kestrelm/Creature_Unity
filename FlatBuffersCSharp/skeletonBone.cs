// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class skeletonBone : Table {
  public static skeletonBone GetRootAsskeletonBone(ByteBuffer _bb) { return GetRootAsskeletonBone(_bb, new skeletonBone()); }
  public static skeletonBone GetRootAsskeletonBone(ByteBuffer _bb, skeletonBone obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public skeletonBone __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Id { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float GetRestParentMat(int j) { int o = __offset(8); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int RestParentMatLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public float GetLocalRestStartPt(int j) { int o = __offset(10); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int LocalRestStartPtLength { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }
  public float GetLocalRestEndPt(int j) { int o = __offset(12); return o != 0 ? bb.GetFloat(__vector(o) + j * 4) : (float)0; }
  public int LocalRestEndPtLength { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }
  public int GetChildren(int j) { int o = __offset(14); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int ChildrenLength { get { int o = __offset(14); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<skeletonBone> CreateskeletonBone(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int id = 0,
      VectorOffset restParentMat = default(VectorOffset),
      VectorOffset localRestStartPt = default(VectorOffset),
      VectorOffset localRestEndPt = default(VectorOffset),
      VectorOffset children = default(VectorOffset)) {
    builder.StartObject(6);
    skeletonBone.AddChildren(builder, children);
    skeletonBone.AddLocalRestEndPt(builder, localRestEndPt);
    skeletonBone.AddLocalRestStartPt(builder, localRestStartPt);
    skeletonBone.AddRestParentMat(builder, restParentMat);
    skeletonBone.AddId(builder, id);
    skeletonBone.AddName(builder, name);
    return skeletonBone.EndskeletonBone(builder);
  }

  public static void StartskeletonBone(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(1, id, 0); }
  public static void AddRestParentMat(FlatBufferBuilder builder, VectorOffset restParentMatOffset) { builder.AddOffset(2, restParentMatOffset.Value, 0); }
  public static VectorOffset CreateRestParentMatVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartRestParentMatVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLocalRestStartPt(FlatBufferBuilder builder, VectorOffset localRestStartPtOffset) { builder.AddOffset(3, localRestStartPtOffset.Value, 0); }
  public static VectorOffset CreateLocalRestStartPtVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartLocalRestStartPtVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLocalRestEndPt(FlatBufferBuilder builder, VectorOffset localRestEndPtOffset) { builder.AddOffset(4, localRestEndPtOffset.Value, 0); }
  public static VectorOffset CreateLocalRestEndPtVector(FlatBufferBuilder builder, float[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddFloat(data[i]); return builder.EndVector(); }
  public static void StartLocalRestEndPtVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddChildren(FlatBufferBuilder builder, VectorOffset childrenOffset) { builder.AddOffset(5, childrenOffset.Value, 0); }
  public static VectorOffset CreateChildrenVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartChildrenVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<skeletonBone> EndskeletonBone(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<skeletonBone>(o);
  }
};


}
