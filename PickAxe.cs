
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PickAxe : UdonSharpBehaviour
{
    [SerializeField]MapGenerator mapGenerator;
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Try Destrou"+ other.gameObject.name);
        mapGenerator.UnloadCell(other.gameObject);
    }
}
