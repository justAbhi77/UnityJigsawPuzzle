using System;
using System.Collections.Generic;
using UnityEngine;

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

    public Vector2Int Offset = new (20, 20);

    private int _tileSize = 100;

    private Dictionary<(Direction, PosNegType), LineRenderer> _mLineRenderers = new();

    public static List<Vector2> BezCurve = BezierCurve.PointList2(BezierVizTemplate.TemplateControlPoints, 0.001f);

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
        var paddingX = Offset.x;
        var paddingY = Offset.y;
        var sw = _tileSize;
        var sh = _tileSize;

        var pts = new List<Vector2>(BezCurve);
        switch (dir)
        {
            case Direction.Up:
                switch (type)
                {
                    case PosNegType.Pos:
                        TranslatePoints(pts, new Vector2(paddingX, paddingY + sh));
                        break;
                    case PosNegType.Neg:
                        InvertY(pts);
                        TranslatePoints(pts, new Vector2(paddingX, paddingY + sh));
                        break;
                    case PosNegType.None:
                    default:
                    {
                        pts.Clear();
                        for (var i = 0; i < 100; i++)
                        {
                            pts.Add(new Vector2(paddingX + i, paddingY + sh));
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
                        TranslatePoints(pts, new Vector2(paddingX + sw, paddingY));
                        break;
                    case PosNegType.Neg:
                        InvertY(pts);
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector2(paddingX + sw, paddingY));
                        break;
                    case PosNegType.None:
                    default:
                    {
                        pts.Clear();
                        for (var i = 0; i < 100; i++)
                        {
                            pts.Add(new Vector2(paddingX + sw, paddingY + i));
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
                        TranslatePoints(pts, new Vector2(paddingX, paddingY));
                        break;
                    case PosNegType.Neg:
                        TranslatePoints(pts, new Vector2(paddingX, paddingY));
                        break;
                    case PosNegType.None:
                    default:
                    {
                        pts.Clear();
                        for (var i = 0; i < 100; i++)
                        {
                            pts.Add(new Vector2(paddingX + i, paddingY));
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
                        TranslatePoints(pts, new Vector2(paddingX, paddingY));
                        break;
                    case PosNegType.Neg:
                        SwapXY(pts);
                        TranslatePoints(pts, new Vector2(paddingX, paddingY));
                        break;
                    case PosNegType.None:
                    default:
                    {
                        pts.Clear();
                        for (var i = 0; i < 100; i++)
                        {
                            pts.Add(new Vector2(paddingX, paddingY + i));
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

        var pts = CreateCurve(dir, type);

        lr.positionCount = pts.Count;
        for(var i = 0; i < pts.Count; i++)
            lr.SetPosition(i, pts[i]);
    }
}