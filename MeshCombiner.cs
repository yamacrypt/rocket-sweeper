
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MeshCombiner : UdonSharpBehaviour
{
    [SerializeField,UnrollAttribute]TilePoolItemOperator itemOperator;
    [SerializeField,UnrollAttribute]ChunkPoolItemOperator chunkOperator;
    [SerializeField,UnrollAttribute]MapGeneratorSetting mapGeneratorSettings;



    /*public  GameObject[] GetAllParts(int count){
        var target=allPartsCache[count];
        if(target==null){
            target=new GameObject[count];
            allPartsCache[count]=target;
        }
        return target;

    }*/

    //CombineInstance[][] combineCache ;
    //GameObject[][] allPartsCache;

    void Start()
    {
        var size=mapGeneratorSettings.chunkSize;
        //combineCache = new CombineInstance[size*size*size][];
        //allPartsCache = new GameObject[10][];
    }

    public void SwitchCombineMesh(bool active, GameObject outline,int outlineId,GameObject[] cells,int[] cellIds,bool[] brokenArr){
        chunkOperator.SetActive(outline,outlineId,active);
        /*for(int i=1;i<outlines.Length;i++){
            itemOperator.SetActive(outlines[i],!active);
        }*/
        if(active){
            for(int i=0;i<cells.Length;i++){
                itemOperator.SetActive(cells[i],cellIds[i],false);
            }
        } else {
            for(int i=0;i<cells.Length;i++){
                itemOperator.SetActive(cells[i],cellIds[i],!brokenArr[i]);
            }
        }
        
    }
    CombineInstance[] combine;
    MeshFilter parentMeshFilter;
    MeshCollider meshCol;
    public GameObject CombineMesh(GameObject fieldParent,MeshFilter[] meshFilters,int[] ids,bool isInDetailRange,bool[] brokenArr,bool hasItem)
    {
        // 親オブジェクトにMeshFilterがあるかどうか確認します。
        parentMeshFilter = fieldParent.GetComponent<MeshFilter>();//CheckParentComponent<MeshFilter>(fieldParent.gameObject);
        /*if(parentMeshFilter==null ){
            Debug.LogError("Parent object has no MeshFilter !");
        }*/
        // 親オブジェクトにMeshRendererがあるかどうか確認します。
        //MeshRenderer parentMeshRenderer = fieldParent.GetComponent<MeshRenderer>();//(fieldParent.gameObject);

        /*if(parentMeshRenderer== null){
            Debug.LogError("Parent object has no  MeshRenderer!");
        }*/
        // 子オブジェクトのMeshFilterへの参照を配列として保持します。
        // ただし、親オブジェクトのメッシュもGetComponentsInChildrenに含まれるので除外します。
        //MeshFilter[] meshFilters = fieldParent.GetComponentsInChildren<MeshFilter>();

       
        /*Material combinedMat = meshFilters[0].GetComponent<MeshRenderer>().material;
        int combineCount=0;
        foreach(var mesh in meshFilters){
            if(mesh.GetComponent<MeshRenderer>().material.name==combinedMat.name){
                combineCount++;
            }
        }*/
        int combineCount=meshFilters.Length;
         // 結合するメッシュの配列を作成します。
        if(combine==null)combine=new CombineInstance[combineCount];
        //CombineInstance[] combine = GetCombine(combineCount);//new CombineInstance[combineCount];//
        //GameObject[] allParts = new GameObject[meshFilters.Length-combineCount+1];
        //GameObject[] allParts = new GameObject[1];
        //allParts[0]=fieldParent;
        // 結合するメッシュの情報をCombineInstanceに追加していきます。
        for(int i=0;i<meshFilters.Length;i++)
        {
            var mesh=meshFilters[i];
            //TODO: destory block dictionaryを参照する
            if(!hasItem){
                combine[i].mesh = mesh.sharedMesh;
                combine[i].transform = mesh.transform.localToWorldMatrix;
            }
            if(isInDetailRange){
                itemOperator.SetActive(mesh.gameObject,ids[i],!brokenArr[i]);// try to spawnでtrueは保証されている
            } else{
                itemOperator.SetActive(mesh.gameObject,ids[i],false); // try to spawnでtrueは保証されている
            }
            /*if(mesh.GetComponent<MeshRenderer>().material.name==combinedMat.name){
                combine[index].mesh = mesh.sharedMesh;
                combine[index].transform = mesh.transform.localToWorldMatrix;
                itemOperator.SetActive(mesh.gameObject,false);
                index++;
            } else {
                //allParts[restIndex]=mesh.gameObject;
                restIndex++;
                itemOperator.SetActive(mesh.gameObject,true);
                Debug.LogWarning("MeshCombiner: MeshRenderer material is not same as parent material. ");
            }*/
        }
        if(!hasItem){
            // 結合したメッシュをセットします。
            parentMeshFilter.mesh = new Mesh();
            parentMeshFilter.mesh.CombineMeshes(combine);

            // 結合したメッシュにマテリアルをセットします。
            //parentMeshRenderer.material = combinedMat;

            meshCol = fieldParent.GetComponent<MeshCollider>();
            meshCol.sharedMesh = parentMeshFilter.mesh;
        }

        // 親オブジェクトを表示します。
        //fieldParent.gameObject.SetActive(true);
        chunkOperator.SetActive(fieldParent,!isInDetailRange);


        return fieldParent;
    }

}
