
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PoliceMeele : PoliceBase
{

    override protected void Update()
    {
        base.Update();

        if (targetHealth!=null && !isRunning)
        {
            if (Vector3.Distance(targetHealth.transform.position, holdPosition.transform.position) <= fightWithinRange)
            {
                navMeshAgent.destination = targetHealth.transform.position;
                return;
            }
        }

        navMeshAgent.destination = holdPosition == null || isRunning ? moveToPosition : holdPosition.position;



    }

}
