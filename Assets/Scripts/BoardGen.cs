using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGen : MonoBehaviour
{
    private string _imageFilename;
    private Sprite _mBaseSpriteOpaque;
    private Sprite _mBaseSpriteTransparent;

    private GameObject _mGameObjectOpaque;
    private GameObject _mGameObjectTransparent;

    public float ghostTransparency = 0.1f;

    private int NumTileX { get; set; }
    private int NumTileY { get; set; }

    private Tile[,] _mTiles;
    private GameObject[,] _mTilesGameObjects;

    public Transform parentForTiles;

    public Menu Menu;

    private List<Rect> regions = new();
    private List<Coroutine> activeCoroutines = new();

    private Sprite LoadBaseTexture()
    {
        var tex = SpriteUtils.LoadTexture(_imageFilename);
        if (!tex.isReadable)
        {
            Debug.LogError("Texture is not readable. Please set the 'Read/Write Enabled' flag in the texture import settings.");
            return null;
        }
        if(tex.width % Tile.TileSize != 0 || tex.height % Tile.TileSize != 0)
        {
            Debug.LogError($"Texture dimensions must be multiples of {Tile.TileSize}. Current size: {tex.width}x{tex.height}");
            return null;
        }

        var newTex = new Texture2D(tex.width + Tile.Padding * 2, tex.height + Tile.Padding * 2, TextureFormat.RGBA32,
            false);

        for (var i = 0; i < tex.width; i++)
        {
            for (var j = 0; j < tex.height; j++)
            {
                var color = tex.GetPixel(i, j);
                color.a = 1.0f;
                newTex.SetPixel(i + Tile.Padding, j + Tile.Padding, color);
            }
        }
        newTex.Apply();
        return SpriteUtils.CreateSpriteFromTexture2D(newTex, 0, 0, newTex.width, newTex.height);
    }

    private void Start()
    {
        _imageFilename = GameManager.Instance.GetJigsawImageName();

        _mBaseSpriteOpaque = LoadBaseTexture();
        _mGameObjectOpaque = new GameObject($"{_imageFilename}_Opaque");
        var spriteRendererOpaque = _mGameObjectOpaque.AddComponent<SpriteRenderer>();
        spriteRendererOpaque.sprite = _mBaseSpriteOpaque;
        spriteRendererOpaque.sortingLayerName = "Opaque";

        _mBaseSpriteTransparent = CreateTransparentView(_mBaseSpriteOpaque.texture);
        _mGameObjectTransparent = new GameObject($"{_imageFilename}_Transparent");
        var spriteRendererTransparent = _mGameObjectTransparent.AddComponent<SpriteRenderer>();
        spriteRendererTransparent.sprite = _mBaseSpriteTransparent;
        spriteRendererTransparent.sortingLayerName = "Transparent";

        _mGameObjectOpaque.SetActive(false);

        SetCameraPos();

        StartCoroutine(nameof(Coroutine_CreateJigsawTiles));
    }

    private Sprite CreateTransparentView(Texture2D tex)
    {
        var newTex = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);

        for (var x = 0; x < newTex.width; x++)
        {
            for (var y = 0; y < newTex.height; y++)
            {
                var c = tex.GetPixel(x, y);
                if (x > Tile.Padding && x < (newTex.width - Tile.Padding) && y > Tile.Padding && y < (newTex.height - Tile.Padding))
                {
                    c.a = ghostTransparency;
                }

                newTex.SetPixel(x, y, c);
            }
        }

        newTex.Apply();

        var sprite = SpriteUtils.CreateSpriteFromTexture2D(newTex, 0, 0, newTex.width, newTex.height);
        return sprite;
    }

    private void SetCameraPos()
    {
        if (Camera.main == null) return;
        if (_mBaseSpriteOpaque.texture == null) return;
        Camera.main.transform.position = new Vector3(_mBaseSpriteOpaque.texture.width / 2.0f, _mBaseSpriteOpaque.texture.height / 2.0f, -10.0f);
        //Camera.main.orthographicSize = _mBaseSpriteOpaque.texture.width / 2.0f;

        var smallerValue = Mathf.Min(_mBaseSpriteOpaque.texture.width, _mBaseSpriteOpaque.texture.height);
        Camera.main.orthographicSize = smallerValue * 0.8f;
    }

    private static GameObject CreateGameObjectFromTile(Tile tile)
    {
        var obj = new GameObject
        {
            name = "TileGameObe_" + tile.XIndex + "_" + tile.YIndex,
            transform =
            {
                position = new Vector3(tile.XIndex * Tile.TileSize, tile.YIndex * Tile.TileSize, 0.0f)
            }
        };

        var spriteRenderer = obj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(tile.FinalCut, 0, 0, Tile.Padding * 2 + Tile.TileSize, Tile.Padding * 2 + Tile.TileSize);

        obj.AddComponent<BoxCollider2D>();
        var tileMovement = obj.AddComponent<TileMovement>();
        tileMovement.Tile = tile;

        return obj;
    }

    // ReSharper disable once UnusedMember.Local
    private void CreateJigsawTiles()
    {
        var baseTexture = _mBaseSpriteOpaque.texture;
        NumTileX = baseTexture.width / Tile.TileSize;
        NumTileY = baseTexture.height / Tile.TileSize;

        _mTiles = new Tile[NumTileX, NumTileY];
        _mTilesGameObjects = new GameObject[NumTileX, NumTileY];

        for (var i = 0; i < NumTileX; i++)
        {
            for (var j = 0; j < NumTileY; j++)
            {
                _mTiles[i, j] = CreateTile(i, j, baseTexture);
                _mTilesGameObjects[i, j] = CreateGameObjectFromTile(_mTiles[i, j]);
                if (parentForTiles != null)
                {
                    _mTilesGameObjects[i, j].transform.SetParent(parentForTiles);
                }
            }
        }
    }

    private IEnumerator Coroutine_CreateJigsawTiles()
    {
        var baseTexture = _mBaseSpriteOpaque.texture;
        NumTileX = baseTexture.width / Tile.TileSize;
        NumTileY = baseTexture.height / Tile.TileSize;

        _mTiles = new Tile[NumTileX, NumTileY];
        _mTilesGameObjects = new GameObject[NumTileX, NumTileY];

        for (var i = 0; i < NumTileX; i++)
        {
            for (var j = 0; j < NumTileY; j++)
            {
                _mTiles[i, j] = CreateTile(i, j, baseTexture);
                _mTilesGameObjects[i, j] = CreateGameObjectFromTile(_mTiles[i, j]);
                if (parentForTiles != null)
                {
                    _mTilesGameObjects[i, j].transform.SetParent(parentForTiles);
                }

                yield return null;
            }
        }

        Menu.SetEnableBottomPanel(true);
        Menu.OnClick = ShuffleTiles;
    }

    private Tile CreateTile(int i, int j, Texture2D baseTexture)
    {
        var tile = new Tile(baseTexture)
        {
            XIndex = i,
            YIndex = j
        };

        // Left side tiles.
        if (i == 0)
        {
            tile.SetCurveType(Tile.Direction.Left, Tile.PosNegType.None);
        }
        else
        {
            // We have to create a tile that has Left direction opposite curve type.
            var leftTile = _mTiles[i - 1, j];
            var rightOp = leftTile.GetCurveType(Tile.Direction.Right);
            tile.SetCurveType(Tile.Direction.Left,
                rightOp == Tile.PosNegType.Neg ? Tile.PosNegType.Pos : Tile.PosNegType.Neg);
        }

        // Bottom side tiles
        if (j == 0)
        {
            tile.SetCurveType(Tile.Direction.Down, Tile.PosNegType.None);
        }
        else
        {
            var downTile = _mTiles[i, j - 1];
            var upOp = downTile.GetCurveType(Tile.Direction.Up);
            tile.SetCurveType(Tile.Direction.Down,
                upOp == Tile.PosNegType.Neg ? Tile.PosNegType.Pos : Tile.PosNegType.Neg);
        }

        // Right side tiles.
        if (i == NumTileX - 1)
        {
            tile.SetCurveType(Tile.Direction.Right, Tile.PosNegType.None);
        }
        else
        {
            var toss = Random.Range(0f, 1f);
            tile.SetCurveType(Tile.Direction.Right, toss < 0.5f ? Tile.PosNegType.Pos : Tile.PosNegType.Neg);
        }

        // Up side tile.
        if (j == NumTileY - 1)
        {
            tile.SetCurveType(Tile.Direction.Up, Tile.PosNegType.None);
        }
        else
        {
            var toss = Random.Range(0f, 1f);
            tile.SetCurveType(Tile.Direction.Up, toss < 0.5f ? Tile.PosNegType.Pos : Tile.PosNegType.Neg);
        }

        tile.Apply();
        return tile;
    }

    private IEnumerator CoroutineMoveOverSeconds(GameObject go, Vector3 end, float seconds)
    {
        var elapsedTime = 0f;
        var start = go.transform.position;
        while (elapsedTime < seconds)
        {
            go.transform.position = Vector3.Lerp(start, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        go.transform.position = end;
    }

    private void Shuffle(GameObject go)
    {
        if (regions.Count == 0)
        {
            regions.Add(new Rect(-300.0f, -100.0f, 50.0f, NumTileY * Tile.TileSize));
            regions.Add(new Rect((NumTileX + 1) * Tile.TileSize, -100.0f, 50.0f, NumTileY * Tile.TileSize));
        }

        var regionIndex = Random.Range(0, regions.Count);
        var x = Random.Range(regions[regionIndex].xMin, regions[regionIndex].xMax);
        var y = Random.Range(regions[regionIndex].yMin, regions[regionIndex].yMax);

        var pos = new Vector3(x, y, 0.0f);
        var moveCoroutine = StartCoroutine(CoroutineMoveOverSeconds(go, pos, 1.0f));
        activeCoroutines.Add(moveCoroutine);
    }

    IEnumerator CoroutineShuffle()
    {
        for (var i = 0; i < NumTileX; ++i)
        {
            for (var j = 0; j < NumTileY; ++j)
            {
                Shuffle(_mTilesGameObjects[i, j]);
                yield return null;
            }
        }

        foreach (var item in activeCoroutines)
        {
            if (item != null)
            {
                yield return null;
            }
        }

        OnFinishedShuffling();
    }

    private void OnFinishedShuffling()
    {
        activeCoroutines.Clear();

        Menu.SetEnableBottomPanel(false);
        StartCoroutine(Coroutine_CallAfterDelay(() => Menu.SetEnableTopPanel(true), 1.0f));
        GameManager.Instance.tileMovementEnabled = true;

        StartTimer();

        for (var i = 0; i < NumTileX; ++i)
        {
            for (var j = 0; j < NumTileY; ++j)
            {
                var tm = _mTilesGameObjects[i, j].GetComponent<TileMovement>();
                tm.OnTileInPlace += OnTileInPlace;
                var spriteRenderer = tm.gameObject.GetComponent<SpriteRenderer>();
                Tile.TileSorting.BringToTop(spriteRenderer);
            }
        }

        Menu.SetTotalTiles(NumTileX * NumTileY);
    }

    IEnumerator Coroutine_CallAfterDelay(System.Action function, float delay)
    {
        yield return new WaitForSeconds(delay);
        function();
    }

    public void StartTimer()
    {
        StartCoroutine(Coroutine_Timer());
    }

    IEnumerator Coroutine_Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            GameManager.Instance.secondsSinceStart += 1;

            Menu.SetTimeInSeconds(GameManager.Instance.secondsSinceStart);
        }
    }

    public void ShuffleTiles()
    {
        StartCoroutine(CoroutineShuffle());
    }

    public void ShowOpaqueImage()
    {
        _mGameObjectOpaque.SetActive(true);
    }

    public void HideOpaqueImage()
    {
        _mGameObjectOpaque.SetActive(false);
    }

    private void OnTileInPlace(TileMovement tm)
    {
        GameManager.Instance.totalTilesInCorrectPosition += 1;

        tm.enabled = false;
        Destroy(tm);

        var spriteRenderer = tm.gameObject.GetComponent<SpriteRenderer>();
        Tile.TileSorting.Remove(spriteRenderer);
        spriteRenderer.sortingLayerName = "Opaque";

        if (GameManager.Instance.totalTilesInCorrectPosition == _mTilesGameObjects.Length)
        {
            //Debug.Log("Game completed. We will implement an end screen later");
            Menu.SetEnableTopPanel(false);
            Menu.SetEnableGameCompletionPanel(true);

            // Reset the values.
            GameManager.Instance.secondsSinceStart = 0;
            GameManager.Instance.totalTilesInCorrectPosition = 0;
        }

        Menu.SetTilesInPlace(GameManager.Instance.totalTilesInCorrectPosition);
    }
}