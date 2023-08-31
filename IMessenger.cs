
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IMessenger : UdonSharpBehaviour
{
    public  virtual void Publish(IMessenger self,GameMessage message){
    }
}
