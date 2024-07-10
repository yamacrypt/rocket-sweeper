
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EnemyIsGrounded : UdonSharpBehaviour
{
    [SerializeField]Renderer[] meshRenderers;
    [SerializeField]BoxCollider triggerCollider;
    [SerializeField]BoxCollider isGroundedCollider;
    void OnTriggerEnter(Collider other)
    {
        if(!triggerCollider.enabled){
            foreach(Renderer renderer in meshRenderers){
                renderer.enabled=true;
            }
            triggerCollider.enabled=true;
            isGroundedCollider.enabled=false;
        }
    }

    public void _OnEnable(){
        foreach(Renderer renderer in meshRenderers){
                renderer.enabled=false;
            }
        triggerCollider.enabled=false;
        isGroundedCollider.enabled=true;
    }

    public void _OnDisable()
    {
        foreach(Renderer renderer in meshRenderers){
                renderer.enabled=false;
            }
        triggerCollider.enabled=false;
        isGroundedCollider.enabled=false;
    }

}
