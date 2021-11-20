
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PoliceMeele : PoliceBase
{

    override protected void Update()
    {
        base.Update();
        
        navMeshAgent.destination = holdPosition == null || isRunning ? moveToPosition : holdPosition.position;

    }

}
