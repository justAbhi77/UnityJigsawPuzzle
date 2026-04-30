using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PointViz : MonoBehaviour
{
    private Vector3 _mOffset = Vector3.zero;

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Camera.main == null) return;

        var mouPos = Mouse.current.position.ReadValue();
        _mOffset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(mouPos.x, mouPos.y, 0.0f));
    }

    private void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        var mouPos = Mouse.current.position.ReadValue();
        var curScreenPoint = new Vector3(mouPos.x, mouPos.y, 0.0f);

        if (Camera.main == null) return;

        var curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _mOffset;
        transform.position = curPosition;
    }

    private void OnMouseUp()
    {
        _mOffset = Vector3.zero;
    }
}