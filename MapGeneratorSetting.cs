
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapGeneratorSetting : UdonSharpBehaviour
{
    public int chunkSize=3;
    public int batchCount=1000;

    public int chunkWidth=100;
    public int chunkDepth=100;
    public int fieldHeight=3;

    public  float scale = 0.015f;
    public float scaleT=0.003f;
    public float scaleH=0.003f;
    public float cellAnimationTime=1f;
    public float TileScale=1.5f;

    public float chunkLoadRange=6f;
    public float chunkUnLoadRange=12f;
    public float chunkDetailRange=1f;
    public float generateAdditionalInterval=5f;
    public float searchAdditionalInterval=3f;

    public float removeBatchCount=100;
    public float removeInterval=0.1f;

    public float waterPercentage=0.2f;
    public int operationBatchCount=1;

    public int searchBatchCount=30;
    public bool removeOn=true;
    public bool searchOn=true;
}
