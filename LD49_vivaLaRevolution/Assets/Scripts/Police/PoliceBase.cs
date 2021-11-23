
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PoliceBase : RTSUnit
{
    [HideInInspector] public PoliceGroup group;
    [HideInInspector] public Transform holdPosition;
    //Running away from a danger source e.g. molotov
    [HideInInspector] public bool isRunning = false;

    //falling back to holdpoint
    [HideInInspector] public bool isFallingBack = false;
    //advancing to holdpoint
    [HideInInspector] public bool isAdvancing = false;

    protected override void Start()
    {
        moveToPosition = holdPosition.position;
        myHealth.onTakeDamage.AddListener((x) => onTakeDamage());
        base.Start();
    }

    public virtual void SetHoldPosition(Transform newPos)
    {
        targetHealth = null;
        holdPosition = newPos;
        SetMovePosition(newPos.position);

    }
    protected override void Update()
    {
        base.Update();
        if ((isAdvancing || isFallingBack) && group.IsNearCurrentHoldPoint(this))
        {
            isAdvancing = false;
            isFallingBack = false;
        }
        // isRunning = myHealth.HealthRatio()<.2f;
        if (targetHealth == null)
        {
            if (!isRunning && !isFallingBack)
            {
                //Staying at holdPoint, so look for enemies
                 FindNewTarget();
            }

        }

    }
    public void onTakeDamage()
    {
        Damage lastDamage = myHealth.lastDamage;
        if (lastDamage.damageType == DamageType.Molotov && lastDamage.originTransform.TryGetComponent<Molotov>(out Molotov molotov))
        {
            //Run away from Molotov
            Vector3 fleeDirection = transform.position - lastDamage.originTransform.position;
            fleeDirection.y = 0;
            moveToPosition += fleeDirection.normalized * 2f;
            isRunning = true;
            molotov.onUseCompleted.AddListener(() => StopRunning());

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
}
