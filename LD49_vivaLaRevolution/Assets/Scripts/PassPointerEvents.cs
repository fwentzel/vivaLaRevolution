using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;



public class PassPointerEvents : MonoBehaviour,IPointerDownHandler, IDragHandler,IPointerUpHandler
{
    public UnityEvent<PointerEventData> onPointerDown;
    public UnityEvent<PointerEventData> onDrag;
    public UnityEvent<PointerEventData> onOnPointerUp;


    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onOnPointerUp?.Invoke(eventData);
    }
}
