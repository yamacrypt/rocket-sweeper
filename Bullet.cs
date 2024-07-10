
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
    [SerializeField]IObjectPool explosionPool;

    protected bool isOwner=false;
    public virtual void Init(
        GunController gc,Vector3 velocity,Vector3 position,Quaternion rotation, IObjectPool bulletPool){
        this.bulletPool=bulletPool;
        isOwner=true;
        if(gc==null)return;
        this.gc=gc;
        rg.MovePosition(position);
        rg.velocity = velocity ;
        transform.localRotation=rotation;
        timer=0f;
        SendCustomEventDelayedSeconds(nameof(ReturnToPool),thresholdTime);
        ////Debug.Log("bullet Init");
    }

    float baseDuration=0.6f;

    float timer=-1f;
    [SerializeField]float thresholdTime=3f;
    public void ReturnToPool(){
        if(isOwner){
            isOwner=false;
            timer=-1f;
            //rg.MovePosition(this.transform.position + Vector3.down*100f);
            SendCustomEventDelayedFrames(nameof(Return),2);
        }
    }

    public void Return(){
        //Debug.Log("bulelt Return");
        isOwner=false;
       bulletPool.Return(this.gameObject);
    }

    GunController gc;
    int penetration=0;
    string GunTarget="GunTarget";
    string Stage="Stage";
    string BulletStage="BulletStage";

    private void OnTriggerEnter(Collider col)
    {
        //Debug.Log("Bullet Enter "+ col.name);
        _OnTriggerEnter(col);
    }
    public void _OnEnable(){
        
    }
    public void _OnDisable(){
        isOwner=false;
        bulletPool=null;
    }
    protected virtual void _OnTriggerEnter(Collider col)
    {
        var explosion=explosionPool.TryToSpawn();
        if(explosion!=null){
            var expComp=explosion.GetComponent<RockerExplosion>();
            expComp.Init(this.transform.position,explosionPool,isOwner);
        }else{
            Debug.LogWarning("explosion is empty");
        }
        if(isOwner)Return();
    }
}
