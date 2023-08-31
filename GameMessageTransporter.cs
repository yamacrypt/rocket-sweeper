
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum GameMessage{
    GameOver,
    EnemyDeath,
    Damage
}
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameMessageTransporter : IMessenger
{
    [SerializeField]IMessenger parent;

    public override void Publish(IMessenger self,GameMessage message){
        parent.Publish(self,message);
    }

}
