
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Police : RTSUnit
{
    public PoliceGroup group;
    public Transform holdPosition;

    public bool isRunning = false;
    public bool isAdvancing = false;

    protected override void Start()
    {
        moveToPosition = holdPosition.position;
        myHealth.onTakeDamage.AddListener((x) => onTakeDamage());
        base.Start();
    }

    private void SetHoldPosition(Transform newPos)
    {
        holdPosition = newPos;
        SetMovePosition(newPos.position);

    }
    protected override void Update()
    {
        if (isAdvancing && group.IsNearCurrentHoldPoint(this))
        {
            isAdvancing = false;
        }
        // isRunning = myHealth.HealthRatio()<.2f;

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

            navMeshAgent.destination = holdPosition == null || isRunning ? moveToPosition : holdPosition.position;
        }
    }
    public void onTakeDamage()
    {
        Damage lastDamage = myHealth.lastDamage;
        if (lastDamage.damageType == DamageType.Molotov)
        {
            //Run away from Molotov
            Vector3 fleeDirection = transform.position - lastDamage.originTransform.position;
            fleeDirection.y = 0;
            
                moveToPosition += fleeDirection.normalized * 2f;
            
            isRunning = true;
            if (lastDamage.originTransform.TryGetComponent<Molotov>(out Molotov molotov))
            {
                molotov.onUseCompleted.AddListener(()=>StopRunning());
            }
        }
    }

    private IEnumerator StopRunningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveToPosition = transform.position;
        isRunning = false;
    }

      private void StopRunning()
    {
        moveToPosition = transform.position;
        isRunning = false;
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

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(moveToPosition, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.green;
        if (!holdPosition)
            return;
        Gizmos.DrawWireSphere(holdPosition.position, fightWithinRange);
    }
}
