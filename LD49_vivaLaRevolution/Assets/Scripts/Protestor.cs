
using UnityEngine;
using UnityEngine.AI;

public class Protestor : RTSUnit
{
    protected override void Update()
    {
        base.Update();
        if(targetHealth==null)
        {
            foreach (Collider collider in Physics.OverlapSphere(transform.position, detectRadius))
            {
                if (collider.tag.Equals("Police"))
                {
                    targetHealth = collider.GetComponent<Health>();
                    break;
                }
            }
            if (moveToPosition != Vector3.zero)
                navMeshAgent.destination = moveToPosition;
        }
    }

    private void OnDrawGizmos()
    {
 Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color=Color.green;
        Gizmos.DrawWireSphere(moveToPosition, fightWithinRange);
       
    }
}
