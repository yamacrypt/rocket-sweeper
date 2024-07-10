
using mmmsys;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RocketShip : GameLifeCycle
{
    [SerializeField]IMessenger missionMessenger;
    [SerializeField]IMessenger gameMessenger;
    [SerializeField]MeshRenderer[] meshRenderers;
    [SerializeField]AudioSource appearSE;
    [SerializeField]BoxCollider interactCollider;
    [SerializeField]Transform missionView;
    [SerializeField]MissionUIManager missionUIManager;


    MissionInfo missionInfo;
    void Start()
    {
        missionInfo=GetComponent<MissionInfo>();
        Debug.Assert(missionInfo!=null,"MissionInfo is null");
    }

    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        missionInfo.Mission=mission;
        foreach(var meshRenderer in meshRenderers){
            meshRenderer.enabled = true;
        }
        interactCollider.enabled = true;
        missionView.gameObject.SetActive(false);
        appearSE.Play();
    }

    public override void GameOver()
    {
        base.GameOver();
        missionInfo.Mission=null;
        foreach(var meshRenderer in meshRenderers){
            meshRenderer.enabled = false;
        }
        interactCollider.enabled = false;
    }

    public override void Interact(){
        //if(!Networking.LocalPlayer.IsOwner(this.gameObject))return;
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DoInteract));
        
    }
    public void DoInteract(){
        gameMessenger.Publish(this.gameObject,GameMessage.GameStart);
    }

    
    
}
