
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
        SetMovePosition(newPos.position);

    }
    protected override void Update()
    {
        base.Update();
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
        PoliceManager.instance.RegisterFatality(this);
    }

    private void OnDrawGizmos()
    {
        if(!holdPosition)
            return;
        
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(holdPosition.position, fightWithinRange);
    }
}
