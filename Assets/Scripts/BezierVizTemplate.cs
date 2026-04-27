using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visualizes control points and a Bezier curve using LineRenderers.
/// Allows adding new control points via double-click.
/// </summary>
public class BezierVizTemplate : BezierVizBase
{
    [Header("Control Points")]
    public List<Vector2> controlPointsList = new()
    {
        new Vector2(0, 0),
        new Vector2(35, 15),
        new Vector2(47, 13),
        new Vector2(45, 5),
        new Vector2(48, 0),
        new Vector2(25, -5),
        new Vector2(15, -18),
        new Vector2(36, -20),
        new Vector2(64, -20),
        new Vector2(85, -18),
        new Vector2(75, -5),
        new Vector2(52, 0),
        new Vector2(55, 5),
        new Vector2(53, 13),
        new Vector2(65, 15),
        new Vector2(100, 0)
    };

    public static readonly List<Vector2> TemplateControlPoints = new()
    {
        new Vector2(0, 0),
        new Vector2(35, 15),
        new Vector2(47, 13),
        new Vector2(45, 5),
        new Vector2(48, 0),
        new Vector2(25, -5),
        new Vector2(15, -18),
        new Vector2(36, -20),
        new Vector2(64, -20),
        new Vector2(85, -18),
        new Vector2(75, -5),
        new Vector2(52, 0),
        new Vector2(55, 5),
        new Vector2(53, 13),
        new Vector2(65, 15),
        new Vector2(100, 0)
    };

    protected override List<Vector2> GetControlPoints()
    {
        return controlPointsList;
    }
}