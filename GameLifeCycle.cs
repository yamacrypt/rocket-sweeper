
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameLifeCycle : GameEventPublisher
{
    bool isStart=false;
    protected bool IsStart=>isStart;
    public virtual bool  IsReadyToGameStart(){
        return !isStart;
    }
    public virtual bool  IsReadyToGameOver(){
        return isStart;
    }
    public virtual void GameStart(Mission mission){
        if(!IsReadyToGameStart()){
            Debug.LogWarning("GameStart is not ready");
        }
        isStart=true;
    }

   

    public virtual void GameOver(){
        if(!IsReadyToGameOver()){
            Debug.LogWarning("GameOver is not ready");
        }
        isStart=false;
    }
}
