
using UnityEngine;
using System.Collections.Generic;
using System;
public class PoliceGroup : MonoBehaviour
{
    public List<HoldPoint> holdPoints;
    public int startSize = 10;
    [Range(0,1)]
    public float musketeerPercentage;    
    public bool ignoreRespawnAndHoldpointCalc = false;
    [HideInInspector] public List<PoliceBase> members;
    public int currentHoldIndex { get; private set; } = 0;

    public void TryGoToPreviousHoldPoint()
    {
        if (currentHoldIndex > 0)
        {
            currentHoldIndex--;
            UpdateHoldPos(false);
        }
    }
    public void TryGoToNextHoldPoint()
    {
        
        if (currentHoldIndex < holdPoints.Count - 1)
        {
            currentHoldIndex++;
            UpdateHoldPos(true);
        }
    }

    internal bool IsNearCurrentHoldPoint(PoliceBase policeEntity)
    {
        return Vector3.Distance(holdPoints[currentHoldIndex].transform.position, policeEntity.transform.position) < policeEntity.fightWithinRange;
    }

    public void UpdateHoldPos(bool isAdvancing)
    {
        foreach (PoliceBase member in members)
        {
            if (member == null || holdPoints[currentHoldIndex] == null)
                continue;
            member.SetHoldPosition(holdPoints[currentHoldIndex].transform)  ;
            if (isAdvancing)
            {
                member.isAdvancing = true;
                member.isFallingBack = false;
            }
            else
            {
                //fleeing
                member.isAdvancing = false;
                member.isFallingBack = true;
            }
        }
    }
    public int CheckChangeHoldPosition()
    {
        int requiredForCurrentHoldPoint = currentHoldIndex * PoliceManager.instance.requiredAmountPerHoldPoint;
        //-1 since it is checked BEFORE a police unit is Killed/Destroyed
        int membersInRangeOfCurrentHoldpoint = GetMembersInRangeOfOrRunningToCurrentHoldpoint()-1;
        // if (ratio <= 0.5f)
        if (membersInRangeOfCurrentHoldpoint < requiredForCurrentHoldPoint)
        {
            return -1;
        }

        if (membersInRangeOfCurrentHoldpoint > requiredForCurrentHoldPoint + PoliceManager.instance.requiredAmountPerHoldPoint)
        {
            return 1;
        }
        return 0;

    }

    private int GetMembersInRangeOfOrRunningToCurrentHoldpoint()
    {
        int amount = 0;
        foreach (PoliceBase member in members)
        {
            if (IsNearCurrentHoldPoint(member) || (member.isFallingBack || member.isAdvancing))
            {
                amount++;
            }
        }
        return amount;
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

    public void SetupCurrentHoldpoint()
    {
        if (holdPoints.Count > 0)
        {
            for (int i = holdPoints.Count - 1; i >= 0; i--)
            {
                if (startSize >= i * PoliceManager.instance.requiredAmountPerHoldPoint)
                {
                    currentHoldIndex = i;
                    return;
                }
            }
        }
    }
}
