
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EventEmitterUI : GameLifeCycle
{
    [SerializeField]IMessenger stageMessenger;
    [SerializeField]IMessenger missionMessenger;
    [SerializeField]TextMeshPro buttonText;
    [SerializeField]int waitTime=3;
    BoxCollider interactCollider;
    //bool isPlaying=false;
    void Start()
    {
        interactCollider=GetComponent<BoxCollider>();
        missionInfo=GetComponent<MissionInfo>();
        Debug.Assert(missionInfo!=null,"MissionInfo is null");
    }
    MissionInfo missionInfo;
    public void SetMission(Mission mission){
        missionInfo.Mission=mission;
    }
    float waitSyncTime=10;
    public override void Interact()
    {
        if(!Networking.LocalPlayer.IsOwner(this.gameObject))return;
        buttonText.text="Not Ready";
        SendCustomEventDelayedSeconds(nameof(NotReadyToPlay),1);
        if(missionInfo.Mission==null){
            Debug.LogWarning("Mission is null");
            return;
        }
        //SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DoInteract));
        stageMessenger.Publish(this.gameObject,GameMessage.GameStart);
       
    }

    public void NotReadyToPlay(){
        if(buttonText.text=="Not Ready"){
            buttonText.text="Play";
        }
    }

    public void DoInteract(){
        //if(isPlaying)return;
        if(Networking.LocalPlayer.IsOwner(this.gameObject))return;
        stageMessenger.Publish(this.gameObject,GameMessage.GameStart);
    }

    public override void GameStartCallback()
    {
        if(!Networking.LocalPlayer.IsOwner(this.gameObject))return;
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DoInteract));
    }

    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        interactCollider.enabled=false;
        //isPlaying=true;
        buttonText.text="Wait";
        SendCustomEventDelayedSeconds(nameof(AddCommma),1f);
    }

    public override void GameOver()
    {
        base.GameOver();
        interactCollider.enabled=true;
        //isPlaying=false;
    }

    int waitIndex=0;


    public void AddCommma(){
        buttonText.text+=".";
        waitIndex++;
        if(waitIndex>=waitTime){
            EmitEvent();
            waitIndex=0;
        }else{
            SendCustomEventDelayedSeconds(nameof(AddCommma),1f);
        }
    }

    public void EmitEvent(){
        buttonText.text="Play";
        missionMessenger.Publish(this.gameObject,GameMessage.GameStart);
    }


}
