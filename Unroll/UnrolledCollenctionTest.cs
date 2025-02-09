﻿using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using VRC.SDKBase;
using VRC.Udon;
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UnrolledCollenctionTest : UdonSharpBehaviour
{
    [SerializeField]
    GameObject[] obj;
    [SerializeField]
    IntKeyGameObjectArrDictionary dict;
    [SerializeField]
    IntKeyBoolDictionary dict2;
    [SerializeField]
    IntQueue queue;
    [SerializeField]
    GameObject empty;
    [SerializeField]
    Material material;
    [SerializeField]
    UdonSavingObjectPool _pool;
    [SerializeField]
    UdonSavingObjectPool[] _pool2;
    UdonSavingObjectPool[] poolArr;
    UdonSavingObjectPool pool => poolArr[0];

    int chunkWidth = 100;
    int chunkDepth = 100;
    int GetChunkIndex(int x, int z)
    {
        return (z - ZSTARTINDEX) * chunkWidth + (x - XSTARTINDEX);
    }

    Vector3 FromChunkIndex(int chunkIndex)
    {
        return new Vector3(chunkIndex % chunkWidth + XSTARTINDEX, 0, chunkIndex / chunkWidth + ZSTARTINDEX);
    }

    public void Spawn()
    {
        var o = pool.TryToSpawn();
        GameObject.Instantiate(o);
        startSpawn = true;
    }

    UdonSavingObjectPool a;
    bool startSpawn;
    int i;
    Vector3 zero;
    Vector3 one = new Vector3(1, 1, 1);
    Quaternion identity;
    void Update()
    {
        if (!startSpawn)
            return;
        for (i = 0; i < 100; i++)
        {
            // no alloc
            float a1 = one.x;
            float a2 = one.y;
            float a3 = one.z;
            MeshRenderer rel;
            rel=iToMdic_GetValue(0);
        //zero=one;
        //zero.x=1;
        //a.x=1;
        //var a=this.name;
        //var b=_pool.name;
        //Instantiate(empty/*,one,identity,transform*/);
        //var a=TestEnum.Test;
        //_pool2[0].PeekIsInstantiated();
        }
    }

    int[][] iToMdic__keys;
    MeshRenderer[][] iToMdic__values;
    int iToMdic__size;
    public int iToMdic_Count => iToMdic__size;

    public int[] iToMdic_GenerateKeysArray()
    {
        int[] keys = new int[iToMdic__size];
        int index = 0;
        for (int i = 0; i < iToMdic__keys.Length; i++)
        {
            var hashKeys = iToMdic__keys[i];
            if (i == 0)
            {
                for (int j = 0; j < hashKeys.Length; j++)
                {
                    if (hashKeys[j] != int.MaxValue)
                    {
                        keys[index] = hashKeys[j];
                        index++;
                    }
                }
            }
            else
            {
                for (int j = 0; j < hashKeys.Length; j++)
                {
                    if (hashKeys[j] != default)
                    {
                        keys[index] = hashKeys[j];
                        index++;
                    }
                }
            }
        }

        return keys;
    }

    [SerializeField]
    int iToMdic_bucketCount = 4;
    public int iToMdic_KeyLength => iToMdic__keys.Length;

    public void iToMdic_SetCapacity(int capacity)
    {
        int len = capacity / iToMdic_bucketCount;
        iToMdic__keys = new int[len][];
        for (int i = 0; i < len; i++)
        {
            iToMdic__keys[i] = new int[iToMdic_bucketCount];
        }

        // 0-value-key 対策
        for (int i = 0; i < iToMdic_bucketCount; i++)
        {
            iToMdic__keys[0][i] = int.MaxValue;
        }

        iToMdic__values = new MeshRenderer[len][];
        for (int i = 0; i < len; i++)
        {
            iToMdic__values[i] = new MeshRenderer[iToMdic_bucketCount];
        }
    }

    public bool iToMdic_HasItem(int key)
    {
        var index = iToMdic_Hash(key);
        var hashKeys = iToMdic__keys[index];
        if (index == 0)
        {
            if (key == int.MaxValue)
            {
                Debug.LogError("not allowed key value");
            }
        }

        for (int i = 0; i < hashKeys.Length; i++)
        {
            if (hashKeys[i] == key)
            {
                return true;
            }
        }

        return false;
    }

    private int iToMdic_Hash(int key)
    {
        var len = iToMdic__keys.Length;
        var mod = key % len;
        if (mod >= 0)
        {
            return mod;
        }
        else
        {
            return mod + len;
        }
    // return 0 to len-1;
    }

    public bool iToMdic_Add(int key, MeshRenderer value)
    {
        int index = iToMdic_Hash(key);
        var hashKeys = iToMdic__keys[index];
        if (index == 0)
        {
            if (key == int.MaxValue)
            {
                Debug.LogError("not allowed key value");
            }

            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    Debug.LogError("IntKeyMeshRendererDictionary: Key already exists " + key.ToString());
                }

                if (hashKeys[i] == int.MaxValue)
                {
                    iToMdic__keys[index][i] = key;
                    iToMdic__values[index][i] = value;
                    iToMdic__size++;
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    Debug.LogError("IntKeyMeshRendererDictionary: Key already exists " + key.ToString());
                }

                if (hashKeys[i] == default)
                {
                    iToMdic__keys[index][i] = key;
                    iToMdic__values[index][i] = value;
                    iToMdic__size++;
                    return true;
                }
            }
        }

        //Debug.LogWarning("InkEyMeshRendererDictionary Add: Dictionary is full "+value.name);
        var preLen = hashKeys.Length;
        iToMdic_ExpandList(index);
        iToMdic__keys[index][preLen] = key;
        iToMdic__values[index][preLen] = value;
        iToMdic__size++;
        return true;
    }

    const int iToMdic_expand = 10;
    void iToMdic_ExpandList(int index)
    {
        var hashKeys = iToMdic__keys[index];
        var hashValues = iToMdic__values[index];
        var preLen = hashKeys.Length;
        var len = preLen + iToMdic_expand;
        var newKeys = new int[len];
        var newValues = new MeshRenderer[len];
        for (int i = 0; i < preLen; i++)
        {
            newKeys[i] = hashKeys[i];
            newValues[i] = hashValues[i];
        }

        if (index == 0)
        {
            for (int i = preLen; i < len; i++)
            {
                newKeys[i] = int.MaxValue;
            }
        }

        iToMdic__keys[index] = newKeys;
        iToMdic__values[index] = newValues;
    }

    public bool iToMdic_AddOrSetValue(int key, MeshRenderer value)
    {
        int index = iToMdic_Hash(key);
        var hashKeys = iToMdic__keys[index];
        if (index == 0)
        {
            if (key == int.MaxValue)
            {
                Debug.LogError("not allowed key value");
            }

            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    iToMdic__keys[index][i] = key;
                    iToMdic__values[index][i] = value;
                    return true;
                }

                if (hashKeys[i] == int.MaxValue)
                {
                    iToMdic__keys[index][i] = key;
                    iToMdic__values[index][i] = value;
                    iToMdic__size++;
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    iToMdic__keys[index][i] = key;
                    iToMdic__values[index][i] = value;
                    return true;
                }

                if (hashKeys[i] == default)
                {
                    iToMdic__keys[index][i] = key;
                    iToMdic__values[index][i] = value;
                    iToMdic__size++;
                    return true;
                }
            }
        }

        //Debug.LogWarning("InkEyMeshRendererDictionary AddOrSet: Dictionary is full");
        var preLen = hashKeys.Length;
        iToMdic_ExpandList(index);
        iToMdic__keys[index][preLen] = key;
        iToMdic__values[index][preLen] = value;
        iToMdic__size++;
        return true;
    }

    public void iToMdic_SetValue(int key, MeshRenderer value)
    {
        int index = iToMdic_Hash(key);
        var hashKeys = iToMdic__keys[index];
        if (index == 0)
        {
            if (key == int.MaxValue)
            {
                Debug.LogError("not allowed key value");
            }

            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    iToMdic__values[index][i] = value;
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    iToMdic__values[index][i] = value;
                    return;
                }
            }
        }

        Debug.LogError("Key not found");
    }

    public MeshRenderer iToMdic_GetValue(int key)
    {
        int index = iToMdic_Hash(key);
        var hashKeys = iToMdic__keys[index];
        var hashValues = iToMdic__values[index];
        if (index == 0)
        {
            if (key == int.MaxValue)
            {
                Debug.LogError("not allowed key value");
            }

            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    return hashValues[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    return hashValues[i];
                }
            }
        }

        Debug.LogError("Key not found");
        return default;
    }

    public void iToMdic_Remove(int key)
    {
        int index = iToMdic_Hash(key);
        var hashKeys = iToMdic__keys[index];
        if (index == 0)
        {
            if (key == int.MaxValue)
            {
                Debug.LogError("not allowed key value");
            }

            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    hashKeys[i] = int.MaxValue;
                    iToMdic__size--;
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < hashKeys.Length; i++)
            {
                if (hashKeys[i] == key)
                {
                    hashKeys[i] = default;
                    iToMdic__size--;
                    return;
                }
            }
        }

        Debug.LogError("Key not found");
    }

    void Start()
    {
        iToMdic_SetCapacity(10);
        iToMdic_AddOrSetValue(0, GetComponent<MeshRenderer>());
        return;
        zero = new Vector3(0, 0, 0);
        identity = Quaternion.identity;
        poolArr = new UdonSavingObjectPool[]
        {
            _pool
        };
        GameObject newObj = Instantiate(empty);
        newObj.GetComponent<MeshRenderer>().sharedMaterial = material;
        SendCustomEventDelayedFrames(nameof(Spawn), 3);
        XSTARTINDEX = -chunkWidth / 2;
        XENDINDEX = chunkWidth - 1 - chunkWidth / 2;
        ZSTARTINDEX = 0;
        ZENDINDEX = chunkDepth;
        GameObject oobj;
        Assert(obj[0].gameObject != obj[1].gameObject);
        oobj = obj[0].gameObject;
        var oobj2 = oobj;
        Assert(oobj == oobj2);
        oobj2 = null;
        Assert(oobj != oobj2);
        Debug.Log("gameObject judhe end");
        Vector3 pos = new Vector3(-50, 0, 0);
        var index = GetChunkIndex((int)pos.x, (int)pos.z);
        var fromPos = FromChunkIndex(index);
        Assert(pos == fromPos);
        pos = new Vector3(49, 0, 0);
        index = GetChunkIndex((int)pos.x, (int)pos.z);
        fromPos = FromChunkIndex(index);
        Assert(pos == fromPos);
        pos = new Vector3(49, 0, 1);
        index = GetChunkIndex((int)pos.x, (int)pos.z);
        fromPos = FromChunkIndex(index);
        Assert(pos == fromPos);
        Debug.Log("pos judhe end");
        dict.SetCapacity(1000);
        dict2.SetCapacity(1000);
        Assert(dict.Count == 0);
        foreach (var o in obj)
        {
            Debug.Log(o.GetInstanceID());
            dict.Add(o.GetInstanceID(), new GameObject[] { o });
        }

        Assert(dict.Count == obj.Length);
        foreach (var o in obj)
        {
            Assert(dict.HasItem(o.GetInstanceID()));
            dict.GetValue(o.GetInstanceID());
        }

        queue.Enqueue(100);
        queue.Enqueue(102);
        Assert(queue.Dequeue() == 100);
        queue.Enqueue(101);
        Assert(queue.Dequeue() == 102);
        Assert(queue.Dequeue() == 101);
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

    int XSTARTINDEX, XENDINDEX, ZSTARTINDEX, ZENDINDEX;
    void Assert(bool b)
    {
        if (!b)
        {
            Debug.LogError("Assertion failed!");
        }
    }
}