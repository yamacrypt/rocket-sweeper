
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public enum AnimDir{
    Up,
    Down
}
public enum TileType{
    Empty,
    Wall,
    Grass
}

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapCHunkOperator : UdonSharpBehaviour
{
    public bool isFree=>target==null;
    GameObject target=null;
    public bool SetTarget(GameObject obj){
        if(target!=null){
            Debug.LogWarning("Target is already set!");
        }
        target=obj;
        autoFree=false;
        this.mesh=obj.GetComponent<MeshRenderer>();
        this.col=obj.GetComponent<MeshCollider>();
        if(mesh==null || col==null){
            return false;
        }
        return true;
    }
    //[SerializeField]Material mat;
    Material mat=>mesh.material;
    MeshCollider col;
    MeshRenderer mesh;
    //[SerializeField]TileManager tileManager;
    bool isUp=true;
    float animHeight=>MapGenerator.AnimHeight;
    bool isAnimating=false;
    int animFrame=0;
    float accelerationAbs;

    /*public void SetTile(TileType type){
        tileManager.SetTile(this,type);
    }
    public void SetTile(Material mat){
        mesh.material=mat;
    }*/
    void SetMatHeight(float height){
        if(mesh==null || col==null){
            Debug.LogError("Target is null!");
        }
        if(Mathf.Abs(height) > animHeight)Debug.LogWarning("Height is too big!");
        mat.SetFloat("_UpDown",height);
    }

    float GetMatHeight(){
        if(mesh==null || col==null){
            Debug.LogError("Target is null!");
        }
        return mat.GetFloat("_UpDown");
    }
    float velocityAbs;
    bool autoFree;
    float delay;
    float animTime;
    public void StartAnimation(AnimDir dir,float animTime,float delay, bool autoFree=true){
        if(mesh==null || col==null){
            Debug.LogError("Target is null!");
        }
        if(isAnimating){
            Debug.LogWarning("Animation is already playing!");
            return;
        }
        this.autoFree=autoFree;
        this.delay=delay;
        isAnimating=true;
        this.animTime=animTime;
        this.animFrame=(int)(animTime / Time.deltaTime);
        if(dir==AnimDir.Up){
            isUp=true;
            col.enabled=true;
            mesh.enabled=true;
            SetMatHeight(-animHeight);
            // v/h =2 となるよう決定
            velocityAbs = 2 * animHeight / animTime;
            accelerationAbs = 2 *animHeight / (animTime*animTime);
            if(updateMode)SendCustomEventDelayedSeconds(nameof(AnimationUpInterval),delay);
        } else if(dir==AnimDir.Down) {
            isUp=false;
            col.enabled=false;
            velocityAbs = 0;
            accelerationAbs = 2 *animHeight / (animTime*animTime);
            if(updateMode)SendCustomEventDelayedSeconds(nameof(AnimationDownInterval),delay);
        } else {
            Debug.LogError("Wrong animation direction!");
        }
    }

    const int animationRefreshFrame=1;
    public void AnimationUpInterval(){
        if(!isAnimating){
            if(autoFree)Free();
            return;
        }
        AnimationUp();
        SendCustomEventDelayedFrames(nameof(AnimationUpInterval),animationRefreshFrame);
    }

    float deltaTime,animCellPosDelta,height;
    void AnimationUp(){
        deltaTime = animationRefreshFrame *Time.deltaTime;
        animCellPosDelta = velocityAbs * deltaTime; //animHeight * animationRefreshFrame / animFrame;
        velocityAbs -= accelerationAbs * deltaTime;
        height=GetMatHeight()+animCellPosDelta;
        if(height>=0){
            height=0;
            isAnimating=false;
        }
        SetMatHeight(height);
    }

    public void AnimationDownInterval(){
        if(!isAnimating){
            if(autoFree)Free();
            return;
        }
        AnimationDown();
        SendCustomEventDelayedSeconds(nameof(AnimationDownInterval),animationRefreshFrame*Time.deltaTime);
    }
    void AnimationDown(){
        //animCellPosDelta = animHeight * animationRefreshFrame / animFrame;
        deltaTime = animationRefreshFrame *Time.deltaTime;
        animCellPosDelta = velocityAbs * deltaTime; //animHeight * animationRefreshFrame / animFrame;
        velocityAbs += accelerationAbs * deltaTime;
        height=GetMatHeight()-animCellPosDelta;
        if(height<=-animHeight){
            height=-animHeight;
            isAnimating=false; 
            SetMatHeight(height);
            //mesh.enabled=false;
            if(generator!=null){
                generator.UnLoadChunk(xIndex,zIndex);
            }
        } else {
            SetMatHeight(height);
        }
    }
    
    bool updateMode=false;

    /*
    void Update()
    {
        if(!isAnimating){
            if(autoFree)Free();
            return;
        }
        if(isUp){
            AnimationUp();
        } else{
            AnimationDown();
        } 
    }*/

    MapGenerator generator;
    int xIndex;
    int zIndex;
    public void SetCallback(MapGenerator generator,int xIndex,int zIndex){
        this.generator=generator;
        this.xIndex=xIndex;
        this.zIndex=zIndex;
    }

    public void Free(){
        if(target==null){
            Debug.LogWarning("Target is null!");
            return;
        }
        target=null;
        generator=null;
        isAnimating=false;
        autoFree=false;
        velocityAbs=0;
        accelerationAbs=0;
    }

}
