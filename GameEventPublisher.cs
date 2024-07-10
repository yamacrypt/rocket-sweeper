
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameEventPublisher : UdonSharpBehaviour
{
     public virtual void GameStartCallback(){

    }

    public virtual void GameOverCallback(){

    }

}
