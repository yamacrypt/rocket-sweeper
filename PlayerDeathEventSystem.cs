
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerDeathEventSystem : UdonSharpBehaviour
{

    [SerializeField]IMessenger messanger;

    public void GameOver(){
        Debug.Log("GameOver");
        messanger.Publish(messanger,GameMessage.GameOver);
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
       GameOver();
    }
}
