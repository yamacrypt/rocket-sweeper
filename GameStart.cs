
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GameStart : UdonSharpBehaviour
{
    [SerializeField]IMessenger messenger;
    void Start()
    {

    }
    bool isStart=false;
    public override void Interact(){
        //messenger.Publish(messenger,GameMessage.GameStart);
        isStart=true;
    }
}
