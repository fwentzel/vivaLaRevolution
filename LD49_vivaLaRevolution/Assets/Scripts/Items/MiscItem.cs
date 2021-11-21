
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class MiscItem : MonoBehaviour
{
    [Header("Pick up")]
    [Range(0, 100)]
    [SerializeField] protected int pickupProbability = 10;
    [SerializeField] protected SphereCollider pickUpCollider;

    public UnityEvent onUse;
    [HideInInspector] public NavMeshAgent holderNavmeshAgent;
    protected Rigidbody rb;

    protected virtual void Awake()
    {
        pickUpCollider.enabled = true;
        rb = GetComponent<Rigidbody>();
    }
    protected virtual void Use()
    {
        onUse?.Invoke();
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
    protected void ReactivatePickUp()
    {
        pickUpCollider.enabled = true;
    }
    protected virtual void PickUp(Protestor protestor)
    {
        protestor.GiveMiscItem(this);
        rb.useGravity = false;
        rb.isKinematic = true;
        pickUpCollider.enabled = false;
    }

}
