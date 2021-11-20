
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
        if (attackSpeed < musket.useTime)
        {
            Debug.Log("Attackspeed is lower than musket usetime. Adjusted attackspeed accordingly");
            attackSpeed = musket.useTime;
        }
        musket.onFinishAttack.AddListener(() => FinishAttack());
    }

    protected override void Update()
    {
        
        if (targetHealth)
        {
            Quaternion neededRotation = Quaternion.LookRotation(targetHealth.transform.position - transform.position,Vector3.up);

           transform.rotation= Quaternion.RotateTowards(transform.rotation, neededRotation, turnspeed);
        }
        base.Update();
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
