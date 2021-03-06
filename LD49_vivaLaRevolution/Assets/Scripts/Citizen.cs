
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
            GameObject spawnedProtestorObject = Instantiate(protestorPrefab, transform.position, transform.rotation, ProtestorManager.instance.protestorParent);
            Protestor spawnedProtestor = spawnedProtestorObject.GetComponent<Protestor>();
            spawnedProtestor.SetMovePosition(protestor.moveToPosition);
            if (RTSSelection.instance.selectedUnits.Contains(protestor))
            {
                //Add new Protestor to Selection aswell
                RTSSelection.instance.AddToSelection(spawnedProtestor);
            }
            Destroy(gameObject);
        }
    }

    protected IEnumerator MoveRandomly()
    {
        NavMeshHit hit;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            if (Vector3.Distance(moveToPosition, transform.position) < 2f)
            {
                moveToPosition += new Vector3(Random.Range(-magnitudeMulitplicator, magnitudeMulitplicator), 0, Random.Range(-magnitudeMulitplicator, magnitudeMulitplicator));
                if (NavMesh.SamplePosition(moveToPosition, out hit, magnitudeMulitplicator * 2, NavMesh.AllAreas))
                {
                    moveToPosition = hit.position;
                }
                else
                {
                    //Reset movePosition for next iteration, stop movepositon from drifitng away
                    moveToPosition = transform.position;
                }
                navMeshAgent.SetDestination(hit.position);
            }

        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(moveToPosition, 1f);

    }

}
