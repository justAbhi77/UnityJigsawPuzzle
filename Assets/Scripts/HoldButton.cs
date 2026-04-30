using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed;

    public UnityEvent onPressAndHold;
    public UnityEvent onRelease;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        onPressAndHold.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        onRelease.Invoke();
    }
}