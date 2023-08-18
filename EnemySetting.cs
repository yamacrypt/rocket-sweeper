
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
public class EnemySetting : UdonSharpBehaviour
{
      float _monsterLifeMultiplier=1;
    float _monsterAmountMultiplier=1;
    float _monsterSpeedMultiplier=1;
     public float LifeMultiplier=>_monsterLifeMultiplier; //* Difficulty;
    public float AmountMultiplier => _monsterAmountMultiplier;//* (1.0f+(float)Math.Sqrt((double)battleStage-1.0)/3.0f);//* Difficulty;
    public float SpeedMultiplier => _monsterSpeedMultiplier;

    int playerCount;
    public int PlayerCount=>playerCount;
    public VRCPlayerApi[] gamePlayers;
    void Start()
    {
        CheckParameter();
    }
    public void CheckParameter(){
        playerCount=VRCPlayerApi.GetPlayerCount();
        gamePlayers=new VRCPlayerApi[playerCount];
        VRCPlayerApi.GetPlayers(gamePlayers);
    }

}