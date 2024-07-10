
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TempGameOverIsReady : GameLifeCycle
{
    void Start()
    {
        
    }
    public bool IsReady=true;
    [SerializeField]ScoreManager scoreManager;
    [SerializeField]IMapGenerator mapGenerator;
    public override void GameOver()
    {
        base.GameOver();
        scoreManager.Half();
        mapGenerator.GenerateOnSpawn();
    }

    public override bool IsReadyToGameOver()
    {
        return IsReady;
    }

    public override bool IsReadyToGameStart()
    {
        return true;
    }
}
