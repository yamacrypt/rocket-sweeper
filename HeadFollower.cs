
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class HeadFollower : UdonSharpBehaviour 
    {
        HumanBodyBones headBone=HumanBodyBones.Head;
        HumanBodyBones handBone=HumanBodyBones.RightHand;


        VRCPlayerApi playerApi;
        bool isInEditor;
        bool isInVR;

        void Start()
        {
            playerApi = Networking.LocalPlayer;
            isInEditor = playerApi == null;
            if (isInEditor)
                return;
            isInVR=playerApi.IsUserInVR();
        }

        [SerializeField]Player player;

        void Update()
        {
            if (isInEditor)
                return;
            if(isInVR){
                var trackingData=playerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                transform.SetPositionAndRotation(trackingData.position, trackingData.rotation);
            } else{
                var t=player.TransformGun;
                var trackingData=playerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                if(t!=null){
                    transform.position=trackingData.position;
                    transform.rotation=t.rotation*trackingData.rotation;
                } else{
                    transform.SetPositionAndRotation(trackingData.position, trackingData.rotation);
                }
            }
        }
    }
