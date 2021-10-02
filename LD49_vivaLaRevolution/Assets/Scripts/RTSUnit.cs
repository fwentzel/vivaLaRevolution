
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
    public float uneasyness = 0.5f;

    public UnityEvent onSelection;
    public UnityEvent onDeselection;

    public Transform target;

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

    protected virtual void Update()
    {

        if (target)
            target.position = moveToPosition;

        if (Vector3.Distance(transform.position, moveToPosition) > fightWithinRange)
            targetHealth = null;

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
        moveToPosition = newPosition;
    }


    protected IEnumerator MoveRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            if (Vector3.Distance(moveToPosition, transform.position) < 2f)
            {
                float magnitude = uneasyness;
                moveToPosition += new Vector3(Random.Range(-magnitude, magnitude), 0, Random.Range(-magnitude, magnitude));
            }

        }
    }


}
