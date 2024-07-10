
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class LocalPlayerXZFollow : GameLifeCycle
{
    [SerializeField]float interval=1f;
    void Start()
    {
        FollowInterval();
    }

    [SerializeField]MeshRenderer meshRenderer;
    [SerializeField]ParticleSystem particleSystem;
    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        if(meshRenderer!=null)meshRenderer.enabled = true;
        if(particleSystem!=null)particleSystem.Play();
    }

    public override void GameOver()
    {
        base.GameOver();
        if(meshRenderer!=null)meshRenderer.enabled = false;
        if(particleSystem!=null)particleSystem.Stop();
    }

    public void FollowInterval(){
        Follow();
        SendCustomEventDelayedSeconds(nameof(FollowInterval),interval);
    }

    void Follow(){
        var localPlayer = Networking.LocalPlayer;
        if(localPlayer==null){
            return;
        }
        var pPos=localPlayer.GetPosition();
        transform.position = new Vector3(pPos.x,transform.position.y,pPos.z);
        //transform.rotation = localPlayer.GetRotation();
    }
}
