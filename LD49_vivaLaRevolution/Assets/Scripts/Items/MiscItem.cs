
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using System.Collections;
public class MiscItem : MonoBehaviour
{
    public UnityEvent onUse;
    public NavMeshAgent holderNavmeshAgent;

    protected virtual void Use()
    {
        onUse?.Invoke();
    }

}
