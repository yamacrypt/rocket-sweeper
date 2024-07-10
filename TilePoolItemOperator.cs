
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TilePoolItemOperator : IPoolItemOperator
{

    public override bool SetActive(GameObject obj,bool active){
        BoxCollider col = obj.GetComponent<BoxCollider>();
        MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        /*if(col==null || mesh==null){
            Debug.LogError("TilePoolItemOperator: BoxCollider or MeshRenderer is null!");
            return false;
        } //gc alloc */
        col.enabled=active;
        mesh.enabled=active;
        //  mesh.material.SetFloat("_UpDown",0); // コメントアウトによってshaderによる落下アニメーションは機能しなくなる
        return true;
    }
     public override bool SetColliderActive(GameObject obj,bool active){
        BoxCollider col = obj.GetComponent<BoxCollider>();
        /*if(col==null || mesh==null){
            Debug.LogError("TilePoolItemOperator: BoxCollider or MeshRenderer is null!");
            return false;
        } //gc alloc */
        col.enabled=active;
        //  mesh.material.SetFloat("_UpDown",0); // コメントアウトによってshaderによる落下アニメーションは機能しなくなる
        return true;
    }

    //[SerializeField]int capacity=300000;

    void Start()
    {
        //idToColDict.SetCapacity(capacity);
        //idToMeshDict.SetCapacity(capacity);
    }

    //[SerializeField,UnrollAttribute]IntToBoxColliderDictionary idToColDict;
    //[SerializeField,UnrollAttribute]IntToMeshRendererDictionary idToMeshDict;
    BoxCollider col;
    MeshRenderer mesh;
    public override bool SetActive(GameObject obj,int instanceID,bool active){
        /*if(idToColDict.HasItem(instanceID)){
            col=idToColDict.GetValue(instanceID);
        }else{
            col = obj.GetComponent<BoxCollider>();
            idToColDict.Add(instanceID,col);
        }
        if(idToMeshDict.HasItem(instanceID)){
            mesh=idToMeshDict.GetValue(instanceID);
        }else{
            mesh = obj.GetComponent<MeshRenderer>();
            idToMeshDict.Add(instanceID,mesh);
        }*/
        col = obj.GetComponent<BoxCollider>();
        mesh = obj.GetComponent<MeshRenderer>();
        /*if(col==null || mesh==null){
            Debug.LogError("TilePoolItemOperator: BoxCollider or MeshRenderer is null!");
            return false;
        }//gc alloc */
        col.enabled=active;
        mesh.enabled=active;
        //  mesh.material.SetFloat("_UpDown",0); // コメントアウトによってshaderによる落下アニメーションは機能しなくなる
        return true;
    }

    public override bool IsActive(GameObject obj){
        BoxCollider col = obj.GetComponent<BoxCollider>();
        MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
        if(col==null || mesh==null){
            Debug.LogError("TilePoolItemOperator: BoxCollider or MeshRenderer is null!");
            return false;
        }
        return col.enabled && mesh.enabled;
    }
    
}
