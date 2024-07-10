
using System;
using UdonObjectPool;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MixedObjectPool : IObjectPool
{
    [SerializeField]IObjectPool[] pools;
    [SerializeField]int[] rates;
    [SerializeField]IntKeyIntDictionary dic;
     const int capcity=200;
    void Start()
    {
        dic.SetCapacity(capcity*pools.Length*10);
    }
    int RateSum(){
        int sum=0;
        for(int i=0;i<rates.Length;i++){
            sum+=rates[i];
        }
        return sum;
    }
    int RateIndex(int _num){
        int num=_num;
        int index=0;
        for(int i=0;i<rates.Length;i++){
            if(num<rates[i]){
                index=i;
                break;
            }
            num-=rates[i];
        }
        return index;
    }
    public override GameObject TryToSpawn(){
        var rateSum=RateSum();
        var rand=UnityEngine.Random.Range(0,rateSum);
        var index=RateIndex(rand);
        var obj=pools[index].TryToSpawn();
        dic.AddOrSetValue(obj.GetInstanceID(),index);
        return obj;

    }

    public override void Return(GameObject obj){
        var index=dic.GetValueOrDefault(obj.GetInstanceID(),-1);
        if(index>=0)pools[index].Return(obj);
        else{
            Debug.LogWarning("Return pool index is invalid");
        }
    }

    public override void Shuffle(){
        foreach(var pool in pools){
            pool.Shuffle();
        }
    }

    public override void Clear(){
        foreach(var pool in pools){
            pool.Clear();
        }
    }
}
