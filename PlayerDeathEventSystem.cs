
using mmmsys;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerDeathEventSystem : GameLifeCycle
{
    [SerializeField]TransformTransporter enemySample;
    [SerializeField]IMessenger messanger;
    //[SerializeField]AudioSource hitSound;

    public override void GameOverCallback()
    {
        //if(hitSound!=null)hitSound.Play();
        if(enemySample!=null){
            var obj=enemySample.target;
            var hitSound=obj.GetComponent<AudioSource>();
            if(hitSound!=null)hitSound.Play();
            var horrorEffect=obj.GetComponent<GameOverDirection>();
            if(horrorEffect!=null)horrorEffect._Interact();
            else Debug.LogWarning("horrorEffect is null");
        } else{
            Debug.LogWarning("enemySample is null");
        }
        base.GameOverCallback();
    }
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if(player==Networking.LocalPlayer){
            messanger.Publish(this.gameObject,GameMessage.GameOver);
        }
    }
}
