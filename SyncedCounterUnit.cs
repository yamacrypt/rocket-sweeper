
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncedCounterUnit : UdonSharpBehaviour
{
    [UdonSynced]long count;
    long _count=0;
    [SerializeField]float syncInterval=1;
    [SerializeField]SyncedCounter counter;
    public override void OnDeserialization()
    {
        //Debug.Log("OnDeserialization SyncedCounterUnit");
        if(Networking.LocalPlayer.IsOwner(counter.gameObject)){
           counter.CountUp(count);
        }
    }
    public override void OnPreSerialization()
    {
        base.OnPreSerialization();
        count=_count;
        _count=0;
    }
    public void Reset(){
        _count=0;
        RequestSerialization();
    }

    public void CountUp(long number){
        if(Networking.LocalPlayer.IsOwner(counter.gameObject)){
            counter.CountUp(number);
        }else{
            _count+=number;
            RequestSerialization();
        }
    }
    /*void Start()
    {
        SyncCount();
    }
    void SyncCount(){
        if(_count!=0){
            count=_count;
            _count=0;
            RequestSerialization();
        }
        SendCustomEventDelayedSeconds(nameof(SyncCount),syncInterval);
    }*/
}
