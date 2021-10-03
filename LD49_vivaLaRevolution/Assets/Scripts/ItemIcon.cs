using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IPointerClickHandler
{
    public Item item;
    public Image iconImage;

    public UnityEvent onSelect;
    public UnityEvent onDeselect;
    public bool isSelected = false;
    
    public void Setup(Item item)
    {
        this.item = item;
    }
    
    
    public void OnPointerUp(PointerEventData eventData)
    {
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("ENTETERARA");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button!= PointerEventData.InputButton.Left)
            return;
        print("SELECTED");
        isSelected = true;
        
        if(isSelected)
            onSelect?.Invoke();
        else
            onDeselect?.Invoke();
    }
}
