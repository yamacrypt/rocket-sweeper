
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
        /*if(col==null||mesh==null){
            Debug.LogError("TilePoolItemOperator: MeshCollider or MeshRenderer is null!");
            return false;
        } gc alloc */
        col.enabled=active;
        mesh.enabled=active;
        //mesh.material.SetFloat("_UpDown",0);// コメントアウトによってshaderによる落下アニメーションは機能しなくなる
        return true;
    }
    //[SerializeField,UnrollAttribute]IntToMeshColliderDictionary idToColDict;
    //[SerializeField,UnrollAttribute]IntToMeshRendererDictionary idToMeshDict;

    /*[SerializeField]int capacity=100000;

    void Start()
    {
        idToColDict.SetCapacity(capacity);
        idToMeshDict.SetCapacity(capacity);
    }*/

    public override bool SetActive(GameObject obj,int instanceID,bool active){
        return SetActive(obj,active);
        /*MeshCollider col;
        if(idToColDict.HasItem(instanceID)){
            col=idToColDict.GetValue(instanceID);
        }else{
            col = obj.GetComponent<MeshCollider>();
            idToColDict.Add(instanceID,col);
        }
        MeshRenderer mesh;
        if(idToMeshDict.HasItem(instanceID)){
            mesh=idToMeshDict.GetValue(instanceID);
        }else{
            mesh = obj.GetComponent<MeshRenderer>();
            idToMeshDict.Add(instanceID,mesh);
        }
        col.enabled=active;
        mesh.enabled=active;
        return true;*/
    }


    public override bool IsActive(GameObject obj){
        MeshCollider col = obj.GetComponent<MeshCollider>();
        MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        if(col==null|| mesh==null){
            Debug.LogError("TilePoolItemOperator: MeshCollider or MeshRenderer is null!");
            return false;
        }
        return mesh.enabled;
    }
}
