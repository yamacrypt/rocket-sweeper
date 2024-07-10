
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Player : GameLifeCycle
{
    [SerializeField]WeaponCollection weaponCollection;
    [SerializeField]ItemHolder holderLeft;
    [SerializeField]ItemHolder holderRight;

    VRC_Pickup yourGun;
    public Transform TransformGun=>yourGun!=null ? yourGun.transform : null;
    bool isEquip=false;
    public bool isRight=true;

    [SerializeField]GameSpeedManager gameSpeedManager;
    [SerializeField]DetectPlayers detectPlayers;
    public void Attach(){
        if(yourGun!=null){
            Debug.LogWarning("yourGun is not null");
        }
        isEquip=false;
        var index=detectPlayers.GetPlayerIndex(Networking.LocalPlayer.playerId);
        var guns=weaponCollection.Guns;
        if(index>=0&&index<guns.Length){
            yourGun=guns[index];
            var gunController=yourGun.GetComponent<GunController>();
            //TransferOwnerShip();
            gunController.TransferOwnerShip(Networking.LocalPlayer);
            //SendCustomEventDelayedSeconds(nameof(TransferOwnerShip),1);
            //Networking.SetOwner(Networking.LocalPlayer,yourGun.gameObject);
            if(isRight){
                holderRight.Attach(yourGun.gameObject);
            }else{
                holderLeft.Attach(yourGun.gameObject);
            }
            gameSpeedManager.SetGun(gunController);
        }else{
            Debug.LogWarning(index+" is out of range");
        }
    }

    public void TransferOwnerShip(){
        if(yourGun!=null)yourGun.GetComponent<GunController>().TransferOwnerShip(Networking.LocalPlayer);
    }



    public void Equip(){
        isEquip=true;
        holderRight.OnEquip();
        holderLeft.OnEquip();
    }

    public void UnEquip(){
        isEquip=false;
        holderRight.OnUnEquip();
        holderLeft.OnUnEquip();
    }

    public void ResetKillCountInterval(){
        killCount=0;
        SendCustomEventDelayedSeconds("ResetKillCountInterval",10);
    }

    public void Detach(){
        Debug.Log("Detach weapon");
        if(yourGun==null){
            Debug.LogWarning("yourGun is null");
        }
        
        isEquip=false;
        holderRight.Detach();
        holderLeft.Detach();
        if(yourGun!=null){
            yourGun.Drop();
            yourGun.transform.position-=new Vector3(0,100f,0);
        }
        yourGun=null;


    }

    /*public void UnTap(){
        yourGun=null;
        isEquip=false;
    }*/

    public bool IsYourGun(GunController gun){
        //if(!isEquip)return false;
        if(yourGun==null || gun==null)return false;
        return yourGun.gameObject.GetInstanceID()==gun.gameObject.GetInstanceID();
    }

    public bool IsYourGun(GameObject gun){
        //if(!isEquip)return false;
        if(yourGun==null || gun==null)return false;
        return yourGun.gameObject.GetInstanceID()==gun.GetInstanceID();
    }


    public bool IsEquipGun(GunController gun){
        return isEquip && IsYourGun(gun);
    }

    public bool IsEquip=>isEquip;

    public bool HasYourGun(){
        return yourGun!=null;
    }
    public override void GameOver(){
        base.GameOver();
        Detach();
        killCount=0;
    }
    public override void GameStart(Mission mission){
        base.GameStart(mission);
        killCount=0;
        Attach();
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

    
}
