
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ScoreView : UdonSharpBehaviour
{
    [SerializeField]TextMeshProUGUI scoreText;
    [SerializeField]TextMeshProUGUI rankText;
    public void SetScore(Int64 score){
        scoreText.text=score.ToString();
    }
    public void SetRank(string rank){
        rankText.text=rank;
    }
}
