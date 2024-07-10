
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EnemyIsNear : UdonSharpBehaviour
{
  

    [SerializeField]AudioSource audioSource;

    void OnTriggerEnter(Collider other)
    {
        var basePos=this.transform.position;
        var dir=other.gameObject.transform.position - basePos;
        dir.Normalize();
        audioSource.transform.position = basePos + dir;
        if(!audioSource.isPlaying){
            audioSource.Play();
        }
    }
}
