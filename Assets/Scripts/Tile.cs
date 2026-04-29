using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Tile
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum PosNegType
    {
        Pos,
        Neg,
        None
    }

    public static int Padding = 20;

    public static int TileSize = 100;

    private readonly Dictionary<(Direction, PosNegType), LineRenderer> _mLineRenderers = new();

    public static List<Vector2> BezCurve = BezierCurve.PointList2(BezierVizTemplate.TemplateControlPoints, 0.001f);

    private readonly Texture2D _mOriginalTexture;

    public Texture2D FinalCut { get; private set; }

    public static readonly Color TransparentColor = new Color(0, 0, 0, 0);

    private readonly PosNegType[] _mCurveTypes = {
        PosNegType.None,
        PosNegType.None,
        PosNegType.None,
        PosNegType.None
    };

    private bool[,] _mVisited;

    private readonly Stack<Vector2Int> _mStack = new();

    public int XIndex = 0;
    public int YIndex = 0;

    public void SetCurveType(Direction dir, PosNegType type)
    {
        _mCurveTypes[(int)dir] = type;
    }

    public PosNegType GetCurveType(Direction dir)
    {
        return _mCurveTypes[(int)dir];
    }

    public Tile(Texture2D originalTexture)
    {
        _mOriginalTexture = originalTexture;
        var tileSizeWithPadding = 2 * Padding + TileSize;

        FinalCut = new Texture2D(tileSizeWithPadding, tileSizeWithPadding, TextureFormat.ARGB32, false);

        for (var i = 0; i < tileSizeWithPadding; i++)
        {
            for (var j = 0; j < tileSizeWithPadding; j++)
            {
                FinalCut.SetPixel(i, j, TransparentColor);
            }
        }
    }

    public void Apply()
    {
        FloodFillInit();
        FloodFill();
        FinalCut.Apply();
    }

    private void FloodFillInit()
    {
        var tileSizeWithPadding = 2 * Padding + TileSize;

        _mVisited = new bool[tileSizeWithPadding, tileSizeWithPadding];
        for (var i = 0; i < tileSizeWithPadding; i++)
        {
            for (var j = 0; j < tileSizeWithPadding; j++)
            {
                _mVisited[i, j] = false;
            }
        }

        var pts = new List<Vector2>();
        for(var i = 0; i< _mCurveTypes.Length; i++)
        {
            pts.AddRange(CreateCurve((Direction)i, _mCurveTypes[i]));
        }

        for (var i = 0; i < pts.Count; i++)
        {
            _mVisited[(int)pts[i].x, (int)pts[i].y] = true;
        }

        var start = new Vector2Int(tileSizeWithPadding / 2, tileSizeWithPadding / 2);
        _mStack.Push(start);
    }

    private void Fill(int x, int y)
    {
        var c = _mOriginalTexture.GetPixel(x + XIndex * TileSize, y+YIndex*TileSize);
        c.a = 1.0f;
        FinalCut.SetPixel(x, y, c);
    }

    private void FloodFill()
    {
        var widthHeight = 2 * Padding + TileSize;

        while (_mStack.Count > 0)
        {
            var v = _mStack.Pop();
            var xx = v.x;
            var yy = v.y;

            Fill(v.x, v.y);

            var x = xx + 1;
            var y = yy;

            if (x < widthHeight)
            {
                var c = FinalCut.GetPixel(x, y);
                if (!_mVisited[x, y])
                {
                    _mVisited[x, y] = true;
                    _mStack.Push(new Vector2Int(x, y));
                }
            }

            x = xx - 1;
            y = yy;
            if (x > 0)
            {
                var c = FinalCut.GetPixel(x, y);
                if (!_mVisited[x, y])
                {
                    _mVisited[x, y] = true;
                    _mStack.Push(new Vector2Int(x, y));
                }
            }

            x = xx;
            y = yy + 1;
            if (y < widthHeight)
            {
                var c = FinalCut.GetPixel(x, y);
                if (!_mVisited[x, y])
                {
                    _mVisited[x, y] = true;
                    _mStack.Push(new Vector2Int(x, y));
                }
            }

            x = xx;
            y = yy - 1;
            if (y >= 0)
            {
                var c = FinalCut.GetPixel(x, y);
                if (!_mVisited[x, y])
                {
                    _mVisited[x, y] = true;
                    _mStack.Push(new Vector2Int(x, y));
                }
            }
        }
    }

    public void DestroyAllCurves()
    {
        foreach (var item in _mLineRenderers)
        {
            Object.Destroy(item.Value.gameObject);
        }
        _mLineRenderers.Clear();
    }

    public static LineRenderer CreateLineRenderer(Color color, float width, string name, Transform parent)
    {
        var lineObj = new GameObject(name)
        {
            transform =
            {
                parent = parent
            }
        };

        var lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = 0;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        return lineRenderer;
    }

    public static void TranslatePoints(List<Vector2> points, Vector2 offset)
    {
        for (var i = 0; i < points.Count; i++)
            points[i] += offset;
    }

    public static void InvertY(List<Vector2> points)
    {
        for (var i = 0; i < points.Count; i++)
            points[i] = new Vector2(points[i].x, -points[i].y);
    }

    public static void SwapXY(List<Vector2> points)
    {
        for (var i = 0; i < points.Count; i++)
            points[i] = new Vector2(points[i].y, points[i].x);
    }

    public List<Vector2> CreateCurve(Direction dir, PosNegType type)
    {
        var sw = TileSize;
        var sh = TileSize;

        var pts = new List<Vector2>(BezCurve);
        switch (dir)
        {
            case Direction.Up:
                switch (type)
                {
                    case PosNegType.Pos:
                        TranslatePoints(pts, new Vector2(Padding, Padding + sh));
                        break;
                    case PosNegType.Neg:
                        InvertY(pts);
                        TranslatePoints(pts, new Vector2(Padding, Padding + sh));
                        break;
                    case PosNegType.None:
                    default:
                    {
                        pts.Clear();
                        for (var i = 0; i < 100; i++)
                        {
                            pts.Add(new Vector2(Padding + i, Padding + sh));
                        }

                        break;
                    }
                }
                break;
            case Direction.Right:
                switch (type)
                {
                    case PosNegType.Pos:
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector2(Padding + sw, Padding));
                        break;
                    case PosNegType.Neg:
                        InvertY(pts);
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector2(Padding + sw, Padding));
                        break;
                    case PosNegType.None:
                    default:
                    {
                        pts.Clear();
                        for (var i = 0; i < 100; i++)
                        {
                            pts.Add(new Vector2(Padding + sw, Padding + i));
                        }

                        break;
                    }
                }
                break;
            case Direction.Down:
                switch (type)
                {
                    case PosNegType.Pos:
                        InvertY(pts);
                        TranslatePoints(pts, new Vector2(Padding, Padding));
                        break;
                    case PosNegType.Neg:
                        TranslatePoints(pts, new Vector2(Padding, Padding));
                        break;
                    case PosNegType.None:
                    default:
                    {
                        pts.Clear();
                        for (var i = 0; i < 100; i++)
                        {
                            pts.Add(new Vector2(Padding + i, Padding));
                        }

                        break;
                    }
                }
                break;
            case Direction.Left:
                switch (type)
                {
                    case PosNegType.Pos:
                        InvertY(pts);
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector2(Padding, Padding));
                        break;
                    case PosNegType.Neg:
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector2(Padding, Padding));
                        break;
                    case PosNegType.None:
                    default:
                    {
                        pts.Clear();
                        for (var i = 0; i < 100; i++)
                        {
                            pts.Add(new Vector2(Padding, Padding + i));
                        }

                        break;
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        return pts;
    }

    public void DrawCurve(Direction dir, PosNegType type, Color color, float width = 0.5f, Transform parent = null)
    {
        if(!_mLineRenderers.ContainsKey((dir, type)))
            _mLineRenderers.Add((dir, type), CreateLineRenderer(color, width, $"{dir}_{type}", parent));

        var lr = _mLineRenderers[(dir, type)];
        lr.startColor = color;
        lr.endColor = color;
        lr.gameObject.name = $"LineRenderer_{dir}_{type}";
        lr.gameObject.SetActive(true);

        var pts = CreateCurve(dir, type);

        lr.positionCount = pts.Count;
        for(var i = 0; i < pts.Count; i++)
            lr.SetPosition(i, pts[i]);
    }

    public void HideAllCurves()
    {
        foreach (var item in _mLineRenderers)
        {
            item.Value.gameObject.SetActive(false);
        }
    }
}