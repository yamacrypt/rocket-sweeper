
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IntKeyStringDictionary : UdonSharpBehaviour
{
    int[][] _keys;
    string[][] _values;
    int _size;
    public int Count => _size;

    public void SetCapacity(int capacity)
    {
        int len=capacity/4;
        _keys=new int[len][];
        for(int i=1;i<len;i++){
            _keys[i]=new int[4];
        }

        // 0-value-key 対策
        for(int i=0;i<4;i++){
            _keys[0][i]=int.MaxValue;
        }

        _values=new string[len][];
        for(int i=1;i<len;i++){
            _values[i]=new string[4];
        }
    }

    private int Hash(int key)
    {
        var len= _keys.Length;
        var mod =Math.Abs(key) % len;
        if(mod>=0){
            return mod; 
        }else {
            return mod+len;
        }
        // return 0 to len-1;
    }

    public void Add(int key, string value)
    {
        int index = Hash(key);
        var hashKeys=_keys[index];
        var hashValues=_values[index];
        if(index==0){
            if(key==int.MaxValue){
                Debug.LogError("not allowed key value");
            }
            for(int i=0;i<hashKeys.Length;i++){
                if(hashKeys[i]==key){
                    Debug.LogError("Key already exists");
                }
                if(hashKeys[i]==int.MaxValue){
                    hashKeys[i]=key;
                    hashValues[i]=value;
                    _size++;
                    return;
                }
            }
        } else {
            for(int i=0;i<hashKeys.Length;i++){
                if(hashKeys[i]==key){
                    Debug.LogError("Key already exists");
                }
                if(hashKeys[i]==default){
                    hashKeys[i]=key;
                    hashValues[i]=value;
                    _size++;
                    return;
                }
            }
        }
    }

    public void SetValue(int key, string value)
    {
        int index = Hash(key);
        var hashKeys=_keys[index];
        var hashValues=_values[index];
        if(index==0){
            if(key==int.MaxValue){
                Debug.LogError("not allowed key value");
            }
            for(int i=0;i<hashKeys.Length;i++){
                if(hashKeys[i]==key){
                    hashValues[i]=value;
                    return;
                }
            }
        } else {
            for(int i=0;i<hashKeys.Length;i++){
                if(hashKeys[i]==key){
                    hashValues[i]=value;
                    return;
                }
            }
        }
        Debug.LogError("Key not found");
    }

    public string GetValue(int key)
    {
        int index = Hash(key);
        var hashKeys=_keys[index];
        var hashValues=_values[index];
        if(index==0){
            if(key==int.MaxValue){
                Debug.LogError("not allowed key value");
            }
            for(int i=0;i<hashKeys.Length;i++){
                if(hashKeys[i]==key){
                    return hashValues[i];
                }
            }
        } else {
            for(int i=0;i<hashKeys.Length;i++){
                if(hashKeys[i]==key){
                    return hashValues[i];
                }
            }
        }
        Debug.LogError("Key not found");
        return default;
    }

    public void Remove(int key)
    {
        int index = Hash(key);
        var hashKeys=_keys[index];
        var hashValues=_values[index];
        if(index==0){
            if(key==int.MaxValue){
                Debug.LogError("not allowed key value");
            }
            for(int i=0;i<hashKeys.Length;i++){
                if(hashKeys[i]==key){
                    hashKeys[i]=int.MaxValue;
                    _size--;
                    return;
                }
            }
        } else {
            for(int i=0;i<hashKeys.Length;i++){
                if(hashKeys[i]==key){
                    hashKeys[i]=default;
                    _size--;
                    return;
                }
            }
        }
        Debug.LogError("Key not found");
    }
}
