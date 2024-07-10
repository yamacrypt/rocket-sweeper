
using System;
using System.Numerics;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ScoreResultAnimation : GameLifeCycle
{
    [SerializeField]ScoreView scoreView;
    [SerializeField]ScoreRankingSlotView[] slots;
    [SerializeField]AudioSource rankUpSE;
    [SerializeField]ScoreManager scoreManager;
    Int64 score=0;
    Int64 endScore=0;
    int slotIndex=-1;
    [SerializeField]AudioSourceExt roomBGM;

    float showTime=0;
    float scorePerSecond;
    bool readyStart=true;
    public bool ReadyEnd=true;
    public bool SyncReadyEnd=true;
    void FixedUpdate()
    {
        if(showTime>0){
            score+=(int)(scorePerSecond*Time.fixedDeltaTime);
            if(score>=endScore){
                score=endScore;
                showTime=0;
                roomBGM.Play();
                readyStart=true;
            }
            Notify();
        }

    }

    public override void GameOver(){
        base.GameOver();
        Debug.Log("gameOver "+scoreManager.Score);
        endScore=scoreManager.Score;
        mission.HighScore=Math.Max(mission.HighScore,endScore);
        score=0;
        readyStart=false;
        Notify();
        StartAnimation();
        //SendCustomEventDelayedSeconds(nameof(StartAnimation),7f);
    }

    float multiplier=1;
    float interval=0.5f;
    public void StartAnimation(){
        showTime=interval;
        for(int i=0;i<slots.Length;i++){
            if(slots[i].ScoreThreshold<=endScore){
                showTime+=interval;
            }
        }
        Debug.Log(showTime);
        Debug.Log(endScore);
        scorePerSecond=endScore/showTime;
        TrySetSlotVisible();
    }
    public void TrySetSlotVisible(){
        if(slotIndex<0){
            return;
        }

        slots[slotIndex].SetVisible(true);
        scoreView.SetRank(slots[slotIndex].RankText);
        rankUpSE.Stop();
        rankUpSE.pitch=1+0.1f*(slots.Length-slotIndex-1);
        rankUpSE.Play();

        if(slots[slotIndex].ScoreThreshold>endScore){
            slotIndex=-1;
            return;
        }
        slotIndex--;
        SendCustomEventDelayedSeconds(nameof(TrySetSlotVisible),interval);

    }

    public override bool IsReadyToGameStart(){
        return base.IsReadyToGameStart()&&slotIndex<0&&showTime==0&&readyStart;
    }
    public override bool IsReadyToGameOver()
    {
        return base.IsReadyToGameOver()&&ReadyEnd&&SyncReadyEnd;
    }
    Mission mission;

    public override void GameStart(Mission mission){
        base.GameStart(mission);
        this.mission=mission;
        ReadyEnd=false;
        SyncReadyEnd=false;
        foreach(var slot in slots){
            slot.SetVisible(false);
        }
        slotIndex=slots.Length-1;
    }

    void Notify(){
        scoreView.SetScore(score);
    }
}
