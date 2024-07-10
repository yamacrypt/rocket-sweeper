
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ResultToMission : GameLifeCycle
{
    [SerializeField]GameObject resultView;  
    [SerializeField]GameObject missionView;  
    public override void Interact(){
        resultView.SetActive(false);
        missionView.SetActive(true);
        meshRenderer.enabled = false;
        interactCollider.enabled = false;
    }

    void Start()
    {
        meshRenderer=GetComponent<MeshRenderer>();
        interactCollider=GetComponent<BoxCollider>();
    }

    MeshRenderer meshRenderer;
    BoxCollider interactCollider;

    public override void GameStart(Mission mission)
    {
        base.GameStart(mission);
        meshRenderer.enabled = false;
        interactCollider.enabled = false;
    }

    public override void GameOver()
    {
        base.GameOver();
        meshRenderer.enabled = true;
        interactCollider.enabled = true;
    }
}
