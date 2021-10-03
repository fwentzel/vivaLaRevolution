
using UnityEngine;
using UnityEngine.AI;

public class Police : RTSUnit
{
    public PoliceGroup group;
    public Transform holdPosition;
    protected override void Start()
    {
        moveToPosition = holdPosition.position;
        base.Start();
    }

    private void SetHoldPosition(Transform newPos)
    {
        holdPosition = newPos;
        moveToPosition = newPos.position;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (targetHealth == null)
        {

            foreach (Collider collider in colliders)
            {
                targetHealth = collider.GetComponent<Health>();
                break;
            }

            navMeshAgent.destination = holdPosition == null ? moveToPosition : holdPosition.position;
        }
    }

    private void OnDestroy()
    {
        group.RegisterFatality(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(holdPosition.position, fightWithinRange);
    }
}
