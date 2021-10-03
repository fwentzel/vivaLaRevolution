
using UnityEngine;
using System.Collections.Generic;
using System;

public class PoliceGroup : MonoBehaviour
{
    public List<HoldPoint> holdPoints;
    public List<Police> members;

    public int currentHoldIndex = 0;

    public int startSize = 10;
    private int startSizeAtHoldpoint;
    private void Awake()
    {


        startSizeAtHoldpoint = startSize;
    }
    public void RegisterFatality(Police member)
    {
        members.Remove(member);
        SetHoldPosition();
    }
    public void SetHoldPosition()
    {
        float ratio = members.Count / (float)startSizeAtHoldpoint;
        if (ratio <= 0.5f && currentHoldIndex < holdPoints.Count - 1)
        {
            startSizeAtHoldpoint = members.Count;
            currentHoldIndex++;
        }

        if (ratio >= 2f && currentHoldIndex > 0)
        {
            startSizeAtHoldpoint = members.Count;
            currentHoldIndex--;
        }

        foreach (Police member in members)
        {
            if (member == null||holdPoints[currentHoldIndex]==null)
                continue;
            member.holdPosition = holdPoints[currentHoldIndex].transform;
        }
    }
    private void OnDrawGizmos()
    {
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

    internal void AddMember(Police police)
    {
        members.Add(police);

        SetHoldPosition();
    }
}
