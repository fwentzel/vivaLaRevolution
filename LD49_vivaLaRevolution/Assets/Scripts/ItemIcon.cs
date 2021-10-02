using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Item item;
    public Image iconImage;


    public void OnPointerDown(PointerEventData eventData)
    {
        print("STUUUFFFF " + eventData.position);
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(eventData.position),
            Camera.main.ScreenPointToRay(eventData.position).direction, Color.red, 1f);
        Debug.DrawLine(Camera.main.transform.position, eventData.position, Color.red, 1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(eventData.position),Camera.main.ScreenPointToRay(eventData.position).direction,Color.red,1f);
    }
}
