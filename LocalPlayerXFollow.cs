
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class LocalPlayerXFollow : UdonSharpBehaviour
{
    [SerializeField]float interval=1f;
    void Start()
    {
        FollowInterval();
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
        transform.position = new Vector3(pPos.x,transform.position.y,transform.position.z);
        //transform.rotation = localPlayer.GetRotation();
    }
}
