
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncedCounter : UdonSharpBehaviour
{
    [UdonSynced]long _count;
    public long Count{
        get{
            return _count;
        }
        private set{
            _count=value;
            RequestSerialization();
        }
    }

    // only called by counterUnit
    public void CountUp(long number){
        Count+=number;
    }

    public void Reset(){
        Count=0;
    }

}
