
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LocalPlayerFollow : UdonSharpBehaviour
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
        transform.position = localPlayer.GetPosition();
        //transform.rotation = localPlayer.GetRotation();
    }
}
