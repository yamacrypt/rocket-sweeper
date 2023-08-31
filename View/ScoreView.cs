
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ScoreView : UdonSharpBehaviour
{
    [SerializeField]TextMeshProUGUI scoreText;
    void Start()
    {
        
    }
    public void SetScore(int score){
        scoreText.text=score.ToString();
    }
}
