
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;



public class DominantHandButton : UdonSharpBehaviour
{
    [SerializeField]bool isRight=true;
    [SerializeField]Player player;
    [SerializeField]GameObject parent;
    

    public bool Selected=>selected;
    bool selected;

    public override void Interact(){
        player.isRight=isRight;
        //parent.SetActive(false);
    }
}
