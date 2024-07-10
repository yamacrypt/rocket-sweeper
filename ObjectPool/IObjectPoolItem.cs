
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IObjectPoolItem : UdonSharpBehaviour
{
    public virtual bool SetActive(bool active,bool forceChange=false){
        Debug.LogError("Need to override SetActive");
        return false;
    }

    // This method does not change activeMode and teleport attached object to initial pos.
    // This is mainly used to return synced pool item by non-owner player 
    public virtual bool _SetActive(bool active){
         Debug.LogError("Need to override _SetActive");
         return false;
    }
}
