
using mmmsys;
using TMPro;
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameMessageSource : LifeCycleEventHandler
{
    //[SerializeField]AudioSource beatSE;

    [SerializeField]GameLifeCycle[] lifeCycles;
    [SerializeField]LifeCycleEventHandler[] otherMessageSources;
    [SerializeField]TextMeshProUGUI deathReasonView;
    [SerializeField]MissionUIManager missionUIManager;
    public override void Publish(GameObject self,GameMessage message){
        switch(message){
            case GameMessage.EnemyDeath:
            break;
            case GameMessage.GameOver:
                if(GameOver(self)){
                    missionUIManager.IsReady=true;
                }
            break;
            /*case GameMessage.TimeOver:
                if(TimeOver(self)){
                    missionUIManager.IsReady=true;
                }
            break; */
            case GameMessage.GameStart:
                var info=self.GetComponent<MissionInfo>();
                if(GameStart(info.Mission)&&self!=null){
                    gameoverSyncMessenger.Publish(self,message);
                    var pub=self.GetComponent<GameEventPublisher>();
                    if(pub)pub.GameStartCallback();
                    missionUIManager.IsReady=false;
                }
            break;
            case GameMessage.Damage:
            break;

            default:
                Debug.LogError("Unknown GameMessage: "+message);
            break;
        }
    }

    [SerializeField]MissionMessageSource gameoverSyncMessenger;
   
    [SerializeField]Transform missionView;

    public override bool IsReadyToGameStart(){
        foreach(var lifeCycle in lifeCycles){
            if(!lifeCycle.IsReadyToGameStart()){
                Debug.Log("GameStart is not ready");
                return false;
            }
        }
        return true;
    }
    [SerializeField]GameObject headLight;
    public bool GameStart(Mission mission){
        if(mission==null)return false;
        if(!IsReadyToGameStart())return false;
        /*foreach(var source in otherMessageSources){
            foreach(var lifeCycle in source.lifeCycles){
                if(!lifeCycle.IsReadyToGameStart()){
                    Debug.Log("GameStart is not ready");
                    return;
                }
            }
        }*/
        foreach(var lifeCycle in gameoverSyncMessenger.lifeCycles){
            if(!lifeCycle.IsReadyToGameStart()){
                Debug.Log("GameStart is not ready");
                return false;
            }
        }
        Debug.Log("GameStart: "+mission.PlanetType);
        missionView.gameObject.SetActive(false);
        headLight.SetActive(true);
        startSwitch.Interact();
        foreach(var renderer in shouldDisableRenderers){
            renderer.enabled=false;
        }
        foreach(var lifeCycle in lifeCycles){
            lifeCycle.GameStart(mission);
        }
        return true;
        //Networking.LocalPlayer.TeleportTo(playerSpawnPoint.position,playerSpawnPoint.rotation);
        /*player.Reset();
        scoreManager.Reset();
        mapGenerator.GenerateInit();
        if(!Networking.LocalPlayer.IsOwner(gameObject))return;
        if(enemyGenerator!=null)enemyGenerator.SpawnInterval();*/
    }
    [SerializeField]FT_TeleportSwitch teleportSwitch;
    [SerializeField]FT_TeleportSwitch startSwitch;

    public override bool IsReadyToGameOver()
    {
        foreach(var lifeCycle in lifeCycles){
            if(!lifeCycle.IsReadyToGameOver()){
                Debug.Log("GameOver is not ready");
                return false;
            }
        }
        /*foreach(var lifeCycle in gameoverSyncMessenger.lifeCycles){
            if(!lifeCycle.IsReadyToGameOver()){
                Debug.Log("GameOver is not ready");
                return false;
            }
        }*/
        return true;
    }
    [SerializeField]Renderer[] shouldDisableRenderers;
    bool _GameOver(GameObject messenger){
        
        /*foreach(var source in otherMessageSources){
            foreach(var lifeCycle in source.lifeCycles){
                if(!lifeCycle.IsReadyToGameOver()){
                    Debug.Log("GameOver is not ready");
                    return false;
                }
            }
        }*/
        if(!IsReadyToGameOver())return false;
        foreach(var lifeCycle in lifeCycles){
            lifeCycle.GameOver();
        }
        DeathReason deathReason=messenger.GetComponent<DeathReason>();
        deathReasonView.text=deathReason.Reason;
        headLight.SetActive(false);
        foreach(var renderer in shouldDisableRenderers){
            renderer.enabled=true;
        }
        Debug.Log("GameOver");
        return true;

    }
    public bool GameOver(GameObject messenger){
        if(!_GameOver(messenger))return false;
        foreach(var otherMessenger in otherMessageSources){
            otherMessenger.Publish(messenger,GameMessage.GameOver);
        }
        /*player.GameOver();
        scoreManager.GameOver();*/
        //beatSE.Play();
        //Teleport();
        return true;
    }
   /* public bool TimeOver(GameObject messenger){
        if(!_GameOver(messenger))return false;
        foreach(var otherMessenger in otherMessageSources){
            otherMessenger.Publish(messenger,GameMessage.TimeOver);
        }
        return true;
  
    }*/
    bool isGameOver=false;
    /*public void Teleport(){
        teleportSwitch.Interact();
        //Networking.LocalPlayer.TeleportTo(MenuPoint.position,MenuPoint.rotation);
    }*/
}
