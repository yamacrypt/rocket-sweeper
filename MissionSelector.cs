
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MissionSelector : GameLifeCycle
{
    public TextMeshPro planetName;

    [SerializeField]IMessenger messenger;
    MissionInfo missionInfo;
    public void SetMission(Mission mission){
        missionInfo.Mission=mission;
        if(mission!=null){
            planetName.text=mission.PlanetName;
            image.color=mission.Locked?Color.gray:themeColor;
            image.sprite=DisabledSprite;
        } else{
            planetName.text="";
            image.color=Color.gray;
        }
    }
    /*Mission[] missions;
    [UdonSynced]int missionIndex;

    void Notify(int index){
        if(index>=0&&index<missions.Length){
            SetMission(missions[index]);
        } else{
            SetMission(null);
        }
    }
    public override void OnDeserialization(){
        Notify(missionIndex);
    }
    public void SetMissionIndex(Mission[] missions,int index){
        this.missions=missions;
        if(Networking.LocalPlayer.IsOwner(this.gameObject)){
            this.missionIndex=index;
            Notify(index);
            RequestSerialization();
        }
    }*/
    [SerializeField]Image image;
    [SerializeField]Image[] others;
    Color themeColor;
    void Start()
    {
        themeColor=image.color;
        missionInfo=GetComponent<MissionInfo>();
        Debug.Assert(missionInfo!=null,"MissionInfo is null");
    }

    public override void Interact(){
        if(missionInfo.Mission==null)return;
        if(missionInfo.Mission.Locked)return;
        Debug.Log("Interact");
        
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(SelectMission));
    }
    [SerializeField]MissionUIManager missionUIManager;


    public void Highlight(){
        image.sprite=PressedSprite;
        image.color=themeColor;
        foreach(var other in others){
            other.sprite=DisabledSprite;
        }
    }

    public override void GameStartCallback()
    {
        base.GameStartCallback();
        Highlight();
    }
    public void SelectMission(){
        messenger.Publish(this.gameObject,GameMessage.GameStart);
    }

    [SerializeField]
    private Sprite PressedSprite;


    [SerializeField]
    private Sprite DisabledSprite;
}
