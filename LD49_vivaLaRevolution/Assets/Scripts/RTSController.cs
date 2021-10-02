﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
        print("COMMAND! " + position.ToString());
        
        if(_rtsSelection.selectedUnits.Count == 0)
            return;

        float distance = _rtsSelection.selectedUnits[0].transform.localScale.x / 2;
        distance *= 1.5f;

        List<Vector3> targetPositions = GetPositionListAround(position, distance, _rtsSelection.selectedUnits.Count);

        for (int i = 0; i <  _rtsSelection.selectedUnits.Count; i++)
        {
            _rtsSelection.selectedUnits[i].SetMovePosition(targetPositions[i]);
        }
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            
            float randomDistance = distance * Random.Range(1, 1.5f);
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotattionToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * randomDistance;
            positionList.Add(position);

        }

        return positionList;
    }

    private Vector3 ApplyRotattionToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, angle, 0) * vec;
    }
}
