
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MegaPhone : GameLifeCycle
{
    [SerializeField]float nearRange=1000;
    public override void GameStart(Mission mission){
        base.GameStart(mission);
        SetVoiceNear(nearRange);
    }

    public override void GameOver(){
        base.GameOver();
        SetVoiceNear(0);
    }

    public void SetVoiceNear(float range){
        Networking.LocalPlayer.SetVoiceDistanceNear(range);
    }
    void Start()
    {
        
    }
}
