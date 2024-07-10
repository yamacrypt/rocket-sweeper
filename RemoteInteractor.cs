
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RemoteInteractor : UdonSharpBehaviour
{
    [SerializeField]EventEmitterUI[] targets;
    void Start()
    {
        
    }
    public override void Interact()
    {
        foreach(var target in targets){
            target.Interact();
        }
    }
}
