
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
public enum MoveType{
    Follow,Random,Idle
}
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EnemyMoveController : UdonSharpBehaviour
{
    [SerializeField]bool execOnStart=false;
    [SerializeField]float stepOffset;
    Rigidbody rg;
    [SerializeField]float velocityMagnitude=1f;
    [SerializeField]double velocityGrounded=0.000001f;
    [SerializeField]double stuckDistance=0.0001f;
    void Start()
    {
        rg=GetComponent<Rigidbody>();
        //CalcDirInterval();
        prePos=transform.position+Vector3.forward;
        if(execOnStart){
            _OnEnable();
        }
    }
    Vector3 prePos;
    double preVel;
    double xDiff,zDiff;
    float velocityY;
    double SquaredDistance(Vector3 a,Vector3 b){
        xDiff=a.x-b.x;
        zDiff=a.z-b.z;
        return xDiff*xDiff+zDiff*zDiff;
    }
    Vector3 currentPos;
    void _FixedUpdate()
    {
        velocityY=rg.velocity.y;
        currentPos=transform.position;
        /*if(velocityY==0&&preVel==0&&prePos.y-currentPos.y>=0&&prePos.y-currentPos.y<velocityGrounded&&!meshRenderer.enabled){
            meshRenderer.enabled=true;
            triggerCollider.enabled=true;
        }*/

        if(isGrounded&&SquaredDistance(prePos,currentPos)<stuckDistance*stuckDistance){
            isClimbing=true;
            //rg.useGravity=false;
            isGrounded=false;
            isClimbComplete=false;
            return;
        } else {
            //rg.useGravity=true;
            isGrounded=Math.Abs(preVel-velocityY)<velocityGrounded;//&&Math.Abs(prePos.y-currentPos.y)<velocityGrounded;
            
            //Debug.Log(velocityY);
            /*vel.x=targetDir.x;
            vel.y=velocityY;
            vel.z=targetDir.z;*/
            rg.velocity=new Vector3(targetDir.x,velocityY,targetDir.z);
        }
        if(isClimbing){
            rg.MovePosition(currentPos+Vector3.up*stepOffset);
            rg.velocity=new Vector3(targetDir.x,0,targetDir.z);
            isClimbing=false;
            isGrounded=false;
            SendCustomEventDelayedFrames(nameof(CompleteClimb),5);
            //Debug.Log("Climb");
            return;
        }
        preVel=velocityY;
        prePos=currentPos;
        
    }


    public void CompleteClimb(){
        isClimbComplete=true;
    }
    Vector3 vel;
    bool isGrounded=false;
    bool isClimbing=false;
    bool isClimbComplete=true;
    Vector3 targetDir;
    [SerializeField]float lookAtRot;
    public void CalcDir(){
        if(Networking.LocalPlayer!=null){
        var playerPos=Networking.LocalPlayer.GetPosition();
        playerPos.y=transform.position.y;
        var diffX=playerPos.x-transform.position.x;
        var diffZ=playerPos.z-transform.position.z;
        var distance=diffX*diffX+diffZ*diffZ;
        bool followMove=moveType==MoveType.Follow;
        if(!followMove&&distance<followDistanceMin*followDistanceMin) targetDir = UnityEngine.Random.insideUnitSphere.normalized*velocityMagnitude*setting.GameSpeed;// * slowMultiplier * setting.SpeedMultiplier * 0.95f;
        else targetDir = (playerPos-this.transform.position).normalized*velocityMagnitude*setting.GameSpeed;// * slowMultiplier * setting.SpeedMultiplier * 0.95f;
        this.transform.LookAt(playerPos);
        this.transform.rotation *= Quaternion.Euler(0,lookAtRot,0);
        }
    }
    [SerializeField]Enemy enemy;
    [SerializeField]EnemyGenerator enemyGenerator;
    float aliveheightMin=-50;
    public void CalcPhysicsInterval(){
        // 落下死判定
        if(this.transform.position.y<aliveheightMin){
            taskExists=false;
            enemy.Death();
            if(enemyGenerator!=null)enemyGenerator.Fall();
            else Debug.LogWarning("EnemyGenerator is null");
            return;
        }
        if(!isInPool){
            CalcDir();
            _FixedUpdate();
            SendCustomEventDelayedSeconds(nameof(CalcPhysicsInterval),physicsInterval);
        }else{
            taskExists=false;
        }
    }
    bool isInPool=false;
    bool taskExists=false;
    //[SerializeField]SkinnedMeshRenderer meshRenderer;
    //[SerializeField]BoxCollider triggerCollider;
    [SerializeField]MoveType moveType=MoveType.Follow;
    [SerializeField]float followDistanceMin=5f;
    public void _OnEnable(){
        //meshRenderer.enabled=false;
        //triggerCollider.enabled=false;
        isInPool=false;
        isClimbing=false;
        isClimbComplete=true;
        isGrounded=false;
        // gracityでvelocityの確保するために遅延実行
        if(moveType!=MoveType.Idle){
            WaitCalcPhysicsInterval();
        }
        //CalcDirInterval();
    }
    public void WaitCalcPhysicsInterval(){
        if(!taskExists){
            prePos=Vector3.zero;
            SendCustomEventDelayedSeconds(nameof(CalcPhysicsInterval),0.2f);
            taskExists=true;
        }else{
            SendCustomEventDelayedSeconds(nameof(WaitCalcPhysicsInterval),physicsInterval);
        }
    }
    [SerializeField]float physicsInterval=0.25f;
    [SerializeField]GameSpeedManager setting;
    public void _OnDisable()
    {
        isInPool=true;
        // meshRenderer.enabled=false;
        //triggerCollider.enabled=false;
    }

    /*void OnCollisionEnter(Collision collisionInfo)
    {
        if(!meshRenderer.enabled&&!triggerCollider.enabled){
            meshRenderer.enabled=true;
            triggerCollider.enabled=true;
        }
    }*/


}
