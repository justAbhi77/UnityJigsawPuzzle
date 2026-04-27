using System;
using UnityEngine;

public class TilesGen : MonoBehaviour
{
    public string imageFilename;
    private Texture2D _mTextureOriginal;

    private Tile _mTile = new Tile();

    private void Start()
    {
        CreateBaseTexture();
        _mTile.DrawCurve(Tile.Direction.Up, Tile.PosNegType.Pos, Color.white, 1f, transform);
        _mTile.DrawCurve(Tile.Direction.Right, Tile.PosNegType.Pos, Color.white, 1f, transform);
        _mTile.DrawCurve(Tile.Direction.Down, Tile.PosNegType.Pos, Color.white, 1f,transform);
        _mTile.DrawCurve(Tile.Direction.Left, Tile.PosNegType.Pos, Color.white, 1f,transform);
    }

    private void CreateBaseTexture()
    {
        _mTextureOriginal = SpriteUtils.LoadTexture(imageFilename);
        if(!_mTextureOriginal.isReadable)
        {
            Debug.LogError("Texture is not readable. Please set the 'Read/Write Enabled' flag in the texture import settings.");
            return;
        }

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(_mTextureOriginal, 0, 0, _mTextureOriginal.width, _mTextureOriginal.height);
    }
}