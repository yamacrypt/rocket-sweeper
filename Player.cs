
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Player : UdonSharpBehaviour
{

    GunController yourGun;
    bool isEquip=false;


    public void Equip(GunController gun){
        yourGun=gun;
        isEquip=true;
    }



    void Start()
    {
        //ResetKillCountInterval();
        localPlayer=Networking.LocalPlayer;
    }

    VRCPlayerApi localPlayer;

    public void ResetKillCountInterval(){
        killCount=0;
        SendCustomEventDelayedSeconds("ResetKillCountInterval",10);
    }

    public void UnEquip(){
        isEquip=false;
    }

    public void UnTap(){
        yourGun=null;
        isEquip=false;
    }

    public bool IsYourGun(GunController gun){
        //if(!isEquip)return false;
        return yourGun==gun;
    }

    public bool IsEquipGun(GunController gun){
        return isEquip && yourGun==gun;
    }

    public bool IsEquip=>isEquip;

    public bool HasYourGun(){
        return yourGun!=null;
    }


    public void Reset(){
        killCount=0;
    }

    int killCount=0;

    public int KillCount10=>Math.Max(0,killCount);

    public void AddKillCount(){
        killCount++;
        SendCustomEventDelayedSeconds(nameof(SubKillCount),10);
    }

    public void SubKillCount(){
        if(killCount>0)killCount--;
    }


    [SerializeField]Vector3 revisionPos=new Vector3(0.25f,-0.2f,-0.1f);
    [SerializeField]Vector3 itemRevisionPos=new Vector3(-0.25f,-0.2f,-0.1f);

    /*void Update()
    {
            var player = Networking.LocalPlayer;
            if (player != null){
                var right = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                yourGun.transform.position = right.position + revisionPos;//new Vector3(0.25f,-0.1f,0.1f);//new Vector3(0.35f,-0.2f,-1f); 
                var rot=right.rotation.eulerAngles;
                yourGun.transform.rotation = Quaternion.Euler(0,rot.y,rot.z) *  Quaternion.Euler(90,0,0);
                var left = player.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand);

            }
        }
    }*/
    
}
