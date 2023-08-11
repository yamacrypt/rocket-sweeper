
using BuildSoft.UdonSharp.Collection;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapCellOperatorGroup : UdonSharpBehaviour
{
    //[SerializeField]Queue queue;
    [SerializeField]MapCellOperator[] operators;

    /*public void Enqueue(GameObject obj){
        queue.Enqueue(obj);
    }*/

    int startIndex=0;
    MapCellOperator op;
    int i;
    public void StartAnimation(GameObject cell,AnimDir dir,float animTime,float delay, bool autoFree=true){
        for(i=startIndex;i<operators.Length;i++){
            op = operators[i%operators.Length];
            if(op.isFree){
                op.SetTarget(cell);
                op.StartAnimation(dir,animTime,delay,autoFree);
                startIndex=i+1;
                startIndex%=operators.Length;
                return;
            }
        }
        StartAnimationInstant(cell,dir);
        Debug.LogWarning("No free cell operator!");
    }

    void StartAnimationInstant(GameObject cell,AnimDir dir){
        var mesh=cell.GetComponent<MeshRenderer>();
        var col=cell.GetComponent<BoxCollider>();
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
