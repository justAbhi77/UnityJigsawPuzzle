using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Utility class for evaluating Bézier curves (2D and 3D).
/// Uses Bernstein polynomial form with precomputed factorials.
/// </summary>
public static class BezierCurve
{
    /// <summary>
    /// Precomputed factorial lookup (0! to 18!)
    /// Used for fast binomial coefficient calculation.
    /// </summary>
    private static readonly float[] Factorials =
    {
        1f, 1f, 2f, 6f, 24f, 120f, 720f, 5040f, 40320f,
        362880f, 3628800f, 39916800f, 479001600f,
        6227020800f, 87178291200f, 1307674368000f,
        20922789888000f, 355687428096000f, 6402373705728000f
    };

    /// <summary>
    /// Computes binomial coefficient: nCi = n! / (i! * (n-i)!)
    /// </summary>
    private static float Binomial(int n, int i)
    {
        if (n is < 0 or > 18) return 1f;
        return Factorials[n] / (Factorials[i] * Factorials[n - i]);
    }

    /// <summary>
    /// Bernstein basis polynomial:
    /// B(n, i, t) = nCi * t^i * (1 - t)^(n - i)
    /// </summary>
    private static float Bernstein(int n, int i, float t)
    {
        return Binomial(n, i) *
               Mathf.Pow(t, i) *
               Mathf.Pow(1f - t, n - i);
    }

    /// <summary>
    /// Ensures control points are within supported limit (max 18).
    /// Prevents factorial overflow.
    /// </summary>
    private static List<T> GetWorkingPoints<T>(List<T> controlPoints, out int n)
    {
        n = controlPoints.Count - 1;

        if (n <= 18) return controlPoints;
        Debug.LogWarning("More than 18 control points. Truncating to 18.");
        n = 17;
        return controlPoints.Take(18).ToList();
    }

    /// <summary>
    /// Evaluate a single point 2D Bézier curve at parameter t (0 → 1).
    /// </summary>
    public static Vector2 Point2(float t, List<Vector2> controlPoints)
    {
        var pts = GetWorkingPoints(controlPoints, out var n);

        // Clamp endpoints
        switch (t)
        {
            case <= 0:
                return pts[0];
            case >= 1:
                return pts[n];
        }

        var result = Vector2.zero;

        // Sum: Σ B(n,i,t) * P_i
        for (var i = 0; i <= n; i++)
            result += Bernstein(n, i, t) * pts[i];

        return result;
    }

    /// <summary>
    /// Evaluate a single point 3D Bézier curve at parameter t (0 → 1).
    /// </summary>
    public static Vector3 Point3(float t, List<Vector3> controlPoints)
    {
        var pts = GetWorkingPoints(controlPoints, out var n);

        switch (t)
        {
            case <= 0:
                return pts[0];
            case >= 1:
                return pts[n];
        }

        var result = Vector3.zero;

        for (var i = 0; i <= n; i++)
            result += Bernstein(n, i, t) * pts[i];

        return result;
    }

    /// <summary>
    /// Generates a list of points along a 2D Bézier curve.
    /// </summary>
    public static List<Vector2> PointList2(List<Vector2> controlPoints, float interval = 0.01f)
    {
        var pts = GetWorkingPoints(controlPoints, out var n);
        var result = new List<Vector2>();

        var steps = Mathf.CeilToInt(1f / interval);

        for (var s = 0; s <= steps; s++)
        {
            var t = s / (float)steps;
            var point = Vector2.zero;

            for (var i = 0; i <= n; i++)
                point += Bernstein(n, i, t) * pts[i];

            result.Add(point);
        }

        return result;
    }

    /// <summary>
    /// Generates a list of points along a 3D Bézier curve.
    /// </summary>
    public static List<Vector3> PointList3(List<Vector3> controlPoints, float interval = 0.01f)
    {
        var pts = GetWorkingPoints(controlPoints, out var n);
        var result = new List<Vector3>();

        var steps = Mathf.CeilToInt(1f / interval);

        for (var s = 0; s <= steps; s++)
        {
            var t = s / (float)steps;
            var point = Vector3.zero;

            for (var i = 0; i <= n; i++)
                point += Bernstein(n, i, t) * pts[i];

            result.Add(point);
        }

        return result;
    }
}
