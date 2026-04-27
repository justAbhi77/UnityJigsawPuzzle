using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visualizes control points and a Bezier curve using LineRenderers.
/// Allows adding new control points via double-click.
/// </summary>
public class BezierViz : BezierVizBase
{
    [Header("Control Points")]
    public List<Vector2> controlPointsList = new()
    {
        new Vector2(-5.0f, -5.0f),
        new Vector2(0.0f, 2.0f),
        new Vector2(5.0f, -2.0f),
    };

    protected override List<Vector2> GetControlPoints()
    {
        return controlPointsList;
    }
}