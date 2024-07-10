
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Bomber : Enemy
{
    /*string strEnemy="EnemyObject";
    string strBombEnemy="EnemyBomb";
    string strCell="block_prefab";
    void Start()
    {
        strEnemy=RockerExplosion.strEnemy;
        strBombEnemy=RockerExplosion.strBombEnemy;
        strCell=RockerExplosion.strCell;
        breakCells=new GameObject[200];
        enemies=new GameObject[200];
    }
    GameObject[] breakCells;
    GameObject[] enemies;
    float explosionSize;*/
    [SerializeField]IObjectPool explosionPool;
    public void DelayExplosion(int delay){
        
        // TODO: instantiate rocket explosion isntance
        SendCustomEventDelayedFrames(nameof(CreateExplosion),delay);
    }   
    public void CreateExplosion(){
        var explosion=explosionPool.TryToSpawn();
        if(explosion!=null){
            var expComp=explosion.GetComponent<RockerExplosion>();
            expComp.Init(this.transform.position,explosionPool,false);
        }else{
            Debug.LogWarning("explosion is empty");
        }
    }
    /*[SerializeField]IMapGenerator mapGenerator;
    public void Explosion(){
        var cols=Physics.OverlapSphere(transform.position, explosionSize,Physics.IgnoreRaycastLayer);
        int cellIndex=0;
        int enemyCount=0;
        for(int i=0;i<cols.Length;i++)
        {
            var item=cols[i];
            if(item==null)continue;
            var name=item.name;
            if(name.Contains(strCell)){
                breakCells[cellIndex]=item.gameObject;
                cellIndex++;
            }else if(name.Contains(strEnemy)){
                enemies[enemyCount]=item.gameObject;
                enemyCount++;
            }else if(name.Contains(strBombEnemy)){
                enemies[enemyCount]=item.gameObject;
                enemyCount++;
                item.GetComponent<Bomber>().DelayExplosion(i*2+1,explosionSize);
                //TriggerBombEvent(item.transform);
            }
        }
        mapGenerator.BreakCells(breakCells,cellIndex);
    }*/
}
