
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
        currentPos=transform.position;
        if(isClimbing){
            rg.MovePosition(currentPos+Vector3.up*stepOffset);
            isClimbing=false;
            isGrounded=false;
            SendCustomEventDelayedFrames(nameof(CompleteClimb),5);
            //Debug.Log("Climb");
            return;
        }
        xDiff=prePos.x-currentPos.x;
        zDiff=prePos.z-currentPos.z;
        if(isGrounded&&SquaredDistance(prePos,currentPos)<stuckDistance*stuckDistance){
            isClimbing=true;
            rg.useGravity=false;
            isGrounded=false;
            isClimbComplete=false;
            return;
        } else {
            rg.useGravity=true;
            velocityY=rg.velocity.y;
            isGrounded=(preVel-velocityY>-velocityGrounded&&preVel-velocityY<velocityGrounded)&&isClimbComplete;
            preVel=velocityY;
            prePos=currentPos;
            //Debug.Log(velocityY);
            /*vel.x=targetDir.x;
            vel.y=velocityY;
            vel.z=targetDir.z;*/
            rg.velocity=new Vector3(targetDir.x,velocityY,targetDir.z);
        }
        
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
        var playerPos=Networking.LocalPlayer.GetPosition();
        playerPos.y=transform.position.y;
        targetDir = (playerPos-transform.position).normalized*velocityMagnitude;// * slowMultiplier * setting.SpeedMultiplier * 0.95f;
        this.transform.LookAt(playerPos);
    }
    public void CalcDirInterval(){
        CalcDir();
        SendCustomEventDelayedSeconds(nameof(CalcDirInterval),1f);
    }
    public void CalcPhysicsInterval(){
        if(canceled){
            canceled=false;
            return;
        }
        if(isInPool)return;
        CalcDir();
        _FixedUpdate();
        SendCustomEventDelayedSeconds(nameof(CalcPhysicsInterval),physicsInterval);
    }
    bool isInPool=false;
    bool taskExists=false;
    public void _OnEnable(){
        isInPool=false;
        isClimbing=false;
        isClimbComplete=true;
        isGrounded=false;
        if(!taskExists)CalcPhysicsInterval();
        taskExists=true;
        //CalcDirInterval();
    }
    [SerializeField]float physicsInterval=0.25f;
     public void _OnDisable()
    {
        isInPool=true;
        if(taskExists)Cancel();
        taskExists=false;
        
    }
    bool canceled=false;
    void Cancel(){
        canceled=true;
    }


}
