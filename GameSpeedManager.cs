
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameSpeedManager : GameLifeCycle
{
    [SerializeField]IMapGenerator mapGenerator;
    float playerSpeed;
    [SerializeField]float playerJumpImpulse=7;
    public float GameSpeed=>speed;
    GunController gunController;
    void Start()
    {
        playerSpeed=Networking.LocalPlayer.GetWalkSpeed();
        //playerJumpImpulse=Networking.LocalPlayer.GetJumpImpulse();
    }
    public void SetGun(GunController gunController){
        this.gunController=gunController;
    }
    public override void GameStart(Mission mission){
        base.GameStart(mission);
        SetSpeed(1);
    }
    public override void GameOver(){
        base.GameOver();
        Reset();
    }
    float speed=1;

    public void SetSpeed(float speed){
        this.speed=speed;
        //Debug.Log("SetSpeed: "+speed);
        Apply();
    }
    void Reset(){
        Physics.gravity=Vector3.down*9.8f;
        Networking.LocalPlayer.SetWalkSpeed(playerSpeed);
        Networking.LocalPlayer.SetJumpImpulse(playerJumpImpulse);
        if(gunController!=null)gunController.SetFireRateMultiplier(1);
    }
    void Apply(){
        Physics.gravity=Vector3.down*mapGenerator.Gravity*speed;
        Networking.LocalPlayer.SetWalkSpeed(playerSpeed*speed);
        Networking.LocalPlayer.SetJumpImpulse(playerJumpImpulse*Mathf.Sqrt(speed));
        if(gunController!=null)gunController.SetFireRateMultiplier(speed);
    }
}
