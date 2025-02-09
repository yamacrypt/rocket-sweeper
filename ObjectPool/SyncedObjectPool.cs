﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonObjectPool{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncedObjectPool : IObjectPool
    {
        [SerializeField]IObjectPoolItem[] pool; 
        public override IObjectPoolItem[] Pool=>pool; 
        void Start()
        {
            for(int i=0;i<Pool.Length;i++){
                Pool[i].SetActive(false);
            }
            if(Pool.Length>poolMaxSize){
                Debug.LogError("WARN Pool.Length<"+poolMaxSize);
            }
        }
        [UdonSynced]bool[] actives=new bool[poolMaxSize];
        [UdonSynced]bool[] syncedChanges=new bool[poolMaxSize];
        bool[] _changes=null;
        const int poolMaxSize=200;

        bool[] changes{
            get{
                if(_changes==null){
                    _changes=new bool[Pool.Length];
                }
                return _changes;
            }
            set{
                _changes=value;
            }
        }

        int[] _shuffles=null;
        int[] shuffles{
            get{
                if(_shuffles==null){
                    _shuffles=new int[Pool.Length];
                    for(int i=0;i<Pool.Length;i++){
                        _shuffles[i]=i;
                    }
                }
                return _shuffles;
            }
            set{
                _shuffles=value;
            }
        }

        public override void OnDeserialization() { 
            for(int i=0;i<Pool.Length;i++){
                Pool[i].SetActive(actives[i],syncedChanges[i]);
            }
        }

        public override void OnPreSerialization() { 
            for(int i=0;i<Pool.Length;i++){
                syncedChanges[i]=changes[i];
                changes[i]=false;
            }
        }
        int index=0;
        public override  GameObject TryToSpawn(){
            //Debug.Log("TryToSpawn");
            if(!Networking.IsOwner(Networking.LocalPlayer,gameObject))return null;
            var next=0;
            var target=0;
            while(next<Pool.Length){
                target=shuffles[(index+next)%Pool.Length];
                if(actives[target]){
                    next++;
                }else{
                    break;
                }
            }
            if(next==Pool.Length)return null;
            var obj=Pool[target];
            var res =obj.SetActive(true);
            if(res){
                actives[target]=true;
                changes[target]=true;
                RequestSerialization();
                index=shuffles[(target+1)%Pool.Length];
            }
            return obj.gameObject;
        }
        public override  void Return(GameObject obj){
            if(obj==null)return;
            for(int i=0;i<Pool.Length;i++){
                if(Pool[i].gameObject==obj){
                    if(Networking.IsOwner(Networking.LocalPlayer,gameObject)){
                        var res =Pool[i].SetActive(false);
                        if(res){
                            actives[i]=false;
                            changes[i]=true;
                            RequestSerialization();
                        }
                    } else {
                        Pool[i]._SetActive(false);
                    }
                    return;
                }
            }
            Debug.Log("Return failed");
            Debug.Log("Return obj is " +obj);
        }
    
        public  override void Shuffle(){
            if(!Networking.IsOwner(Networking.LocalPlayer,gameObject))return;
            index=0;
            for(int i=0;i<Pool.Length;i++){
                var target = (index+i)%Pool.Length;
                var r=Random.Range(0,Pool.Length);
                var tmp=shuffles[target];
                shuffles[target]=shuffles[r];
                shuffles[r]=tmp;
            }
        }

        public  override void Clear(){
            if(!Networking.IsOwner(Networking.LocalPlayer,gameObject))return;
            for(int i=0;i<Pool.Length;i++){
                Pool[i].SetActive(false);
                actives[i]=false;
                changes[i]=true;
            }
            RequestSerialization();
            index=0;
        }
    }
}