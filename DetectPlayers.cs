
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DetectPlayers : UdonSharpBehaviour
{
    [SerializeField]TextMeshPro players;
    [UdonSynced]int[] playerIDs=new int[8];
    public int GetPlayerIndex(int id){
        for(int i=0;i<playerIDs.Length;i++){
            if(playerIDs[i]==id){
                return i;
            }
        }
        return -1;
    }
    public bool IsJoined(){
        var id=Networking.LocalPlayer.playerId;
        for(int i=0;i<playerIDs.Length;i++){
            if(playerIDs[i]==id){
                return true;
            }
        }
        return false;
    }
    public override void OnDeserialization(){
        Notify();
    }
    public int[] PlayerIDs(){
        var length=0;
        foreach(var index in playerIDs){
            if(index>=0)length++;
        }
        var indexes=new int[length];
        for(int i=0;i<length;i++){
            indexes[i]=playerIDs[i];
        }
        return indexes;
    }
    void Start()
    {
        for(int i=0;i<playerIDs.Length;i++){
            playerIDs[i]=-1;
        }
    }

    public void Notify(){
        Debug.Log("Notify: players list");
        var text="";
        foreach(var index in PlayerIDs()){
            text+=VRCPlayerApi.GetPlayerById(index).displayName+"\n";
        }
        players.text=text;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if(!Networking.LocalPlayer.IsOwner(gameObject))return;
        var id=player.playerId;
        for(int i=0;i<playerIDs.Length;i++){
            if(playerIDs[i]==id){
                return;
            }
            if(playerIDs[i]==-1){
                playerIDs[i]=id;
                Notify();
                RequestSerialization();
                break;
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if(!Networking.LocalPlayer.IsOwner(gameObject))return;
        var id=player.playerId;
        for(int i=0;i<playerIDs.Length;i++){
            if(playerIDs[i]==id){
                for(int j=i;j<playerIDs.Length-1;j++){
                    playerIDs[j]=playerIDs[j+1];
                }
                Notify();
                RequestSerialization();
                break;
            }
        }
    }
}
