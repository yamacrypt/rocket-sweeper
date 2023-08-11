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
    void Start()
    {
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
        zStartChunkIndex=GetInRangeStartZChunkIndex(playerPos,chunkLoadRange);
        zEndChunkIndex=GetInRangeEndZChunkIndex(playerPos,chunkLoadRange);
        xStartChunkIndex=GetInRangeStartXChunkIndex(playerPos,zStartChunkIndex,chunkLoadRange);
        xEndChunkIndex=GetInRangeEndXChunkIndex(playerPos,zStartChunkIndex,chunkLoadRange);
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

    void GenerateAdditional(){
        if(isGenerating)return;
        prePlayerPos=playerPos;
        playerPos = Networking.LocalPlayer.GetPosition() / (chunkSize*TileScale);
        Debug.Log("GenerateAdditional");
        Debug.Log("player pos: "+playerPos);
        zStartChunkIndex=GetInRangeStartZChunkIndex(playerPos,chunkLoadRange);
        zEndChunkIndex=GetInRangeEndZChunkIndex(playerPos,chunkLoadRange);
        xStartChunkIndex=GetInRangeStartXChunkIndex(playerPos,zStartChunkIndex,chunkLoadRange);//-1;
        xEndChunkIndex=GetInRangeEndXChunkIndex(playerPos,zStartChunkIndex,chunkLoadRange);//-1;
        exceptXStartChunkIndex=GetInRangeStartXChunkIndex(prePlayerPos,zStartChunkIndex,chunkLoadRange);
        exceptXEndChunkIndex=GetInRangeEndXChunkIndex(prePlayerPos,zStartChunkIndex,chunkLoadRange);

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
    Vector3 prePlayerPos;
    public int GetInRangeStartXChunkIndex(Vector3 pos,float z,float range ){
        float diff =range*range - (pos.z-z)*(pos.z-z);
        if(diff<=0)return chunkWidth/2;
        return Math.Max(XSTARTINDEX,(int)(pos.x-Mathf.Sqrt(diff)));
    }

    public int GetInRangeEndXChunkIndex(Vector3 pos,float z,float range ){
        float diff = Math.Max(0,range*range - (pos.z-z)*(pos.z-z));
        if(diff==0)return -chunkWidth/2;
        return Math.Min(XENDINDEX,(int)(pos.x+Mathf.Sqrt(diff)));
    }

    public int GetInRangeStartZChunkIndex(Vector3 pos,float range ){
        return Math.Max(ZSTARTINDEX,(int)(pos.z-range));
    }

    public int GetInRangeEndZChunkIndex(Vector3 pos,float range ){
        return Math.Min(ZENDINDEX,(int)(pos.z+range));
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
    void StoreCell(){
        var pool=GetBiomePool(biomeIndex);
        pool.Store();
        isInstantiated=pool.PeekIsInstantiated();
        biomeIndex++;
        biomeIndex%=(int)Biome.Dark;
    }
    void Update()
    {
        if(isGenerating){
            //if(isUnLoading)return;
            for(int cost=0;cost<batchCount*isInstantiatedCost;){
                //delay=generateInterval * count / batchCount;
                GenerateChunkMode mode=GenerateChunk();
                if(isInstantiated){
                    cost+=isInstantiatedCost;
                    isInstantiated=false;
                } else if(mode==GenerateChunkMode.Pass){
                    storeCount++;
                    if(storeCount==storeMax){
                        StoreCell();
                        storeCount=0;
                    }
                    if(isInstantiated){
                        cost+=isInstantiatedCost;
                        isInstantiated=false;
                    }else{
                        cost++;
                    }
                } else{
                    cost+=poolCost;
                }
                if(mode==GenerateChunkMode.Complete || mode==GenerateChunkMode.Pass){  
                    chunkXIndex=0;
                    chunkZIndex=0;
                    chunkYIndex=0;
                    xStartChunkIndex++;
                    if(xStartChunkIndex>xEndChunkIndex){
                        zStartChunkIndex++;
                        xStartChunkIndex=GetInRangeStartXChunkIndex(playerPos,zStartChunkIndex,chunkLoadRange);
                        xEndChunkIndex=GetInRangeEndXChunkIndex(playerPos,zStartChunkIndex,chunkLoadRange);
                        if(zStartChunkIndex>=zEndChunkIndex){
                            isGenerating=false;
                            GenerateAdditional();
                            //generateAdditionalDelta=generateAdditionalInterval;
                            //SendCustomEventDelayedSeconds(nameof(GenerateAdditional),generateAdditionalInterval);
                            break;
                        }
                    }
                }
                if(mode==GenerateChunkMode.Complete){
                    break;
                }
            }
        } 
        /*if(generateAdditionalDelta>=0){
            generateAdditionalDelta-=Time.deltaTime;
            if(generateAdditionalDelta<0){
                GenerateAdditional();
            }
        }*/
        if(isRemoving&&removeOn){
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
        }
        /*if(searchAdditionalDelta>=0){
            searchAdditionalDelta-=Time.deltaTime;
            if(searchAdditionalDelta<0&&searchQueue.Count==0){
                SearchAdditional();
            }
        }*/

        if(isSearching){
            //if(isUnLoading)return;
            for(int index=0;index<searchBatchCount;index++){
                var x=xSearchStartChunkIndex;
                var z=zSearchStartChunkIndex;
                var chunkIndex=GetChunkIndex(x,z);
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
                        //searchAdditionalDelta=searchAdditionalInterval;
                        break;
                    }
                }
            }
        } else{
            if(searchQueue.Count==0&&detailQueue.Count==0){
                SearchAdditional();
            }
        }
        for(int i=0;i<operationBatchCount;i++){
            Vector4 operation;
            if(detailQueue.Count>0){
                operation=detailQueue.Dequeue();
            } else {
                if(searchQueue.Count==0)break;
                operation=searchQueue.Dequeue();
            }
            int mode = (int)operation.w;
            if(mode==(int)ChunkOperation.Detail){
                Debug.Log("operation detail: "+operation);
                DetailChunk((int)operation.x,(int)operation.z);
            } else if(mode==(int)ChunkOperation.UnDetail){
                Debug.Log("operation undetail: "+operation);
                UnDetailChunk((int)operation.x,(int)operation.z);
            } else if(mode==(int)ChunkOperation.UnLoad){
                Debug.Log("operation unload: "+operation);
                UnLoadChunk((int)operation.x,(int)operation.z,true);
            } else {
                Debug.LogError("Invalid operation mode: "+mode);
            }
        }
    }
    float searchMargin=>chunkDetailRange*3;

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
        int chunkIndex=GetChunkIndex(removeXIndex,removeZIndex);
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

    int GetChunkIndex(int x,int z){
        return (z-ZSTARTINDEX)*chunkWidth + (x-XSTARTINDEX);
    }

    Vector3 FromChunkIndex(int chunkIndex){
        return new Vector3(chunkIndex%chunkWidth +XSTARTINDEX,0,chunkIndex/chunkWidth+ZSTARTINDEX);
    }
    public const float AnimHeight=10f;

    [SerializeField]ChunkOperationQueue searchQueue;
    [SerializeField]ChunkOperationQueue detailQueue;
    
    [SerializeField]IntKeyBoolDictionary detailChunkIndexDict;

    void DetailChunk(int xIndex,int zIndex){
        int chunkIndex=GetChunkIndex(xIndex,zIndex);
        if(!detailChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
            if(outlineDict.HasItem(chunkIndex)){
                Debug.Log("DetailChunk: "+chunkIndex);
                var outlines = outlineDict.GetValue(chunkIndex);
                var cells=chunkToCellsDict.GetValue(chunkIndex);
                meshCombiner.SwitchCombineMesh(false,outlines,cells);
                detailChunkIndexDict.AddOrSetValue(chunkIndex,true);
            }else {
                Debug.LogWarning("DetailChunk: chunkIndexDict not has item");
            }
        }
    }

    void UnDetailChunk(int xIndex,int zIndex){
        int chunkIndex=GetChunkIndex(xIndex,zIndex);
        if(detailChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
            if(outlineDict.HasItem(chunkIndex)){
                Debug.Log("UnDetailChunk: "+chunkIndex);
                var outlines = outlineDict.GetValue(chunkIndex);
                var cells=chunkToCellsDict.GetValue(chunkIndex);
                meshCombiner.SwitchCombineMesh(true,outlines,cells);
                detailChunkIndexDict.AddOrSetValue(chunkIndex,false);
            } else {
                Debug.LogWarning("UnDetailChunk: chunkIndexDict not has item");
            }
        }
    }

    public void UnLoadChunk(int xIndex,int zIndex,bool setOnly=false){
        int chunkIndex=GetChunkIndex(xIndex,zIndex);
        if(!loadedChunkIndexDict.GetValueOrDefault(chunkIndex,false)){
            Debug.Log("UnLoadChunk: not yet loaded");
            //遅延実行の関係上このケースは起こりえる
            return;
        }
        if(outlineDict.HasItem(chunkIndex)){
            Debug.Log("UnLoadChunk:"+chunkIndex);
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
            foreach(var cell in removeCells){
                if(cell!=null){
                    UnloadCell(cell);
                } else {
                    Debug.LogWarning("Cell pool is empty!");
                }
            }

        }
    }

    public void UnloadCell(GameObject cell){
        if(grassPool.IsMine(cell)){
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
        } 
  
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
    GenerateChunkMode GenerateChunk(){
        int chunkIndex=GetChunkIndex(xStartChunkIndex,zStartChunkIndex);
        if(loadedChunkIndexDict.GetValueOrDefault(chunkIndex,false))return GenerateChunkMode.Pass;
        if(removedChunkIndexDict.HasItem(chunkIndex)){
            if(chunkObj!=null){
                chunkPool.Return(chunkObj,true);
                foreach(var cell in generatingCells){
                    if(cell==null)break;
                    UnloadCell(cell);
                }
                chunkObj=null;
            }
            return GenerateChunkMode.Pass;
        }
        bool isInit=false;
        if(chunkXIndex==0&&chunkZIndex==0&&chunkYIndex==0){
            Debug.Log("GenerateChunk: "+chunkIndex);
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
        var resCell=GenerateCell(xStartChunkIndex*chunkSize+chunkXIndex,zStartChunkIndex*chunkSize+chunkZIndex,chunkYIndex-chunkSize+1,chunkObj.transform);
        meshFilters[(chunkXIndex*chunkSize+chunkZIndex)*chunkSize+chunkYIndex]=resCell.GetComponent<MeshFilter>();
        generatingCells[(chunkXIndex*chunkSize+chunkZIndex)*chunkSize+chunkYIndex]=resCell;
        chunkYIndex++;
        if(chunkYIndex==chunkSize){
            chunkYIndex=0;
            chunkZIndex++;
            if(chunkZIndex==chunkSize){
                chunkZIndex=0;
                chunkXIndex++;
                if(chunkXIndex==chunkSize){
                    chunkToCellsDict.AddOrSetValue(chunkIndex,generatingCells);
                    var CurrentChunkObj=chunkObj;
                    var allParts = meshCombiner.CombineMesh(CurrentChunkObj,meshFilters);
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



    public GameObject GenerateCell(int xIndex,int zIndex,int yIndex,Transform parent){
        xValue = xIndex * scale + _seedX;
        zValue = zIndex  * scale+ _seedZ;
        perlinValue = fbm(xValue,zValue,4,0.4f);//Mathf.PerlinNoise(xValue, zValue);
        height = fieldHeight * (perlinValue-waterPercentage);
        height = Mathf.Round(height);
        spawnPos.x=xIndex*TileScale;
        spawnPos.y=(height+yIndex)*TileScale;
        spawnPos.z=zIndex*TileScale;
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
