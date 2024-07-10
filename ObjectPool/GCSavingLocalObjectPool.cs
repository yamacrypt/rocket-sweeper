
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonObjectPool{
    
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class GCSavingLocalObjectPool : IObjectPool
    {
        [SerializeField]IObjectPoolItem[] pool; 
        public override IObjectPoolItem[] Pool=>pool; 
        [UnrollAttribute,SerializeField]IntKeyIntDictionary dic;
        // shuffle周りでバグありそう
        void Start()
        {
            for(int i=0;i<Pool.Length;i++){
                Pool[i].SetActive(false);
            }
            int maxSpawnCount = actives.Length;
            if(Pool.Length>maxSpawnCount){
                Debug.LogError("UdonObjectPool Setting Error: Pool Length must be less than"+maxSpawnCount);
            }
            dic.SetCapacity(capcity*10);
        }
        const int capcity=200;
        bool[] actives=new bool[capcity]; // You can change this value to fit your needs.

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

        int index=0;
        public override  GameObject TryToSpawn(){
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
            dic.AddOrSetValue(obj.gameObject.GetInstanceID(),target);
            var res =obj.SetActive(true);
            if(res){
                actives[target]=true;
                index=shuffles[(target+1)%Pool.Length];
            }
            return obj.gameObject;
        }
        public override  void Return(GameObject obj){
            //if(obj==null)return;
            var index=dic.GetValueOrDefault(obj.gameObject.GetInstanceID(),-1);
            if(index==-1){
                Debug.LogWarning("Return failed");
                return;
            }
            var res=pool[index].SetActive(false);
            if(res){
                actives[index]=false;
            }else{
                Debug.Log("Return failed");
            }
        }

        public  override void Shuffle(){
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
            for(int i=0;i<Pool.Length;i++){
                Pool[i].SetActive(false);
                actives[i]=false;
            }
            index=0;
        }


    }
}