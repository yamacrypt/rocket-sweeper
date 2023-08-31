
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IPoolItemOperator : UdonSharpBehaviour
{
    public virtual bool SetActive(GameObject obj,bool active){
        Debug.LogWarning("SetActive is not implemented!");
        return false;
    }

    public virtual bool IsActive(GameObject obj){
        Debug.LogWarning("IsActive is not implemented!");
        return false;
    }

    public virtual bool SetActive(GameObject obj,int instanceID,bool active){
        Debug.LogWarning("SetActive is not implemented!");
        return false;
    }
}
