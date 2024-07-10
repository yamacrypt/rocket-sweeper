
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum PlanetType{
    Earth,
    Moon,
    Mars
}
public class IMapGenerator : GameLifeCycle
{
   public virtual void BreakCell(GameObject cell){}
   public virtual void BreakCells(GameObject[] cells,int length){}
   public virtual void SetGravity(float gravity){}
   public virtual float Gravity{get;}
   public virtual void GenerateOnSpawn(){}
}
