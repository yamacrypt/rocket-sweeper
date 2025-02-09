﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonObjectPool{
    public enum ActiveMode{
        Undefined,Active,Deactive
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ObjectPoolItem : IObjectPoolItem
    {
        [SerializeField]SkinnedMeshRenderer[] skinnedMeshRenderers;
        [SerializeField]MeshRenderer[] meshRenderers;
        [SerializeField]Rigidbody[] rigidbodies;
        [SerializeField]CharacterController characterController;
        [SerializeField]BoxCollider[] boxColliders;
        [SerializeField]CapsuleCollider[] capsuleColliders;
        [SerializeField]SphereCollider[] sphereColliders;

        [SerializeField]MeshCollider[] meshColliders;

        [SerializeField]ParticleSystem[] particleSystems;

        [SerializeField]UdonSharpBehaviour[] callbacks;

        [SerializeField]SpriteRenderer[] spriteRenderers;
        [SerializeField]AudioSource audioSource;
        [SerializeField]Animator animator;

        [SerializeField]bool useInitialPos=true;

        Vector3 initialPos;

        public ActiveMode ActiveMode=>activeMode;

        ActiveMode activeMode=ActiveMode.Undefined;

        public override bool SetActive(bool active,bool forceChange=false){
            if(!forceChange){
                if(activeMode==ActiveMode.Active && active)return false;
                if(activeMode==ActiveMode.Deactive && !active)return false;
            }
            activeMode = active?ActiveMode.Active:ActiveMode.Deactive;
            if(active && useInitialPos){
                transform.position=initialPos;
            }
            Toggle(active);
            return true;
        }

        void Toggle(bool active){
                for(int i=0;i<skinnedMeshRenderers.Length;i++){
                    skinnedMeshRenderers[i].enabled=active;
                }
                for(int i=0;i<meshRenderers.Length;i++){
                    meshRenderers[i].enabled=active;
                }
                for(int i=0;i<rigidbodies.Length;i++){
                    rigidbodies[i].isKinematic=!active;
                }
                for(int i=0;i<boxColliders.Length;i++){
                    boxColliders[i].enabled=active;
                }
                for(int i=0;i<capsuleColliders.Length;i++){
                    capsuleColliders[i].enabled=active;
                }
                for(int i=0;i<sphereColliders.Length;i++){
                    sphereColliders[i].enabled=active;
                }
                for(int i=0;i<meshColliders.Length;i++){
                    meshColliders[i].enabled=active;
                }
                for(int i=0;i<particleSystems.Length;i++){
                    if(active){
                        particleSystems[i].Play();
                    }else{
                        particleSystems[i].Stop();
                    }
                }
            if(characterController!=null){
                characterController.enabled=active;
            }
                for(int i=0;i<spriteRenderers.Length;i++){
                    spriteRenderers[i].enabled=active;
                }
            if(audioSource!=null){
                if(active){
                    audioSource.Stop();
                    audioSource.Play();
                } else {
                    audioSource.Stop();
                }
            }
            if(animator!=null){
                animator.enabled=active;
            }
                for(int i=0;i<callbacks.Length;i++){
                
                callbacks[i].SendCustomEvent(active?"_OnEnable":"_OnDisable");
                }
        }

        // This method does not change activeMode and teleport attached object to initial pos.
        // This is mainly used to return synced pool item by non-owner player 
        public override bool _SetActive(bool active){
            if(activeMode==ActiveMode.Active && active)return false;
            if(activeMode==ActiveMode.Deactive && !active)return false;
            Toggle(active);
            return true;
        }
        void OnEnable()
        {
            initialPos=transform.position;
        }
    }
}
