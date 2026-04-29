
using UnityEngine;
using UnityEngine.InputSystem;

public class TilesGen : MonoBehaviour
{
    public string imageFilename;
    private Texture2D _mTextureOriginal;

    private Tile _mTile = null;
    private Sprite _mSprite = null;

    private void Start()
    {
        CreateBaseTexture();
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
        _mSprite = SpriteUtils.CreateSpriteFromTexture2D(_mTextureOriginal, 0, 0, _mTextureOriginal.width, _mTextureOriginal.height);
        spriteRenderer.sprite = _mSprite;
    }

    private (Tile.PosNegType, Color) GetRandomType()
    {
        var posNegType = Random.value > 0.5f ? Tile.PosNegType.Pos : Tile.PosNegType.Neg;
        var color = posNegType == Tile.PosNegType.Pos ? Color.white : Color.black;
        return (posNegType, color);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) TestRandomCurves();
        else if (Keyboard.current.fKey.wasPressedThisFrame) TestTileFloodFill();
    }

    private void TestRandomCurves()
    {
        if (_mTile != null)
        {
            _mTile.DestroyAllCurves();
            _mTile = null;
        }
        _mTile = new Tile(_mTextureOriginal);
        _mTile.HideAllCurves();

        var (posNegType, color) = GetRandomType();
        _mTile.DrawCurve(Tile.Direction.Up, posNegType, color, 1f, transform);

        (posNegType, color) = GetRandomType();
        _mTile.DrawCurve(Tile.Direction.Right, posNegType, color, 1f, transform);

        (posNegType, color) = GetRandomType();
        _mTile.DrawCurve(Tile.Direction.Down, posNegType, color, 1f, transform);

        (posNegType, color) = GetRandomType();
        _mTile.DrawCurve(Tile.Direction.Left, posNegType, color, 1f, transform);

        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = _mSprite;
    }

    private void TestTileFloodFill()
    {
        if (_mTile != null)
        {
            _mTile.DestroyAllCurves();
            _mTile = null;
        }

        _mTile = new Tile(_mTextureOriginal);

        var (posNegType, color) = GetRandomType();
        _mTile.DrawCurve(Tile.Direction.Up, posNegType, color, 1f, transform);
        _mTile.SetCurveType(Tile.Direction.Up, posNegType);

        (posNegType, color) = GetRandomType();
        _mTile.DrawCurve(Tile.Direction.Right, posNegType, color, 1f, transform);
        _mTile.SetCurveType(Tile.Direction.Right, posNegType);

        (posNegType, color) = GetRandomType();
        _mTile.DrawCurve(Tile.Direction.Down, posNegType, color, 1f, transform);
        _mTile.SetCurveType(Tile.Direction.Down, posNegType);

        (posNegType, color) = GetRandomType();
        _mTile.DrawCurve(Tile.Direction.Left, posNegType, color, 1f, transform);
        _mTile.SetCurveType(Tile.Direction.Left, posNegType);

        _mTile.Apply();

        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(_mTile.FinalCut, 0, 0, _mTile.FinalCut.width, _mTile.FinalCut.height);
    }
}