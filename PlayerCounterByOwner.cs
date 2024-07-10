
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PlayerCounterByOwner : GameLifeCycle
{
     public int PlayerCount=>playerCount;
    public void CountUpPlayerCount(){
        playerCount++;
        Debug.Log("playerCount: "+playerCount);
    }
    public void CountUpGameOverCount(){
        gameoverCount++;
        Debug.Log("gameoverCount: "+gameoverCount);
        if(gameoverCount==playerCount){
            gameoverCount=0;
            playerCount=0;
            GameOverToAll();
        }
    }

    public void GameOverToAll(){
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(GameOverSync));
    }
    public void GameOverSync(){
        scoreResultAnimation.SyncReadyEnd=true;
        Debug.Log("GameOverSync");
        gameoverSyncMessenger.Publish(null,GameMessage.GameOver);
    }
    [SerializeField]IMessenger gameoverSyncMessenger;
    [SerializeField]ScoreResultAnimation scoreResultAnimation;
    int playerCount,gameoverCount;

    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(CountUpPlayerCount));
    }

    public override void GameOver()
    {
        base.GameOver();
        SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(CountUpGameOverCount));
    }
}
