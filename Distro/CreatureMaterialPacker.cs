using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

// A mapping from slot name => skin name
using CreatureMaterialAttachmentSet = System.Collections.Generic.Dictionary<string, string>;

public class CreatureMaterialPacker : MonoBehaviour {
  public Material baseMaterial = null;

  /// <summary>
  /// PNG image assets to extract skin regions from.
  /// </summary>
  public List<Sprite> attachmentSprites = null;

  /// <summary>
  /// JSON definition of where different parts of the skin regions reside.
  /// </summary>
  public TextAsset attachmentRegionsJSON = null;

  // Cache of the last texture and its attachments for incremental rendering.
  Texture2D _texture = null;
  CreatureMaterialAttachmentSet _attachments = null;

  Dictionary<string, Rect> _attachmentRegions;

  /// <summary>
  /// The parsed skin regions, name => {x,y,width,height}
  /// </summary>
  /// <value>The skin regions.</value>
  protected Dictionary<string, Rect> attachmentRegions {
    get {
      if (_attachmentRegions == null && attachmentRegionsJSON != null) {
        Dictionary<string, object> dict = null;
        _attachmentRegions = new Dictionary<string, Rect>();
        dict = JsonFx.Json.JsonReader.Deserialize(
          attachmentRegionsJSON.text, typeof(Dictionary<string, object>)
        ) as Dictionary<string, object>;
        foreach (string key in dict.Keys.ToList()) {
          Dictionary<string, object> packed = (Dictionary<string, object>)dict[key];
          Rect r = new Rect();
          if (packed.ContainsKey("x")) r.x = float.Parse(packed["x"].ToString());
          if (packed.ContainsKey("y")) r.y = float.Parse(packed["y"].ToString());
          if (packed.ContainsKey("width")) r.width = float.Parse(packed["width"].ToString());
          if (packed.ContainsKey("height")) r.height = float.Parse(packed["height"].ToString());
          if (r.width < 1 || r.height < 1) {
            Debug.LogError("The attachmentRegion " + key + " was less than 1x1 in size.");
          } else {
            _attachmentRegions[key] = r;
          }
        }
      }
      return _attachmentRegions;
    }
  }

  /// <summary>
  /// Generates a material dynamically based upon the requested SkinSet.
  /// </summary>
  /// <returns>The material for use with a renderer</returns>
  public Material GetMaterial(CreatureMaterialAttachmentSet equipmentSkinSet = null) {
    Texture2D tex = BuildTexture(equipmentSkinSet);
    if (tex == null) {
      return baseMaterial;
    }
    Material mat = new Material(baseMaterial.shader);
    mat.mainTexture = tex;
    return mat;
  }

  /// <summary>
  /// Incrementally build the Texture2D based upon the requested attachments.
  /// </summary>
  /// <returns>The rendered texture for use in a Material.</returns>
  /// <param name="attachmentSet">The attached items.</param>
  protected Texture2D BuildTexture(CreatureMaterialAttachmentSet attachments) {
    if (attachmentSprites == null || !attachmentSprites.Any() || attachments == null) {
      return _texture;
    }
    if (baseMaterial == null || baseMaterial.name != attachmentSprites.First().name) {
      Debug.LogError("The baseMaterial should match the first attachmentSprite.");
      return _texture;
    }
    if (!Application.isPlaying) {
      // In edit mode, always fully rebuild everything so its easy to work with asset changes.
      _attachmentRegions = null;
      _texture = null;
      _attachments = null;
    }
    if (_texture == null) {
      _texture = Instantiate(attachmentSprites.First().texture) as Texture2D;
    }
    if (_attachments == null) {
      _attachments = new CreatureMaterialAttachmentSet() { };
    }
    foreach (string slotName in attachmentRegions.Keys) {
      Rect r = attachmentRegions[slotName];
      bool inNewSet = attachments.ContainsKey(slotName);
      bool inOldSet = _attachments.ContainsKey(slotName);
      bool changed = inNewSet != inOldSet || (inNewSet && inOldSet && attachments[slotName] != _attachments[slotName]);
      if (!changed) {
        continue;
      }
      if (inNewSet) {
        // Copy from the sprite into the output texture.
        Sprite sprite = FindSprite(attachments[slotName]);
        if (sprite == null) {
          Debug.LogError("No sprite found for " + attachments[slotName]);
          continue;
        }
        Graphics.CopyTexture(
          sprite.texture, 0, 0, (int)r.x, (int)r.y, (int)r.width, (int)r.height,
          _texture, 0, 0, (int)r.x, (int)r.y
        );
      } else {
        // TODO: If it was in the old set but not in the new, do we need to delete it from the texture?
      }
    }
    _texture.Apply();
    _attachments = attachments;
    // File.WriteAllBytes("/Users/zaneclaes/Documents/test.png", _texture.EncodeToPNG());
    return _texture;
  }

  protected Sprite FindSprite(string spriteName) {
    return attachmentSprites.FirstOrDefault(s => s.name == spriteName);
  }
}
