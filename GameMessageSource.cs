
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameMessageSource : IMessenger
{
    [SerializeField]Transform MenuPoint;
    [SerializeField]AudioSource beatSE;
    public override void Publish(IMessenger self,GameMessage message){
        switch(message){
            case GameMessage.EnemyDeath:
            break;
            case GameMessage.GameOver:
            beatSE.Play();
            SendCustomEventDelayedSeconds(nameof(GameOver),1);
            break; 
            case GameMessage.Damage:
            break;
        }
    }

    public void GameOver(){
            Networking.LocalPlayer.TeleportTo(MenuPoint.position,MenuPoint.rotation);
    }
}
