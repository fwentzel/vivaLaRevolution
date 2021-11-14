
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class RTSController : MonoBehaviour
{
    private RTSSelection _rtsSelection;
    public LayerMask defaultLayer;

    InputActions.SelectionActions selectionInput;



    private void Start()
    {
        selectionInput = InputActionsManager.instance.inputActions.Selection;
        selectionInput.Enable();
        _rtsSelection = GetComponent<RTSSelection>();
        selectionInput.RightClick.performed += ctx => HandleRightClick();
    }

    private void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(selectionInput.Point.ReadValue<Vector2>());
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, 1000, defaultLayer))
        {
            Building building = hit.transform.GetComponent<Building>();
            if (building)
            {
                OrderGoLoot(building);
            }
            else
            {
                OrderMove(hit.point);
            }
        }
    }


    public void OrderGoLoot(Building building)
    {
        if (!building||!building.lootable)
            return;

        for (int i = 0; i < _rtsSelection.selectedUnits.Count; i++)
        {
            if (_rtsSelection.selectedUnits[i] == null)
                continue;
            _rtsSelection.selectedUnits[i].TryEnterBuilding(building);
        }

    }

    public void OrderMove(Vector3 position)
    {

        if (_rtsSelection.selectedUnits.Count == 0)
            return;

        float distance = _rtsSelection.selectedUnits[0].transform.localScale.x / 2;
        distance *= 1.5f;

        List<Vector3> targetPositions = GetPositionListAround(position, new float[] { 1, 2, 3, 4, 5, 6 }, new int[] { 5, 10, 20, 30, 40, 100 });


        for (int i = 0; i < _rtsSelection.selectedUnits.Count; i++)
        {
            if (_rtsSelection.selectedUnits[i] == null)
                continue;
            _rtsSelection.selectedUnits[i].SetMovePosition(targetPositions[i]);
        }
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistances, int[] ringPositionCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistances.Length; i++)
        {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistances[i], ringPositionCount[i]));
        }

        return positionList;
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
