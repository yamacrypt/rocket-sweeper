
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RockerExplosion : UdonSharpBehaviour
{
    [SerializeField]IMapGenerator mapGenerator;
    IObjectPool explosionPool;
    /*private void OnTriggerExit(Collider col)
    {
        Debug.Log("col exit"+ col.name);
        mapGenerator.BreakCell(col.gameObject);
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("col enter"+ col.name);
        if(col.name.Contains("Enemy")){
            Enemy enemy=col.gameObject.GetComponent<Enemy>();
            if(enemy!=null){
                enemy.TakeDamage(5);
            }
        }else{
            mapGenerator.BreakCell(col.gameObject);
        }
    }*/
    [SerializeField]SphereCollider col;
     [SerializeField]float power=15f;
    void Start()
    {
        radiusMax=col.radius;
        breakCells=new GameObject[200];
        enemies=new GameObject[200];
    }
    float radiusMax;
    float t=0;
    //[SerializeField]AudioSource explosionSE;
    bool scoring=false;
    public virtual void Init(Vector3 position,IObjectPool explosionPool,bool score=true){
        this.transform.position=position;
        this.explosionPool=explosionPool;
        isOwner=true;
        scoring=score;
    }
    public void _OnEnable()
    {
        //BecomeBiggerInterval();
        playerTriggerDone=false;
        //explosionSE.Stop();
        //explosionSE.Play();
        SendCustomEventDelayedFrames(nameof(BreakCells),2);
        SendCustomEventDelayedSeconds(nameof(Return),returnTime/Mathf.Sqrt(gameSpeedManager.GameSpeed));
    }
    protected bool isOwner=false;
    public void _OnDisable(){
        playerTriggerDone=true;
        isOwner=false;
        scoring=false;
    }
    [SerializeField]float triggerInterval=0.1f;
    bool playerTriggerDone=false;
    [SerializeField]GameSpeedManager gameSpeedManager;
    [SerializeField]ExplosionSetting setting;
    
    public void CheckOnPlayerTriggerInInterval(){
        if(playerTriggerDone)return;
        var distance=Vector3.Distance(this.transform.position,Networking.LocalPlayer.GetPosition());
        if(distance<explosionSize/2){
           Networking.LocalPlayer.SetVelocity(Vector3.up*power*Mathf.Sqrt(gameSpeedManager.GameSpeed)*setting.ExplosionPowerMultiplier);
           playerTriggerDone=true;
        }
        else if(distance<explosionSize){
            var playerVel=(this.transform.position-Networking.LocalPlayer.GetPosition()).normalized;
            //playerVel.y=-Mathf.Abs(playerVel.y);
            Networking.LocalPlayer.SetVelocity(playerVel*-1f*power*Mathf.Sqrt(gameSpeedManager.GameSpeed)*setting.ExplosionPowerMultiplier);
            playerTriggerDone=true;
        }else{
            SendCustomEventDelayedSeconds(nameof(CheckOnPlayerTriggerInInterval),triggerInterval);
        }
    }
    [SerializeField]float returnTime=2f;
    [SerializeField]float explosionSize=3f;
    public const string strEnemy="EnemyObject";
    public const string strBombEnemy="EnemyBomb";
    public const string strCell="block_prefab";
    GameObject[] breakCells;
    GameObject[] enemies;
    [SerializeField]ScoreManager scoreManager;
    [SerializeField]EnemyGenerator enemyGenerator;
    public void BreakCells(){
        //var mask=1<<LayerMask.NameToLayer("Enemy") + 1<<LayerMask.NameToLayer("Cell");
        var mask=1<<30;
        var cols=Physics.OverlapSphere(this.transform.position, explosionSize*setting.ExplosionRangeMultiplier,mask);
        int cellIndex=0;
        int enemyCount=0;
        int delay=0;
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
                /*if(enemy!=null){
                    enemy.TakeDamage(10);
                    enemyCount++;
                }*/
            }else if(name.Contains(strBombEnemy)){
                enemies[enemyCount]=item.gameObject;
                enemyCount++;
                delay++;
                item.GetComponent<Bomber>().DelayExplosion(delay*2);
                //TriggerBombEvent(item.transform);
            }
        }
        enemyGenerator.Return(enemies,enemyCount);
        mapGenerator.BreakCells(breakCells,cellIndex);
        if(isOwner&&scoring)scoreManager.AddCombo(enemyCount);
        CheckOnPlayerTriggerInInterval();
        
    }

    public void Return(){
        if(isOwner)explosionPool.Return(this.gameObject);
        isOwner=false;
    }
}
