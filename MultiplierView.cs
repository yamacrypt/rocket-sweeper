
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MultiplierView : UdonSharpBehaviour
{
    [SerializeField]TextMeshProUGUI scoreMultiplierText;
    [SerializeField]TextMeshProUGUI gravityMultiplierText;
    void Start(){
        Hide();
    }
    public void SetMultiplier(int score,float gravity){
        scoreMultiplierText.text = $"Score × {(float)score}";
        gravityMultiplierText.text = $"Speed × {gravity}";
    }
    public void Hide(){
        scoreMultiplierText.enabled=false;
        gravityMultiplierText.enabled=false;
    }
    public void ShowTemp(float time=1f){
        scoreMultiplierText.enabled=true;
        gravityMultiplierText.enabled=true;
        SendCustomEventDelayedSeconds(nameof(Hide),time);
    }
}
