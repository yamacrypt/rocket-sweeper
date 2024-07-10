
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncedBullet : Bullet
{
    //IObjectPool bulletSyncedPool;

    [UdonSynced] private Vector3 _syncedVelocity;
    [UdonSynced] private Vector3 _syncedPosition;
    [UdonSynced] private Quaternion _syncedRotation;
    public override void OnDeserialization() { 
        //Debug.Log("OnDeserialization"+_syncedPosition+" "+_syncedVelocity);
        rg.MovePosition(_syncedPosition);
        rg.velocity = _syncedVelocity;
        transform.localRotation=_syncedRotation;
        isOwner=false;
        //float eliminationTime=2f*(1f+_syncedBlackPower*0.1f);
        //SendCustomEventDelayedSeconds(nameof(ReturnToPool),eliminationTime);
    }
    //float velMag=1f;

    public override void Init(GunController gc,Vector3 velocity,Vector3 position,Quaternion rotation, IObjectPool bulletPool){
        base.Init(gc,velocity,position,rotation,bulletPool);
        isOwner=true;
        _syncedVelocity=rg.velocity;
        _syncedPosition=position;
        _syncedRotation=rotation;
        RequestSerialization();
    }

    //float baseDuration=1f;


}
