using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TileMovement : MonoBehaviour
{
    public Tile Tile;
    public Vector3 mOffset = Vector3.zero;
    public float snapDistance = 25.0f;

    private SpriteRenderer _spriteRenderer;

    public delegate void DelegateOnTileInPlace(TileMovement tm);
    public DelegateOnTileInPlace OnTileInPlace;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private Vector3 GetCorrectPosition()
    {
        return new Vector3(Tile.XIndex * 100f, Tile.YIndex * 100f, 0);
    }

    private void OnMouseDown()
    {
        if(!GameManager.Instance.tileMovementEnabled) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Camera.main == null) return;

        var mouPos = Mouse.current.position.ReadValue();
        mOffset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(mouPos.x, mouPos.y, 0.0f));

        Tile.TileSorting.BringToTop(_spriteRenderer);
    }

    private void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Camera.main == null) return;

        var mouPos = Mouse.current.position.ReadValue();
        var curScreenPoint = new Vector3(mouPos.x, mouPos.y, 0.0f);

        var curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + mOffset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Camera.main == null) return;

        var corDist = GetCorrectPosition();
        var dist = (transform.position - corDist).magnitude;

        if (!(dist < snapDistance)) return;
        transform.position = corDist;
        OnTileInPlace?.Invoke(this);
    }
}
