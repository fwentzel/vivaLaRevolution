
using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    public Sprite icon;

    public UnityEvent onUseCompleted;
    public UnityEvent onUse;

    public LayerMask obstacleLayer;

    private float maxDistance = 50;

    public virtual void Use(Vector3 position)
    {
        if(!transform)
            return;
        
        // Correct for Distance
        Vector3 difference = position - transform.position;
        if (difference.magnitude > maxDistance)
        {
            position = transform.position + difference.normalized * maxDistance;
        }
        
        //Correct for Hits
        RaycastHit hit;
        difference = position - transform.position;
        if (Physics.Raycast(transform.position, difference, out hit, difference.magnitude, obstacleLayer))
        {
            position = hit.point;
        }


        
        transform.parent = null;
        
        onUse?.Invoke();
        transform.DOMove(position, 0.3f).OnComplete(UseCompleted);
    }
    
    public virtual void UseCompleted()
    {
        onUseCompleted?.Invoke();
        
        Destroy(gameObject);
    }
}
