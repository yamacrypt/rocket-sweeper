
using System;
using BuildSoft.UdonSharp.Collection;
using Pokeyi.UdonSharp;
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
    //[SerializeField]LocalObjectPool bulletPool;
    [SerializeField]SyncedObjectPool bulletSyncedPool;
    /*[UdonSynced]Vector3 gunPos;
    [UdonSynced]Quaternion gunRot;

    public override void OnDeserialization()
    {
        rg.MovePosition(gunPos);
        rg.MoveRotation(gunRot);
    }


    public void SyncTransform(){
        if(player.IsYourGun(this)){
            gunPos=this.transform.position;
            gunRot=this.transform.rotation;
            RequestSerialization();
        }
    }*/
    [SerializeField]Player player;
    Rigidbody rg;
    void Start()
    {
        if(bulletSyncedPool==null){
            Debug.Log("WARNING: bulletSyncedPool is null");
        }
        rg=this.GetComponent<Rigidbody>();
    }


    [SerializeField]AudioSource shootAudioSource;

    void OnDisable()
    {
    }
    
    // requestされた側が処理するわけでもなさそう
    /*public override bool OnOwnershipRequest(VRC.SDKBase.VRCPlayerApi requestingPlayer, VRC.SDKBase.VRCPlayerApi requestedOwner){
        if(player.IsYourGun(this)){
            return true;
        }
        return false;
    }*/

    float ThresholdFireTime => 1.0f / FireRate();
    float fireRateMultiplier=1f;
    public void SetFireRateMultiplier(float f){
        fireRateMultiplier=f;
    }
    [SerializeField]ExplosionSetting explosionSetting;
    float FireRate(){
        return baseFireRate*fireRateMultiplier*explosionSetting.FireRateMultiplier;
    }

    float deltaTime=0f;
    float isStuckingTime=0f;
    [SerializeField]LineRenderer lineRenderer;
    [SerializeField]MeshRenderer rocketHead;
    void Update()
    {
        lineRenderer.SetPosition(0,bulletSource.position);
        lineRenderer.SetPosition(1,bulletSource.position+getVelDir()*30);   
        //SyncTransform();
    }
    bool isOwner=false;
    void FixedUpdate()
    {
        if(!isOwner)return;
        deltaTime-=Time.deltaTime;
        if(deltaTime<=0){
            rocketHead.enabled=true;
        }
        if(isPickingUp&&onTrigger){
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
    [SerializeField]P_HapticsProfile p_HapticesProfile;
    [SerializeField]GameSpeedManager gameSpeedManager;
    void Fire(){
        if(shootAudioSource!=null){
            shootAudioSource.Stop();
            shootAudioSource.Play();
        }
        rocketHead.enabled=false;
        p_HapticesProfile._TriggerHaptics();
        var syncedBullet = bulletSyncedPool!=null? bulletSyncedPool.TryToSpawn() :null;
        if(syncedBullet!=null){
            var bc=syncedBullet.GetComponent<SyncedBullet>();
            if(bc!=null){
                isStacking=false;
                bc.Init(this,getVelDir()*velocityMag*gameSpeedManager.GameSpeed,bulletSource.position,rotTarget.transform.rotation,bulletSyncedPool);
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
        onTrigger=true;
    }

    [SerializeField]float baseFireRate=1f;
    [SerializeField]Transform bulletSource;

    bool isPickingUp=false;

    public override void Interact(){
        //Debug.Log("OnInteract");
    }
    public void TransferOwnerShip(VRCPlayerApi player){
        Networking.SetOwner(player,this.gameObject);
        SendCustomEventDelayedSeconds(nameof(ClearSyncPool),10f);
        if(bulletSyncedPool!=null && !player.IsOwner(bulletSyncedPool.gameObject)){
            Networking.SetOwner(player,bulletSyncedPool.gameObject);
            foreach(var obj in bulletSyncedPool.Pool){
               Networking.SetOwner(player,obj.gameObject);
            }
        }
    }
    public override void OnPickup(){
        if(!player.IsYourGun(this)){
            GetComponent<VRC_Pickup>().Drop();
            return;
        }else if(player.IsEquipGun(this)){
            return;
        }
        //TransferOwnerShip(Networking.LocalPlayer);
        isOwner=Networking.LocalPlayer.IsOwner(this.gameObject);
        //this.transform.SetParent(player.transform);
        //Debug.Log("OnPickup: "+isOwner);
        player.Equip();
        isPickingUp=true;
        deltaTime=0f;
        

    }
    //[SerializeField]IObjectPool explosionSyncedPool;

    //[SerializeField]DamageSyncByOne damageSyncByOne;

    public void ClearSyncPool(){
        if(bulletSyncedPool!=null)bulletSyncedPool.Clear();
        //if(explosionSyncedPool!=null)explosionSyncedPool.Clear();
    }

    /*public void SetBulletSyncPool(SyncedObjectPool pool){
        bulletSyncedPool=pool;
    }*/

    public override void OnDrop(){
        /*if(player.IsEquipGun(this)){
            player.UnEquip();
        }*/
        Debug.Log("OnDrop");
        isPickingUp=false;
        onTrigger=false;
        player.UnEquip();
        /*foreach(var holder in itemHolders){
            holder.OnDropCallback();
        }*/
    }
    public override void OnPickupUseUp(){
        onTrigger=false;
        //deltaTime=0f;
    }

}
