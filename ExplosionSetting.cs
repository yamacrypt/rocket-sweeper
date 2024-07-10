
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ExplosionSetting : GameLifeCycle
{
    public float ExplosionRangeMultiplier;
    public float ExplosionPowerMultiplier;
    public float FireRateMultiplier;
    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        ExplosionPowerMultiplier=mission.ExplosionPowerMultiplier;
        ExplosionRangeMultiplier=mission.ExplosionRangeMultiplier;
        FireRateMultiplier=mission.FireRateMultiplier;
    }

    public override void GameOver()
    {
        base.GameOver();
        ExplosionPowerMultiplier=1;
        ExplosionRangeMultiplier=1;
        FireRateMultiplier=1;
    }
}
