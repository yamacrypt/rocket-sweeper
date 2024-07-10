
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LightSetting : GameLifeCycle
{
    [SerializeField]Light light;
    float initialIntensity;
    void Start()
    {
        initialIntensity=light.intensity;
    }

    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        light.intensity=mission.LightIntensity;
    }

    public override void GameOver()
    {
        base.GameOver();
        light.intensity=initialIntensity;
    }
}
