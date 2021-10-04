using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemManager : MonoBehaviour
{
    private RTSSelection _rtsSelection;
    public RectTransform content;
    public ItemIcon iconPreset;
    private List<ItemIcon> _itemIcons = new List<ItemIcon>();
    public CanvasGroup canvasGroup;

    public Transform aimingRecticle;
    
    
    private void Start()
    {
        _rtsSelection = FindObjectOfType<RTSSelection>();
        if (_rtsSelection != null)
            _rtsSelection.OnUnitSelection.AddListener(OnSelectedUnits);
    }

    public void OnSelectedUnits(List<Protestor> protestors)
    {
        PopulateList(protestors);
    }

    public void PopulateList(List<Protestor> protestors)
    {
        ClearList();
        iconPreset.gameObject.SetActive(false);
        foreach (var protestor in protestors)
        {
            if(!protestor.item)
                continue;

            GameObject itemIconObj = Instantiate(iconPreset.gameObject, content);
            ItemIcon itemIcon = itemIconObj.GetComponent<ItemIcon>();
            itemIcon.Setup(protestor.item);
            itemIconObj.SetActive(true);
            _itemIcons.Add(itemIcon);
        }

    }

    public void ClearList()
    {
        foreach (var itemIcon in _itemIcons)
        {
            if(!itemIcon)
                continue;
            Destroy(itemIcon.gameObject);
        }
        _itemIcons.Clear();
    }


    public void Update()
    {
        Vector3 position = RTSSelection.CastToGround(Input.mousePosition);
        ItemIcon selectedItem = null;
        foreach (var itemIcon in _itemIcons)
        {
            if(!itemIcon)
                continue;
            if(Input.GetMouseButtonDown(1))
                itemIcon.Deselect();
            
            if (itemIcon.isSelected)
                selectedItem = itemIcon;
        }
        
        aimingRecticle.gameObject.SetActive(selectedItem!=null);
        if (selectedItem && selectedItem.item)
        {
            
            position = selectedItem.item.GetImprovedPosition(position);
            
            aimingRecticle.position = position + Vector3.up * 0.5f;
            aimingRecticle.transform.localScale = Vector3.one * 2 * selectedItem.item.influenceRadius;
            
            
            if (Input.GetMouseButtonDown(0))
            {
                selectedItem.Release();
            }
        }
        
        
            
    }
}
