
using BuildSoft.UdonSharp.Collection;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
public enum PoolOperatorType{
    TryToSpawn,Return,Store,Clear
}
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class UdonSavingObjectPool : UdonSharpBehaviour
{
    [SerializeField]GameObject prefab;
    public void SetPrefab(GameObject prefab){
        this.prefab=prefab;
    }
    [SerializeField][UnrollAttribute]IntKeyGameObjectDictionary idToObjDict;
    [SerializeField]int Capacity=50021;
    [SerializeField][UnrollAttribute]IPoolItemOperator itemOperator;
    int poolSize;
    [SerializeField][UnrollAttribute]IntQueue notActiveObjQueue;
    //[SerializeField]Transform parent;
    void Start()
    {
        idToObjDict.SetCapacity(Capacity);
        notActiveObjQueue.SetCapacity(Capacity);
    }
    public bool PeekIsInstantiated(){
        return isInstantiated;
    }
    bool isInstantiated=false;

    public void Store(Transform p=null){
        if(idToObjDict.Count>idToObjDict.KeyLength)return;
        isInstantiated=true;
        GameObject instance;
        int id;
        //do{
        instance = GameObject.Instantiate(prefab/*,Vector3.zero,Quaternion.identity,p==null?parent:p*/);
        //gc alloc debug only
        /*if(instance==null){
            Debug.LogError("UdonObjectPool Setting Error: Prefab is null");
        }*/
        //id = GetInstanceID(instance);/*instance.GetInstanceID();*///.GetHashCode();
        id =instance.GetInstanceID();
        itemOperator.SetActive(instance,id,false);
        idToObjDict.Add(id,instance);
        notActiveObjQueue.Enqueue(id);
    }
    GameObject instance;
    // falseで出してmeshcombinerで切り替える
    int uniqueId=0;
    public GameObject TryToSpawn(){
        int id;
        isInstantiated=true;
        if(notActiveObjQueue.Count>0){
            isInstantiated=false;
            id = notActiveObjQueue.Dequeue();
            if(idToObjDict.HasItem(id)){
                var obj= idToObjDict.GetValue(id);
                //itemOperator.SetActive(obj,id,true);
                return obj;
            } else {
                Debug.LogWarning("UdonObjectPool Error: InstanceID is not found!");
            }
        }
        //do{
        instance = GameObject.Instantiate(prefab/*,Vector3.zero,Quaternion.identity,p==null?parent:p*/);
        //gc alloc
        /*if(instance==null){
            Debug.LogError("UdonObjectPool Setting Error: Prefab is null");
        }*/
        id =instance.GetInstanceID();
        idToObjDict.Add(id,instance);// gc alloc
        //itemOperator.SetActive(instance,id,true);
        //}while(!idToObjDict.Add(id,instance));
        //if(idToObjDict.HasItem(id))Debug.LogError("UdonObjectPool Error: InstanceID is duplicated!");
        return instance;
    }
    public GameObject TryToSpawn(int id){
        isInstantiated=false;
        if(idToObjDict.HasItem(id)){
            var obj= idToObjDict.GetValue(id);
            itemOperator.SetActive(obj,id,false);
            return obj;
        } else {
            Debug.LogWarning("UdonObjectPool Error: InstanceID is not found!");
            return null;
        }
    }

    public  void Return(GameObject obj,bool force=false,bool enqueue=true){
        // gc alloc debug only
        //if(obj==null)return; // gc alloc debug only
        int id = obj.GetInstanceID();//obj.GetInstanceID();
        if(!idToObjDict.HasItem(id)){
            Debug.LogWarning("UdonObjectPool Error: InstanceID is not found!");
            return;
        }
        // comment out ok
        /*var isActive=itemOperator.IsActive(obj);
        if(!isActive && !force){
            Debug.LogWarning("UdonObjectPool Error: Object is already returned!");
            return;
        }*/
        //
        itemOperator.SetActive(obj,id,false);
        if(enqueue)notActiveObjQueue.Enqueue(id);
        
    }

     public  void Return(GameObject obj,int id,bool force=false,bool enqueue=true){
        //if(obj==null)return; // gc alloc debug only
        if(!idToObjDict.HasItem(id)){
            Debug.LogWarning("UdonObjectPool Error: InstanceID is not found!");
            return;
        }
        // comment out ok
        /*var isActive=itemOperator.IsActive(obj);
        if(!isActive && !force){
            Debug.LogWarning("UdonObjectPool Error: Object is already returned!");
            return;
        }*/
        //
        itemOperator.SetActive(obj,id,false);
        if(enqueue)notActiveObjQueue.Enqueue(id);
        
    }

    public bool IsMine(GameObject obj){
        return obj.name.Contains(prefab.name);
    }


    public   void Clear(){
        notActiveObjQueue.Clear();
        foreach(var obj in idToObjDict.GenerateKeysArray()){
            itemOperator.SetActive(idToObjDict.GetValue(obj),false);
            notActiveObjQueue.Enqueue(obj);
        }
    }
}
