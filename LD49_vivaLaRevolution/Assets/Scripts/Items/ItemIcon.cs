﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    public Item item;
    public Image iconImage;

    public UnityEvent onSelect;
    public UnityEvent onDeselect;
    public bool isSelected = false;
    
    public void Setup(Item item)
    {
        this.item = item;
        this.iconImage.sprite=item.icon;
    }
    
    

    public void Release()
    {
        if (!item)
        {
            Destroy(gameObject);
            return;
        }
        //TODO
        
        Vector3 position = RTSSelection.CastToGround(InputSystem.GetDevice<Pointer>().position.ReadValue());
        
        Debug.DrawRay(position,Vector3.up*100,Color.magenta,1f);
        
        item.Use(position);
        Destroy(gameObject);
    }


    public void Select()
    {
        isSelected = true;
        onSelect?.Invoke();
    }

    public void Deselect()
    {
        isSelected = false;   
        onDeselect?.Invoke();
    }
}
