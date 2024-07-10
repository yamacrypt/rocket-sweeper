
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class InyQueueSync : UdonSharpBehaviour
{
    [UdonSynced]int[] syncedIndexes= new int[50]; 
    int[] indexes= new int[50]; 
    
    public override void OnDeserialization()
    {
        for(int i=0;i<syncedIndexes.Length;i++){
            indexes[i]=syncedIndexes[i];
        }
    }
}
