
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SyncedRockerExplosion : RockerExplosion
{
    [UdonSynced] private Vector3 _syncedPosition;
    public override void OnDeserialization() { 
        //Debug.Log("OnDeserialization"+_syncedPosition+" "+_syncedVelocity);
        this.transform.position=_syncedPosition;
        isOwner=false;
        //float eliminationTime=2f*(1f+_syncedBlackPower*0.1f);
        //SendCustomEventDelayedSeconds(nameof(ReturnToPool),eliminationTime);
    }
    //float velMag=1f;

    public override void Init(Vector3 position,IObjectPool explosionPool,bool score=true){
        base.Init(position,explosionPool,score);
        isOwner=true;
        _syncedPosition=position;
        RequestSerialization();
    }
}
