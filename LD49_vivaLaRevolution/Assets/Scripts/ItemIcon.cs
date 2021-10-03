using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Item item;
    public Image iconImage;

    private bool isDragging = false;

    public void Setup(Item item)
    {
        this.item = item;
    }
    
    
    
    
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
        isDragging = true;
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(eventData.position),Camera.main.ScreenPointToRay(eventData.position).direction,Color.red,1f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && isDragging)
            Release();
        
        if(!isDragging)
            return;


        Vector3 position = RTSSelection.CastToGround(Input.mousePosition);
        
        Debug.DrawRay(position,Vector3.up,Color.magenta,1f);

    }

    public void Release()
    {
        if (!item)
        {
            Destroy(gameObject);
            return;
        }
        
        Vector3 position = RTSSelection.CastToGround(Input.mousePosition);
        
        Debug.DrawRay(position,Vector3.up*100,Color.magenta,1f);
        
        item.Use(position);
        Destroy(gameObject);
    }
}
