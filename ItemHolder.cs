
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ItemHolder : UdonSharpBehaviour
{
    [SerializeField]HumanBodyBones boneType;
    [SerializeField]VRC_Pickup.PickupHand pickupHand;
    [SerializeField]Transform holdSphere;
    [SerializeField]Transform detachParent;
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
                _Attach(holdItem.gameObject);
            }
        }
        SendCustomEventDelayedSeconds(nameof(CheckAutoHoldInterval),1f);
    }

    public void OnEquip(){
        holdItem = Networking.LocalPlayer.GetPickupInHand(pickupHand);
        if(holdItem!=null){
            holdItem.transform.SetParent(detachParent);
        }
    }

    public void OnUnEquip(){
        if(holdItem!=null){
            var distance=Vector3.Distance(holdItem.transform.position,holdSphere.transform.position);
            if(distance<manualHoldDistance){
                _Attach(holdItem.gameObject);
            }
        }
    }
    Quaternion initRot=Quaternion.identity;
    void _Attach(GameObject item){
        if(item!=null){
            item.transform.SetParent(this.transform);
            item.transform.position=holdSphere.transform.position;
            item.transform.rotation=initRot;
        }
    }
    public void Attach(GameObject item){
        if(item!=null){
            if(initRot==Quaternion.identity)initRot=item.transform.rotation;
            _Attach(item);
        }
    }

    public void Detach(){
        if(holdItem!=null){
            holdItem.transform.SetParent(detachParent);
            holdItem.Drop();
            holdItem.transform.position-=new Vector3(0,100f,0);
        }
        holdItem=null;
    }


    

}
