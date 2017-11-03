using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class CreatureSkinPacker : MonoBehaviour {
  public List<Sprite> sprites = null;

  public Material GetMaterial() {
    if (!sprites.Any()) {
      return null;
    }
    Texture2D template = sprites[0].texture;
    Texture2D tex = Instantiate(template) as Texture2D;

    // Texture2D tex = new Texture2D(template.width, template.height, template.format, false);
    int hh = template.height / 2;
    int hw = template.width / 2;

    for (int i = 0; i < sprites.Count; i++) {
      Texture2D src = sprites[i].texture;
      int y = i % 2 == 0 ? hh : 0;
      int x = i > 1 ? hw : 0;
      Graphics.CopyTexture(src, 0, 0, x, y, hw, hh, tex, 0, 0, x, y);
    }
    tex.Apply();
    File.WriteAllBytes("/Users/zaneclaes/Documents/test.png", tex.EncodeToPNG());

    Material mat = new Material(Shader.Find("Sprites/Default"));
    mat.mainTexture = tex;
    return mat;
  }
}
