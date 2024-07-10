
using System;
using System.Numerics;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ScoreManager : GameLifeCycle
{
    [SerializeField]AudioSource[] comboAudios;
    [SerializeField]AudioSource heartAudio;
    [SerializeField]ScoreView scoreView;

    [SerializeField]GameObject resultView;


    //Int64 score=0;
    [SerializeField]float comboMultiplier=2f;
    public override void GameOver(){
        base.GameOver();
        Notify();
        resultView.SetActive(true);
        scoreView.SetRank("G");
        scoreView.SetScore(0);
        heartAudio.Stop();
        
    }

    public void VisibleReset(){
        scoreView.SetRank("G");
        scoreView.SetScore(0);
    }

    float multiplier=1;
    float interval=0.5f;
    SyncedCounterUnit myCounterUnit;
    [SerializeField]DetectPlayers detectPlayers;
    void Start(){
        resultView.SetActive(false);
        
        //NotifyInterval();
    }

    /*public void NotifyInterval(){
        Notify();
        SendCustomEventDelayedSeconds(nameof(NotifyInterval),interval);
    }*/
    public override void GameStart(Mission mission){
        base.GameStart(mission);
        resultView.SetActive(false);
        //score=0;
        var index=detectPlayers.GetPlayerIndex(Networking.LocalPlayer.playerId);
        if(index>=0&&index<syncedCounterUnits.Length){
            myCounterUnit=syncedCounterUnits[index];
            Networking.SetOwner(Networking.LocalPlayer,myCounterUnit.gameObject);
        }
        // owner関係なくリセットしたいため
        foreach(var unit in syncedCounterUnits){
            unit.Reset();
        }
        if(Networking.LocalPlayer.IsOwner(syncedCounter.gameObject)){
            syncedCounter.Reset();
        }
        combo=0;
        multiplier=1;
        Notify();
    }

    int _combo;
    int combo{
        get{
            return _combo;
        }
        set{
            bool isUp=_combo<value;
            _combo=value;
            if(_combo==0){
                multiplier=1;
            } else{
                multiplier=Mathf.Pow(comboMultiplier,_combo+comboStart);
            }
            heartAudio.pitch=1+0.1f*Mathf.Max(0,_combo-1);
            float gravityMultiplier=1+0.05f*_combo;
            multiplierView.SetMultiplier((int)multiplier,gravityMultiplier);
            if(isUp){ 
                multiplierView.ShowTemp();
            }
            gameSpeedManager.SetSpeed(gravityMultiplier);
        }
    }
    [SerializeField]int comboStart=2;
    public Int64 Score=>syncedCounter.Count;
    [SerializeField]GameSpeedManager gameSpeedManager;

    [SerializeField]int basePoint=100;
    [SerializeField]MultiplierView multiplierView;
    [SerializeField]SyncedCounter syncedCounter;
    [SerializeField]SyncedCounterUnit[] syncedCounterUnits;
    [SerializeField]PlayerCounterByOwner playerCounter;
    int audioIndex;

    public void AddCombo(int enemyCount){
        //var playerMultiplier=Math.Sqrt(Math.Max(1,playerCounter.PlayerCount));
        //playerMultiplier*=Math.Sqrt(playerMultiplier);
        //Int64 baseModifiedPoint = (Int64)(Math.Floor(basePoint*playerMultiplier/10)*10); 
        var point=basePoint*(Int64)multiplier*enemyCount;
        myCounterUnit.CountUp(point);
        int precombo =combo;
        if(enemyCount==0){
            combo=Math.Max(0,combo-1);
            if(combo==0)heartAudio.Stop();
            if(precombo!=0){
                comboAudios[0].Play();
            }
        }else{
            if(!heartAudio.isPlaying)heartAudio.Play();
            combo++;
        }
        /*if(precombo!=combo){
            audioIndex=Math.Min(comboAudios.Length-1,combo);
            var preIndex=Math.Max(0,audioIndex-1);
            comboAudios[preIndex].Stop();
            comboAudios[audioIndex].Play();
        }*/

        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Notify));
    }
    public void Half(){
        combo/=2;
    }
    public void Notify(){
        scoreView.SetScore(syncedCounter.Count);
    }
}
