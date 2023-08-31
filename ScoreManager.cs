
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ScoreManager : UdonSharpBehaviour
{
    [SerializeField]AudioSource[] comboAudios;
    [SerializeField]ScoreView scoreView;
    int score=0;
    public void StartMeasure(){
        score=0;
        combo=0;
        Notify();
    }
    int combo;
    [SerializeField]int basePoint=100;
    public void AddCombo(int enemyCount){
        int precombo =combo;
        if(enemyCount==0){
            combo=0;
        }else{
            combo++;
        }
        if(precombo!=combo){
            comboAudios[Math.Min(comboAudios.Length-1,combo)].Play();
        }
        score+=basePoint*combo*enemyCount;
        Notify();
    }

    void Notify(){
        scoreView.SetScore(score);
    }

    public void Publish(){

    }
}
