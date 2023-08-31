
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Bullet : UdonSharpBehaviour
{
    Rigidbody _rg;
    protected Rigidbody rg{
        get{
            if(_rg==null)_rg = this.GetComponent<Rigidbody>();
            return _rg;
        }
    }
    IObjectPool bulletPool;
    
    float velMag;

    protected bool isOwner=false;
    public virtual void Init(
        GunController gc,Vector3 velocity,Vector3 position, IObjectPool bulletPool){
        this.bulletPool=bulletPool;
        isOwner=true;
        if(gc==null)return;
        this.gc=gc;
        rg.MovePosition(position);
        rg.velocity = velocity ;
        timer=0f;
        SendCustomEventDelayedSeconds(nameof(ReturnToPool),thresholdTime);
        ////Debug.Log("bullet Init");
    }

    float baseDuration=0.6f;

    float timer=-1f;
    [SerializeField]float thresholdTime=2f;
    public void ReturnToPool(){
        if(isOwner){
            isOwner=false;
            timer=-1f;
            //rg.MovePosition(this.transform.position + Vector3.down*100f);
            SendCustomEventDelayedFrames(nameof(Return),2);
        }
    }

    public void Return(){
        Debug.Log("bulelt Return");
       bulletPool.Return(this.gameObject);
    }

    GunController gc;
    int penetration=0;
    string GunTarget="GunTarget";
    string Stage="Stage";
    string BulletStage="BulletStage";

    [SerializeField]IObjectPool explosionPool;
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Bullet Enter "+ col.name);
        _OnTriggerEnter(col);
    }
    protected virtual void _OnTriggerEnter(Collider col)
    {
        if(!isOwner)return;
        isOwner=false;
        var explosion=explosionPool.TryToSpawn();
        if(explosion!=null){
            explosion.transform.position = this.transform.position;
            var expComp=explosion.GetComponent<RockerExplosion>();
            expComp.Init(rg.velocity.normalized);
        }else{
            Debug.LogError("explosion is empty");
        }

        Return();
    }


    bool TryAttack(Enemy enemy){
        if(enemy == null || !enemy.isAlive || enemy.IsInPool){
            return false;
        }
        ////Debug.Log("TryAttack: count"+enemy.name);
        if(gc==null || !isOwner)return false;
        return true;
    }
}
