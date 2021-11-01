using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class HealItem : Item
{
    public ParticleSystem flameParticles;
    public ParticleSystem healParticles;
    public LayerMask healLayer;
    public int duration = 10;
    public float ticksPerSecond = 2;
    public int healPerTick = 5;

    public float timeToRotate = 10;
    private void Start()
    {
        flameParticles.Play();
    }
    public override void UseCompleted()
    {
        healParticles.transform.DORotate(new Vector3(0, 360, 0), timeToRotate, RotateMode.WorldAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
        healParticles.Play();

        EffectAudioManager.instance.PlayHealingClip(transform.position);

        StartCoroutine(HealOverTime());

    }

    private IEnumerator HealOverTime()
    {
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, influenceRadius, healLayer);
            foreach (var collider in colliders)
            {
                if (collider.transform.TryGetComponent(out Health health))
                {
                    health.heal(healPerTick);
                }
            }
            yield return new WaitForSeconds(1 / ticksPerSecond);
        }

        DoBaseCompleted();

    }

    public void DoBaseCompleted()
    {
        healParticles.transform.DOKill();
       
        base.UseCompleted();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, influenceRadius);
    }
}
