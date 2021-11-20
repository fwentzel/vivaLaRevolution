
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;

public class Musket : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int damage;
    public int useTime;
    [SerializeField] private LayerMask enemyLayer;

    [Header("VFX")]
    [SerializeField] private ParticleSystem onFireParticles;
    [SerializeField] private Transform meshTransform;
    [HideInInspector] public float attackRange;
     [HideInInspector] public UnityEvent onFinishAttack;
    public void Attack()
    {
        StartAttackProcess();

    }
    private void Start() {
      meshTransform.DOLocalRotate(new Vector3(-90,0,0),2f);   
    }
    private void StartAttackProcess()
    {
         meshTransform.DOKill();
        TakeAim();
    }

    private void TakeAim()
    {
       
        meshTransform.DOLocalRotate(new Vector3(0,0,0),useTime).OnComplete(()=>Fire());
    }

    private void Update() {
       Debug.DrawRay(transform.position,Vector3.forward,Color.green,.1f);
    }
    
  
        private void Fire()
    {
        if(onFireParticles){
            onFireParticles.Play();
        }
        RaycastHit hit;
       if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit,attackRange,enemyLayer)){
            Health health = hit.transform.GetComponent<Health>();
            health.takeDamage(new Damage(DamageType.Ranged,damage,transform));
       }
       FinishAttack();
    }


    public void FinishAttack()
    {
        onFinishAttack?.Invoke();
        meshTransform.DOKill();
        meshTransform.DOLocalRotate(new Vector3(-90,0,0),useTime);
    }



    private void OnDestroy()
    {
        meshTransform.DOKill();
    }

    private void OnDrawGizmos() {
        Gizmos.color=Color.red;
        Gizmos.DrawRay(transform.position,transform.TransformDirection(Vector3.forward)*attackRange);
    }


}
