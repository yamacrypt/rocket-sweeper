
using System;
using System.Text.RegularExpressions;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[AttributeUsage(AttributeTargets.Field)]
public class UnrollAttribute : Attribute
{

}
[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class InterfaceToClassAttribute : Attribute
{
   public string InterfaceName;
   public string ClassName;
    public InterfaceToClassAttribute(string interfaceName,string className)
    {
        this.InterfaceName=interfaceName;
        this.ClassName=className;
    }

 
}
public enum TestEnum{Test}
public class CollenctionTest : UdonSharpBehaviour
{
    [SerializeField]GameObject[] obj;
    [SerializeField]IntKeyGameObjectArrDictionary dict;
    [SerializeField]IntKeyBoolDictionary dict2;
    [SerializeField]IntQueue queue;
    [SerializeField]GameObject empty;
    [SerializeField]Material material;
    [SerializeField]UdonSavingObjectPool _pool;
    [SerializeField]UdonSavingObjectPool[] _pool2;
    UdonSavingObjectPool[] poolArr;
    UdonSavingObjectPool pool => poolArr[0];
    int chunkWidth=100;
    int chunkDepth=100;
     int GetChunkIndex(int x,int z){
        return (z-ZSTARTINDEX)*chunkWidth + (x-XSTARTINDEX);
    }

    Vector3 FromChunkIndex(int chunkIndex){
        return new Vector3(chunkIndex%chunkWidth +XSTARTINDEX,0,chunkIndex/chunkWidth+ZSTARTINDEX);
    }

    public void Spawn(){
        var o=pool.TryToSpawn();
        GameObject.Instantiate(o);
        startSpawn=true;
    }
    UdonSavingObjectPool a;
    bool startSpawn;
    int i;
    Vector3 zero;
    Vector3 one=new Vector3(1,1,1);
    Quaternion identity;
    GameObject[] objArr;
    string strTest="test";
    [SerializeField]UnrolledMapGenerator generator;
    int GetInRangeStartZChunkIndex(Vector3 pos){
        return 0;
    }
    void Update()
    {
        //var hoge=new GameObject[10];
        for(i=0;i<40;i++){
            // no alloc
            //float a1=one.x;
            //float a2=one.y;
            //float a3=one.z;
            //Mathf.PerlinNoise(i,i);
            //var ka2=ka.Contains(strTest);
            /*foreach(var aa in str){
                var k=aa;
            }*/
            GetInRangeStartZChunkIndex(one);
            //var a= new GameObject[5];
            //var c=obj;
            //Debug.Log(obj[0]!=null);
            //obj[0]=null;
            //obj=null;
            //GameObject[] w=objArr;
            //Debug.Log(obj[0]==null);
            //Debug.Log(temp!=null);

            // alloc
            //var ls="test_123".Split('_');
            //var dd=int.Parse(ls[ls.Length-1]);
            //Debug.Log(dd);
            //var ka=empty.name;
            //zero=one;
            //zero.x=1;
            //a.x=1;
            //var a=this.name;
            //var b=_pool.name;
            //var obj=Instantiate(empty/*,one,identity,transform*/);
            //obj.GetInstanceID();
            //obj.GetInstanceID();
            //obj.GetComponent<MeshFilter>();
            //objArr[0]=obj;
            //var a=Vector4.one;
            
            //var a=TestEnum.Test;
            //_pool2[0].PeekIsInstantiated();
        }
    }

    [SerializeField,UnrollAttribute]IntToMeshRendererDictionary iToMdic;
    void Start()
    {
        objArr=new GameObject[10];
        var hoge=new GameObject[10];
        for(i=0;i<10;i++){
            // no alloc
            //float a1=one.x;
            //float a2=one.y;
            //float a3=one.z;
            //Mathf.PerlinNoise(i,i);
            //var ka2=ka.Contains(strTest);
            /*foreach(var aa in str){
                var k=aa;
            }*/
            GameObject temp;
            //var c=obj;
            //Debug.Log(obj[0]!=null);
            temp=objArr[i];
            hoge[i]=temp;
            //obj[0]=null;
            //obj=null;
            //GameObject[] w=objArr;
            //Debug.Log(obj[0]==null);
            //Debug.Log(temp!=null);
        }
        for(i=0;i<10;i++){
            objArr[i]=null;
            Debug.Log(hoge[i]!=objArr[i]);
        }
        if(!startSpawn)return;
        
        iToMdic.SetCapacity(10);
        iToMdic.AddOrSetValue(0,GetComponent<MeshRenderer>());
        zero=new Vector3(0,0,0);
        identity=Quaternion.identity;
        poolArr=new UdonSavingObjectPool[]{_pool};
        GameObject newObj=Instantiate(empty);
        newObj.GetComponent<MeshRenderer>().sharedMaterial=material;
        SendCustomEventDelayedFrames(nameof(Spawn),3);
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
