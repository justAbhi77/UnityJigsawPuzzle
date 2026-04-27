using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Visualizes control points and a Bezier curve using LineRenderers.
/// Allows adding new control points via double-click.
/// </summary>
public class BezierVizBase : MonoBehaviour
{
    public GameObject pointPrefab;

    [Header("Line Settings")]
    public float lineWidth = 0.05f;
    public float lineWidthBezier = 0.08f;

    public Color lineColor = new(0.5f, 0.5f, 0.5f, 0.8f);
    public Color bezierCurveColor = new(0.5f, 0.6f, 0.8f, 0.8f);

    private LineRenderer _controlLine;
    private LineRenderer _curveLine;

    private readonly List<GameObject> _points = new();
    private readonly List<Vector2> _cachedPositions = new();

    private Camera _mainCamera;

    private const int MaxControlPoints = 18;

    #region Unity Methods

    private void Start()
    {
        _mainCamera = Camera.main;

        InitializeLineRenderers();
        SpawnInitialPoints();
    }

    private void Update()
    {
        UpdateControlPointsCache();
        DrawControlPolygon();
        DrawBezierCurve();
    }

    private void OnGUI()
    {
        // Detect double left-click to insert new control point
        var e = Event.current;

        if (!_mainCamera || !e.isMouse)
            return;

        if (e.clickCount != 2 || e.button != 0) return;

        var mouPos = Mouse.current.position.ReadValue();
        var mouseWorld = _mainCamera.ScreenToWorldPoint(mouPos);
        InsertNewControlPoint(new Vector2(mouseWorld.x, mouseWorld.y));
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Creates and configures the line renderers.
    /// </summary>
    private void InitializeLineRenderers()
    {
        _controlLine = CreateLineRenderer("ControlPolygon", lineColor, lineWidth);
        _curveLine = CreateLineRenderer("BezierCurve", bezierCurveColor, lineWidthBezier);
    }

    /// <summary>
    /// Instantiates initial control point objects.
    /// </summary>
    private void SpawnInitialPoints()
    {
        var controlPointsList = GetControlPoints();
        for (var i = 0; i < controlPointsList.Count; i++)
            CreateControlPoint(controlPointsList[i], i);
    }

    #endregion

    #region Rendering

    /// <summary>
    /// Updates cached positions from GameObjects (avoids LINQ allocations).
    /// </summary>
    private void UpdateControlPointsCache()
    {
        _cachedPositions.Clear();

        foreach (var point in _points)
            _cachedPositions.Add(point.transform.position);
    }

    /// <summary>
    /// Draws the control polygon.
    /// </summary>
    private void DrawControlPolygon()
    {
        _controlLine.positionCount = _cachedPositions.Count;

        for (var i = 0; i < _cachedPositions.Count; i++)
        {
            _controlLine.SetPosition(i, _cachedPositions[i]);
        }
    }

    /// <summary>
    /// Draws the Bezier curve using calculated points.
    /// </summary>
    private void DrawBezierCurve()
    {
        var curvePoints = BezierCurve.PointList2(_cachedPositions);

        _curveLine.positionCount = curvePoints.Count;

        for (var i = 0; i < curvePoints.Count; i++)
            _curveLine.SetPosition(i, curvePoints[i]);
    }

    #endregion

    #region Control Point Management

    /// <summary>
    /// Adds a new control point at runtime.
    /// </summary>
    private void InsertNewControlPoint(Vector2 position)
    {
        if (_points.Count >= MaxControlPoints)
        {
            Debug.LogWarning($"Cannot insert more than {MaxControlPoints} control points.");
            return;
        }

        CreateControlPoint(position, _points.Count);
    }

    /// <summary>
    /// Instantiates and registers a control point.
    /// </summary>
    private void CreateControlPoint(Vector2 position, int index)
    {
        var obj = Instantiate(pointPrefab, position, quaternion.identity);
        obj.name = $"ControlPoint_{index}";
        _points.Add(obj);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Creates a configured LineRenderer.
    /// </summary>
    private static LineRenderer CreateLineRenderer(string name, Color color, float width)
    {
        var obj = new GameObject(name);
        var lr = obj.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.useWorldSpace = true;

        return lr;
    }

    /// <summary>
    /// Virtual function to get the control points.
    /// </summary>
    /// <returns>
    /// The control Points used.
    /// </returns>
    protected virtual List<Vector2> GetControlPoints()
    {
        return new List<Vector2>()
        {
            new (-5.0f, -5.0f),
            new (0.0f, 2.0f),
            new (5.0f, -2.0f)
        };
    }

    #endregion
}