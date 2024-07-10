
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameTimer : GameLifeCycle
{
    [SerializeField]float timeLimit=90f;
    [SerializeField]IMessenger messenger;
    bool countDown=false;
    [SerializeField]AudioSource countDownSE;
    [SerializeField]AudioSourceExt gameBGM;
    [SerializeField]AudioSourceExt roomBGM;
    [SerializeField]GameOverDirection gameOverDirection;
    
    void Update()
    {
        if(!timeStart)return;
        if(time<timeLimit){
            time+=Time.deltaTime;
            if(time+3>=timeLimit&&!countDown){
                countDown=true;
                countDownSE.Play();
            }
            if(time>=timeLimit){
                messenger.Publish(this.gameObject,GameMessage.GameOver);
            }
        }
    }
    float time=0;
    bool timeStart=false;
    public override bool IsReadyToGameStart(){
        return base.IsReadyToGameStart()&&!timeStart;;
    }

    public override void GameOverCallback()
    {
        base.GameOverCallback();
        if(gameOverDirection!=null)gameOverDirection._Interact();
    }

    public override void GameStart(Mission mission) {
        base.GameStart(mission);
        timeStart=true;
        time=0;
        countDown=false;
        roomBGM.FadeOut(2);
        SendCustomEventDelayedSeconds(nameof(PlayGameBGM),2);
    }

    public void PlayGameBGM(){
        gameBGM.Play();

    }

    public void PlayRoomBGM(){
        roomBGM.Play();

    }


    public override void GameOver(){
        base.GameOver();
        timeStart=false;
        gameBGM.FadeOut(2);
        //SendCustomEventDelayedSeconds(nameof(PlayRoomBGM),7);
    }
}
