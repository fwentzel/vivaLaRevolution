
using UnityEngine;

using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MiscStone : MiscItem
{
    public LayerMask policeLayer;

    [SerializeField]
    SphereCollider pickUpCollider;


    [SerializeField]
    SphereCollider rangeCollider;

    [SerializeField]
    int maxThrowRange = 15;
    [SerializeField]
    int uncertainty = 1;
    [SerializeField]
    [Range(0, 100)]
    int pickupProbability = 10;
    int baseSpeed = 10;
    [SerializeField]
    int baseForce = 4;
    [SerializeField]
    int useTime = 2;

    Collider[] hits = new Collider[1];
    bool isUsing = false;

    Rigidbody rb;
    private void Awake()
    {
        pickUpCollider.enabled = true;
        rangeCollider.enabled = false;
        rangeCollider.radius = maxThrowRange;
        rangeCollider.isTrigger = true;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Physics.OverlapSphereNonAlloc(transform.position, maxThrowRange, hits, policeLayer);
        if (rangeCollider.enabled&&!isUsing && hits[0])
        {
            Use();
        }
    }

    protected override void Use()
    {
        isUsing = true;
        Transform aimTransform = hits[0].transform;
        rangeCollider.enabled = false;
        holderNavmeshAgent.enabled = false;
        transform.DOMoveY(transform.position.y + 5, useTime);
        holderNavmeshAgent.transform.DOLookAt(aimTransform.position, useTime).OnComplete(() => Fly(aimTransform));
        base.Use();
    }

    private void Fly(Transform aimTransform)
    {
        transform.parent = null;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce((aimTransform.position - transform.position + Vector3.up * 3) * baseForce, ForceMode.VelocityChange);
        holderNavmeshAgent.enabled = true;
        Invoke("ReactivatePickUp", 4f);
    }
    private void ReactivatePickUp()
    {
        pickUpCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!pickUpCollider.enabled)
        {
            return;
        }
        if (other.TryGetComponent<Protestor>(out Protestor protestor) &&
        protestor.CanGiveMiscItem() &&
        Random.Range(0, 100) <= pickupProbability)
        {
            PickUp(protestor);
        }
    }

    private void PickUp(Protestor protestor)
    {
        protestor.GiveMiscItem(this);
        rangeCollider.enabled = true;
        rb.useGravity = false;
        rb.isKinematic = true;
        pickUpCollider.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (hits[0])
        {
            Gizmos.DrawWireSphere(hits[0].transform.position, 4f);
        }
    }




}
