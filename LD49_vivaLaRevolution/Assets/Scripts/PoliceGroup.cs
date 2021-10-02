
using UnityEngine;
using System.Collections.Generic;
public class PoliceGroup : MonoBehaviour
{
    public List<HoldPoint> holdPoints;
    public List<Police> members;

    public int currentHoldIndex = 0;

    public int startSize = 10;

    public void RegisterFatality(Police member){
        members.Remove(member);
        if(members.Count/(float)startSize< currentHoldIndex/(float)holdPoints.Count){
            SetHoldPositionToNext();
        }
    }
    public void SetHoldPositionToNext()
    {
    print("New Pos");
        if (holdPoints.Count <= currentHoldIndex)
            return;
        foreach (Police member in members)
        {
            member.holdPosition = holdPoints[currentHoldIndex].transform;
        }
        currentHoldIndex++;
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
                Gizmos.DrawSphere(holdPoints[i].transform.position, .5f);
            }
        }
    }
}
