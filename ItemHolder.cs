
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ItemHolder : UdonSharpBehaviour
{
    [SerializeField]HumanBodyBones boneType;
    [SerializeField]VRC_Pickup.PickupHand pickupHand;
    [SerializeField]GameObject holdSphere;
    void Start()
    {
        CheckAutoHoldInterval();
    }
    [SerializeField]Vector3 offset=new Vector3(0,-0.2f,0.1f);
    void Update(){
        if(Networking.LocalPlayer==null)return;
        var pos =Networking.LocalPlayer.GetBonePosition(boneType);
        this.transform.position=pos+offset;

    }
    VRC_Pickup  holdItem;
    [SerializeField]float autoHoldDistance =2f;
    [SerializeField]float manualHoldDistance =0.2f;
    public void CheckAutoHoldInterval(){
        if(holdItem!=null){
            var distance=Vector3.Distance(holdItem.transform.position,holdSphere.transform.position);
            if(distance>autoHoldDistance){
                Hold();
            }
        }
        SendCustomEventDelayedSeconds(nameof(CheckAutoHoldInterval),1f);
    }
    public void OnPickUpCallback(){
        holdItem = Networking.LocalPlayer.GetPickupInHand(pickupHand);
    }

    public void OnDropCallback(){
        if(holdItem!=null){
            var distance=Vector3.Distance(holdItem.transform.position,holdSphere.transform.position);
            if(distance<manualHoldDistance){
                Hold();
            }
        }
    }

    void Hold(){
         if(holdItem!=null){
            holdItem.transform.SetParent(this.transform);
            holdItem.transform.position=holdSphere.transform.position;
        }
    }

}
