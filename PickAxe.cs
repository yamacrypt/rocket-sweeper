
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PickAxe : UdonSharpBehaviour
{
    [SerializeField]IMapGenerator mapGenerator;
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Try Destrou"+ other.gameObject.name);
        mapGenerator.BreakCell(other.gameObject);
    }
}
