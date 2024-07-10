
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MissionMessageSource : LifeCycleEventHandler
{
    public GameLifeCycle[] lifeCycles;
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
            /*case GameMessage.SelectMission:
                if(!IsReadyToGameStart())return;
                info=self.GetComponent<MissionInfo>();
                missionUIManager.Show(info.Mission);
                var frame=self.GetComponent<MissionSelector>();
                if(frame!=null){
                    frame.Highlight();
                }
            break;*/
            default:
                Debug.LogError("Unknown GameMessage: "+message);
            break;
        }
    }

    public override bool IsReadyToGameStart(){
        foreach(var lifeCycle in lifeCycles){
            if(!lifeCycle.IsReadyToGameStart()){
                Debug.Log("GameStart is not ready");
                return false;
            }
        }
        return true;
    }

    public override bool IsReadyToGameOver(){
        foreach(var lifeCycle in lifeCycles){
            if(!lifeCycle.IsReadyToGameOver()){
                Debug.Log("GameOver is not ready");
                return false;
            }
        }
        return true;
    }
    bool MissionStart(Mission mission){
        if(mission==null)return false ;
        if(!IsReadyToGameStart())return false;
        Debug.Log("GameStart: "+mission.PlanetType);
        foreach(var lifeCycle in lifeCycles){
            lifeCycle.GameStart(mission);
        }
        return true;
    }

    bool MissionOver(){
        if(!IsReadyToGameOver())return false;
        foreach(var lifeCycle in lifeCycles){
            lifeCycle.GameOver();
        }
        Debug.Log("MissionOver");
        return true;
    }
}
