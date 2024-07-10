
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class MissionUpDownOperator : UdonSharpBehaviour
{
    [SerializeField]MissionManager missionManager;
    [SerializeField]bool up=true;
    void Start()
    {
        
    }

    public override void Interact()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DoInteract));
    }

    public void DoInteract(){
        if(up){
            missionManager.UpMissions();
        } else {
            missionManager.DownMissions();
        }
    }
}
