// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationMeshOpacity : Table {
  public static animationMeshOpacity GetRootAsanimationMeshOpacity(ByteBuffer _bb) { return GetRootAsanimationMeshOpacity(_bb, new animationMeshOpacity()); }
  public static animationMeshOpacity GetRootAsanimationMeshOpacity(ByteBuffer _bb, animationMeshOpacity obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationMeshOpacity __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public float Opacity { get { int o = __offset(6); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<animationMeshOpacity> CreateanimationMeshOpacity(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      float opacity = 0) {
    builder.StartObject(2);
    animationMeshOpacity.AddOpacity(builder, opacity);
    animationMeshOpacity.AddName(builder, name);
    return animationMeshOpacity.EndanimationMeshOpacity(builder);
  }

  public static void StartanimationMeshOpacity(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddOpacity(FlatBufferBuilder builder, float opacity) { builder.AddFloat(1, opacity, 0); }
  public static Offset<animationMeshOpacity> EndanimationMeshOpacity(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationMeshOpacity>(o);
  }
};


}
