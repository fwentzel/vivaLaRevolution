
using UnityEngine;
using UnityEngine.AI;

public class Police : RTSUnit
{
    public PoliceGroup group;
    public Transform holdPosition;

    public bool isRunning { get; private set; } = false;
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
        isRunning = !group.IsNearCurrentHoldPoint(this);
        // isRunning = myHealth.HealthRatio()<.2f;
        if (isRunning)
        {
            targetHealth = null;
        }

        base.Update();

        if (targetHealth == null)
        {
            if (!isRunning)
            {
                //Staying at holdPoint, so look for enemies
                foreach (Collider collider in colliders)
                {
                    targetHealth = collider.GetComponent<Health>();
                    break;
                }
            }

            navMeshAgent.destination = holdPosition == null ? moveToPosition : holdPosition.position;
        }
    }

    override public void OnKill()
    {
        PoliceManager.instance.RegisterFatality(this);
        base.OnKill();
    }
    void OnDestroy()
    {
            group.members.Remove(this);
        //if (group.members.Contains(this))
    }

    private void OnDrawGizmos()
    {
        if (!holdPosition)
            return;


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(holdPosition.position, fightWithinRange);
    }
}
