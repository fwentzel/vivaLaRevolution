using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : Item
{
    public ParticleSystem explosionParticles;
    public LayerMask enemyLayer;
    public int damage = 100;

    public override void UseCompleted()
    {
        explosionParticles.Play();
        EffectAudioManager.instance.PlayMolotovClip(transform.position);

        Collider[] colliders = Physics.OverlapSphere(transform.position, influenceRadius, enemyLayer);
        foreach (var collider in colliders)
        {
            if (collider.transform.TryGetComponent(out Health health))
            {
                health.takeDamage(damage);
            }
        }

        explosionParticles.Play();

        Invoke("DoBaseCompleted", explosionParticles.main.duration);
    }

    public void DoBaseCompleted()
    {

        base.UseCompleted();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, influenceRadius);
    }
}
