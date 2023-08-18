
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MeshCombiner : UdonSharpBehaviour
{
    [SerializeField]TilePoolItemOperator itemOperator;
    [SerializeField]ChunkPoolItemOperator chunkOperator;
    [SerializeField]MapGeneratorSetting mapGeneratorSettings;


    public  CombineInstance[] GetCombine(int count){
        if(count>combineCache.Length){
            Debug.LogError("CombineInstance cache is not enough "+count + " > " + combineCache.Length );
            return null;
        }
        var target=combineCache[count-1];
        if(target==null){
            target=new CombineInstance[count];
            combineCache[count-1]=target;
        }
        return target;

    }

    /*public  GameObject[] GetAllParts(int count){
        var target=allPartsCache[count];
        if(target==null){
            target=new GameObject[count];
            allPartsCache[count]=target;
        }
        return target;

    }*/

    CombineInstance[][] combineCache ;
    //GameObject[][] allPartsCache;

    void Start()
    {
        var size=mapGeneratorSettings.chunkSize;
        combineCache = new CombineInstance[size*size*size][];
        //allPartsCache = new GameObject[10][];
    }

    public void SwitchCombineMesh(bool active, GameObject[] outlines,GameObject[] cells,bool[] brokenArr){
        chunkOperator.SetActive(outlines[0],active);
        /*for(int i=1;i<outlines.Length;i++){
            itemOperator.SetActive(outlines[i],!active);
        }*/
        if(active){
            foreach(var o in cells){
                itemOperator.SetActive(o,false);
            }
        } else {
            for(int i=0;i<cells.Length;i++){
                var o=cells[i];
                itemOperator.SetActive(o,!brokenArr[i]);
            }
        }
        
    }

    public GameObject[] CombineMesh(GameObject fieldParent,MeshFilter[] meshFilters,bool isInDetailRange,bool[] brokenArr)
    {
        // 親オブジェクトにMeshFilterがあるかどうか確認します。
        MeshFilter parentMeshFilter = fieldParent.GetComponent<MeshFilter>();//CheckParentComponent<MeshFilter>(fieldParent.gameObject);
        if(parentMeshFilter==null ){
            Debug.LogError("Parent object has no MeshFilter !");
        }
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
        CombineInstance[] combine = GetCombine(combineCount);//new CombineInstance[combineCount];//
        //GameObject[] allParts = new GameObject[meshFilters.Length-combineCount+1];
        GameObject[] allParts = new GameObject[1];
        allParts[0]=fieldParent;
        int restIndex=1;
        // 結合するメッシュの情報をCombineInstanceに追加していきます。
        int index=0;
        for(int i=0;i<meshFilters.Length;i++)
        {
            var mesh=meshFilters[i];
            //TODO: destory block dictionaryを参照する
            combine[index].mesh = mesh.sharedMesh;
            combine[index].transform = mesh.transform.localToWorldMatrix;
            if(isInDetailRange){
                if(brokenArr[i])itemOperator.SetActive(mesh.gameObject,false);// try to spawnでtrueは保証されている
            } else{
                itemOperator.SetActive(mesh.gameObject,false); // try to spawnでtrueは保証されている
            }
            index++;
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

        // 結合したメッシュをセットします。
        parentMeshFilter.mesh = new Mesh();
        parentMeshFilter.mesh.CombineMeshes(combine);

        // 結合したメッシュにマテリアルをセットします。
        //parentMeshRenderer.material = combinedMat;

        MeshCollider meshCol = fieldParent.GetComponent<MeshCollider>();
        meshCol.sharedMesh = parentMeshFilter.mesh;

        // 親オブジェクトを表示します。
        //fieldParent.gameObject.SetActive(true);
        chunkOperator.SetActive(fieldParent,!isInDetailRange);


        return allParts;
    }

}
