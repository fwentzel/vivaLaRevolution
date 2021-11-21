
using UnityEngine;

using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MiscStone : MiscItem
{

    [Header("Throw settings")]
    [SerializeField] private SphereCollider rangeCollider;
    [SerializeField] private int maxThrowRange = 15;
    [SerializeField] private int baseForce = 4;
    [SerializeField] private int useTime = 2;

    private Collider[] hits = new Collider[1];
    private bool isUsing = false;

    private LayerMask policeLayer;
    protected override void Awake()
    {
        rangeCollider.enabled = false;
        rangeCollider.radius = maxThrowRange;
        rangeCollider.isTrigger = true;
        policeLayer = LayerMask.GetMask("Police");
    }

    private void FixedUpdate()
    {
        Physics.OverlapSphereNonAlloc(transform.position, maxThrowRange, hits, policeLayer);
        if (rangeCollider.enabled && !isUsing && hits[0])
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




    protected override void PickUp(Protestor protestor)
    {
        rangeCollider.enabled = true;
        base.PickUp(protestor);
    }





}
