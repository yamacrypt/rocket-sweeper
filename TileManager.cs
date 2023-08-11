
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TileManager : UdonSharpBehaviour
{
    [SerializeField]Material grass;
    [SerializeField]Material water;
    
    /*public void SetTile(MapCell cell, TileType tileType){
        switch(tileType){
            case TileType.Empty:
                cell.SetTile(water);
                break;
            case TileType.Grass:
                cell.SetTile(grass);
                break;
            default:
                Debug.LogError("Wrong tile type!");
                break;
        }
    }*/
}
