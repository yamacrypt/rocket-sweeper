
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class WeaponCollection : UdonSharpBehaviour
{
    public VRC_Pickup[] Guns;
    void Start()
    {
        
    }
}
