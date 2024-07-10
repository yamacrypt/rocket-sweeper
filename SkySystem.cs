
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SkySystem : GameLifeCycle
{
    void SetSkyBoxes(Material[] skyboxes){
        skybox=skyboxes;
    }
    Material[] skybox;
    [SerializeField] Material defaultSkybox;
    [SerializeField]float interval=20f;
    public override bool IsReadyToGameStart(){
        return base.IsReadyToGameStart()&&canceled;
    }
    public override void GameOver()
    {
        base.GameOver();
        RenderSettings.skybox = defaultSkybox;
    }
    [SerializeField]Material[] earthSkyboxes;
    [SerializeField]Material[] moonSkyboxes;
    [SerializeField]Material[] marsSkyboxes;
    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        Debug.Log("SkySystem GameStart");
        switch(mission.PlanetType){
            case PlanetType.Earth:
                SetSkyBoxes(earthSkyboxes);
            break;
            case PlanetType.Moon:
                SetSkyBoxes(moonSkyboxes);
            break;
            case PlanetType.Mars:
                SetSkyBoxes(marsSkyboxes);
            break;
            default:
                Debug.LogError("Unknown PlanetType: "+mission.PlanetType);
            break;
        }
        canceled=false;
        index=0;
        SendCustomEventDelayedSeconds(nameof(Change),1);
    }
    
    bool canceled=true;
    int index=0;
    public void Change(){
        if(!IsStart){
            canceled=true;
            return;
        }
        RenderSettings.skybox = skybox[index];
        index++;
        if(index>=skybox.Length){
            canceled=true;
            return;
        }
        SendCustomEventDelayedSeconds(nameof(Change),interval);
    }
}
