using System.Runtime.InteropServices;
using System;

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonObjectPool;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapGenerator : IMapGenerator
{

    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool grassPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool tundraPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool desertPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool dirtPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool rockPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool sandPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool waterPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool darkPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool orangeRockPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool redRockPool;
    [InterfaceToClassAttribute("IPoolItemOperator","TilePoolItemOperator")][SerializeField,UnrollAttribute]MapGeneratorSetting settings;
    
    Biome[][] biomeEarthMap=new Biome[][]{
        new Biome[]{Biome.Dirt,Biome.Dirt,Biome.Dark,Biome.Dark,Biome.Rock},
        new Biome[]{Biome.Dirt,Biome.Dirt,Biome.Dark,Biome.Grass,Biome.Rock},
        new Biome[]{Biome.Tundra,Biome.Dark,Biome.Grass,Biome.Rock,Biome.Sand},
        new Biome[]{Biome.Tundra,Biome.Grass,Biome.Rock,Biome.Desert,Biome.Sand},
        new Biome[]{Biome.Tundra,Biome.Grass,Biome.Desert,Biome.Desert,Biome.Sand},
    };

    Biome[][] biomeMoonMap=new Biome[][]{
        new Biome[]{Biome.Dark,Biome.Dark,Biome.Dark,Biome.Dark,Biome.Rock},
        new Biome[]{Biome.Dark,Biome.Rock,Biome.Rock,Biome.Rock,Biome.OrangeRock},
        new Biome[]{Biome.Rock,Biome.OrangeRock,Biome.OrangeRock,Biome.OrangeRock,Biome.Rock},
        new Biome[]{Biome.OrangeRock,Biome.Rock,Biome.Rock,Biome.Rock,Biome.Sand},
        new Biome[]{Biome.Rock,Biome.Sand,Biome.Sand,Biome.Sand,Biome.Sand},
    };

    Biome[][] biomeMarsMap=new Biome[][]{
        new Biome[]{Biome.Rock,Biome.Rock,Biome.Rock,Biome.RedRock,Biome.RedRock},
        new Biome[]{Biome.Rock,Biome.RedRock,Biome.RedRock,Biome.RedRock,Biome.OrangeRock},
        new Biome[]{Biome.RedRock,Biome.RedRock,Biome.RedRock,Biome.OrangeRock,Biome.OrangeRock},
        new Biome[]{Biome.RedRock,Biome.RedRock,Biome.OrangeRock,Biome.OrangeRock,Biome.Sand},
        new Biome[]{Biome.OrangeRock,Biome.OrangeRock,Biome.Sand,Biome.Sand,Biome.Sand},
    };

    bool BiomePeekIsInstantiated(Biome biome){
        switch(biome){
            case Biome.Grass:
                return grassPool.PeekIsInstantiated();
            case Biome.Tundra:
                return tundraPool.PeekIsInstantiated();
            case Biome.Desert:
                return desertPool.PeekIsInstantiated();
            case Biome.Rock:
                return rockPool.PeekIsInstantiated();
            case Biome.Sand:
                return sandPool.PeekIsInstantiated();
            case Biome.Water:
                return waterPool.PeekIsInstantiated();
            case Biome.Dirt:
                return dirtPool.PeekIsInstantiated();
            case Biome.Dark:
                return darkPool.PeekIsInstantiated();
            case Biome.OrangeRock:
                return orangeRockPool.PeekIsInstantiated();
            case Biome.RedRock:
                return redRockPool.PeekIsInstantiated();
            default:
                Debug.LogError("Invalid biome: "+biome);
                return false;
        }
    }
    GameObject BiomeTryToSpawn(Biome biome){
        switch(biome){
            case Biome.Grass:
                return grassPool.TryToSpawn();
            case Biome.Tundra:
                return tundraPool.TryToSpawn();
            case Biome.Desert:
                return desertPool.TryToSpawn();
            case Biome.Rock:
                return rockPool.TryToSpawn();
            case Biome.Sand:
                return sandPool.TryToSpawn();
            case Biome.Water:
                return waterPool.TryToSpawn();
            case Biome.Dirt:
                return dirtPool.TryToSpawn();
            case Biome.Dark:
                return darkPool.TryToSpawn();
            case Biome.OrangeRock:
                return orangeRockPool.TryToSpawn();
            case Biome.RedRock:
                return redRockPool.TryToSpawn();
            default:
                Debug.LogError("Invalid biome: "+biome);
                return null;
        }
    }
     void BiomeReturn(Biome biome,GameObject cell,int id){
        switch(biome){
            case Biome.Grass:
                grassPool.Return(cell,id);
                return;
            case Biome.Tundra:
                tundraPool.Return(cell,id);
                return;
            case Biome.Desert:
                desertPool.Return(cell,id);
                return;
            case Biome.Rock:
                rockPool.Return(cell,id);
                return;
            case Biome.Sand:
                sandPool.Return(cell,id);
                return;
            case Biome.Water:
                waterPool.Return(cell,id);
                return;
            case Biome.Dirt:
                dirtPool.Return(cell,id);
                return;
            case Biome.Dark:
                darkPool.Return(cell,id);
                return;
            case Biome.OrangeRock:
                orangeRockPool.Return(cell,id);
                return;
            case Biome.RedRock:
                redRockPool.Return(cell,id);
                return;
            default:
                Debug.LogError("Invalid biome: "+biome);
                return;
        }
    }

    void BiomeStore(int biomeIndex){
        switch(biomeIndex){
            case (int)Biome.Grass:
                grassPool.Store();
                return;
            case (int)Biome.Tundra:
                tundraPool.Store();
                return;
            case (int)Biome.Desert:
                desertPool.Store();
                return;
            case (int)Biome.Rock:
                rockPool.Store();
                return;
            case (int)Biome.Sand:
                sandPool.Store();
                return;
            case (int)Biome.Water:
                waterPool.Store();
                return;
            case (int)Biome.Dirt:
                dirtPool.Store();
                return;
            case (int)Biome.Dark:
                darkPool.Store();
                return;
            case (int)Biome.OrangeRock:
                orangeRockPool.Store();
                return;
            case (int)Biome.RedRock:
                redRockPool.Store();
                return;
            default:
                Debug.LogError("Invalid biome: "+biomeIndex);
                return;
        }
    }

    bool BiomePeekIsInstantiated(int biomeIndex){
        switch(biomeIndex){
            case (int)Biome.Grass:
                return grassPool.PeekIsInstantiated();
                
            case (int)Biome.Tundra:
                return tundraPool.PeekIsInstantiated();
                
            case (int)Biome.Desert:
                return desertPool.PeekIsInstantiated();
                
            case (int)Biome.Rock:
                return rockPool.PeekIsInstantiated();
                
            case (int)Biome.Sand:
                return sandPool.PeekIsInstantiated();
                
            case (int)Biome.Water:
                return waterPool.PeekIsInstantiated();
                
            case (int)Biome.Dirt:
                return dirtPool.PeekIsInstantiated();
                
            case (int)Biome.Dark:
                return darkPool.PeekIsInstantiated();
            case (int)Biome.OrangeRock:
                return orangeRockPool.PeekIsInstantiated();
            case (int)Biome.RedRock:
                return redRockPool.PeekIsInstantiated();
            default:
                Debug.LogError("Invalid biome: "+biomeIndex);
                return false;
        }
    }
    int chunkSize=>settings.chunkSize;
    int chunkSizeY=>settings.chunkSizeY;

    int chunkWidth=>settings.chunkWidth;
    int chunkDepth=>settings.chunkDepth;
    int fieldHeight=>settings.fieldHeight;
    int xStartChunkIndex,zStartChunkIndex;
    int xEndChunkIndex,zEndChunkIndex;
    int xSearchStartChunkIndex,zSearchStartChunkIndex;
    int xSearchEndChunkIndex,zSearchEndChunkIndex;
    int xQuickSearchStartChunkIndex,zQuickSearchStartChunkIndex;
    int xQuickSearchEndChunkIndex,zQuickSearchEndChunkIndex;
    int batchCount=>settings.batchCount;
    float chunkLoadRange=>settings.chunkLoadRange;
    float generateAdditionalInterval=>settings.generateAdditionalInterval *2.5f / Networking.LocalPlayer.GetWalkSpeed();
    float cellAnimationTime=>settings.cellAnimationTime;
    float removeBatchCount=>settings.removeBatchCount;
    float removeInterval=>settings.removeInterval;
    float waterPercentage;
    float chunkUnLoadRange=>settings.chunkUnLoadRange;
    float chunkDetailRange=>settings.chunkDetailRange;
    float operationBatchCount=>settings.operationBatchCount;
    [SerializeField]int chunkCapacity=15000;

    public override void OnDeserialization()
    {
        Debug.Log("OnDeserialization: MapGenerator");
        isSyncedLocal=true;
    }
    bool isSyncedLocal=true;
    [UdonSynced]bool isSyncedInit=false;
    void Assert(bool b){
        if(!b){
            Debug.LogError("Assertion failed!");
        }
    }

    int[] detailChunkCellInstanceIds;
    [SerializeField,UnrollAttribute]IntKeyChunkOperationDictionary chunkOperationDictionary;
    [SerializeField,UnrollAttribute]IntKeyIntDictionary chunkIndexToInstanceIdDictionary;
    int[] permanentBreakCellArr;
    void Start()
    {
        waterPercentage=settings.waterPercentage;


        outlineDict.SetCapacity(chunkCapacity);
        chunkToCellsDict.SetCapacity(chunkCapacity);
        chunkStateDict.SetCapacity(chunkCapacity);
        instanceIDToGlobalCellIndexDictionary.SetCapacity(20011);
        undetailQueue.SetCapacity(chunkCapacity);
        detailQueue.SetCapacity(chunkCapacity);
        priorityDetailQueue.SetCapacity(chunkCapacity);
        unloadQueue.SetCapacity(chunkCapacity);
        chunkIndexToCellBiomesDictionary.SetCapacity(chunkCapacity);
        chunkToCellInstanceIdsDictionary.SetCapacity(chunkCapacity);
        //tempBreakCellDictionary.SetCapacity(chunkCapacity);
        permanentBreakCellDictionary.SetCapacity(chunkCapacity);
        permanentBreakCellArr=new int[20011];
        chunkIndexToInstanceIdDictionary.SetCapacity(chunkCapacity);
        chunkOperationDictionary.SetCapacity(chunkCapacity);
        instanceIDToBiomeDictionary.SetCapacity(20011);
        brokenArr=new bool[chunkSize*chunkSize*chunkSizeY];
        colOnlyArr=new bool[chunkSize*chunkSize*chunkSizeY];
        MapSync();
        SendCustomEventDelayedSeconds(nameof(ReqSync), 5f);
        SendCustomEventDelayedSeconds(nameof(ReqSync), 10f);
        /*chunkWidth=settings.chunkWidth;
        chunkDepth=settings.chunkDepth;
        chunkSize=settings.chunkSize;
        batchCount=settings.batchCount;
        generateInterval=settings.generateInterval;
        chunkLoadRange=settings.chunkLoadRange;
        generateAdditionalInterval=settings.generateAdditionalInterval *2.5f / Networking.LocalPlayer.GetWalkSpeed();
        cellAnimationTime=settings.cellAnimationTime;*/
        meshFilters= new MeshFilter[chunkSize*chunkSize*chunkSizeY];
        detailChunkCellInstanceIds= new int[chunkSize*chunkSize*chunkSizeY];
        XSTARTINDEX=-chunkWidth/2;
        XENDINDEX=chunkWidth-1-chunkWidth/2;
        ZSTARTINDEX=0;
        ZENDINDEX = chunkDepth; 
        QuickSearchInterval();
    }
    [SerializeField]float quickSearchInterval=0.5f;
    public void QuickSearchInterval(){
        if(IsStart){
            QuickSearch();
        }
        SendCustomEventDelayedSeconds(nameof(QuickSearchInterval),quickSearchInterval);
    }
    [SerializeField,UnrollAttribute]IntKeyIntArrDictionary chunkToCellInstanceIdsDictionary;
    int XSTARTINDEX,XENDINDEX,ZSTARTINDEX,ZENDINDEX;
    public void MapSync()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            isSyncedLocal=true;
            isSyncedInit=true;
            _seedX = UnityEngine.Random.value * (float)Int16.MaxValue;
            _seedZ = UnityEngine.Random.value * (float)Int16.MaxValue;
            _seedTX = UnityEngine.Random.value * (float)Int16.MaxValue;
            _seedTZ = UnityEngine.Random.value * (float)Int16.MaxValue;
            _seedHX = UnityEngine.Random.value * (float)Int16.MaxValue;
            _seedHZ = UnityEngine.Random.value * (float)Int16.MaxValue;
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(ReqSync), 5f);
        }
    }

    public void ReqSync(){
        RequestSerialization();
    }
    int[] unloadIndexes;
    int unloadIndexStart;
    public override void GameOver(){
        base.GameOver();
        gameOverPos=GetPlayerPosition();
        zClearStartChunkIndex=GetInRangeStartZChunkIndex(gameOverPos,chunkUnLoadRange+clearMargin);
        zClearEndChunkIndex=GetInRangeEndZChunkIndex(gameOverPos,chunkUnLoadRange+clearMargin);
        xClearStartChunkIndex=GetInRangeStartXChunkIndex(gameOverPos,zClearStartChunkIndex,chunkUnLoadRange+clearMargin);
        xClearEndChunkIndex=GetInRangeEndXChunkIndex(gameOverPos,zClearStartChunkIndex,chunkUnLoadRange+clearMargin);
        unloadIndexes=chunkIndexToInstanceIdDictionary.GenerateKeysArray();
        unloadIndexStart=0;
        isClearing=true;
        isPermanentCellsClearing=false;
        unloadQueue.Clear();
        detailQueue.Clear();
        undetailQueue.Clear();
        priorityDetailQueue.Clear();
        isGenerating=false;
        isSearching=false;
        SetGravity(9.8f);
        isSyncedLocal=false;
        MapSync();
    }
    public override bool IsReadyToGameStart(){
        Debug.Log("IsReadyToGameStart: "+base.IsReadyToGameStart() + " isSyncedLocal: "+isSyncedLocal+" isSynced: "+isSyncedInit+" isSearching: "+isSearching+"_seedX: "+_seedX);
        return base.IsReadyToGameStart()&&!isClearing&&!isPermanentCellsClearing&&!isGenerating&&isSyncedLocal&&isSyncedInit&&!isSearching;
;
    }
    Vector3 gameOverPos;
    bool isClearing = false;
    Mission mission;
    public override void GameStart(Mission mission){
        base.GameStart(mission);
        this.mission=mission;
        waterPercentage=mission.WaterPercentage;
        isSyncedLocal=false;
        SetGravity(mission.Gravity);
        //GenerateInitCellInterval();

        GenerateAdditional();
        SearchAdditional();
        
        //RemoveChunkInterval();
    }

    float searchAdditionalInterval=>settings.searchAdditionalInterval;
    Vector3 posCorrection=Vector3.zero;
    Vector3 GetPlayerPosition(){
        return Networking.LocalPlayer.GetPosition() / (chunkSize*TileScale);
    }
    float baseGravity;
    public override float Gravity=>baseGravity;

    public override void SetGravity(float gravity){
        Physics.gravity=Vector3.down*gravity;
        baseGravity=gravity;
    }
    void CalcPosCorrection(){
        playerRot = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        // 極座標へ変換
        float r = 2.5f; // 半径は通常1とするか、必要に応じて変更
        var rad=Mathf.PI /2 - playerRot.eulerAngles.y*Mathf.Deg2Rad;
        posCorrection.x=Mathf.Cos(rad)*r;
        posCorrection.z=Mathf.Sin(rad)*r;
        //Debug.Log("posCorrection: "+posCorrection);
    }
    void GenerateAdditional(){
        if(isGenerating||!IsStart)return;
        prePlayerPos=playerPos;
        playerPos = GetPlayerPosition();//Networking.LocalPlayer.GetPosition() / (chunkSize*TileScale);
        CalcPosCorrection();
        chunkXIndex=0;
        chunkZIndex=0;
        chunkYIndex=0;
        //Debug.Log("GenerateAdditional");
        //Debug.Log("player pos: "+playerPos);
        zStartChunkIndex=GetInRangeStartZChunkIndex(playerPos+posCorrection,chunkLoadRange);
        zEndChunkIndex=GetInRangeEndZChunkIndex(playerPos+posCorrection,chunkLoadRange);
        xStartChunkIndex=GetInRangeStartXChunkIndex(playerPos+posCorrection,zStartChunkIndex,chunkLoadRange);//-1;
        xEndChunkIndex=GetInRangeEndXChunkIndex(playerPos+posCorrection,zStartChunkIndex,chunkLoadRange);//-1;
        //exceptXStartChunkIndex=GetInRangeStartXChunkIndex(prePlayerPos+posCorrection,zStartChunkIndex,chunkLoadRange);
        //exceptXEndChunkIndex=GetInRangeEndXChunkIndex(prePlayerPos+posCorrection,zStartChunkIndex,chunkLoadRange);

        isGenerating=true;
        //GenerateCellInterval();
    }

    public override void GenerateOnSpawn(){
        if(!IsStart)return;
        prePlayerPos=playerPos;
        playerPos = playerSpawnPos.position / (chunkSize*TileScale);//Networking.LocalPlayer.GetPosition() / (chunkSize*TileScale);
        CalcPosCorrection();
        chunkXIndex=0;
        chunkZIndex=0;
        chunkYIndex=0;
        //Debug.Log("GenerateAdditional");
        //Debug.Log("player pos: "+playerPos);
        zStartChunkIndex=GetInRangeStartZChunkIndex(playerPos+posCorrection,chunkLoadRange);
        zEndChunkIndex=GetInRangeEndZChunkIndex(playerPos+posCorrection,chunkLoadRange);
        xStartChunkIndex=GetInRangeStartXChunkIndex(playerPos+posCorrection,zStartChunkIndex,chunkLoadRange);//-1;
        xEndChunkIndex=GetInRangeEndXChunkIndex(playerPos+posCorrection,zStartChunkIndex,chunkLoadRange);//-1;
        //exceptXStartChunkIndex=GetInRangeStartXChunkIndex(prePlayerPos+posCorrection,zStartChunkIndex,chunkLoadRange);
        //exceptXEndChunkIndex=GetInRangeEndXChunkIndex(prePlayerPos+posCorrection,zStartChunkIndex,chunkLoadRange);

        isGenerating=true;
    }

    void SearchAdditional(){
        if(searchOn){
            if(!IsStart)return;
            if(isSearching){
                Debug.LogWarning("already searching");
                return;
            }
            zSearchStartChunkIndex=GetInRangeStartZChunkIndex(playerPos,chunkUnLoadRange+searchMargin);;
            zSearchEndChunkIndex=GetInRangeEndZChunkIndex(playerPos,chunkUnLoadRange+searchMargin);;
            xSearchStartChunkIndex=GetInRangeStartXChunkIndex(playerPos,zSearchStartChunkIndex,chunkUnLoadRange+searchMargin);
            xSearchEndChunkIndex=GetInRangeEndXChunkIndex(playerPos,zSearchStartChunkIndex,chunkUnLoadRange+searchMargin);
        
            isSearching=true;
            researchFlag=false;
        }
    }

    

 
    Vector3 playerPos;
    Quaternion playerRot;
    Vector3 prePlayerPos;
    int GetInRangeStartXChunkIndex(Vector3 pos,float z,float range ){
        float diff =range*range - (pos.z-z)*(pos.z-z);
        if(diff<=0)return chunkWidth/2;
        return Math.Max(XSTARTINDEX,(int)(pos.x-Mathf.Sqrt(diff)));
    }

    int GetInRangeEndXChunkIndex(Vector3 pos,float z,float range ){
        float diff = Math.Max(0,range*range - (pos.z-z)*(pos.z-z));
        if(diff==0)return -chunkWidth/2;
        return Math.Min(XENDINDEX,(int)(pos.x+Mathf.Sqrt(diff)));
    }

    int GetInRangeStartZChunkIndex(Vector3 pos,float range ){
        return Math.Max(ZSTARTINDEX,(int)(pos.z-range));
    }

    int GetInRangeEndZChunkIndex(Vector3 pos,float range ){
        return Math.Min(ZENDINDEX,(int)(pos.z+range));
    }

    bool IsInDetailRange(Vector3 pPos,int x,int z){
        return ((pPos.x-x)*(pPos.x-x)+(pPos.z-z)*(pPos.z-z))<chunkDetailRange*chunkDetailRange;
    }

    int exceptXStartChunkIndex;
    int exceptXEndChunkIndex;
    int exceptZStartChunkIndex;
    int exceptZEndChunkIndex;
    bool isGenerating=false;
    [SerializeField,UnrollAttribute]IntKeyGameObjectDictionary outlineDict;
    float generateAdditionalDelta=-1f;
    float searchAdditionalDelta=-1f;

    [SerializeField]int isInstantiatedCost=10;
    [SerializeField]int poolCost=4;
    [SerializeField]int unloadCost=6;
    int storeCount=0;
    int storeMax=3;
    int biomeIndex=0;
    UdonSavingObjectPool storeBiomePool;


    void StoreCell(){
        BiomeStore(biomeIndex);
        isInstantiated=BiomePeekIsInstantiated(biomeIndex);//storeBiomePool.PeekIsInstantiated();
        biomeIndex++;
        biomeIndex%=(int)Biome.Dark;
    }
    int costIndex;
    GenerateChunkMode mode;

    bool researchFlag=false;
    public void RequestResearch(){
        researchFlag=true;
        QuickSearch();
    }

    [SerializeField]bool detailFlag=false;
    [SerializeField]bool undetailFlag=false;
    [SerializeField]bool unloadFlag=false;

    void QuickSearch(){
        if(searchOn){
            var ppos=GetPlayerPosition();
            zQuickSearchStartChunkIndex=GetInRangeStartZChunkIndex(ppos,quickSearchRange);
            zQuickSearchEndChunkIndex=GetInRangeEndZChunkIndex(ppos,quickSearchRange);
            xQuickSearchStartChunkIndex=GetInRangeStartXChunkIndex(ppos,zQuickSearchStartChunkIndex,quickSearchRange);
            xQuickSearchEndChunkIndex=GetInRangeEndXChunkIndex(ppos,zQuickSearchStartChunkIndex,quickSearchRange);
            while(true){
                if(!IsStart)break;
                var x=xQuickSearchStartChunkIndex;
                var z=zQuickSearchStartChunkIndex;
                var chunkIndex=ToChunkIndex(x,z);
                if(chunkStateDict.IsUnDetailed(chunkIndex)){
                    var diffx=x-ppos.x;
                    var diffz=z-ppos.z;
                    var distance = diffx*diffx+diffz*diffz;
                    if(distance<=chunkDetailRange*chunkDetailRange){
                        chunkOperationDictionary.AddOrSetValue(chunkIndex,ChunkOperation.Detail);
                        priorityDetailQueue.Enqueue(chunkIndex);
                    }
                }
                xQuickSearchStartChunkIndex++;
                if(xQuickSearchStartChunkIndex>xQuickSearchEndChunkIndex){
                    zQuickSearchStartChunkIndex++;
                    xQuickSearchStartChunkIndex=GetInRangeStartXChunkIndex(ppos,zQuickSearchStartChunkIndex,quickSearchRange);
                    xQuickSearchEndChunkIndex=GetInRangeEndXChunkIndex(ppos,zQuickSearchStartChunkIndex,quickSearchRange);
                    if(zQuickSearchStartChunkIndex>=zQuickSearchEndChunkIndex){
                        break;
                    }
                }
            }
        }
    }
    int xClearStartChunkIndex,zClearStartChunkIndex;
    int xClearEndChunkIndex,zClearEndChunkIndex;
    bool isPermanentCellsClearing=false;
    void Update()
    {
        if(!IsStart){
            if(isClearing){
                costIndex=0;
                for(int i=0;i<operationBatchCount;i++){
                    UnLoadChunk(unloadIndexes[unloadIndexStart],true);
                    unloadIndexStart++;
                    if(unloadIndexStart==unloadIndexes.Length){
                        isClearing=false;
                        isPermanentCellsClearing=true;
                        break;
                    }
                    /*var x=xClearStartChunkIndex;
                    var z=zClearStartChunkIndex;
                    var chunkIndex=ToChunkIndex(x,z);
                    if(chunkStateDict.IsLoaded(chunkIndex)){
                        UnLoadChunk(chunkIndex,true);
                        costIndex++;
                    }
                    xClearStartChunkIndex++;
                    if(xClearStartChunkIndex>xClearEndChunkIndex){
                        zClearStartChunkIndex++;
                        xClearStartChunkIndex=GetInRangeStartXChunkIndex(gameOverPos,zClearStartChunkIndex,chunkUnLoadRange+clearMargin);
                        xClearEndChunkIndex=GetInRangeEndXChunkIndex(gameOverPos,zClearStartChunkIndex,chunkUnLoadRange+clearMargin);
                        if(zClearStartChunkIndex>=zClearEndChunkIndex){
                            isClearing=false;
                            isPermanentCellsClearing=true;
                            break;
                        }
                    }*/
                }
            }
            if(isPermanentCellsClearing){
                for(int i=0;i<batchCount*2;i++){
                    permanentBreakCellArrLength--;
                    if(permanentBreakCellArrLength>=0){
                        var cellIndex=permanentBreakCellArr[permanentBreakCellArrLength];
                        permanentBreakCellDictionary.Remove(cellIndex);
                    }else{
                        permanentBreakCellArrLength=0;
                        isPermanentCellsClearing=false;
                        unloadQueue.Clear();
                        detailQueue.Clear();
                        undetailQueue.Clear();
                        priorityDetailQueue.Clear();
                        isGenerating=false;
                        isSearching=false;
                    }
                }
            }
            
            return;
        }
        
        if(isSearching){
            //if(isUnLoading)return;
            for(costIndex=0;costIndex<searchBatchCount;costIndex++){
                var x=xSearchStartChunkIndex;
                var z=zSearchStartChunkIndex;
                var chunkIndex=ToChunkIndex(x,z);
                if(chunkStateDict.IsLoaded(chunkIndex)){
                    var diffx=x-playerPos.x;
                    var diffz=z-playerPos.z;
                    var distance = diffx*diffx+diffz*diffz;
                    var detailMargin=1;
                    if(distance>=chunkUnLoadRange*chunkUnLoadRange){
                        chunkOperationDictionary.AddOrSetValue(chunkIndex,ChunkOperation.UnLoad);
                        unloadQueue.Enqueue(chunkIndex);
                        //UnLoadChunk((int)x,(int)z,true);
                    }  else if(distance>=(chunkDetailRange+detailMargin)*(chunkDetailRange+detailMargin)){
                        if(chunkStateDict.IsDetailed(chunkIndex)){
                            chunkOperationDictionary.AddOrSetValue(chunkIndex,ChunkOperation.UnDetail);
                            undetailQueue.Enqueue(chunkIndex);
                        }
                        //UnDetailChunk((int)x,(int)z);
                    }else if(distance<=chunkDetailRange*chunkDetailRange){
                        if(chunkStateDict.IsUnDetailed(chunkIndex)){
                            chunkOperationDictionary.AddOrSetValue(chunkIndex,ChunkOperation.Detail);
                            detailQueue.Enqueue(chunkIndex);
                        }
                        //DetailChunk((int)x,(int)z);
                    }
                }
                xSearchStartChunkIndex++;
                if(xSearchStartChunkIndex>xSearchEndChunkIndex){
                    zSearchStartChunkIndex++;
                    xSearchStartChunkIndex=GetInRangeStartXChunkIndex(playerPos,zSearchStartChunkIndex,chunkUnLoadRange+searchMargin);
                    xSearchEndChunkIndex=GetInRangeEndXChunkIndex(playerPos,zSearchStartChunkIndex,chunkUnLoadRange+searchMargin);
                    if(zSearchStartChunkIndex>=zSearchEndChunkIndex){
                        isSearching=false;
                        break;
                    }
                }
                //if(taskFlag)break;
            }
        } else{
            if(searchAdditionalDelta>=0){
                searchAdditionalDelta-=Time.deltaTime;
                if(searchAdditionalDelta<0 || researchFlag){
                    SearchAdditional();
                }
            } else{
                //if(unloadQueue.Count==0&&detailQueue.Count==0){
                    searchAdditionalDelta=searchAdditionalInterval; // search処理で最後に一回だけ呼ばれる
                //;
            }
        }
        // detail
        costIndex=0;
        if(detailFlag){
            while(costIndex<operationBatchCount){
                if(priorityDetailQueue.Count==0)break;
                var chunkIndex=priorityDetailQueue.Dequeue();
                var shouldOperate=chunkOperationDictionary.GetValueOrDefault(chunkIndex,ChunkOperation.None) == ChunkOperation.Detail;
                chunkOperationDictionary.SetValue(chunkIndex,ChunkOperation.None);
                if(shouldOperate&&DetailChunk(chunkIndex))costIndex++;
            }
            while(costIndex<operationBatchCount){
                if(detailQueue.Count==0)break;
                var chunkIndex=detailQueue.Dequeue();
                var shouldOperate=chunkOperationDictionary.GetValueOrDefault(chunkIndex,ChunkOperation.None) == ChunkOperation.Detail;
                chunkOperationDictionary.SetValue(chunkIndex,ChunkOperation.None);
                if(shouldOperate&&DetailChunk(chunkIndex))costIndex++;
            }
        }

        if(unloadFlag){
            while(costIndex<operationBatchCount){
                if(unloadQueue.Count==0)break;
                var chunkIndex=unloadQueue.Dequeue();
                var shouldOperate=chunkOperationDictionary.GetValueOrDefault(chunkIndex,ChunkOperation.None) == ChunkOperation.UnLoad;
                chunkOperationDictionary.SetValue(chunkIndex,ChunkOperation.None);
                if(shouldOperate&&UnLoadChunk(chunkIndex,true))costIndex++;
            }
        }
        if(undetailFlag){
            while(costIndex<operationBatchCount){
                if(undetailQueue.Count==0)break;
                var chunkIndex=undetailQueue.Dequeue();
                var shouldOperate=chunkOperationDictionary.GetValueOrDefault(chunkIndex,ChunkOperation.None) == ChunkOperation.UnDetail;
                chunkOperationDictionary.SetValue(chunkIndex,ChunkOperation.None);
                if(shouldOperate&&UnDetailChunk(chunkIndex))costIndex++;
            }
        }
        if(costIndex==operationBatchCount)return;
        
        if(isGenerating){
            //if(isUnLoading)return;
            for(costIndex=0;costIndex<batchCount*isInstantiatedCost;){
                //delay=generateInterval * count / batchCount;
               
                mode=GenerateChunk();
                if(isInstantiated){
                    costIndex+=isInstantiatedCost;
                    isInstantiated=false;
                } else if(mode==GenerateChunkMode.Pass){
                    storeCount++;
                    costIndex+=isInstantiatedCost/2;
                    /*if(storeCount==storeMax){
                        StoreCell();
                        storeCount=0;
                    }
                    if(isInstantiated){
                        costIndex+=isInstantiatedCost;
                        isInstantiated=false;
                    }else{
                        costIndex++;
                    }*/
                } else{
                    costIndex+=poolCost;
                }
                if(mode==GenerateChunkMode.Complete || mode==GenerateChunkMode.Pass){  
                    chunkXIndex=0;
                    chunkZIndex=0;
                    chunkYIndex=0;
                    xStartChunkIndex++;
                    if(xStartChunkIndex>xEndChunkIndex){
                        zStartChunkIndex++;
                        xStartChunkIndex=GetInRangeStartXChunkIndex(playerPos+posCorrection,zStartChunkIndex,chunkLoadRange);
                        xEndChunkIndex=GetInRangeEndXChunkIndex(playerPos+posCorrection,zStartChunkIndex,chunkLoadRange);
                        if(zStartChunkIndex>=zEndChunkIndex){
                            isGenerating=false;
                            //GenerateAdditional();
                            if(generateAdditionalInterval==0){
                                GenerateAdditional();
                            } else {
                                generateAdditionalDelta=generateAdditionalInterval;
                            }
                            break;
                        }
                    }
                }
                if(mode==GenerateChunkMode.Complete){
                    break;
                }
            }
        } else {
            if(generateAdditionalDelta>=0){
                generateAdditionalDelta-=Time.deltaTime;
                if(generateAdditionalDelta<0){
                    GenerateAdditional();
                }
            }
        }
        

        
        
        
       
    }
    float searchMargin=>chunkDetailRange*2;
    float clearMargin=>chunkDetailRange*6;

    bool isSearching=false;

    int searchBatchCount=>settings.searchBatchCount;

 

    bool searchOn=> settings.searchOn;
    

  
    public const float AnimHeight=10f;

    [SerializeField,UnrollAttribute]IntQueue unloadQueue;
    [SerializeField,UnrollAttribute]IntQueue undetailQueue;
    [SerializeField,UnrollAttribute]IntQueue detailQueue;
    [SerializeField,UnrollAttribute]IntQueue priorityDetailQueue;
    
    [SerializeField,UnrollAttribute]IntKeyChunkStateDictionary chunkStateDict;

    bool DetailChunk(int chunkIndex){
        //int chunkIndex=ToChunkIndex(xIndex,zIndex);
        if(chunkStateDict.IsUnDetailed(chunkIndex)){
            if(outlineDict.HasItem(chunkIndex)){
                //Debug.Log("DetailChunk: "+chunkIndex);
                var outline = outlineDict.GetValue(chunkIndex);
                chunkToCells=chunkToCellsDict.GetValue(chunkIndex);
                cellInstanceIds=chunkToCellInstanceIdsDictionary.GetValue(chunkIndex);
                for(int i=0;i<chunkToCells.Length;i++){
                    brokenArr[i]=permanentBreakCellDictionary.GetValueOrDefault(ToGlobalCellIndex(i,chunkIndex),false);
                }
                var outlineId=chunkIndexToInstanceIdDictionary.GetValue(chunkIndex);
                //WARNING: colOnlyArr は全てで同じなのでそのまま突っ込んだ
                meshCombiner.SwitchCombineMesh(false,outline,outlineId,chunkToCells,cellInstanceIds,brokenArr);
                chunkStateDict.SetValue(chunkIndex,ChunkState.Detailed);
                return true;
            }else {
                Debug.LogWarning("DetailChunk: chunkIndexDict not has item");
            }
        }
        return false;
    }

    bool[] brokenArr;
    bool[] colOnlyArr;

    bool UnDetailChunk(int chunkIndex){
        //int chunkIndex=ToChunkIndex(xIndex,zIndex);
        if(chunkStateDict.IsDetailed(chunkIndex)){
            if(outlineDict.HasItem(chunkIndex)){
                //Debug.Log("UnDetailChunk: "+chunkIndex);
                var outline = outlineDict.GetValue(chunkIndex);
                chunkToCells=chunkToCellsDict.GetValue(chunkIndex);
                cellInstanceIds=chunkToCellInstanceIdsDictionary.GetValue(chunkIndex);
                var outlineId=chunkIndexToInstanceIdDictionary.GetValue(chunkIndex);
                //WARNING: colOnlyArr は全てで同じなのでそのまま突っ込んだ
                meshCombiner.SwitchCombineMesh(true,outline,outlineId,chunkToCells,cellInstanceIds,brokenArr);
                chunkStateDict.SetValue(chunkIndex,ChunkState.UnDetailed);
                return true;
            } else {
                Debug.LogWarning("UnDetailChunk: chunkIndexDict not has item");
            }
        }
        return false;
    }

    GameObject[] chunkToCells;
    int[] cellInstanceIds;
    [SerializeField]float quickSearchRange=6;
    public bool UnLoadChunk(int chunkIndex,bool setOnly=false){
        //int chunkIndex=ToChunkIndex(xIndex,zIndex);
        if(!chunkStateDict.IsLoaded(chunkIndex)){
            //Debug.Log("UnLoadChunk: not yet loaded");
            //遅延実行の関係上このケースは起こりえる
            return false;
        }
        if(outlineDict.HasItem(chunkIndex)){
            //Debug.Log("UnLoadChunk:"+chunkIndex);
            //chunkIndexDict.Remove(chunkIndex);
            var chunkMesh=outlineDict.GetValue(chunkIndex);
            //var id=chunkMesh.GetInstanceID();
            //Debug.Log("unload chunkID: "+id);
            if(!chunkToCellsDict.HasItem(chunkIndex)){
                Debug.LogWarning("unload chunk id not exists");
            }
            chunkToCells = chunkToCellsDict.GetValue(chunkIndex);
            cellInstanceIds=chunkToCellInstanceIdsDictionary.GetValue(chunkIndex);
            var chunkId=chunkIndexToInstanceIdDictionary.GetValue(chunkIndex);
            chunkPool.Return(chunkMesh,chunkId,true,false);
            // gc alloc debug only
            /*if(chunkMesh!=null){
                var id=chunkIndexToInstanceIdDictionary.GetValue(chunkIndex);
                chunkPool.Return(chunkMesh,id,true,false);
            } else {
                Debug.LogWarning("Chunk pool is empty!");
            }*/
            var biomeTypes=chunkIndexToCellBiomesDictionary.GetValue(chunkIndex);
            for(int i=0;i<chunkToCells.Length;i++){
                var cell=chunkToCells[i];
                // gc alloc debug only
                /*if(cell!=null){
                    var id = cellInstanceIds[i];
                    UnloadCell(cell,id,biomeTypes[i]);
                } else {
                    Debug.LogError("Cell pool is empty!");
                }*/
                var id = cellInstanceIds[i];
                if(!permanentBreakCellDictionary.GetValueOrDefault(ToGlobalCellIndex(i,chunkIndex),false)){
                    UnloadCell(cell,id,biomeTypes[i]);
                }
            }
            // unloadされたチャンクはdetail dictからkey 削除
            chunkStateDict.SetValue(chunkIndex,ChunkState.UnLoaded); //AddOrSetValue(chunkIndex,false );
            return true;
        }
        return false;
    }

    //[SerializeField,UnrollAttribute]IntKeyBoolDictionary tempBreakCellDictionary;
    [SerializeField,UnrollAttribute]IntKeyBoolDictionary permanentBreakCellDictionary;
    [SerializeField,UnrollAttribute]IntKeyBiomeArrDictionary chunkIndexToCellBiomesDictionary;
    [SerializeField,UnrollAttribute]IntKeyIntDictionary instanceIDToGlobalCellIndexDictionary;
    [SerializeField,UnrollAttribute]IntKeyBiomeDictionary instanceIDToBiomeDictionary;
    public override void BreakCell(GameObject cell){
        // gc alloc debug only
        /*if(cell!=null){
            var cellId=cell.GetInstanceID();
            var globalCellIndex=instanceIDToGlobalCellIndexDictionary.GetValue(cellId);
            permanentBreakCellDictionary.AddOrSetValue(globalCellIndex,true);
            //tempBreakCellDictionary.AddOrSetValue(cellId,true); // may colision this is beacause not broken cells disappaer bug haapens 
            UnloadCell(cell,cellId);
        } else {
            Debug.LogWarning("Cell pool is empty!");
        }*/
        var cellId=cell.GetInstanceID();
        var globalCellIndex=instanceIDToGlobalCellIndexDictionary.GetValueOrDefault(cellId,int.MaxValue);
        if(globalCellIndex!=int.MaxValue){
            permanentBreakCellDictionary.AddOrSetValue(globalCellIndex,true);
            if(permanentBreakCellArrLength<permanentBreakCellArr.Length){
                permanentBreakCellArr[permanentBreakCellArrLength]=globalCellIndex;
                permanentBreakCellArrLength++;
            }
        }
        else{
            Debug.LogWarning(cellId +" :globalCellIndex is -1");
        }
        //tempBreakCellDictionary.AddOrSetValue(cellId,true); // may colision this is beacause not broken cells disappaer bug haapens 
        var biome=instanceIDToBiomeDictionary.GetValue(cellId);
        UnloadCell(cell,cellId,biome);
    }
    int permanentBreakCellArrLength=0;
    public override void BreakCells(GameObject[] cells,int length){
        for(int i=0;i<length;i++){
            var cell=cells[i];
            var cellId=cell.GetInstanceID();
            var globalCellIndex=instanceIDToGlobalCellIndexDictionary.GetValueOrDefault(cellId,int.MaxValue);
            if(globalCellIndex!=int.MaxValue){
                permanentBreakCellDictionary.AddOrSetValue(globalCellIndex,true);
                if(permanentBreakCellArrLength<permanentBreakCellArr.Length){
                    permanentBreakCellArr[permanentBreakCellArrLength]=globalCellIndex;
                    permanentBreakCellArrLength++;
                }
            }
            else{
                Debug.LogWarning(cellId +" :globalCellIndex is -1");
            }
            var biome=instanceIDToBiomeDictionary.GetValue(cellId);
            //tempBreakCellDictionary.AddOrSetValue(cellId,true); // may colision this is beacause not broken cells disappaer bug haapens 
            // breakしたセルはunloadのみにのみreturnしないとundetail,detailまわりの処理がおかしくなる
            meshCombiner.BreakCell(cell,cellId);
            //UnloadCell(cell,cellId,biome);
        }
        
    }
    void UnloadCell(GameObject cell,int id,Biome biomeType){
        if(!instanceIDToGlobalCellIndexDictionary.TryRemove(id)){
            Debug.LogWarning("UnloadCell: instanceIDToGlobalCellIndexDictionary not has item "+ id);
        }
        BiomeReturn(biomeType,cell,  id);
    }

     float scale => settings.scale;
    float TileScale=>settings.TileScale;

    float xValue,zValue,perlinValue,height,xTValue,zTValue,xHValue,zHValue;
    Vector3 spawnPos;
    [UdonSynced]float _seedX;
    [UdonSynced]float _seedZ;
    [UdonSynced]float _seedTX;
    [UdonSynced]float _seedTZ;
    [UdonSynced]float _seedHX;
    [UdonSynced]float _seedHZ;

    [SerializeField]Transform chunkParent;
    //[SerializeField]GameObject chunkPrefab;
    [SerializeField,UnrollAttribute]MeshCombiner meshCombiner;
    MeshFilter[] meshFilters;
    [SerializeField,UnrollAttribute]IntKeyGameObjectArrDictionary chunkToCellsDict;
    [InterfaceToClassAttribute("IPoolItemOperator","ChunkPoolItemOperator")][SerializeField,UnrollAttribute]UdonSavingObjectPool chunkPool;
    GameObject[] generatingCells;
    Biome[] generatingCellBiomes;
    int[] generatingCellInstanceIds;

    int chunkXIndex,chunkYIndex,chunkZIndex;
    int chunkID;
    public const string IndestructibleTag="Cell";
    public const string CellTag="Cell";

    int ToChunkIndex(int x,int z){
        return (z-ZSTARTINDEX)*chunkWidth + (x-XSTARTINDEX);
    }
    int ToLocalCellIndex(int xIndex,int yIndex,int zIndex){
        return (xIndex*chunkSize+zIndex)*chunkSizeY+yIndex;
    }

    int ToGlobalCellIndex(int localCellIndex,int chunkIndex){
        return localCellIndex+chunkIndex*chunkSize*chunkSize*chunkSizeY;
    }
    GameObject chunkObj; 
    GenerateChunkMode GenerateChunk(){
        int chunkIndex=ToChunkIndex(xStartChunkIndex,zStartChunkIndex);
        if(chunkStateDict.IsLoaded(chunkIndex))return GenerateChunkMode.Pass;
        bool isInit=false;
        if(chunkXIndex==0&&chunkZIndex==0&&chunkYIndex==0){
            //Debug.Log("GenerateChunk: "+chunkIndex);
            isInit=true;
            //gc alloc debug only
            //if(chunkObj!=null)Debug.LogError("chunkObj is not null");
            if(chunkIndexToInstanceIdDictionary.HasItem(chunkIndex)){
                int id=chunkIndexToInstanceIdDictionary.GetValue(chunkIndex);
                chunkObj = chunkPool.TryToSpawn(id);
            }else{
                chunkObj = chunkPool.TryToSpawn();
                chunkIndexToInstanceIdDictionary.Add(chunkIndex,chunkObj.GetInstanceID());
            }
            //Debug.Log("generate chunkID: "+chunkIndex);
            if(chunkToCellsDict.HasItem(chunkIndex)){
                generatingCells=chunkToCellsDict.GetValue(chunkIndex);
                generatingCellBiomes=chunkIndexToCellBiomesDictionary.GetValue(chunkIndex);
                generatingCellInstanceIds=chunkToCellInstanceIdsDictionary.GetValue(chunkIndex);
            } else {
                generatingCells= new GameObject[chunkSize*chunkSize*chunkSizeY];
                generatingCellBiomes=new Biome[chunkSize*chunkSize*chunkSizeY];
                generatingCellInstanceIds=new int[chunkSize*chunkSize*chunkSizeY];
            }  
        }
        //MeshFilter[] meshFilters = new MeshFilter[chunkSize*chunkSize];
        var resCell=GenerateCell(xStartChunkIndex*chunkSize+chunkXIndex,zStartChunkIndex*chunkSize+chunkZIndex,chunkYIndex-chunkSizeY+1,null);
        /*if(chunkYIndex<indestructibleLine){
            resCell.layer=LayerMask.NameToLayer(IndestructibleTag);
        } else {
            resCell.layer=LayerMask.NameToLayer(CellTag);
        }*/

        int localIndex=ToLocalCellIndex(chunkXIndex,chunkYIndex,chunkZIndex);
        int instanceId=resCell.GetInstanceID();
        meshFilters[localIndex]=resCell.GetComponent<MeshFilter>();
        generatingCells[localIndex]=resCell;
        generatingCellInstanceIds[localIndex]=instanceId;
        generatingCellBiomes[localIndex]=currentCellBiome;
        //colOnlyArr[localIndex]=chunkYIndex==chunkSizeY-1;
        //visibleArr[localIndex]=(chunkYIndex==chunkSizeY-1) || (chunkXIndex==0 || chunkXIndex==chunkSize-1 ) || (chunkZIndex==0 || chunkZIndex==chunkSize-1);
        instanceIDToGlobalCellIndexDictionary.Add(instanceId,ToGlobalCellIndex(localIndex,chunkIndex));
        instanceIDToBiomeDictionary.AddOrSetValue(instanceId,currentCellBiome);
        chunkYIndex++;
        if(chunkYIndex==chunkSizeY){
            chunkYIndex=0;
            chunkZIndex++;
            //resCell.name+=IndestructibleTag; // 底は破壊不可能にする
            if(chunkZIndex==chunkSize){
                chunkZIndex=0;
                chunkXIndex++;
                if(chunkXIndex==chunkSize){
                    if(!chunkToCellsDict.HasItem(chunkIndex)){
                        chunkToCellsDict.Add(chunkIndex,generatingCells);
                        chunkIndexToCellBiomesDictionary.Add(chunkIndex,generatingCellBiomes);
                        chunkToCellInstanceIdsDictionary.Add(chunkIndex,generatingCellInstanceIds);
                    } else {
                        // no need to call
                        /*chunkToCellsDict.SetValue(chunkIndex,generatingCells);
                        chunkIndexToCellBiomesDictionary.SetValue(chunkIndex,generatingCellBiomes);
                        chunkToCellInstanceIdsDictionary.SetValue(chunkIndex,generatingCellInstanceIds);
                        */
                    }
                    bool isInDetailRange=IsInDetailRange(GetPlayerPosition(),xStartChunkIndex,zStartChunkIndex);
                    for(int i=0;i<generatingCells.Length;i++){
                        brokenArr[i]=isInDetailRange&& permanentBreakCellDictionary.GetValueOrDefault(ToGlobalCellIndex(i,chunkIndex),false);
                    }
                    chunkStateDict.AddOrSetValue(chunkIndex,isInDetailRange?ChunkState.Detailed:ChunkState.UnDetailed);
                    var hasIItem=false;//outlineDict.HasItem(chunkIndex);
                    meshCombiner.CombineMesh(chunkObj,meshFilters,generatingCellInstanceIds,isInDetailRange,brokenArr,hasIItem);
                    //if(!hasIItem){
                    outlineDict.AddOrSetValue(chunkIndex,chunkObj);
                    //}
                    //chunkObj=null;
                    return GenerateChunkMode.Complete;
                }
            }
        }
        if(isInit)return GenerateChunkMode.Init;
        return GenerateChunkMode.Generating;
    }


    [SerializeField]int indestructibleLine=2;
    [SerializeField]Transform playerSpawnPos;
    [SerializeField]float IndestructibleRange=5;
    public GameObject GenerateCell(int xIndex,int zIndex,int yIndex,Transform parent){
        xValue = xIndex * scale + _seedX;
        zValue = zIndex  * scale+ _seedZ;
        perlinValue = fbm(xValue,zValue,4,0.4f);//Mathf.PerlinNoise(xValue, zValue);
        height = fieldHeight * (perlinValue-waterPercentage);
        height = Mathf.Round(height);
        spawnPos=new Vector3(xIndex,height+yIndex,zIndex)*TileScale;
        //spawnPos = new Vector3((xIndex)*TileScale,height,zIndex*TileScale);

        
        biomeCell = GenerateBiome(xIndex,zIndex,parent);
        biomeCell.transform.localPosition = spawnPos;
        ///////////////////////////
        // WARNING: 特別処理
        float playerSpawnChunkDist = (playerSpawnPos.position.x-spawnPos.x)*(playerSpawnPos.position.x-spawnPos.x)+(playerSpawnPos.position.z-spawnPos.z)*(playerSpawnPos.position.z-spawnPos.z);
        if (playerSpawnChunkDist<IndestructibleRange*IndestructibleRange)
        {
            biomeCell.layer = 22;
        }
        ////////////////////////////
        //gc alloc
        /*if(biomeCell!=null){
            biomeCell.transform.localPosition = spawnPos;
        } else {
            Debug.LogWarning("Chunk pool is empty!");
        }*/
        return biomeCell;
    }

    GameObject biomeCell;

    float scaleT=>settings.scaleT;
    float scaleH=>settings.scaleH;

    bool isInstantiated=false;
    Biome currentCellBiome=0;
    GameObject GenerateBiome(int xIndex,int zIndex,Transform parent){
        xTValue = xIndex * scaleT + _seedTX ;
        zTValue = zIndex  * scaleT+ _seedTZ;
        xHValue = xIndex * scaleH + _seedHX ;
        zHValue = zIndex  * scaleH+ _seedHZ;
        float temperature = Mathf.PerlinNoise(xTValue, zTValue);
        float humidity = Mathf.PerlinNoise(xHValue, zHValue);
        //humidity *= Mathf.Sqrt(1-(1-temperature)*(1-temperature));
        Biome biomeType;
        if(height<=0)biomeType=Biome.Water;
        else{
            int tempIndex=Math.Min((int)(temperature * 5), 4);
            int humIndex=Math.Min((int)(humidity * 5), 4);
            switch(mission.PlanetType){
                case PlanetType.Earth:
                    biomeType=biomeEarthMap[tempIndex][humIndex];
                    break;
                case PlanetType.Moon:
                    biomeType=biomeMoonMap[tempIndex][humIndex];
                    break;
                case PlanetType.Mars:
                    biomeType=biomeMarsMap[tempIndex][humIndex];
                    break;
                default:
                    Debug.LogError("PlanetType is not set");
                    biomeType=Biome.Water;
                    break;
            }
        }
        var cell=BiomeTryToSpawn(biomeType);
        currentCellBiome=biomeType;
        isInstantiated=BiomePeekIsInstantiated(biomeType);
        return cell;
    }

    float fbmTotal,fbmFrequency,fbmAmplitude,fbmMaxValue;
    int fbmIndex;


    float fbm (float x, float y, int octaves, float persistence) {
        fbmTotal = 0;
        fbmFrequency = 1;
        fbmAmplitude = 1;
        fbmMaxValue = 0;  // Used for normalizing result to 0.0 - 1.0

        for(fbmIndex=0; fbmIndex<octaves; fbmIndex++) {
            fbmTotal += Mathf.PerlinNoise(x * fbmFrequency, y * fbmFrequency) * fbmAmplitude;
            
            fbmMaxValue += fbmAmplitude;
            
            fbmAmplitude *= persistence;
            fbmFrequency *= 2;
        }
        return fbmTotal/fbmMaxValue;
    }

}
