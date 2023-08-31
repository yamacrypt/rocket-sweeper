
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IMapGenerator : UdonSharpBehaviour
{
   public virtual void GenerateInit(){}
   public virtual void BreakCell(GameObject cell){}
   public virtual void BreakCells(GameObject[] cells,int length){}
}
