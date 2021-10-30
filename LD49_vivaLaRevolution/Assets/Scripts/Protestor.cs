
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Collections;

public class Protestor : RTSUnit
{

    public Building buildingToLoot;
    public float lootTime = 10;

    public Item item;
    public Transform itemHold;


    private Coroutine enterCoroutine;
    private Coroutine moveRandomlyCoroutine;


    protected override void Update()
    {
        if (Vector3.Distance(transform.position, moveToPosition) > fightWithinRange)
            targetHealth = null;
        base.Update();

        if (targetHealth == null)
        {
            foreach (Collider collider in colliders)
            {
                targetHealth = collider.GetComponent<Health>();
                break;
            }
            if (navMeshAgent.enabled)
                navMeshAgent.destination = moveToPosition;
        }
    }

    public void TryEnterBuilding(Building building)
    {
        if (!gameObject.activeSelf)
            return;

        if (buildingToLoot == building)
            return;
        if (enterCoroutine != null)
            StopCoroutine(enterCoroutine);


        doRandomly = false;
        enterCoroutine = StartCoroutine(EnterBuilding(building));
    }

    public void LeaveBuilding()
    {
        buildingToLoot = null;
        if (enterCoroutine != null)
            StopCoroutine(enterCoroutine);
        enterCoroutine = null;
        transform.DOScale(initialScale, .5f).OnComplete(() => navMeshAgent.enabled = true);
        gameObject.SetActive(true);

        doRandomly = true;
    }

    private IEnumerator EnterBuilding(Building toBuilding)
    {
        moveToPosition = toBuilding.transform.position;
        if (!navMeshAgent.enabled)
            yield break;

        navMeshAgent.destination = moveToPosition;

        while (toBuilding.CanEnter())
        {
            yield return new WaitForSeconds(0.3f);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
            foreach (var collider in colliders)
            {
                if (collider.transform.TryGetComponent(out Building foundBuilding))
                {
                    if (foundBuilding == toBuilding)
                    {
                        navMeshAgent.enabled = false;
                        if (toBuilding.protestors.Count == 0)
                            EffectAudioManager.instance.PlayWindowClip(transform.position);
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

    public bool GiveItem(Item item)
    {
        if (!item)
            return false;
        if (!CanGiveItem())
            return false;

        this.item = item;
        item.onUse.AddListener(() => this.item = null);
        Vector3 scaleBeforeParent = item.transform.localScale;
        item.transform.SetParent(itemHold);
        item.transform.DOLocalMove(Vector3.zero, 0.3f);
        item.transform.localScale = scaleBeforeParent;

        if(RTSSelection.instance.selectedUnits.Contains(this)){
            ItemManager.instance.AddToList(this);
        }

        return true;
    }
    public override void OnKill()
    {
        ProtestorManager.instance.OnProtestorDeath();
        base.OnKill();
    }
    public bool CanGiveItem()
    {
        return item == null;
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
