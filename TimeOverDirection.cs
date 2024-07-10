
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TimeOverDirection : GameOverDirection {

    [SerializeField, HeaderAttribute("FT_FadeObject (deny none)")] private GameObject FadeObject;
    [SerializeField, HeaderAttribute("テレポート先座標 Z+が前 (deny none)")] private GameObject teleportPoint;
    [SerializeField, HeaderAttribute("テレポート中無効化するオブジェクト (deny none)")] private GameObject[] disableObjects;
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
    private bool playSEOnTeleport;
    public void Start() {
        startTime = Time.time;
    }

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
            if (teleportSE != null && !playSEOnTeleport) audioSource.PlayOneShot(teleportSE);
            isTeleporting = true;
            startTime = Time.time;
        }
    }

    [SerializeField]IMessenger gameoverSyncMessenger;
    [SerializeField]ScoreResultAnimation scoreResultAnimation;
    [SerializeField]ScoreManager scoreManager;


    public void TriggerGameOver(){
        gameoverSyncMessenger.Publish(this.gameObject,GameMessage.GameOver);
    }

    private void Update() {

        if (isTeleporting) {
            float t = Time.time - startTime;

            if (teleportTime > 0 && t > teleportTime) {
                if (teleportSE != null && playSEOnTeleport) audioSource.PlayOneShot(teleportSE);
                //FadeObject.transform.position = teleportPoint.transform.position;
                this.transform.position = teleportPoint.transform.position;
               
                Networking.LocalPlayer.TeleportTo(teleportPoint.transform.position, teleportPoint.transform.rotation);
                teleportTime = -1f;
            }

            if (t <= fadein_end) {
                fadeMaterial.SetFloat("_fade", t / fadein_end);
                return;
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
                if (!canMove) Networking.LocalPlayer.Immobilize(false);
                Networking.LocalPlayer.SetVelocity(Vector3.zero);
                scoreResultAnimation.ReadyEnd=true;
                SendCustomEventDelayedSeconds(nameof(TriggerGameOver),1);
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