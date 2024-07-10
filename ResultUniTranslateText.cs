
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ResultUniTranslateText : UniTranslateText
{
    void Start()
    {
        
    }
    [SerializeField]MissionManager missionManager;

    public override string GetText(){
        var text=base.GetText();
        text+="\n";
        text+=missionManager.ALlMissionScore();
        return text;

    }
}
