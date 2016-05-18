// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animation : Table {
  public static animation GetRootAsanimation(ByteBuffer _bb) { return GetRootAsanimation(_bb, new animation()); }
  public static animation GetRootAsanimation(ByteBuffer _bb, animation obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animation __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public animationClip GetClips(int j) { return GetClips(new animationClip(), j); }
  public animationClip GetClips(animationClip obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ClipsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<animation> Createanimation(FlatBufferBuilder builder,
      VectorOffset clips = default(VectorOffset)) {
    builder.StartObject(1);
    animation.AddClips(builder, clips);
    return animation.Endanimation(builder);
  }

  public static void Startanimation(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddClips(FlatBufferBuilder builder, VectorOffset clipsOffset) { builder.AddOffset(0, clipsOffset.Value, 0); }
  public static VectorOffset CreateClipsVector(FlatBufferBuilder builder, Offset<animationClip>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartClipsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<animation> Endanimation(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animation>(o);
  }
};


}
