
using UnityEngine;
using UnityEngine.AI;

public class RTSUnit : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    [SerializeField]
    private Vector3 moveToPosition;
    private bool isPolice = false;
    private Health targetHealth;
    public float attackSpeed = 1;
    private float nextAttackTime = 0;
    public int attackDamage = 5;

    public float detectRadius = 4f;
    public float attackRange = 2f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        isPolice = tag.Equals("Police");
    }
    void Start()
    {

    }


    void Update()
    {
        if (targetHealth != null)
        {
            navMeshAgent.destination = targetHealth.transform.position;
            if (Vector3.Distance(targetHealth.transform.position, transform.position) < attackRange)
            {
                if (Time.time > nextAttackTime)
                {
                    targetHealth.takeDamage(attackDamage);
                    nextAttackTime = Time.time + attackSpeed;
                }
            }
        }
        else
        {
            foreach (Collider collider in Physics.OverlapSphere(transform.position, detectRadius))
            {
                if (collider.tag.Equals(isPolice ? "Protestor" : "Police"))
                {
                    targetHealth = collider.GetComponent<Health>();
                    break;
                }
            }
            if (moveToPosition != Vector3.zero)
                navMeshAgent.destination = moveToPosition;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }

}
