// automatically generated, do not modify

namespace CreatureFlatData
{

using FlatBuffers;

public sealed class animationClip : Table {
  public static animationClip GetRootAsanimationClip(ByteBuffer _bb) { return GetRootAsanimationClip(_bb, new animationClip()); }
  public static animationClip GetRootAsanimationClip(ByteBuffer _bb, animationClip obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public animationClip __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public animationBonesList Bones { get { return GetBones(new animationBonesList()); } }
  public animationBonesList GetBones(animationBonesList obj) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public animationMeshList Meshes { get { return GetMeshes(new animationMeshList()); } }
  public animationMeshList GetMeshes(animationMeshList obj) { int o = __offset(8); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public animationUVSwapList UvSwaps { get { return GetUvSwaps(new animationUVSwapList()); } }
  public animationUVSwapList GetUvSwaps(animationUVSwapList obj) { int o = __offset(10); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public animationMeshOpacityList MeshOpacities { get { return GetMeshOpacities(new animationMeshOpacityList()); } }
  public animationMeshOpacityList GetMeshOpacities(animationMeshOpacityList obj) { int o = __offset(12); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }

  public static Offset<animationClip> CreateanimationClip(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      Offset<animationBonesList> bones = default(Offset<animationBonesList>),
      Offset<animationMeshList> meshes = default(Offset<animationMeshList>),
      Offset<animationUVSwapList> uvSwaps = default(Offset<animationUVSwapList>),
      Offset<animationMeshOpacityList> meshOpacities = default(Offset<animationMeshOpacityList>)) {
    builder.StartObject(5);
    animationClip.AddMeshOpacities(builder, meshOpacities);
    animationClip.AddUvSwaps(builder, uvSwaps);
    animationClip.AddMeshes(builder, meshes);
    animationClip.AddBones(builder, bones);
    animationClip.AddName(builder, name);
    return animationClip.EndanimationClip(builder);
  }

  public static void StartanimationClip(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddBones(FlatBufferBuilder builder, Offset<animationBonesList> bonesOffset) { builder.AddOffset(1, bonesOffset.Value, 0); }
  public static void AddMeshes(FlatBufferBuilder builder, Offset<animationMeshList> meshesOffset) { builder.AddOffset(2, meshesOffset.Value, 0); }
  public static void AddUvSwaps(FlatBufferBuilder builder, Offset<animationUVSwapList> uvSwapsOffset) { builder.AddOffset(3, uvSwapsOffset.Value, 0); }
  public static void AddMeshOpacities(FlatBufferBuilder builder, Offset<animationMeshOpacityList> meshOpacitiesOffset) { builder.AddOffset(4, meshOpacitiesOffset.Value, 0); }
  public static Offset<animationClip> EndanimationClip(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<animationClip>(o);
  }
};


}
