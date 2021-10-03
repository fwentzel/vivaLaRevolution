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
    

    private void Update()
    {
        transform.position = GetMid();
    }

    Vector3 GetMid()
    {

        if (_rtsSelection == null || _rtsSelection.selectedUnits.Count == 0)
            return transform.position;


        Vector3 mid = Vector3.zero;
        int count = 0;
        foreach (var unit in _rtsSelection.selectedUnits)
        {
            if (unit == null) continue;
            if(unit.item==null) continue;
            mid += unit.transform.position;
            count++;
        }
        
        if(count== 0)
            return transform.position;
        
        mid /= count;

        return mid;
    }

    public void UpdateCanvas()
    {

    }
}
