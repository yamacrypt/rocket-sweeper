
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GameStart : UdonSharpBehaviour
{
    [SerializeField]MapGenerator mapGenerator;
    [SerializeField]UnrolledMapGenerator unrolledMapGenerator;
    [SerializeField]EnemyGenerator enemyGenerator;
    [SerializeField]bool unroll=true;
    void Start()
    {
        mapGenerator.gameObject.SetActive(!unroll);
        unrolledMapGenerator.gameObject.SetActive(unroll);

    }
    public override void Interact(){

        if(!unroll)mapGenerator.GenerateInit();
        else unrolledMapGenerator.GenerateInit();
        enemyGenerator.SpawnInterval();
    }
}
