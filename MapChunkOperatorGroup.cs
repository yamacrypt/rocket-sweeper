
using BuildSoft.UdonSharp.Collection;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapChunkOperatorGroup : UdonSharpBehaviour
{
    //[SerializeField]Queue queue;
    [SerializeField]MapCHunkOperator[] operators;
    [SerializeField]MapCellOperatorGroup cellOperatorGroup;

    /*public void Enqueue(GameObject obj){
        queue.Enqueue(obj);
    }*/

    int startIndex=0;
    int i;

    public void StartAnimation(GameObject cell,AnimDir dir,float animTime,float delay,MapGenerator generator,int xIndex,int zIndex, bool autoFree=true){
        for(i=startIndex;i<operators.Length;i++){
            MapCHunkOperator op = operators[i%operators.Length];
            if(op.isFree){
                bool success=op.SetTarget(cell);
                if(generator!=null){
                    op.SetCallback(generator,xIndex,zIndex);
                }
                if(success){
                    op.StartAnimation(dir,animTime,delay,autoFree);
                } else {
                    Debug.LogError("Failed to set target!");
                }
                startIndex=i+1;
                startIndex%=operators.Length;
                return;
            }
        }
        StartAnimationInstant(cell,dir);
        generator.UnLoadChunk(xIndex,zIndex);
        Debug.LogWarning("No free chunk operator!");
    }

    void StartAnimationInstant(GameObject chunk,AnimDir dir){
        var mesh=chunk.GetComponent<MeshRenderer>();
        var col=chunk.GetComponent<MeshCollider>();
        if(mesh!=null || col!=null){
            Debug.LogWarning("Cell"+chunk.name+ " has MeshRenderer or MeshCollider!");
            return;
        }
        if(dir==AnimDir.Up){
            mesh.enabled=true;
            col.enabled=true;
            mesh.material.SetFloat("_UpDown",0);
        } else if(dir==AnimDir.Down) {
            mesh.enabled=false;
            col.enabled=false;
        }else{
            Debug.LogError("Invalid direction!");
        }
    }



}
