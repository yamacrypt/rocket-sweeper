
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BomberIsNear : UdonSharpBehaviour
{
  

    public const string strBombEnemy="EnemyBomb";

    void OnTriggerEnter(Collider other)
    {
        var enemy=other.transform.parent;
        if(enemy.name.Contains(strBombEnemy)){
            var bomber=enemy.GetComponent<Bomber>();
            if(bomber!=null){
                bomber.Death();
                bomber.DelayExplosion(2);

            }else{
                Debug.LogWarning("bomber is null");
            }
        }
    }
}
