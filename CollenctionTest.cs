
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CollenctionTest : UdonSharpBehaviour
{
    [SerializeField]GameObject[] obj;
    [SerializeField]IntKeyGameObjectArrDictionary dict;
    [SerializeField]IntKeyBoolDictionary dict2;
    [SerializeField]IntQueue queue;
    int chunkWidth=100;
    int chunkDepth=100;
     int GetChunkIndex(int x,int z){
        return (z-ZSTARTINDEX)*chunkWidth + (x-XSTARTINDEX);
    }

    Vector3 FromChunkIndex(int chunkIndex){
        return new Vector3(chunkIndex%chunkWidth +XSTARTINDEX,0,chunkIndex/chunkWidth+ZSTARTINDEX);
    }
    void Start()
    {
        XSTARTINDEX=-chunkWidth/2;
        XENDINDEX=chunkWidth-1-chunkWidth/2;
        ZSTARTINDEX=0;
        ZENDINDEX=chunkDepth;

        GameObject oobj;
        Assert(obj[0].gameObject!=obj[1].gameObject);
        oobj=obj[0].gameObject;
        var oobj2=oobj;
        Assert(oobj==oobj2);
        oobj2=null;
        Assert(oobj!=oobj2);
        Debug.Log("gameObject judhe end");

        Vector3 pos=new Vector3(-50,0,0);
        var index=GetChunkIndex((int)pos.x,(int)pos.z);
        var fromPos=FromChunkIndex(index);
        Assert(pos==fromPos);
        pos=new Vector3(49,0,0);
        index=GetChunkIndex((int)pos.x,(int)pos.z);
        fromPos=FromChunkIndex(index);
        Assert(pos==fromPos);
        pos=new Vector3(49,0,1);
        index=GetChunkIndex((int)pos.x,(int)pos.z);
        fromPos=FromChunkIndex(index);
        Assert(pos==fromPos);

         Debug.Log("pos judhe end");
        

        dict.SetCapacity(1000);
        dict2.SetCapacity(1000);
        Assert(dict.Count==0);
        foreach(var o in obj){
            Debug.Log(o.GetInstanceID());
            dict.Add(o.GetInstanceID(),new GameObject[]{o});
        }
        Assert(dict.Count==obj.Length);
         foreach(var o in obj){
            Assert(dict.HasItem(o.GetInstanceID()));
            dict.GetValue(o.GetInstanceID());
        }

        queue.Enqueue(100);
        queue.Enqueue(102);
        Assert(queue.Dequeue()==100);
        queue.Enqueue(101);
        Assert(queue.Dequeue()==102);
        Assert(queue.Dequeue()==101);
        /*foreach(var o in dict.GetKeysArray()){
            if(o==0)break;
            Assert(dict.HasItem(o));
            Debug.Log(o);
        }*/
        //Assert(dict.HasItem(obj[0].GetInstanceID()));
        Debug.Log("done");
        /*dict.GetValue(obj[0].GetInstanceID());
        //Assert(dict.GetValue(obj[0].GetInstanceID())[0]==obj[0]);
        dict2.SetValue(100,true);
        Assert(dict2.GetValue(100));*/

        
    }
    int XSTARTINDEX,XENDINDEX,ZSTARTINDEX,ZENDINDEX;

    void Assert(bool b){
        if(!b){
            Debug.LogError("Assertion failed!");
        }
    }
}
