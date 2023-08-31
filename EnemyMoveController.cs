
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

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
            CalcPhysicsInterval();
            taskExists=true;
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
        if(velocityY==0&&preVel==0&&prePos.y-currentPos.y>=0&&prePos.y-currentPos.y<velocityGrounded&&!meshRenderer.enabled){
            meshRenderer.enabled=true;
            triggerCollider.enabled=true;
        }
        if(isClimbing){
            rg.MovePosition(currentPos+Vector3.up*stepOffset);
            isClimbing=false;
            isGrounded=false;
            SendCustomEventDelayedFrames(nameof(CompleteClimb),5);
            //Debug.Log("Climb");
            return;
        }
        if(isGrounded&&SquaredDistance(prePos,currentPos)<stuckDistance*stuckDistance){
            isClimbing=true;
            //rg.useGravity=false;
            isGrounded=false;
            isClimbComplete=false;
            return;
        } else {
            //rg.useGravity=true;
            isGrounded=(preVel-velocityY>-velocityGrounded&&preVel-velocityY<velocityGrounded)&&isClimbComplete;
            
            //Debug.Log(velocityY);
            /*vel.x=targetDir.x;
            vel.y=velocityY;
            vel.z=targetDir.z;*/
            rg.velocity=new Vector3(targetDir.x,velocityY,targetDir.z);
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
    public void CalcDir(){
        if(Networking.LocalPlayer!=null){
        var playerPos=Networking.LocalPlayer.GetPosition();
        playerPos.y=transform.position.y;
        targetDir = (playerPos-transform.position).normalized*velocityMagnitude;// * slowMultiplier * setting.SpeedMultiplier * 0.95f;
        this.transform.LookAt(playerPos);
        }
    }

    public void CalcPhysicsInterval(){
        if(!isInPool){
            CalcDir();
            _FixedUpdate();
        }else{
            taskExists=false;
            meshRenderer.enabled=false;
            triggerCollider.enabled=false;
        }
        SendCustomEventDelayedSeconds(nameof(CalcPhysicsInterval),physicsInterval);
    }
    bool isInPool=false;
    bool taskExists=false;
    [SerializeField]SkinnedMeshRenderer meshRenderer;
    [SerializeField]BoxCollider triggerCollider;
    public void _OnEnable(){
        meshRenderer.enabled=false;
        triggerCollider.enabled=false;
        isInPool=false;
        isClimbing=false;
        isClimbComplete=true;
        isGrounded=false;
        // gracityでvelocityの確保するために遅延実行
        WaitCalcPhysicsInterval();
        //CalcDirInterval();
    }
    public void WaitCalcPhysicsInterval(){
        if(!taskExists){
            prePos=Vector3.zero;
            SendCustomEventDelayedSeconds(nameof(CalcPhysicsInterval),0.2f);
            taskExists=true;
        }else{
            SendCustomEventDelayedSeconds(nameof(WaitCalcPhysicsInterval),physicsInterval/2f);
        }
    }
    [SerializeField]float physicsInterval=0.25f;
     public void _OnDisable()
    {
        isInPool=true;
        meshRenderer.enabled=false;
        triggerCollider.enabled=false;
    }



}
