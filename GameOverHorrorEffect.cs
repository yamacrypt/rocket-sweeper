
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameOverHorrorEffect : GameOverDirection {

    [SerializeField, HeaderAttribute("FT_FadeObject (deny none)")] private GameObject FadeObject;
    [SerializeField, HeaderAttribute("テレポート先座標 Z+が前 (deny none)")] private GameObject teleportPoint;
    [SerializeField, HeaderAttribute("テレポート中無効化するオブジェクト (deny none)")] private GameObject[] disableObjects;
    [SerializeField]Renderer[] enemyRenderers;
    private bool isTeleporting = false;
    private bool canMove;
    private float startTime;
    private float fadein_end;
    private float teleportTime;
    private float dark_end;
    private float fadeout_end;
    private float interval_end;

    private Material fadeMaterial;
    private AudioClip teleportSE;
    private AudioSource audioSource;
    [SerializeField]AudioSource beatSE;
    [SerializeField]Animator animator;
    private bool playSEOnTeleport;
    public void Start() {
        startTime = Time.time;
        if(enemyRenderers!=null){
            foreach(Renderer renderer in enemyRenderers){
                    renderer.enabled=false;
                }
            animator.SetBool("AttackStart",false);
        }
    }
    [SerializeField]float lookAtRot;
    [SerializeField]float upOffset=0.5f;
    [SerializeField]float frontOffset=2f;
    [SerializeField]TempGameOverIsReady tempGameOverIsReady;
    public override void _Interact() {
        if (!isTeleporting) {
            UdonBehaviour udon = (UdonBehaviour)FadeObject.GetComponent(typeof(UdonBehaviour));
            fadein_end = (float)udon.GetProgramVariable("fadeinTime");
            dark_end = (float)udon.GetProgramVariable("darkTime") + fadein_end;
            teleportTime = (float)udon.GetProgramVariable("darkTime") / 2 + fadein_end;
            if (teleportTime <= 0) teleportTime = 0.1f;
            fadeout_end = (float)udon.GetProgramVariable("fadeoutTime") + dark_end;
            interval_end = (float)udon.GetProgramVariable("intervalTime") + fadeout_end;
            canMove = (bool)udon.GetProgramVariable("canMove");
            teleportSE = (AudioClip)udon.GetProgramVariable("teleportSE");
            playSEOnTeleport = (bool)udon.GetProgramVariable("playSEOnTeleport");
            audioSource = FadeObject.GetComponent<AudioSource>();

            FadeObject.SetActive(true);
            FadeObject.transform.position = this.transform.position;
            fadeMaterial = FadeObject.GetComponent<Renderer>().sharedMaterial;
            fadeMaterial.SetFloat("_fade", 0f);
            if (disableObjects.Length > 0) {
                foreach (GameObject obj in disableObjects) {
                    if (obj != null) obj.SetActive(false);
                }
            }
            if (!canMove) Networking.LocalPlayer.Immobilize(true);
            Networking.LocalPlayer.SetVelocity(Vector3.zero);
            if (teleportSE != null && !playSEOnTeleport) audioSource.PlayOneShot(teleportSE);
            isTeleporting = true;
            startTime = Time.time;
            if(enemyRenderers!=null){
                foreach(Renderer renderer in enemyRenderers){
                        renderer.enabled=false;
                    }
                animator.SetBool("AttackStart",false);
            }
            isEnemyTeleportDone=false;
            tempGameOverIsReady.IsReady=false;
            
        }
    }

    public void Attack(){
        animator.SetBool("AttackStart",true);
        beatSE.Play();
    }
     //[SerializeField]IMessenger gameoverSyncMessenger;
     //[SerializeField]ScoreResultAnimation scoreResultAnimation;

    /* public void TriggerGameOver(){
        gameoverSyncMessenger.Publish(this.gameObject,GameMessage.GameOver);
    }*/
    bool isEnemyTeleportDone=false;
    [SerializeField]AudioSource cloneSE;
    public void SetCanMove(){
        if (!canMove) Networking.LocalPlayer.Immobilize(false);
        Networking.LocalPlayer.SetVelocity(Vector3.zero);
    }
    private void Update() {

        if (isTeleporting) {
            Networking.LocalPlayer.SetVelocity(Vector3.zero);
            float t = Time.time - startTime;

            if (teleportTime > 0 && t > teleportTime) {
                if (teleportSE != null && playSEOnTeleport) audioSource.PlayOneShot(teleportSE);
                //FadeObject.transform.position = teleportPoint.transform.position;
                this.transform.position = teleportPoint.transform.position;
                if(enemyRenderers!=null){
                    foreach(Renderer renderer in enemyRenderers){
                        renderer.enabled=false;
                    }
                    animator.SetBool("AttackStart",false);
                }
                Networking.LocalPlayer.TeleportTo(teleportPoint.transform.position, teleportPoint.transform.rotation);
                teleportTime = -1f;
                
            }

            if (t <= fadein_end) {
                fadeMaterial.SetFloat("_fade", t / fadein_end);
                return;
            } else if(t<=teleportTime){
                if(enemyRenderers!=null&&!isEnemyTeleportDone){
                    var playerPos=Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position; //Networking.LocalPlayer.GetPosition();
                    var playerRot=Networking.LocalPlayer.GetRotation();
                    transform.position= playerPos + Vector3.up * upOffset +playerRot *Vector3.forward * frontOffset ;
                    transform.LookAt(playerPos+ Vector3.up * upOffset);
                    this.transform.rotation *= Quaternion.Euler(0,lookAtRot,0);
                    foreach(Renderer renderer in enemyRenderers){
                        renderer.enabled=true;
                    }
                    isEnemyTeleportDone=true;
                    SendCustomEventDelayedSeconds(nameof(Attack),0.3f);
                }
            }
            if (t <= dark_end) {
                fadeMaterial.SetFloat("_fade", 1f);
                return;
            }
            if (t <= fadeout_end) {
                fadeMaterial.SetFloat("_fade", 1 - (t - dark_end) / (fadeout_end - dark_end));
                return;
            }
            if (fadeout_end > 0) {
                fadeMaterial.SetFloat("_fade", 0f);
                FadeObject.SetActive(false);
                fadeout_end = -1;
                SetCanMove();
                //scoreResultAnimation.ReadyEnd=true;
                tempGameOverIsReady.IsReady=true;
                cloneSE.Play();
                //SendCustomEventDelayedSeconds(nameof(TriggerGameOver),1);
                return;
            }
            if (t > interval_end) {
                if (disableObjects.Length > 0) {
                    foreach (GameObject obj in disableObjects) {
                        if (obj != null) obj.SetActive(true);
                    }
                }
                isTeleporting = false;
                return;
            }
        }
    }
}