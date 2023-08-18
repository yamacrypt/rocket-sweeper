using System;

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonObjectPool;
enum GenerateChunkMode{
    Init,
    Pass,
    Generating,
    Complete
}
enum Biome{
    Grass,
    Tundra,
    Desert,
    Rock,
    Sand,
    Water,
    Dirt,
    Dark
}
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapGenerator : UdonSharpBehaviour
{
    [SerializeField]UdonSavingObjectPool grassPool;
    [SerializeField]UdonSavingObjectPool tundraPool;
    [SerializeField]UdonSavingObjectPool desertPool;
    [SerializeField]UdonSavingObjectPool dirtPool;
    [SerializeField]UdonSavingObjectPool rockPool;
    [SerializeField]UdonSavingObjectPool sandPool;
    [SerializeField]UdonSavingObjectPool waterPool;
    [SerializeField]UdonSavingObjectPool darkPool;
    [SerializeField]MapGeneratorSetting settings;

    Biome[][] biomeMap=new Biome[][]{
        new Biome[]{Biome.Dirt,Biome.Dirt,Biome.Dark,Biome.Dark,Biome.Rock},
        new Biome[]{Biome.Dirt,Biome.Dirt,Biome.Dark,Biome.Grass,Biome.Rock},
        new Biome[]{Biome.Tundra,Biome.Dark,Biome.Grass,Biome.Rock,Biome.Sand},
        new Biome[]{Biome.Tundra,Biome.Grass,Biome.Rock,Biome.Desert,Biome.Sand},
        new Biome[]{Biome.Tundra,Biome.Grass,Biome.Desert,Biome.Desert,Biome.Sand},
    };

    UdonSavingObjectPool GetBiomePool(Biome biome){
        switch(biome){
            case Biome.Grass:
                return grassPool;
            case Biome.Tundra:
                return tundraPool;
            case Biome.Desert:
                return desertPool;
            case Biome.Rock:
                return rockPool;
            case Biome.Sand:
                return sandPool;
            case Biome.Water:
                return waterPool;
            case Biome.Dirt:
                return dirtPool;
            case Biome.Dark:
                return darkPool;
            default:
                Debug.LogError("Invalid biome: "+biome);
                return null;
        }
    }

    UdonSavingObjectPool GetBiomePool(int biomeIndex){
        switch(biomeIndex){
            case (int)Biome.Grass:
                return grassPool;
            case (int)Biome.Tundra:
                return tundraPool;
            case (int)Biome.Desert:
                return desertPool;
            case (int)Biome.Rock:
                return rockPool;
            case (int)Biome.Sand:
                return sandPool;
            case (int)Biome.Water:
                return waterPool;
            case (int)Biome.Dirt:
                return dirtPool;
            case (int)Biome.Dark:
                return darkPool;
            default:
                Debug.LogError("Invalid biome: "+biomeIndex);
                return null;
        }
    }

    int chunkSize=>settings.chunkSize;

    int chunkWidth=>settings.chunkWidth;
    int chunkDepth=>settings.chunkDepth;
    int fieldHeight=>settings.fieldHeight;
    int xStartChunkIndex,zStartChunkIndex;
    int xEndChunkIndex,zEndChunkIndex;
    int xSearchStartChunkIndex,zSearchStartChunkIndex;
    int xSearchEndChunkIndex,zSearchEndChunkIndex;
    int batchCount=>settings.batchCount;
    float chunkLoadRange=>settings.chunkLoadRange;
    float generateAdditionalInterval=>settings.generateAdditionalInterval *2.5f / Networking.LocalPlayer.GetWalkSpeed();
    float cellAnimationTime=>settings.cellAnimationTime;
    float removeBatchCount=>settings.removeBatchCount;
    float removeInterval=>settings.removeInterval;
    float waterPercentage=>settings.waterPercentage;
    float chunkUnLoadRange=>settings.chunkUnLoadRange;
    float chunkDetailRange=>settings.chunkDetailRange;
    float operationBatchCount=>settings.operationBatchCount;
    [SerializeField]int chunkCapacity=15000;

    public override void OnDeserialization()
    {
        isSynced=true;
    }
    bool isSynced=false;
    void Assert(bool b){
        if(!b){
            Debug.LogError("Assertion failed!");
        }
    }

    UdonSavingObjectPool[] pools;
    string[] prefabNames;
    void Start()
    {
        pools = new UdonSavingObjectPool[]{
            grassPool,
            tundraPool,
            desertPool,
            dirtPool,
            rockPool,
            sandPool,
            waterPool,
            darkPool
        };
        prefabNames = new string[pools.Length];
        for(int i=0;i<pools.Length;i++){
            prefabNames[i]=pools[i].prefabName;
        }

        Assert(outlineDict!=chunkToCellsDict);
        Assert(removedChunkIndexDict!=loadedChunkIndexDict);
        Assert(loadedChunkIndexDict!=detailChunkIndexDict);
        outlineDict.SetCapacity(chunkCapacity);
        removedChunkIndexDict.SetCapacity(chunkCapacity);
        loadedChunkIndexDict.SetCapacity(chunkCapacity);
        chunkToCellsDict.SetCapacity(chunkCapacity);
        detailChunkIndexDict.SetCapacity(chunkCapacity);
        searchQueue.SetCapacity(1000);
        detailQueue.SetCapacity(1000);
        tempBreakCellDictionary.SetCapacity(chunkCapacity);
        permanentBreakCellDictionary.SetCapacity(150000);
        brokenArr=new bool[chunkSize*chunkSize*chunkSize];
        _seedX = UnityEngine.Random.value * (float)Int16.MaxValue;
        _seedZ = UnityEngine.Random.value * (float)Int16.MaxValue;
        _seedTX = UnityEngine.Random.value * (float)Int16.MaxValue;
        _seedTZ = UnityEngine.Random.value * (float)Int16.MaxValue;
        _seedHX = UnityEngine.Random.value * (float)Int16.MaxValue;
        _seedHZ = UnityEngine.Random.value * (float)Int16.MaxValue;
        isSynced=true;
        RequestSerialization();
        /*chunkWidth=settings.chunkWidth;
        chunkDepth=settings.chunkDepth;
        chunkSize=settings.chunkSize;
        batchCount=settings.batchCount;
        generateInterval=settings.generateInterval;
        chunkLoadRange=settings.chunkLoadRange;
        generateAdditionalInterval=settings.generateAdditionalInterval *2.5f / Networking.LocalPlayer.GetWalkSpeed();
        cellAnimationTime=settings.cellAnimationTime;*/
        meshFilters= new MeshFilter[chunkSize*chunkSize*chunkSize];
        XSTARTINDEX=-chunkWidth/2;
        XENDINDEX=chunkWidth-1-chunkWidth/2;
        ZSTARTINDEX=0;
        ZENDINDEX=chunkDepth;
    }
    int XSTARTINDEX,XENDINDEX,ZSTARTINDEX,ZENDINDEX;
    public void GenerateInit(){
        if(!isSynced)return;
        if(isGenerating)return;
        isGenerating=true;
        playerPos = Networking.LocalPlayer.GetPosition() /(chunkSize*TileScale);
        CalcPosCorrection();
        zStartChunkIndex=GetInRangeStartZChunkIndex(playerPos+posCorrection,chunkLoadRange);
        zEndChunkIndex=GetInRangeEndZChunkIndex(playerPos+posCorrection,chunkLoadRange);
        xStartChunkIndex=GetInRangeStartXChunkIndex(playerPos+posCorrection,zStartChunkIndex,chunkLoadRange);
        xEndChunkIndex=GetInRangeEndXChunkIndex(playerPos+posCorrection,zStartChunkIndex,chunkLoadRange);
        //GenerateInitCellInterval();
        removeXIndex=XSTARTINDEX;
        removeZIndex=ZSTARTINDEX;

        chunkXIndex=0;
        chunkZIndex=0;
        chunkYIndex=0;
        isRemoving=true;
        isSearching=true;
        //RemoveChunkInterval();
    }

    float searchAdditionalInterval=>settings.searchAdditionalInterval;
    Vector3 posCorrection=Vector3.zero;
    Vector3 GetPlayerPosition(){
        return Networking.LocalPlayer.GetPosition() / (chunkSize*TileScale);
    }
    void CalcPosCorrection(){
        playerRot = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        // 極座標へ変換
        float r = 2.5f; // 半径は通常1とするか、必要に応じて変更
        var rad=Mathf.PI /2 - playerRot.eulerAngles.y*Mathf.Deg2Rad;
        posCorrection.x=Mathf.Cos(rad)*r;
        posCorrection.z=Mathf.Sin(rad)*r;
        Debug.Log("posCorrection: "+posCorrection);
    }
    void GenerateAdditional(){
        if(isGenerating)return;
        prePlayerPos=playerPos;
        playerPos = GetPlayerPosition();//Networking.LocalPlayer.GetPosition() / (chunkSize*TileScale);
        CalcPosCorrection();
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

    void SearchAdditional(){
        if(searchOn){
            if(isSearching){
                Debug.LogWarning("already searching");
            }
            zSearchStartChunkIndex=GetInRangeStartZChunkIndex(playerPos,chunkUnLoadRange+searchMargin);;
            zSearchEndChunkIndex=GetInRangeEndZChunkIndex(playerPos,chunkUnLoadRange+searchMargin);;
            xSearchStartChunkIndex=GetInRangeStartXChunkIndex(playerPos,zSearchStartChunkIndex,chunkUnLoadRange+searchMargin);
            xSearchEndChunkIndex=GetInRangeEndXChunkIndex(playerPos,zSearchStartChunkIndex,chunkUnLoadRange+searchMargin);
        
            isSearching=true;
        }
    }

 
    Vector3 playerPos;
    Quaternion playerRot;
    Vector3 prePlayerPos;
    float ellipseZ=1f;
    public int GetInRangeStartXChunkIndex(Vector3 pos,float z,float range ){
        float diff =range*range - (pos.z-z)*(pos.z-z)/ (ellipseZ * ellipseZ);
        if(diff<=0)return chunkWidth/2;
        return Math.Max(XSTARTINDEX,(int)(pos.x-Mathf.Sqrt(diff)));
    }

    public int GetInRangeEndXChunkIndex(Vector3 pos,float z,float range ){
        float diff = Math.Max(0,range*range - (pos.z-z)*(pos.z-z)/ (ellipseZ * ellipseZ));
        if(diff==0)return -chunkWidth/2;
        return Math.Min(XENDINDEX,(int)(pos.x+Mathf.Sqrt(diff)));
    }

    public int GetInRangeStartZChunkIndex(Vector3 pos,float range ){
        return Math.Max(ZSTARTINDEX,(int)(pos.z-range*ellipseZ));
    }

    public int GetInRangeEndZChunkIndex(Vector3 pos,float range ){
        return Math.Min(ZENDINDEX,(int)(pos.z+range*ellipseZ));
    }

    bool IsInDetailRange(Vector3 pPos,int x,int z){
        return (pPos.x-x)*(pPos.x-x)+(pPos.z-z)*(pPos.z-z)/(ellipseZ*ellipseZ)<chunkDetailRange*chunkDetailRange;
    }

    int exceptXStartChunkIndex;
    int exceptXEndChunkIndex;
    int exceptZStartChunkIndex;
    int exceptZEndChunkIndex;
    bool isGenerating=false;
    [SerializeField]IntKeyGameObjectArrDictionary outlineDict;
    [SerializeField]IntKeyBoolDictionary removedChunkIndexDict;
    [SerializeField]IntKeyBoolDictionary loadedChunkIndexDict;
    float generateAdditionalDelta=-1f;
    float searchAdditionalDelta=-1f;

    int isInstantiatedCost=10;
    int poolCost=3;
    int storeCount=0;
    int storeMax=3;
    int biomeIndex=0;
    UdonSavingObjectPool storeBiomePool;


    void StoreCell(){
        storeBiomePool=GetBiomePool(biomeIndex);
        storeBiomePool.Store();
        isInstantiated=storeBiomePool.PeekIsInstantiated();
        biomeIndex++;
        biomeIndex%=(int)Biome.Dark;
    }
    int costIndex;
    GenerateChunkMode mode;

    void Update()
    {
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
                    if(storeCount==storeMax){
                        StoreCell();
                        storeCount=0;
                    }
                    if(isInstantiated){
                        costIndex+=isInstantiatedCost;
                        isInstantiated=false;
                    }else{
                        costIndex++;
                    }
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
                            //SendCustomEventDelayedSeconds(nameof(GenerateAdditional),generateAdditionalInterval);
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
        /*if(isRemoving&&removeOn){
            //if(isUnLoading)return;
            for( removeCount=0;removeCount<removeBatchCount;removeCount++){
                if(removeXIndex>=XENDINDEX){
                    removeZIndex++;
                    removeXIndex=XSTARTINDEX;
                    if(removeZIndex>=ZENDINDEX){
                        isRemoving=false;
                        break;
                    }
                } else {
                    removeXIndex++;
                }
                float delay=removeInterval * removeCount / removeBatchCount;
                removeCount+=RemoveChunk(delay);
            }
        }*/
        

        if(isSearching){
            //if(isUnLoading)return;
            for(costIndex=0;costIndex<searchBatchCount;costIndex++){
                var x=xSearchStartChunkIndex;
                var z=zSearchStartChunkIndex;
                var chunkIndex=ToChunkIndex(x,z);
                if(loadedChunkIndexDict.HasItem(chunkIndex)&&loadedChunkIndexDict.GetValue(chunkIndex)){
                    var diffx=x-playerPos.x;
                    var diffz=z-playerPos.z;
                    var distance = diffx*diffx+diffz*diffz;
                    if(distance>=chunkUnLoadRange*chunkUnLoadRange){
                        searchQueue.Enqueue(new Vector4(x,0,z,(int)ChunkOperation.UnLoad));
                        //UnLoadChunk((int)x,(int)z,true);
                    } else if(distance<=chunkDetailRange*chunkDetailRange){
                        if(!detailChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
                            detailQueue.Enqueue(new Vector4(x,0,z,(int)ChunkOperation.Detail));
                        }
                        //DetailChunk((int)x,(int)z);
                    } else {
                        if(detailChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
                            searchQueue.Enqueue(new Vector4(x,0,z,(int)ChunkOperation.UnDetail));
                        }
                        //UnDetailChunk((int)chunkPos.x,(int)chunkPos.z);
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
            }
        } else{
            if(searchAdditionalDelta>=0){
                searchAdditionalDelta-=Time.deltaTime;
                if(searchAdditionalDelta<0){
                    SearchAdditional();
                }
            } else{
                if(searchQueue.Count==0&&detailQueue.Count==0){
                    searchAdditionalDelta=searchAdditionalInterval; // search処理で最後に一回だけ呼ばれる
                };
            }
        }
        if(searchQueue.Count>0||detailQueue.Count>0){
            for(costIndex=0;costIndex<operationBatchCount;){
                Vector4 operation;
                if(detailQueue.Count>0){
                    operation=detailQueue.Dequeue();
                } else {
                    if(searchQueue.Count==0)break;
                    operation=searchQueue.Dequeue();
                }
                int mode = (int)operation.w;
                if(mode==(int)ChunkOperation.Detail){
                    //Debug.Log("operation detail: "+operation);
                    if(DetailChunk((int)operation.x,(int)operation.z))costIndex++;
                } else if(mode==(int)ChunkOperation.UnDetail){
                    //Debug.Log("operation undetail: "+operation);
                    if(UnDetailChunk((int)operation.x,(int)operation.z))costIndex++;
                } else if(mode==(int)ChunkOperation.UnLoad){
                    //Debug.Log("operation unload: "+operation);
                    if(UnLoadChunk((int)operation.x,(int)operation.z,true))costIndex++;
                } else {
                    Debug.LogError("Invalid operation mode: "+mode);
                }
            }

        }
    }
    float searchMargin=>chunkDetailRange*2;

    bool isSearching=false;

    int searchBatchCount=>settings.searchBatchCount;

    int removeXIndex=0;
    int removeZIndex=0;
    int removeCount=0;
    bool isRemoving=false;

    bool removeOn=>settings.removeOn;
    bool searchOn=> settings.searchOn;
    
    [SerializeField]MapChunkOperatorGroup chunkOperatorGroup;
    [SerializeField]MapCellOperatorGroup cellOperatorGroup;

    public int RemoveChunk(float delay){
        int chunkIndex=ToChunkIndex(removeXIndex,removeZIndex);
        //Debug.Log("Try RemoveChunk: "+chunkIndex);
        removedChunkIndexDict.Add(chunkIndex,true);    
        if(outlineDict.HasItem(chunkIndex) && loadedChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
            UnDetailChunk((int)removeXIndex,(int)removeZIndex);
            var allParts = outlineDict.GetValue(chunkIndex);
            Debug.Log("RemoveChunk: "+chunkIndex);
            chunkOperatorGroup.StartAnimation(allParts[0],AnimDir.Down,cellAnimationTime,delay,this,removeXIndex,removeZIndex);
            for(int i=1;i<allParts.Length;i++){
                cellOperatorGroup.StartAnimation(allParts[i],AnimDir.Down,cellAnimationTime,delay);
                // TODO: set call back for cell return;
            }
        }
        return chunkSize*chunkSize*chunkSize;
    }

    int ToChunkIndex(int x,int z){
        return (z-ZSTARTINDEX)*chunkWidth + (x-XSTARTINDEX);
    }
    public const float AnimHeight=10f;

    [SerializeField]ChunkOperationQueue searchQueue;
    [SerializeField]ChunkOperationQueue detailQueue;
    
    [SerializeField]IntKeyBoolDictionary detailChunkIndexDict;

    bool DetailChunk(int xIndex,int zIndex){
        int chunkIndex=ToChunkIndex(xIndex,zIndex);
        if(!detailChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
            if(outlineDict.HasItem(chunkIndex)){
                //Debug.Log("DetailChunk: "+chunkIndex);
                var outlines = outlineDict.GetValue(chunkIndex);
                var cells=chunkToCellsDict.GetValue(chunkIndex);
                for(int i=0;i<cells.Length;i++){
                    brokenArr[i]=false;
                    if(tempBreakCellDictionary.HasItem(cells[i].GetInstanceID())||
                    permanentBreakCellDictionary.HasItem(ToGlobalCellIndex(i,chunkIndex))){
                        brokenArr[i]=true;
                    }
                }
                meshCombiner.SwitchCombineMesh(false,outlines,cells,brokenArr);
                detailChunkIndexDict.AddOrSetValue(chunkIndex,true);
                return true;
            }else {
                Debug.LogWarning("DetailChunk: chunkIndexDict not has item");
            }
        }
        return false;
    }

    bool[] brokenArr;

    bool UnDetailChunk(int xIndex,int zIndex){
        int chunkIndex=ToChunkIndex(xIndex,zIndex);
        if(detailChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
            if(outlineDict.HasItem(chunkIndex)){
                //Debug.Log("UnDetailChunk: "+chunkIndex);
                var outlines = outlineDict.GetValue(chunkIndex);
                var cells=chunkToCellsDict.GetValue(chunkIndex);
                meshCombiner.SwitchCombineMesh(true,outlines,cells,brokenArr);
                detailChunkIndexDict.AddOrSetValue(chunkIndex,false);
                return true;
            } else {
                Debug.LogWarning("UnDetailChunk: chunkIndexDict not has item");
            }
        }
        return false;
    }

    public bool UnLoadChunk(int xIndex,int zIndex,bool setOnly=false){
        int chunkIndex=ToChunkIndex(xIndex,zIndex);
        if(!loadedChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
            Debug.Log("UnLoadChunk: not yet loaded");
            //遅延実行の関係上このケースは起こりえる
            return false;
        }
        if(outlineDict.HasItem(chunkIndex)){
            //Debug.Log("UnLoadChunk:"+chunkIndex);
            var allParts = outlineDict.GetValue(chunkIndex);
            if(setOnly){
                loadedChunkIndexDict.SetValue(chunkIndex,false);
            } else {
                loadedChunkIndexDict.AddOrSetValue(chunkIndex,false);
            }
            //chunkIndexDict.Remove(chunkIndex);
            var chunkMesh=allParts[0];
            GameObject[] removeCells=null;
            //var id=chunkMesh.GetInstanceID();
            //Debug.Log("unload chunkID: "+id);
            if(chunkToCellsDict.HasItem(chunkIndex)){
                removeCells = chunkToCellsDict.GetValue(chunkIndex);
            } else {
                Debug.LogWarning("unload chunk id not exists");
            }
            if(chunkMesh!=null){
                chunkPool.Return(chunkMesh,true);
            } else {
                Debug.LogWarning("Chunk pool is empty!");
            }
            for(int i=0;i<removeCells.Length;i++){
                var cell=removeCells[i];
                if(cell!=null){
                    if(tempBreakCellDictionary.TryRemove(cell.GetInstanceID())){
                        permanentBreakCellDictionary.Add(ToGlobalCellIndex(i,chunkIndex),true);
                        Debug.Log("permnanet break:" + ToGlobalCellIndex(i,chunkIndex));
                    } else{
                        UnloadCell(cell);
                    }
                } else {
                    Debug.LogWarning("Cell pool is empty!");
                }
            }
            return true;
        }
        return false;
    }

    [SerializeField]IntKeyBoolDictionary tempBreakCellDictionary;
    [SerializeField]IntKeyBoolDictionary permanentBreakCellDictionary;
    public void BreakCell(GameObject cell){
        if(cell!=null){
            var cellIndex=cell.GetInstanceID();
            tempBreakCellDictionary.AddOrSetValue(cellIndex,true);
            UnloadCell(cell);
        } else {
            Debug.LogWarning("Cell pool is empty!");
        }
    }

    void UnloadCell(GameObject cell){
        for(int i=0;i<prefabNames.Length;i++){
            if(cell.name.Contains(prefabNames[i])){
                pools[i].Return(cell,true);
                return;
            }
        }
        Debug.LogWarning("Cell "+ cell.name+" pool is empty! Or Make sure obj is cell type");
        /*if(grassPool.IsMine(cell)){
            grassPool.Return(cell,true);
        } else if(sandPool.IsMine(cell)){
            sandPool.Return(cell,true);
        } else if(waterPool.IsMine(cell)){
            waterPool.Return(cell,true);
        } else if(rockPool.IsMine(cell)){
            rockPool.Return(cell,true);
        } else if(dirtPool.IsMine(cell)){
            dirtPool.Return(cell,true);
        } else if(desertPool.IsMine(cell)){
            desertPool.Return(cell,true);
        } else if(tundraPool.IsMine(cell)){
            tundraPool.Return(cell,true);
        } else if(darkPool.IsMine(cell)){
            darkPool.Return(cell,true);
        } else {
            Debug.LogWarning("Cell "+ cell.name+" pool is empty! Or Make sure obj is cell type");
        } */
  
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
    [SerializeField]MeshCombiner meshCombiner;
    MeshFilter[] meshFilters;
    [SerializeField]IntKeyGameObjectArrDictionary chunkToCellsDict;
    [SerializeField]UdonSavingObjectPool chunkPool;
    GameObject[] generatingCells;

    int chunkXIndex,chunkYIndex,chunkZIndex;
    GameObject chunkObj; 
    int chunkID;
    public const string IndestructibleTag="Ignore Raycast";
    public const string CellTag="Cell";


    int ToLocalCellIndex(int xIndex,int yIndex,int zIndex){
        return (xIndex*chunkSize+zIndex)*chunkSize+yIndex;
    }

    int ToGlobalCellIndex(int localCellIndex,int chunkIndex){
        return localCellIndex+chunkIndex*chunkSize*chunkSize*chunkSize;
    }

    GenerateChunkMode GenerateChunk(){
        int chunkIndex=ToChunkIndex(xStartChunkIndex,zStartChunkIndex);
        if(loadedChunkIndexDict.GetValueOrDefault(chunkIndex,false))return GenerateChunkMode.Pass;
        /*if(removedChunkIndexDict.HasItem(chunkIndex)){
            if(chunkObj!=null){
                chunkPool.Return(chunkObj,true);
                foreach(var cell in generatingCells){
                    if(cell==null)break;
                    UnloadCell(cell);
                }
                chunkObj=null;
            }
            return GenerateChunkMode.Pass;
        }*/
        bool isInit=false;
        if(chunkXIndex==0&&chunkZIndex==0&&chunkYIndex==0){
            //Debug.Log("GenerateChunk: "+chunkIndex);
            isInit=true;
            if(chunkObj!=null)Debug.LogError("chunkObj is not null");
            chunkObj = chunkPool.TryToSpawn();
            //chunkID=chunkObj.GetInstanceID();
            //Debug.Log("generate chunkID: "+chunkIndex);
            if(chunkToCellsDict.HasItem(chunkIndex)){
                generatingCells=chunkToCellsDict.GetValue(chunkIndex);
            } else {
                generatingCells= new GameObject[chunkSize*chunkSize*chunkSize];
            }  
        }
        //GameObject.Instantiate(chunkPrefab,Vector3.zero,Quaternion.identity,chunkParent);
        //MeshFilter[] meshFilters = new MeshFilter[chunkSize*chunkSize];
        var resCell=GenerateCell(xStartChunkIndex*chunkSize+chunkXIndex,zStartChunkIndex*chunkSize+chunkZIndex,chunkYIndex-chunkSize+1,null);
        if(chunkYIndex<indestructibleLine){
            resCell.layer=LayerMask.NameToLayer(IndestructibleTag);
        } else {
            resCell.layer=LayerMask.NameToLayer(CellTag);
        }
        meshFilters[ToLocalCellIndex(chunkXIndex,chunkYIndex,chunkZIndex)]=resCell.GetComponent<MeshFilter>();
        generatingCells[ToLocalCellIndex(chunkXIndex,chunkYIndex,chunkZIndex)]=resCell;
        chunkYIndex++;
        if(chunkYIndex==chunkSize){
            chunkYIndex=0;
            chunkZIndex++;
            //resCell.name+=IndestructibleTag; // 底は破壊不可能にする
            if(chunkZIndex==chunkSize){
                chunkZIndex=0;
                chunkXIndex++;
                if(chunkXIndex==chunkSize){
                    chunkToCellsDict.AddOrSetValue(chunkIndex,generatingCells);
                    bool isInDetailRange=IsInDetailRange(GetPlayerPosition(),xStartChunkIndex,zStartChunkIndex);
                    if(isInDetailRange){
                        for(int i=0;i<generatingCells.Length;i++){
                            brokenArr[i]=false;
                            if(permanentBreakCellDictionary.HasItem(ToGlobalCellIndex(i,chunkIndex))){
                                brokenArr[i]=true;
                            }
                        }
                    }
                    var allParts = meshCombiner.CombineMesh(chunkObj,meshFilters,isInDetailRange,brokenArr);
                    chunkObj=null;
                    outlineDict.AddOrSetValue(chunkIndex,allParts);
                    loadedChunkIndexDict.AddOrSetValue(chunkIndex,true);
                    return GenerateChunkMode.Complete;
                }
            }
        }
        if(isInit)return GenerateChunkMode.Init;
        return GenerateChunkMode.Generating;
    }


    int indestructibleLine=2;
    public GameObject GenerateCell(int xIndex,int zIndex,int yIndex,Transform parent){
        xValue = xIndex * scale + _seedX;
        zValue = zIndex  * scale+ _seedZ;
        perlinValue = fbm(xValue,zValue,4,0.4f);//Mathf.PerlinNoise(xValue, zValue);
        height = fieldHeight * (perlinValue-waterPercentage);
        height = Mathf.Round(height);
        spawnPos=new Vector3(xIndex,height+yIndex,zIndex)*TileScale;
        //spawnPos = new Vector3((xIndex)*TileScale,height,zIndex*TileScale);

        
        biomeCell = GenerateBiome(xIndex,zIndex,parent);
        if(biomeCell!=null){
            biomeCell.transform.localPosition = spawnPos;
        } else {
            Debug.LogWarning("Chunk pool is empty!");
        }
        return biomeCell;
    }

    GameObject biomeCell;

    float scaleT=>settings.scaleT;
    float scaleH=>settings.scaleH;

    bool isInstantiated=false;
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
            biomeType=biomeMap[tempIndex][humIndex];
        }
        var pool = GetBiomePool(biomeType);
        var cell=pool.TryToSpawn();
        isInstantiated=pool.PeekIsInstantiated();
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
