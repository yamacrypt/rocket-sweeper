
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChunkPoolItemOperator : IPoolItemOperator
{
    public override bool SetActive(GameObject obj,bool active){
         MeshCollider col = obj.GetComponent<MeshCollider>();
        MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        if(col==null || mesh==null){
            Debug.LogError("TilePoolItemOperator: BoxCollider or MeshRenderer is null!");
            return false;
        }
        col.enabled=active;
        mesh.enabled=active;
        mesh.material.SetFloat("_UpDown",0);
        return true;
    }

    public override bool IsActive(GameObject obj){
        MeshCollider col = obj.GetComponent<MeshCollider>();
        MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        if(col==null || mesh==null){
            Debug.LogError("TilePoolItemOperator: BoxCollider or MeshRenderer is null!");
            return false;
        }
        return col.enabled && mesh.enabled;
    }
}
