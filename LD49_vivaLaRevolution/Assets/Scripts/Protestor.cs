
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Collections;

public class Protestor : RTSUnit
{

    public Building buildingToLoot;
    bool isLooting = false;
    public float lootTime = 10;

    protected override void Update()
    {
        base.Update();

        if (buildingToLoot != null && !isLooting && navMeshAgent.remainingDistance < 0.6f)
        {
            isLooting = true;
            navMeshAgent.isStopped = true;
            StartCoroutine(Loot());
        }

        if (targetHealth == null)
        {
            foreach (Collider collider in Physics.OverlapSphere(transform.position, detectRadius))
            {
                if (collider.tag.Equals("Police"))
                {
                    targetHealth = collider.GetComponent<Health>();
                    break;
                }
            }
            navMeshAgent.destination = moveToPosition;
        }
    }

    public void GoLoot(Building building)
    {
        buildingToLoot = building;
        moveToPosition = building.transform.position;
    }

    private IEnumerator Loot()
    {
        Vector3 startPos = transform.position;
        float endTime = Time.time + lootTime;
       transform.DOScale(Vector3.zero,1);
        yield return new WaitForSeconds(lootTime);
        transform.DOScale(Vector3.one,.5f);
        transform.position=startPos;
        navMeshAgent.isStopped = false;
        buildingToLoot = null;
        isLooting = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(moveToPosition, fightWithinRange);

    }


}
