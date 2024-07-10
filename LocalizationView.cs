
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public enum Localization{
    JP,EN
}
public class LocalizationView : UdonSharpBehaviour
{
    [SerializeField]Localization localization;
    [SerializeField]GameObject parent;
    [SerializeField]UdonLab.TranslateManager translateManager;    
    [SerializeField]MissionManager missionManager;
    void Start()
    {
        
    }

    public bool Selected=>selected;
    bool selected;

    public override void Interact(){
        switch(localization){
            case Localization.JP:
                translateManager.setLanguage(0);
                translateManager.updateUI();
                missionManager.InitialSelect();
                break;
            case Localization.EN:
                translateManager.setLanguage(1);
                translateManager.updateUI();
                missionManager.InitialSelect();
                break;
        }
        parent.SetActive(false);
    }
}
