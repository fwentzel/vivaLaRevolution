
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    [SerializeField] private GameObject protestorPrefab;

    [SerializeField] private float magnitudeMulitplicator;
    protected Vector3 moveToPosition;
    private NavMeshAgent navMeshAgent;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        moveToPosition = transform.position;

    }

    private void OnEnable()
    {
        StartCoroutine(MoveRandomly());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Protestor>(out Protestor protestor))
        {
            GameObject spawnedProtestor = Instantiate(protestorPrefab, transform.position, transform.rotation,ProtestorManager.instance.protestorParent);
            if (RTSSelection.instance.selectedUnits.Contains(protestor))
            {
                //Add new Protestor to Selection aswell
                RTSSelection.instance.AddToSelection(spawnedProtestor.GetComponent<Protestor>());
            }
            Destroy(gameObject);
        }
    }

    protected IEnumerator MoveRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            if (Vector3.Distance(moveToPosition, transform.position) < 2f)
            {
                moveToPosition += new Vector3(Random.Range(-magnitudeMulitplicator, magnitudeMulitplicator), 0, Random.Range(-magnitudeMulitplicator, magnitudeMulitplicator));
                navMeshAgent.SetDestination(moveToPosition);
            }

        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(moveToPosition, 1f);

    }

}
