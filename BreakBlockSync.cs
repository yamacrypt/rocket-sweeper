
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BreakBlockSync : UdonSharpBehaviour
{
    [UdonSynced]int[] syncedBreakCellIndexes= new int[maxSize];
   int[] breakCellIndexes= new int[maxSize];

    /*bool IsOwnerOfEnemyPool(){
        return Networking.IsOwner(localPlayer,enemyPools[0].gameObject);
    }*/
    public override void OnDeserialization()
    {
        Debug.Log("OnDeserialization DamageSyncController");
        for(var i=0;i<syncedBreakCellIndexes.Length;i++){
            var cellIndex = syncedBreakCellIndexes[i];
        }
    }
    byte size=0;
    [UdonSynced]public byte syncedSize=0;
    public override void OnPreSerialization(){
        Debug.Log("OnPreSerialization DamageSyncController");
        for(var i=0;i<size;i++){
            syncedBreakCellIndexes[i]=breakCellIndexes[i];
            breakCellIndexes[i]=0;
        }
        syncedSize=size;
    }


    VRCPlayerApi localPlayer;
    void Start()
    {
        localPlayer = Networking.LocalPlayer; 
    }

    const byte maxSize=15;
    public void MarkBroken(int cellIndex){
        if(!Networking.IsOwner(localPlayer,this.gameObject)){
            Debug.Log("mark block failed");
            return;
        }
        if(size>=maxSize){
            Debug.LogWarning("WARN size <=15");
        } else {
            breakCellIndexes[size]=cellIndex;
            size++;
        }
    }

    public void TransferOwnership(VRCPlayerApi player){
        if(Networking.IsOwner(player,this.gameObject))return;
        Networking.SetOwner(player,this.gameObject);
        this.player=player;
        isOwner=true;
    }

    [SerializeField]float intervalTime=1f;

    private float _timeElapsed;
    VRCPlayerApi player;

    //TODO: player leftじに更新
   //bool setting.IsMaster[0];
    bool isOwner=false;
    private void FixedUpdate()
    {
        if(localPlayer != player || !isOwner)return;
        // all pool are owned by instance owner
        //if(!Networking.IsOwner(Networking.LocalPlayer,enemyPools[0].gameObject)) return;
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed  > intervalTime)
        {   
            RequestSerialization();
            _timeElapsed = 0f;
        }
    }

    public void SyncBreakBlocks(){
        if(!Networking.IsOwner(localPlayer,this.gameObject)){
            Debug.Log("damage sync failed");
            return;
        }
        for(var i=0;i<size;i++){
            RequestSerialization();
        }
    }

}
