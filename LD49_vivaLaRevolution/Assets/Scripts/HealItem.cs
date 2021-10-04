using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Item
{
    public ParticleSystem explosionParticles;
    public LayerMask healLayer;
    public int heal = 100;

    public override void UseCompleted()
    {
        explosionParticles.Play();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, influenceRadius,healLayer);
        foreach (var collider in colliders)
        {
            if (collider.transform.TryGetComponent(out Health health))
            {
                health.heal(heal);
            }
        }

    

        
        explosionParticles.Play();
        
        Invoke("DoBaseCompleted",explosionParticles.main.duration);
    }

    public void DoBaseCompleted()
    {
        
        base.UseCompleted();
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,influenceRadius);
    }
}
