
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class LifeCycleEventHandler : IMessenger
{
    public virtual bool IsReadyToGameStart(){
        Debug.LogError("IsReadyToGameStart is not implemented");
        return false;
    }  

    public virtual bool IsReadyToGameOver(){
        Debug.LogError("IsReadyToGameOver is not implemented");
        return false;
    }    

    
}
