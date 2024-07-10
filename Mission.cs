
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Mission : UdonSharpBehaviour
{
    [SerializeField]IObjectPool enemyPool;
    [SerializeField]float gravity;
    [SerializeField]string planetName;
    [SerializeField]string missionName;
    Mission nextMission;
    public void SetNextMission(Mission mission){
        nextMission=mission;
    }
    
    [SerializeField]UniTranslateText explainText;
    [SerializeField]PlanetType planetType;
    [SerializeField]float explosionRangeMultiplier=1;
    [SerializeField]float explosionPowerMultiplier=1;
    [SerializeField]float fireRateMultiplier=1;    
    [SerializeField]float spawnInterval=0.15f; 
    [SerializeField]int targetScore=10000;
    [SerializeField]float waterPercentage=0.2f;
    [SerializeField]float lightIntensity=0.02f;
    [SerializeField]Sprite enemySprite;
    [SerializeField]UniTranslateText memoText;

    public Sprite EnemySprite=>enemySprite;
    public IObjectPool EnemyPool=>enemyPool;
    public float LightIntensity=>lightIntensity;

    public float Gravity=>gravity;
    public string PlanetName=>planetName;
    public string MissionName=>missionName;
    public string ExplainText=>explainText.GetText();
    public string MemoText=>memoText.GetText();
    public float WaterPercentage=>waterPercentage;
    public int TargetScore=>targetScore;
    public PlanetType PlanetType=>planetType;
    public float ExplosionRangeMultiplier=>explosionRangeMultiplier;
    public float ExplosionPowerMultiplier=>explosionPowerMultiplier;
    public float FireRateMultiplier=>fireRateMultiplier;
    public float SpawnInterval=>spawnInterval;
    public bool Locked=true;
    public long HighScore{
        get{
            return highScore;
        }
        set{
            highScore=value;
            if(highScore>=targetScore){
                if(nextMission!=null)nextMission.Locked=false;
                missionManager.UpdateMissions();
            }
            Notify();
        }
    }
    [SerializeField]MissionUIManager missionUIManager;
    [SerializeField]MissionManager missionManager;
    void Notify(){
        if(missionUIManager!=null){
            missionUIManager.Show(this);
        }
    }
    long highScore=0;

}
