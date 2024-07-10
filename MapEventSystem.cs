
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapEventSystem : UdonSharpBehaviour
{
    [SerializeField]IMapGenerator mapGenerator;
     public void BreakCell(GameObject cell){
        mapGenerator.BreakCell(cell);
     }
}
