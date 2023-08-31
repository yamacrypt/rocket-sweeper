
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RockerExplosion : UdonSharpBehaviour
{
    [SerializeField]IMapGenerator mapGenerator;
    [SerializeField]IObjectPool explosionPool;
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

    Vector3 bulletDir;

    public void Init(Vector3 dir){
        bulletDir=dir.normalized;
    }

    [SerializeField]SphereCollider col;
     [SerializeField]float power=15f;

    void Start()
    {
        radiusMax=col.radius;
        breakCells=new GameObject[200];
    }
    float radiusMax;
    float t=0;
    //[SerializeField]AudioSource explosionSE;
    public void _OnEnable()
    {
        //BecomeBiggerInterval();
        playerTriggerDone=false;
        //explosionSE.Stop();
        //explosionSE.Play();
        SendCustomEventDelayedSeconds(nameof(Return),returnTime);
        SendCustomEventDelayedFrames(nameof(BreakCells),5);
    }
    public void _OnDisable(){
        playerTriggerDone=true;
    }
    [SerializeField]float triggerInterval=0.1f;
    bool playerTriggerDone=false;
    public void CheckOnPlayerTriggerInInterval(){
        if(playerTriggerDone)return;
        var distance=Vector3.Distance(this.transform.position,Networking.LocalPlayer.GetPosition());
        if(distance<explosionSize/2){
            Networking.LocalPlayer.SetVelocity(Vector3.up*power);
        }
        else if(distance<explosionSize){
            var playerVel=(this.transform.position-Networking.LocalPlayer.GetPosition()).normalized;
            //playerVel.y=-Mathf.Abs(playerVel.y);
            Networking.LocalPlayer.SetVelocity(playerVel*-1f*power);
            playerTriggerDone=true;
        }
        SendCustomEventDelayedSeconds(nameof(CheckOnPlayerTriggerInInterval),triggerInterval);
    }
    [SerializeField]float returnTime=2f;
    [SerializeField]float explosionSize=3f;
    string strZombie="Zombie";
    string strCell="block_prefab";
    GameObject[] breakCells;
    [SerializeField]ScoreManager scoreManager;
    public void BreakCells(){
        var cols=Physics.OverlapSphere(this.transform.position, explosionSize);
        int cellIndex=0;
        int enemyCount=0;
        foreach (var item in cols)
        {
            if(item==null)continue;
            var name=item.name;
            if(name.Contains(strZombie)){
                var enemy=item.GetComponent<Enemy>();
                if(enemy!=null){
                    enemy.TakeDamage(10);
                    enemyCount++;
                }
            } else if(name.Contains(strCell)){
                breakCells[cellIndex]=item.gameObject;
                cellIndex++;
            }
        }
        mapGenerator.BreakCells(breakCells,cellIndex);
        scoreManager.AddCombo(enemyCount);
        CheckOnPlayerTriggerInInterval();
        
    }
    public void Return(){
        explosionPool.Return(this.gameObject);
    }
}
