
using BuildSoft.UdonSharp.Collection;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class UdonSavingObjectPool : UdonSharpBehaviour
{
    [SerializeField]GameObject prefab;
    [SerializeField]IntKeyGameObjectDictionary idToObjDict;
    [SerializeField]int InitGenerateCount=0;
    [SerializeField]int Capacity=100000;
    [SerializeField]IPoolItemOperator itemOperator;
    int poolSize;
    [SerializeField]IntQueue notActiveObjQueue;
    [SerializeField]Transform parent;
    void Start()
    {
        poolSize=InitGenerateCount;
        idToObjDict.SetCapacity(Capacity);
        notActiveObjQueue.SetCapacity(Capacity);
        for(int i=0;i<InitGenerateCount;i++){
            int id;GameObject instance;
            do{
            instance = Object.Instantiate(prefab,Vector3.zero,Quaternion.identity,parent);
            if(instance==null){
                Debug.LogError("UdonObjectPool Setting Error: Prefab is null");
            }
            id = instance.GetInstanceID().GetHashCode();
            }while(!idToObjDict.Add(id,instance));
            itemOperator.SetActive(instance,false);
            notActiveObjQueue.Enqueue(id);
        }
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
        do{
        instance = Object.Instantiate(prefab,Vector3.zero,Quaternion.identity,p==null?parent:p);
        if(instance==null){
            Debug.LogError("UdonObjectPool Setting Error: Prefab is null");
        }
        id = instance.GetInstanceID().GetHashCode();
        itemOperator.SetActive(instance,false);
        }while(!idToObjDict.Add(id,instance));
        notActiveObjQueue.Enqueue(id);
    }
    // falseで出してmeshcombinerで切り替える
    public GameObject TryToSpawn(Transform p=null){
        int id;
        isInstantiated=true;
        if(notActiveObjQueue.Count>0){
            isInstantiated=false;
            id = notActiveObjQueue.Dequeue();
            if(idToObjDict.HasItem(id)){
                var obj= idToObjDict.GetValue(id);
                itemOperator.SetActive(obj,false);
                return obj;
            } else {
                Debug.LogWarning("UdonObjectPool Error: InstanceID is not found!");
            }
        }
        GameObject instance;
        do{
            instance = Object.Instantiate(prefab,Vector3.zero,Quaternion.identity,p==null?parent:p);
            if(instance==null){
                Debug.LogError("UdonObjectPool Setting Error: Prefab is null");
            }
            id = instance.GetInstanceID().GetHashCode();
        }while(!idToObjDict.Add(id,instance));
        //if(idToObjDict.HasItem(id))Debug.LogError("UdonObjectPool Error: InstanceID is duplicated!");
        return instance;
    }
    public  void Return(GameObject obj,bool force=false){
        if(obj==null)return;
        int id = obj.GetInstanceID().GetHashCode();
        if(!idToObjDict.HasItem(id)){
            Debug.LogWarning("UdonObjectPool Error: InstanceID is not found!");
            return;
        }
        // comment out ok
        var isActive=itemOperator.IsActive(obj);
        if(!isActive && !force){
            Debug.LogWarning("UdonObjectPool Error: Object is already returned!");
            return;
        }
        //
        itemOperator.SetActive(obj,false);
        notActiveObjQueue.Enqueue(id);
        
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
