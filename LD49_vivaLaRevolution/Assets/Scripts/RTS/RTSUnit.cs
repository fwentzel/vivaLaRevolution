
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class RTSUnit : MonoBehaviour
{
    [Header("Attackcking")]
    public int attackDamage = 5;
    public float attackSpeed = 1;
    public float attackRange = 2f;
    [Header("Enemy detection")]
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    public float detectRadius = 4f;
    public float fightWithinRange = 5f;
    public float magnitudeMulitplicator = 5f;
    [Range(0, 360)]
    public float viewAngle;
    protected List<Transform> visibleTargets = new List<Transform>();

    [Header("Selection")]
    public Transform meshTransform;
    public UnityEvent onSelection;
    public UnityEvent onDeselection;


    public Vector3 moveToPosition { get; protected set; }
    protected NavMeshAgent navMeshAgent;
    protected Health targetHealth;
    protected Health myHealth;
    protected float nextAttackTime = 0;
    protected Collider[] enemysInDetectRange;
    protected Collider[] enemysInAttackRange = new Collider[1];
    protected Vector3 initialScale;
    protected Vector3 initialScaleMesh;
    protected bool doRandomly = true;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemysInDetectRange = new Collider[] { GetComponent<Collider>() };
        myHealth = GetComponent<Health>();
        moveToPosition = transform.position;
        initialScaleMesh = meshTransform.localScale;
        initialScale = transform.localScale;

    }
    protected virtual void Start()
    {
        // SetMovePosition(transform.position);
    }

    protected virtual void Update()
    {
        enemysInDetectRange = Physics.OverlapSphere(transform.position, detectRadius, enemyLayer);
        if (targetHealth != null && navMeshAgent.enabled)
        {
            if (Vector3.Distance(targetHealth.transform.position, transform.position) < attackRange)
            {
                if (Time.time > nextAttackTime)
                {
                    Attack();
                }
            }
            else
            {
                Debug.Log("Looking for target within reach", gameObject);
                //Target is our of reach
                //Try to find new target that is within reach to prevent units not attacking when blocked by other units

                Physics.OverlapSphereNonAlloc(transform.position, attackRange, enemysInAttackRange, enemyLayer);
                if (enemysInAttackRange[0])
                {
                    targetHealth = enemysInAttackRange[0].GetComponent<Health>();
                }

            }
        }
    }


    //https://www.youtube.com/watch?v=rQG9aUWarwE&ab_channel=SebastianLague
    protected void FindVisibleTargets()
    {
        visibleTargets.Clear();
        for (int i = 0; i < enemysInDetectRange.Length; i++)
        {
            Transform target = enemysInDetectRange[i].transform;
            if (CanSeeTarget(target))
            {
                visibleTargets.Add(target);
            }
        }
    }

    protected bool CanSeeTarget(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
        {
            float dstToTarget = Vector3.Distance(transform.position, target.position);
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleLayer))
            {
                return true;
            }
        }
        return false;
    }

    protected virtual void FindNewTarget()
    {
        FindVisibleTargets();
        bool foundTarget = false;
        for (int i = 0; i < visibleTargets.Count; i++)
        {
            if (visibleTargets[i] != null)
            {
                targetHealth = visibleTargets[i].GetComponent<Health>();
                foundTarget = true;
            }
        }
        if (!foundTarget)
        {
            targetHealth = null;
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
        // //decide wether it will listen to Order

        // if (Random.Range(0, .5f) > myHealth.HealthRatio())
        // {
        //     return;
        // }
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

    //For debug Lines https://www.youtube.com/watch?v=rQG9aUWarwE&ab_channel=SebastianLague
    public Vector3 DirFromAngle(float angleInDegrees, bool globalAngle)
    {
        if (!globalAngle)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.DrawLine(transform.position, transform.position + DirFromAngle(-viewAngle / 2, false) * detectRadius);
        Gizmos.DrawLine(transform.position, transform.position + DirFromAngle(viewAngle / 2, false) * detectRadius);
    }

}
