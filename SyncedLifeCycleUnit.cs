
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncedLifeCycleUnit : UdonSharpBehaviour
{
    [UdonSynced]bool isReadyToGameStart;
    [UdonSynced]bool isReadyToGameOver;
    public bool IsReadyToGameStart=>isReadyToGameStart;
    public bool IsReadyToGameOver=>isReadyToGameOver;
    bool isOwner=false;
    public void SetOwner(VRCPlayerApi player){
        Networking.SetOwner(player,gameObject);
        isOwner=true;
        CheckInterval();
    }
    [SerializeField]SyncedLifeCycleManager manager;
    [SerializeField]float syncInterval=1;
    

    public void Reset(){
        if(isOwner){
            isReadyToGameOver=false;
            isReadyToGameStart=false;
            RequestSerialization();
        }
    }

    public void CheckInterval(){
        if(isOwner){
            isReadyToGameStart=CheckIsReadyToGameStart();
            isReadyToGameOver=CheckIsReadyToGameOver();
            RequestSerialization();
        }
        SendCustomEventDelayedSeconds(nameof(CheckInterval),syncInterval);
    }
  
    /*public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        if(isOwner){
            cancel=true;
            RequestSerialization();
        }
    }

    public override void GameOver()
    {
        base.GameOver();
        if(isOwner){
            cancel=false;
            CheckInterval();
        }
    }*/


    bool CheckIsReadyToGameStart(){
        foreach(var lifeCycle in manager.LifeCycles){
            if(!lifeCycle.IsReadyToGameStart()){
                return false;
            }
        }
        return true;
    }

    bool CheckIsReadyToGameOver(){
        foreach(var lifeCycle in manager.LifeCycles){
            if(!lifeCycle.IsReadyToGameOver()){
                return false;
            }
        }
        return true;
    }
}
