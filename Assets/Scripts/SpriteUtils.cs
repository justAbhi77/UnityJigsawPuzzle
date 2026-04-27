using UnityEngine;
using System.IO;

public class SpriteUtils
{
    public static Sprite CreateSpriteFromTexture2D(Texture2D spriteTexture, int x, int y, int w, int h, float pixelsPerUnit = 1.0f, SpriteMeshType spriteMeshType = SpriteMeshType.Tight)
    {
        return Sprite.Create(spriteTexture, new Rect(x, y, w, h), new Vector2(0f, 0f), pixelsPerUnit, 0, spriteMeshType);
    }

    public static Texture2D LoadTexture(string resourcePath)
    {
        return Resources.Load<Texture2D>(resourcePath);
    }
}