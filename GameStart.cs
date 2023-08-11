
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GameStart : UdonSharpBehaviour
{
    [SerializeField]MapGenerator mapGenerator;
    public override void Interact(){
        mapGenerator.GenerateInit();
    }
}
