
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlanetInfo : UdonSharpBehaviour
{
    [SerializeField]PlanetType planetType;
    [SerializeField]Biome[] biomeMap1;
    [SerializeField]Biome[] biomeMap2;
    [SerializeField]Biome[] biomeMap3;
    [SerializeField]Biome[] biomeMap4;
    [SerializeField]Biome[] biomeMap5;
    [SerializeField]float waterPercentage;

    public PlanetType PlanetType => planetType;
    public Biome[][] BiomeMap(){
        var map = new Biome[5][];
        map[0]=biomeMap1;
        map[1]=biomeMap2;
        map[2]=biomeMap3;
        map[3]=biomeMap4;
        map[4]=biomeMap5;
        return map;
    }
    public float WaterPercentage => waterPercentage;
}
