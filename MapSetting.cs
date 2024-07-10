
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapSetting : UdonSharpBehaviour
{
    [SerializeField]float waterPercentage=0.1f;  
    [SerializeField]GameObject grassPrefab; 
    [SerializeField]GameObject tundraPrefab; 
    [SerializeField]GameObject desertPrefab; 
    [SerializeField]GameObject rockPrefab; 
    [SerializeField]GameObject sandPrefab; 
    [SerializeField]GameObject waterPrefab; 
    [SerializeField]GameObject dirtPrefab; 
    [SerializeField]GameObject darkPrefab; 
    public float WaterPercentage=>waterPercentage;
    public GameObject GrassPrefab=>grassPrefab;
    public GameObject TundraPrefab=>tundraPrefab;
    public GameObject DesertPrefab=>desertPrefab;
    public GameObject RockPrefab=>rockPrefab;
    public GameObject SandPrefab=>sandPrefab;
    public GameObject WaterPrefab=>waterPrefab;
    public GameObject DirtPrefab=>dirtPrefab;
    public GameObject DarkPrefab=>darkPrefab;
}
