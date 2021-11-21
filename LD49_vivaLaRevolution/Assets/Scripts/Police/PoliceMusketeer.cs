
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PoliceMusketeer : PoliceBase
{
    [Header("Musket Fields")]
    [SerializeField] private Transform musketHolderTransform;
    [SerializeField] private GameObject musketPrefab;
    [SerializeField] private float turnspeed = 1;
    private Musket musket;

    private bool aiming = false;

    protected override void Awake()
    {
        base.Awake();

        musket = Instantiate(musketPrefab, musketHolderTransform.position, musketHolderTransform.transform.rotation, musketHolderTransform).GetComponent<Musket>();
        musket.attackRange = attackRange;
        if (attackSpeed < musket.useTime*2)
        {
            Debug.Log("Attackspeed is lower than double musket usetime. Adjusted attackspeed accordingly");
            attackSpeed = musket.useTime*2;
        }
        musket.onFinishAttack.AddListener(() => FinishAttack());
    }

    protected override void Update()
    {
        base.Update();

        if (targetHealth)
        {
            Quaternion neededRotation = Quaternion.LookRotation(targetHealth.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, neededRotation, turnspeed);
        }
        else
        {
            if (isRunning)
            {
                navMeshAgent.destination = moveToPosition;
            }
            else
            {
                navMeshAgent.destination = holdPosition.position;
            }
        }

    }
    public override void SetHoldPosition(Transform newPos)
    {
        musket.FinishAttack();
        base.SetHoldPosition(newPos);
    }

    protected override void Attack()
    {
        nextAttackTime = Time.time + attackSpeed;
        navMeshAgent.isStopped = true;
        musket.Attack();
        aiming = true;
    }

    private void FinishAttack()
    {
        navMeshAgent.isStopped = false;
        aiming = false;
    }
}
