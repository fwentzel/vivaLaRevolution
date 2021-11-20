
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RTSUnit : MonoBehaviour
{
    [Header("Attackcking")]
    public int attackDamage = 5;
    public float attackSpeed = 1;
    public float attackRange = 2f;
    [Header("Enemy detection")]
    public LayerMask enemyLayer;
    public float detectRadius = 4f;
    public float fightWithinRange = 5f;
    public float magnitudeMulitplicator = 5f;
    [Header("Selection")]
    public Transform moveTarget;
    public Transform meshTransform;
    public UnityEvent onSelection;
    public UnityEvent onDeselection;
    protected Vector3 moveToPosition;
    protected NavMeshAgent navMeshAgent;
    protected Health targetHealth;
    protected Health myHealth;
    protected float nextAttackTime = 0;
    protected Collider[] colliders;
    protected Vector3 initialScale;
    protected Vector3 initialScaleMesh;
    protected bool doRandomly = true;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        colliders = new Collider[] { GetComponent<Collider>() };
        myHealth = GetComponent<Health>();
        moveToPosition = transform.position;
        initialScaleMesh = meshTransform.localScale;
        initialScale = transform.localScale;

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

    public virtual void OnKill()
    {
        meshTransform.DOKill();
        transform.DOKill();
        Destroy(gameObject);
    }


    public float GetRemainingDistance()
    {
        return navMeshAgent.remainingDistance;
    }

    protected virtual void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, detectRadius, enemyLayer);

        if (targetHealth != null && navMeshAgent.enabled)
        {
            if (Vector3.Distance(targetHealth.transform.position, transform.position) < attackRange)
            {
                if (Time.time > nextAttackTime)
                {
                    Attack();
                }
            }
        }
    }

    protected virtual void Attack()
    {
        targetHealth.takeDamage(new Damage(DamageType.Meele, attackDamage, transform));
        nextAttackTime = Time.time + attackSpeed;
        transform.DOScale(initialScale * 1.2f, 0.1f).OnComplete(() =>
       {
           if (transform != null)
           {
               transform.DOScale(initialScale, 0.3f);
           }
       });
    }

    public virtual void SetMovePosition(Vector3 newPosition)
    {
        //decide wether it will listen to Order

        if (Random.Range(0, .5f) > myHealth.HealthRatio())
        {
            return;
        }
        moveToPosition = newPosition;       

    }
    protected IEnumerator MoveRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            if (Vector3.Distance(moveToPosition, transform.position) < 2f)
            {
                float magnitude = (1 - myHealth.HealthRatio()) * magnitudeMulitplicator;
                if (doRandomly)
                    moveToPosition += new Vector3(Random.Range(-magnitude, magnitude), 0, Random.Range(-magnitude, magnitude));
            }

        }
    }


}
