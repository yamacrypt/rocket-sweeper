
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EnemyGenerator : UdonSharpBehaviour
{
    [SerializeField]SyncedObjectPool enemyPool;
    [SerializeField]float interval=0.2f;
    void Start()
    {
        enemyPool.Shuffle();
        //SpawnInterval();
    }

    public void SpawnInterval(){
        Spawn();
        SendCustomEventDelayedSeconds(nameof(SpawnInterval),interval);
    }
    [SerializeField]LocalPlayerFollow follower;

    public void Spawn(){
        var enemy = enemyPool.TryToSpawn();
        if(enemy==null){
            return;
        }
    }
}
