
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EnemyGenerator : GameLifeCycle
{
    IObjectPool enemyPool;
    public void Return(GameObject[] obj,int length){
        if(enemyPool!=null)enemyPool.Return(obj,length);
    }
    float interval=0.2f;
    void Start()
    {
        //SpawnInterval();
    }
    bool isSpawning=false;

    public void SpawnInterval(){
        if(!IsStart){
            isSpawning=false;
            return;
        }
        Spawn();
        SendCustomEventDelayedSeconds(nameof(SpawnInterval),interval);
    }
    [SerializeField]int fallThreshold=3;
    int fallCount=0;
    public void Fall(){
        fallCount++;
        if(fallCount%fallThreshold==0){
            Spawn();
        }
    }
    public void Spawn(){
        if(isSpawning)enemyPool.TryToSpawn();
    }
    public override bool IsReadyToGameStart(){
        return base.IsReadyToGameStart()&&!isSpawning;
    }
    public override void GameOver(){
        base.GameOver();
        //if(Networking.LocalPlayer.IsOwner(this.gameObject)){
            enemyPool.Clear();
        //}
    }
    public override void GameStart(Mission mission){
        base.GameStart(mission);
        enemyPool=mission.EnemyPool;
        this.interval=mission.SpawnInterval;
        fallCount=0;
        enemyPool.Clear();
        enemyPool.Shuffle();
        //if(Networking.LocalPlayer.IsOwner(this.gameObject)){
            isSpawning=true;
        SendCustomEventDelayedSeconds(nameof(SpawnInterval),interval);
        //}
    }
}
