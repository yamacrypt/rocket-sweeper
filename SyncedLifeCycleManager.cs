
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncedLifeCycleManager : LifeCycleEventHandler
{
    public GameLifeCycle[] LifeCycles;
    [SerializeField]DetectPlayers detectPlayers;
    [SerializeField]SyncedLifeCycleUnit[] syncedLifeCycleUnits;
    void Start()
    {
        var id=Networking.LocalPlayer.playerId;
        Debug.Log("playerID: "+id);
        if(id>=0&&id<syncedLifeCycleUnits.Length){
            syncedLifeCycleUnits[id].SetOwner(Networking.LocalPlayer);
        }
    }
    public override void Publish(GameObject self,GameMessage message){
        MissionInfo info;
        switch(message){
            case GameMessage.GameStart:
                info=self.GetComponent<MissionInfo>();
                if(MissionStart(info.Mission)&&self!=null){
                    var pub=self.GetComponent<GameEventPublisher>();
                    if(pub)pub.GameStartCallback();
                }
            break; 
            case GameMessage.GameOver:
                if(MissionOver()&&self!=null){
                    var pub=self.GetComponent<GameEventPublisher>();
                    if(pub)pub.GameOverCallback();
                }
            break;
            /*case GameMessage.TimeOver:
                if(MissionOver()&&self!=null){
                    var pub=self.GetComponent<GameEventPublisher>();
                    if(pub)pub.GameOverCallback();
                }
            break;*/
            default:
                Debug.LogError("Unknown GameMessage: "+message);
            break;
        }
    }

    public override bool  IsReadyToGameStart(){
        if(Networking.LocalPlayer.IsOwner(this.gameObject)){
            var indexes=detectPlayers.PlayerIDs();
            if(indexes.Length==0)return false;
            foreach(var index in indexes){
                if(!syncedLifeCycleUnits[index].IsReadyToGameStart)return false;
            }
            joinPlayers=indexes;
        } else{
            if(!detectPlayers.IsJoined())return false;
            foreach(var cycle in LifeCycles){
                if(!cycle.IsReadyToGameStart())return false;
            }
        }
        return true;
    }
    int[] joinPlayers;
    bool MissionStart(Mission mission){
        if(mission==null)return false ;
        if(!IsReadyToGameStart()){
            Debug.LogWarning("Not Ready To GameStart");
            return false;
        }
        Debug.Log("Synced GameStart: "+mission.PlanetType);
        foreach(var lifeCycle in LifeCycles){
            lifeCycle.GameStart(mission);
        }
        return true;
    }
    public override bool  IsReadyToGameOver(){
        /*foreach(var index in joinPlayers){
            if(!syncedLifeCycleUnits[index].IsReadyToGameOver)return false;
        }*/
        foreach(var cycle in LifeCycles){
            if(!cycle.IsReadyToGameOver())return false;
        }
        return true;
    }
    bool MissionOver(){
        if(!IsReadyToGameOver())return false;
        foreach(var lifeCycle in LifeCycles){
            lifeCycle.GameOver();
        }
        return true;

    }
}
