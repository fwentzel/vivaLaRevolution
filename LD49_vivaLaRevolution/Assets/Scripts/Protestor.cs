
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Collections;

public class Protestor : RTSUnit
{

    public Transform moveTarget;

    [Header("Items")]
    public Item item;
    public Transform itemHold;
    public MiscItem miscItem;
    public Transform miscItemHold;

    [Header("Building")]
    public float lootTime = 10;
    private Building buildingToLoot;
    private Coroutine enterCoroutine;
    private Coroutine moveRandomlyCoroutine;


    protected override void Update()
    {
        if (Vector3.Distance(transform.position, moveToPosition) > fightWithinRange)
            targetHealth = null;

        base.Update();

        if (moveTarget)
            moveTarget.position = moveToPosition;

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
        else
        {
            navMeshAgent.destination = targetHealth.transform.position;
        }
    }

    public override void SetMovePosition(Vector3 newPosition)
    {
        base.SetMovePosition(newPosition);
        if (moveTarget)
        {
            moveTarget.transform.DOKill();
            moveTarget.transform.localScale = Vector3.one;
            moveTarget.transform.DOScale(Vector3.zero, 1);
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
        transform.DOMove(buildingToLoot.entryPoint.position, .5f);
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
        moveToPosition = toBuilding.entryPoint.position;
        if (!navMeshAgent.enabled)
            yield break;

        navMeshAgent.destination = moveToPosition;

        while (toBuilding.CanEnter())
        {
            yield return new WaitForSeconds(0.1f);
            if (Vector3.Distance(toBuilding.entryPoint.position, transform.position) < 2f)
            {
                navMeshAgent.enabled = false;
                if (toBuilding.protestors.Count == 0)
                    EffectAudioManager.instance.PlayWindowClip(transform.position);
                transform.DOMove(toBuilding.transform.position, 1f);
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

        ItemManager.instance.AddToList(this);


        return true;
    }
    public bool GiveMiscItem(MiscItem miscItem)
    {
        if (!miscItem)
            return false;
        if (!CanGiveMiscItem())
            return false;

        this.miscItem = miscItem;
        miscItem.holderNavmeshAgent = navMeshAgent;

        miscItem.onUse.AddListener(() => this.miscItem = null);
        miscItem.transform.SetParent(miscItemHold);
        miscItem.transform.DOLocalMove(Vector3.zero, 0.3f);
        miscItem.transform.DOLocalRotate(Vector3.zero, 0.3f);

        return true;
    }
    public override void OnKill()
    {
        moveTarget.DOKill();
        if (item)
        {
            ItemManager.instance.RemoveItemFromList(item);
        }
        RTSSelection.instance.RemoveFromSelection(this);
        ProtestorManager.instance.OnProtestorDeath();
        base.OnKill();
    }
    public bool CanGiveItem()
    {
        return item == null;
    }

    public bool CanGiveMiscItem()
    {
        return miscItem == null;
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
