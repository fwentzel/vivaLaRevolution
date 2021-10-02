
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Collections;

public class Protestor : RTSUnit
{

    public Building buildingToLoot;
    bool isLooting = false;
    public float lootTime = 10;


    private Coroutine enterCoroutine;
    

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
            if(navMeshAgent.enabled)
                navMeshAgent.destination = moveToPosition;
        }
    }

    public void TryEnterBuilding(Building building)
    {
        if(!gameObject.activeSelf)
            return;
        
        
        if(building != this.buildingToLoot && enterCoroutine!=null)
            StopCoroutine(enterCoroutine);
        
        
        
        enterCoroutine = StartCoroutine(EnterBuilding(building));
    }

    public void LeaveBuilding()
    {
        buildingToLoot = null;
        if(enterCoroutine != null)
            StopCoroutine(enterCoroutine);
        enterCoroutine = null;
        transform.DOScale(Vector3.one,.5f).OnComplete(()=>navMeshAgent.enabled = true);
        gameObject.SetActive(true);
        
        
    }

    private IEnumerator EnterBuilding(Building toBuilding)
    {
        moveToPosition = toBuilding.transform.position;
        navMeshAgent.destination = moveToPosition;
        
        while (true && toBuilding.CanEnter())
        {
            yield return new WaitForSeconds(0.3f);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1.1f );
            foreach (var collider in colliders)
            {
                if (collider.transform.TryGetComponent(out Building foundBuilding))
                {
                    if (foundBuilding == toBuilding)
                    {
                        navMeshAgent.enabled = false;
                        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
                        {
                            gameObject.SetActive(false);
                            toBuilding.EnterBuilding(this);
                            buildingToLoot = toBuilding;
                        });
                        yield break;
                    }
                }
            }
        }
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
