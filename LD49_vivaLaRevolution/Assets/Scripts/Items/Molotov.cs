using System;
using System.Collections;
using UnityEngine;

public class Molotov : Item
{
    [Header("Action")]
    [SerializeField] private LayerMask enemyLayer;
    public int duration = 10;
    public float ticksPerSecond = 2;
    public int damagePerTick = 5;

    [Header("VFX")]
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem burnDamageParticles;
    [SerializeField] private ParticleSystem flameParticles;

    private void Start()
    {
        flameParticles.Play();
    }
    public override void UseCompleted()
    {
        explosionParticles.Play();

        EffectAudioManager.instance.PlayMolotovClip(transform.position);

        StartCoroutine(DealDamageOverTime());
    }

    private IEnumerator DealDamageOverTime()
    {
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            burnDamageParticles.Play();
            Collider[] colliders = Physics.OverlapSphere(transform.position, influenceRadius, enemyLayer);
            foreach (var collider in colliders)
            {
                if (collider.transform.TryGetComponent(out Health health))
                {
                    health.takeDamage(new Damage(DamageType.Molotov, damagePerTick, transform));
                }
            }
            yield return new WaitForSeconds(1 / ticksPerSecond);
        }


        DoBaseCompleted();

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
