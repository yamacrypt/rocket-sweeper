
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MissionUIManager : GameLifeCycle
{
    [SerializeField] TextMeshPro missionName;
    [SerializeField] TextMeshPro explainText;
    [SerializeField]EventEmitterUI[] playButtons;
    [SerializeField]TextMeshPro targetScoreText;
    [SerializeField]TextMeshPro highScoreText;
    [SerializeField]Image enemyImage; 
    [SerializeField]TextMeshPro MemoText;
    public bool IsReady=true;
        public override bool IsReadyToGameStart()
    {
        return IsReady;
    }

    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        Show(mission);
    }
    public void Show(Mission mission){
        missionName.text=mission.MissionName;
        explainText.text=mission.ExplainText;
        enemyImage.sprite=mission.EnemySprite;
        targetScoreText.text=mission.TargetScore.ToString();
        highScoreText.text=mission.HighScore.ToString();
        MemoText.text=mission.MemoText;
        foreach(var playButton in playButtons){
            playButton.SetMission(mission);
        }
        
    }

}
