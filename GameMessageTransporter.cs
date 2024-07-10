
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum GameMessage{
    GameOver,
    LocalGameOver,
    //TimeOver,
    EnemyDeath,
    Damage,
    GameStart,
}
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameMessageTransporter : IMessenger
{
    [SerializeField]IMessenger parent;

    public override void Publish(GameObject self,GameMessage message){
        parent.Publish(self,message);
    }

}
