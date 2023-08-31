
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GameStart : UdonSharpBehaviour
{
    [SerializeField]IMapGenerator mapGenerator;
    [SerializeField]EnemyGenerator enemyGenerator;
    [SerializeField]ScoreManager scoreManager;
    void Start()
    {

    }
    bool isStart=false;
    public override void Interact(){
        scoreManager.StartMeasure();
        if(!isStart){
            mapGenerator.GenerateInit();
            if(!Networking.LocalPlayer.IsOwner(gameObject))return;
            if(enemyGenerator!=null)enemyGenerator.SpawnInterval();
        }
        isStart=true;
    }
}
