﻿
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using BuildSoft.UdonSharp.Collection;
using System;
using UdonObjectPool;


public enum EnemyAura{
    None,White,Black,Red,Blue,Yellow
}
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Enemy : UdonSharpBehaviour
{
    public bool IsBoss=>isBoss;
    [SerializeField]int Life=10;
    [SerializeField]int Attack=5;

    [SerializeField]IObjectPool pool;

    public IObjectPool myPool=>pool;

    [SerializeField]LifeBar lifeBar;


    bool isInPool=true;
    public bool IsInPool=>isInPool;

    [UdonSynced, FieldChangeCallback(nameof(syncedLife))] private byte _syncedLife =10;

    public byte syncedLife{
        get{return _syncedLife;}
        set{
            _syncedLife=value;
            if(Networking.LocalPlayer!=null && Networking.LocalPlayer.isMaster){
                localSyncedLife=value;
            } else {
                if(syncedLife>0){
                    localSyncedLife = syncedLife;
                }
                //else DelayDeath();
            }
        }
    }


    byte _damagePerSync=0;
    byte damagePerSync{
        get{return _damagePerSync;}
        set{
            _damagePerSync=value;
        }
    }
    
    [SerializeField]int _localSyncedLife=10;
    public int localSyncedLife{
        get{return _localSyncedLife;}
        set{
            _localSyncedLife=value;
            if(lifeBar!=null)lifeBar.Percentage=(float)CurrentLife/(float)MaxLife;
        }
    }

    int CurrentLife=>Math.Max(0,localSyncedLife-damagePerSync);

    //[SerializeField]GameObject VisibleObj;

    public byte CheckLocalDamage(){
        var damage = damagePerSync;
        damagePerSync=0;
        
        return damage;
    }
    float slowMultiplier=1f ;
    public void TakeDamage(int damage){
        if(IsInPool || damage==0)return;
        float fd=isBoss ? damage*bossDamageReduction : damage;
        int damageInt=(int)fd;
        if(damageInt>byte.MaxValue)damageInt=byte.MaxValue;
        byte d = (byte)damageInt;
        if(Networking.LocalPlayer.isMaster){
            int overflowCheck=syncedLife;
            overflowCheck-=d;
            if(overflowCheck>byte.MinValue){
                syncedLife-=d;
            } else {
                syncedLife=0;
                BeKilled(); 
            }
            RequestSerialization();
        } else {
            //currentLife-=d;
            int overflowCheck=damagePerSync;
            overflowCheck+=d;
            if(overflowCheck<byte.MaxValue){
                damagePerSync+=d;
            } else {
                damagePerSync=byte.MaxValue;
            }
            if(lifeBar!=null)lifeBar.Percentage=(float)CurrentLife/(float)MaxLife;
            if(!isAlive){
                BeKilled(); 
            }
        }
    }

    public void TakeSyncDamage(byte damage){
        if(!Networking.LocalPlayer.isMaster)return;
        if(IsInPool || damage==0)return;
        byte d=damage;
        int overflowCheck=syncedLife;
        overflowCheck-=d;
        if(overflowCheck>byte.MinValue){
            syncedLife-=d;
        } else {
            syncedLife=0;
            BeKilled(); 
        }
        RequestSerialization();
    }
    bool isTriggerKillEvent=false;
    public bool BeKilled(){
        if(isTriggerKillEvent || IsInPool)return false;
        isTriggerKillEvent=true;
        //Debug.Log("Death");
        Death();
        return true;
    }

    [SerializeField]bool isBoss=false;

    public void BeKilledProcess(){
        DelayDeath();
    }

    public void DelayDeath(){
        SendCustomEventDelayedFrames(nameof(Death),2);
    }

    [SerializeField]AudioSource deathSound;
    [SerializeField]AudioSource appearSound;
    [SerializeField]Renderer renderer;
    [SerializeField]Renderer[] renderers;
    [SerializeField]BoxCollider enemyCollider;

    public void Death(){
        if(isInPool)return;
        if(isBoss&& deathSound!=null){
            deathSound.Stop();
            deathSound.Play();
        }
        pool.Return(this.gameObject);
    }
    float apparentDeathTime=0.5f;
    public void ApparentDeath(){
        if(isInPool)return;
        if(renderer!=null)renderer.enabled=false;
        if(renderers!=null){
            foreach(Renderer r in renderers){
                r.enabled=false;
            }
        }
        if(enemyCollider!=null)enemyCollider.enabled=false;
        SendCustomEventDelayedSeconds(nameof(Death),apparentDeathTime);
    }

    public bool isAlive=>CurrentLife>0 ;


    [SerializeField]float bossDamageReduction=0.2f;

    Vector3 target;


    //bool Networking.LocalPlayer.isMaster=false;//Networking.IsOwner(Networking.LocalPlayer, gameObject);

    byte MaxLife=>Life> byte.MaxValue ? byte.MaxValue : (byte)Life;
    public void _OnEnable()
    {
        //Debug.Log("Enemy OnEnable");
        damagePerSync=0;
        slowMultiplier=1f;
        isInPool=false;
        isTriggerKillEvent=false;
        //VisibleObj.SetActive(true);
        if(Networking.LocalPlayer!=null && !Networking.LocalPlayer.isMaster){
            //Debug.Log(Life * setting.MonsterLifeMultiplier);
            localSyncedLife= MaxLife;
            //preSyncedLife=currentLife;
        } else {
            // WARNING BYTE value
            syncedLife= MaxLife;
        }
        //CalcDir();
        if(isBoss&& appearSound!=null){
            appearSound.Stop();
            appearSound.Play();
        }
        SendCustomEventDelayedSeconds(nameof(ApparentDeath),aliveTime);

    }

    [SerializeField]float aliveTime=60;

    public void _OnDisable()
    {
        //Debug.Log("Enemy OnDisable");
        //damagePerSync=0;  damgesync対策でコメントアウトしてる
        slowMultiplier=1f;
        isInPool=true;
        isTriggerKillEvent=true;
    }



    /*void FixedUpdate()
    {
        if( IsInPool)return;
        //var dir = (target-transform.position).normalized;
        if(cc !=null){
            cc.SimpleMove(targetDir);
        } else if(rg!=null){
            rg.velocity=targetDir;
        }
    }*/
    //[SerializeField]VRCObjectPool dropPool;
}
