
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PoliceMusketeer : PoliceBase
{
      [SerializeField] private Transform musketHolderTransform;
   [SerializeField] private  GameObject musketPrefab;
    private  Musket musket;

    private bool aiming=false;
    
    protected override void Awake() {
        base.Awake();

        musket=Instantiate(musketPrefab,musketHolderTransform.position,musketHolderTransform.transform.rotation,musketHolderTransform).GetComponent<Musket>();
        musket.attackRange=attackRange;
        if(attackSpeed<musket.useTime){
            Debug.Log("Attackspeed is lower than musket usetime. Adjusted attackspeed accordingly");
            attackSpeed=musket.useTime;
        }
        musket.onFinishAttack.AddListener(()=>FinishAttack());
    }

    protected override void Update()
    {
        if(aiming&&targetHealth){
            Vector3 targetPosition = targetHealth.transform.position;
            transform.LookAt(new Vector3(targetPosition.x,transform.position.y,targetPosition.z),Vector3.up);
        }
        base.Update();
    }

    protected override void Attack()
    {
        nextAttackTime = Time.time + attackSpeed;
        navMeshAgent.isStopped=true;
        musket.Attack();
        aiming=true;
    }

    private void FinishAttack(){
        navMeshAgent.isStopped=false;
        aiming=false;
    }
}
