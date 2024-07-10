
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerTrackingUI : UdonSharpBehaviour
{
    [SerializeField]VRCPlayerApi.TrackingDataType trackingDataType=VRCPlayerApi.TrackingDataType.Head;
    Vector3 playerPosition;
    Quaternion playerRotation;
    VRCPlayerApi player;
    bool isEditor;
    void Start()
    {
        player = Networking.LocalPlayer;
        bool isEditor = player == null;
        if (!isEditor){
            var transform = player.GetTrackingData(trackingDataType);
            this.transform.position = transform.position;
            this.transform.rotation = transform.rotation;
        } 
        //startPosition=this.transform.position;
        //startRotation=this.transform.rotation;
        //UpdateInterval();;
    }

    void Update(){
        if (!isEditor){
            var transform = player.GetTrackingData(trackingDataType);
            playerPosition =transform.position;
            playerRotation = transform.rotation;
        }
    }

    /*public void UpdateInterval(){
        _Update();
        SendCustomEventDelayedSeconds(nameof(UpdateInterval),0.666f);
    }*/

    private void LateUpdate()
    {
        transform.position = playerPosition;//Vector3.Lerp(transform.position, playerPosition, followMoveSpeed);

        //上と同じく回転をLerp関数で滑らかに補完
        //lockHorizonがtrueの場合はxzの角度を0にすることで水平に固定
        transform.rotation = playerRotation;//Quaternion.Lerp(transform.rotation, rot, followRotateSpeed);
        
    }
}
