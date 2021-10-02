
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;

public class RTSUnit : MonoBehaviour
{
    public float attackSpeed = 1;

    public int attackDamage = 5;

    public float detectRadius = 4f;
    public float attackRange = 2f;
    public float fightWithinRange = 5f;

    [SerializeField]
    protected Vector3 moveToPosition;
    protected NavMeshAgent navMeshAgent;

    protected Health targetHealth;
    protected float nextAttackTime = 0;


    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    protected virtual void Start()
    {

    }


    protected virtual void Update()
    {
        if(Vector3.Distance(transform.position,moveToPosition)>fightWithinRange)
        targetHealth=null;
        
        if (targetHealth != null )
        {
            navMeshAgent.destination = targetHealth.transform.position;
            if (Vector3.Distance(targetHealth.transform.position, transform.position) < attackRange)
            {
                if (Time.time > nextAttackTime)
                {
                    Attack();
                }
            }
        }

    }

    private void Attack()
    {
        targetHealth.takeDamage(attackDamage);
        nextAttackTime = Time.time + attackSpeed;
        transform.DOScale(Vector3.one * 1.3f, 0.1f).OnComplete(() => transform.DOScale(Vector3.one, 0.3f));
    }

    public void SetMovePosition(Vector3 newPosition)
    {
        moveToPosition = newPosition;
    }



}
