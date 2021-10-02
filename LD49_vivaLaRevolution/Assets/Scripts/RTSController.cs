
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RTSController : MonoBehaviour
{
    private RTSSelection _rtsSelection;

    private void Start()
    {
        _rtsSelection = GetComponent<RTSSelection>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OrderUnits(_rtsSelection.CastToGround(Input.mousePosition));
        }
    }

    public void OrderUnits(Vector3 position)
    {
        foreach (var unit in _rtsSelection.selectedUnits)
        {
            //unit.moveTo
        }
    }
}
