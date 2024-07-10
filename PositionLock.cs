
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PositionLock : GameLifeCycle
{
    Vector3 initPos;
    void Start()
    {
        initPos=this.transform.position;
    }
    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        this.transform.position=initPos;
    }

    public override void GameOver()
    {
        base.GameOver();
        this.transform.position=Networking.LocalPlayer.GetPosition();
        Networking.LocalPlayer.Immobilize(true);
    }
}
