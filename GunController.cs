
using System;
using BuildSoft.UdonSharp.Collection;
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
//[RequireComponent(typeof(WeaponList),typeof(EnemyList))]
public class GunController : UdonSharpBehaviour
{
    [SerializeField]LocalObjectPool bulletPool;
    [SerializeField]SyncedObjectPool bulletSyncedPool;

    [SerializeField]Player player;

    void Start()
    {
        if(bulletSyncedPool==null){
            Debug.Log("WARNING: bulletSyncedPool is null");
        }
    }


    [SerializeField]AudioSource shootAudioSource;

    void OnDisable()
    {
    }

    public override bool OnOwnershipRequest(VRC.SDKBase.VRCPlayerApi requestingPlayer, VRC.SDKBase.VRCPlayerApi requestedOwner){
        player.UnTap();
        return true;
    }

    float ThresholdFireTime => 1.0f / FireRate();

    float FireRate(){
        float multi=0f;
        return baseFireRate;
    }

    float deltaTime=0f;
    float isStuckingTime=0f;
    [SerializeField]LineRenderer lineRenderer;
    void Update()
    {
        lineRenderer.SetPosition(0,bulletSource.position);
        lineRenderer.SetPosition(1,bulletSource.position+getVelDir()*30);   
    }
    void FixedUpdate()
    {
        deltaTime-=Time.deltaTime;
        if(isPickingUp && onTrigger){
            if(deltaTime<=0){
                deltaTime=ThresholdFireTime;
                if(player.IsEquipGun(this))Fire();
            }
        }
        /*if(isStacking){
            isStuckingTime+=Time.deltaTime;
            if(isStuckingTime>5){
                isStacking=false;
                isStuckingTime=0;
                if(bulletSyncedPool!=null)bulletSyncedPool.Clear();
            }
        } else{
            isStuckingTime=0;
        }*/
    }
    [SerializeField]UnrolledMapGenerator mapGenerator;

    public void RequestResearch(){
        mapGenerator.RequestResearch();
    }
    [SerializeField]float recoilPower=4f;
    void Fire(){
        if(shootAudioSource!=null){
            shootAudioSource.Stop();
            shootAudioSource.Play();
        }
        var syncedBullet = bulletSyncedPool!=null? bulletSyncedPool.TryToSpawn() :null;
        if(syncedBullet!=null){
            var bc=syncedBullet.GetComponent<SyncedBullet>();
            if(bc!=null){
                isStacking=false;
                bc.transform.localRotation=rotTarget.transform.rotation;
                bc.Init(this,getVelDir()*velocityMag,bulletSource.position,bulletSyncedPool);
                Networking.LocalPlayer.SetVelocity(getVelDir()*-1f*recoilPower+Networking.LocalPlayer.GetVelocity());
                SendCustomEventDelayedSeconds(nameof(RequestResearch),1.5f);
                /*Ray ray = new Ray( bulletSource.position, getVel() );
                //Debug.DrawRay(ray.origin, ray.direction * 30, Color.red, 5.0f); 
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit,Mathf.Infinity,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore)) // もしRayを投射して何らかのコライダーに衝突したら
                {
                    string name = hit.collider.gameObject.name; // 衝突した相手オブジェクトの名前を取得
                    //Debug.Log(name); // コンソールに表示[]
                    //if(name.Contains(MapGenerator.IndestructibleTag)){
                    if(name.Contains("Enemy")){
                        Enemy enemy=hit.collider.gameObject.GetComponent<Enemy>();
                        if(enemy!=null){
                            enemy.TakeDamage(5);
                        }
                    }else{
                        mapGenerator.BreakCell(hit.collider.gameObject);
                    }
                    //}
                }*/
            }
        } else{
            Debug.LogError("bullet is empty");
        }
    }

    [SerializeField]Transform rotStart;

    [SerializeField]Transform bulletDest;
    [SerializeField]float velocityMag=20f;
    Vector3 getVelDir(){
        return (bulletDest.position - bulletSource.position).normalized;
    }

    [SerializeField]GameObject rotTarget;
    Quaternion getRot(){
        Vector3 from = rotStart.position - bulletSource.position;
        Vector3 to = bulletDest.position - bulletSource.position;

        Quaternion rotation = Quaternion.FromToRotation(from, to);
        return rotation;
    }


    bool isStacking=false;

  
    

    bool onTrigger=false;

    public override void OnPickupUseDown(){
        if(!player.IsEquipGun(this)){
            return;
        }
        //deltaTime=0f;
        onTrigger=!onTrigger;
        if(!Networking.LocalPlayer.IsOwner(this.gameObject)){
            Networking.SetOwner(Networking.LocalPlayer,this.gameObject);
        }
    }

    [SerializeField]float baseFireRate=1f;
    [SerializeField]Transform bulletSource;

    bool isPickingUp=false;

    public override void Interact(){
        //Debug.Log("OnInteract");
    }
    public override void OnPickup(){
        if(player.IsEquipGun(this)){
            return;
        }
        this.transform.SetParent(player.transform);
        foreach(var holder in itemHolders){
            holder.OnPickUpCallback();
        }
        Debug.Log("OnPickup");
        player.Equip(this);
        isPickingUp=true;
        deltaTime=0f;
        if(!Networking.LocalPlayer.IsOwner(this.gameObject)){
            Networking.SetOwner(Networking.LocalPlayer,this.gameObject);
            SendCustomEventDelayedSeconds(nameof(ClearSyncPool),10f);
        }
        if(bulletSyncedPool!=null && !Networking.LocalPlayer.IsOwner(bulletSyncedPool.gameObject)){
            Networking.SetOwner(Networking.LocalPlayer,bulletSyncedPool.gameObject);
            foreach(var obj in bulletSyncedPool.Pool){
               Networking.SetOwner(Networking.LocalPlayer,obj.gameObject);
            }
        }
        if(damageSyncByOne!=null)damageSyncByOne.TransferOwnership(Networking.LocalPlayer);
        else {
            Debug.Log("WARNING: damageSyncByOne is null");
        }

    }

    [SerializeField]DamageSyncByOne damageSyncByOne;

    public void ClearSyncPool(){
        if(bulletSyncedPool!=null)bulletSyncedPool.Clear();
    }

    /*public void SetBulletSyncPool(SyncedObjectPool pool){
        bulletSyncedPool=pool;
    }*/

    public override void OnDrop(){
        if(player.IsEquipGun(this)){
            player.UnEquip();
        }
        Debug.Log("OnDrop");
        isPickingUp=false;
        onTrigger=false;
        foreach(var holder in itemHolders){
            holder.OnDropCallback();
        }
    }
    [SerializeField]ItemHolder[] itemHolders;
    public override void OnPickupUseUp(){
        onTrigger=false;
        //deltaTime=0f;
    }

}
