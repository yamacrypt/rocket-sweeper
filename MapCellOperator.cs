
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MapCellOperator : UdonSharpBehaviour
{ 
    public bool isFree=>target==null;
    GameObject target=null;
    public void SetTarget(GameObject obj){
        if(target!=null){
            Debug.LogWarning("Target is already set!");
            return;
        }
        target=obj;
        autoFree=false;
        this.mesh=obj.GetComponent<MeshRenderer>();
        this.col=obj.GetComponent<BoxCollider>();
    }
    //[SerializeField]Material mat;
    Material mat=>mesh.material;
    BoxCollider col;
    MeshRenderer mesh;
    //[SerializeField]TileManager tileManager;
    bool isUp=true;
    float animHeight=>MapGenerator.AnimHeight;
    bool isAnimating=false;
    int animFrame=0;
    float accelerationAbs;


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
        this.animFrame=(int)(animTime / Time.fixedDeltaTime);
        if(dir==AnimDir.Up){
            isUp=true;
            col.enabled=true;
            mesh.enabled=true;
            SetMatHeight(-animHeight);
            // v/h =2 となるよう決定
            velocityAbs = 2 * animHeight / animTime;
            accelerationAbs = 2 *animHeight / (animTime*animTime);
            SendCustomEventDelayedSeconds(nameof(AnimationUpInterval),delay);
        } else if(dir==AnimDir.Down) {
            isUp=false;
            col.enabled=false;
            velocityAbs = 0;
            accelerationAbs = 2 *animHeight / (animTime*animTime);
            SendCustomEventDelayedSeconds(nameof(AnimationDownInterval),delay);
        } else {
            Debug.LogError("Wrong animation direction!");
        }
    }

    const int animationRefreshFrame=2;
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
        deltaTime = animationRefreshFrame *Time.fixedDeltaTime;
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
        SendCustomEventDelayedFrames(nameof(AnimationDownInterval),animationRefreshFrame);
    }

    void AnimationDown(){
        //animCellPosDelta = animHeight * animationRefreshFrame / animFrame;
        deltaTime = animationRefreshFrame *Time.fixedDeltaTime;
        animCellPosDelta = velocityAbs * deltaTime; //animHeight * animationRefreshFrame / animFrame;
        velocityAbs += accelerationAbs * deltaTime;
        height=GetMatHeight()-animCellPosDelta;
        if(height<=-animHeight){
            height=-animHeight;
            isAnimating=false; 
            mesh.enabled=false;
        }
        SetMatHeight(height);
    }

    public void Free(){
        if(target==null){
            Debug.LogWarning("Target is null!");
            return;
        }
        target=null;
        autoFree=false;
    }

}
