
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
public class PoliceGroup : MonoBehaviour
{
    public List<HoldPoint> holdPoints;
    public List<Police> members;

    public int currentHoldIndex = 0;

    public int startSize = 10;

    public int requiredPerHoldPoint = 6;



    public void TryGoToPreviousHoldPoint()
    {
        if (currentHoldIndex > 0)
        {
            currentHoldIndex--;
            UpdateHoldPos();
        }
    }
    public void TryGoToNextHoldPoint()
    {
        if (currentHoldIndex < holdPoints.Count - 1)
        {
            currentHoldIndex++;
            UpdateHoldPos();
        }
    }

    public void UpdateHoldPos()
    {
        foreach (Police member in members)
        {
            if (member == null || holdPoints[currentHoldIndex] == null)
                continue;
            member.holdPosition = holdPoints[currentHoldIndex].transform;
        }
    }
    public int CheckChangeHoldPosition()
    {
        int requiredForCurrentHoldPoint = currentHoldIndex * requiredPerHoldPoint;
        // if (ratio <= 0.5f)
        if (members.Count < requiredForCurrentHoldPoint)
        {
            return -1;
        }

        if (members.Count > requiredForCurrentHoldPoint + requiredPerHoldPoint)
        {
            return 1;
        }
        return 0;

    }
    private void OnDrawGizmos()
    {
        if (!Selection.Contains(gameObject))
            return;
        Gizmos.color = Color.yellow;
        if (holdPoints.Count > 2)
        {
            Gizmos.DrawSphere(holdPoints[0].transform.position, .5f);
            for (int i = 1; i < holdPoints.Count; i++)
            {
                Gizmos.DrawLine(holdPoints[i - 1].transform.position, holdPoints[i].transform.position);

            }
        }
    }

}
