
using UnityEngine;

using DG.Tweening;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    public Sprite icon;

    public UnityEvent onUseCompleted;
    public UnityEvent onUse;

    public LayerMask obstacleLayer;

    public virtual void Use(Vector3 position)
    {
        if(!transform)
            return;
        
        
        
        
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
