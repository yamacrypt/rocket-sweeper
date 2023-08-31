
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum GenerateChunkMode{
    Init,
    Pass,
    Generating,
    Complete
}
public enum Biome{
    Grass,
    Tundra,
    Desert,
    Rock,
    Sand,
    Water,
    Dirt,
    Dark
}
public class MapEnum : UdonSharpBehaviour
{
    void Start()
    {
        
    }
}
