using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemManager : MonoBehaviour
{
    private RTSSelection _rtsSelection;
    public RectTransform content;
    
    
    private void Start()
    {
        _rtsSelection = FindObjectOfType<RTSSelection>();
        _rtsSelection.OnUnitSelection.AddListener(OnSelectedUnits);
    }

    public void OnSelectedUnits(List<Protestor> protestors)
    {
        
    }

    private void Update()
    {
        transform.position = GetMid();
    }

    Vector3 GetMid()
    {
        if (_rtsSelection.selectedUnits.Count == 0)
            return transform.position;
        
        
        Vector3 mid = Vector3.zero;
        foreach (var unit in _rtsSelection.selectedUnits)
        {
            mid += unit.transform.position;
        }

        mid /= _rtsSelection.selectedUnits.Count;
        
        return mid;
    }

    public void UpdateCanvas()
    {
        
    }
}
