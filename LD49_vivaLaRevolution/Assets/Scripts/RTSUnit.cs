
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RTSUnit : MonoBehaviour
{
    public float attackSpeed = 1;

    public int attackDamage = 5;

    public float detectRadius = 4f;
    public float attackRange = 2f;
    public float fightWithinRange = 5f;

    public float magnitudeMulitplicator = 5f;

    public LayerMask enemyDetection;


    public UnityEvent onSelection;
    public UnityEvent onDeselection;

    public Transform target;


    protected Vector3 moveToPosition;
    protected NavMeshAgent navMeshAgent;

    protected Health targetHealth;
    protected Health myHealth;
    protected float nextAttackTime = 0;
    protected Collider[] colliders;
    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        colliders = new Collider[] { GetComponent<Collider>() };
        myHealth = GetComponent<Health>();
        moveToPosition = transform.position;
    }
    protected virtual void Start()
    {
        SetMovePosition(transform.position);
    }


    private void OnEnable()
    {

        StartCoroutine(MoveRandomly());
    }

    public virtual void OnSelection()
    {
        onSelection?.Invoke();
    }

    public virtual void OnDeselection()
    {
        onDeselection?.Invoke();
    }

    public float GetRemainingDistance()
    {
        return navMeshAgent.remainingDistance;
    }

    protected virtual void FixedUpdate()
    {
        colliders = Physics.OverlapSphere(transform.position, detectRadius, enemyDetection);


        if (target)
            target.position = moveToPosition;


        if (targetHealth != null && navMeshAgent.enabled)
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
        //decide wether it will listen to Order

        if (Random.Range(0, .5f) > myHealth.HealthRatio())
        {
            print("NÖÖ");
            return;
        }

        moveToPosition = newPosition;
        if (target)
        {
            target.transform.localScale = Vector3.one;
            target.transform.DOScale(Vector3.zero, 1);
        }

    }


    protected IEnumerator MoveRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            if (Vector3.Distance(moveToPosition, transform.position) < 2f)
            {
                float magnitude = (1-myHealth.HealthRatio()) * magnitudeMulitplicator;
              
                    moveToPosition += new Vector3(Random.Range(-magnitude, magnitude), 0, Random.Range(-magnitude, magnitude));
                

            }

        }
    }


}
