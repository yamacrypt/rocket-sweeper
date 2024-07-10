
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class AudioSourceExt : UdonSharpBehaviour
{
    [SerializeField]AudioSource audioSource;

    public void Play(){
        audioSource.volume=startVolume;
        audioSource.Play();
    }

    public void Stop(){
        audioSource.Stop();
    }

    public void SetLoop(bool loop){
        audioSource.loop=loop;
    }
    bool isFadeOut=false;
    public void FadeOut(float duration){
        FadeOutSeconds=duration;
        FadeDeltaTime=0f;
        isFadeOut=true;
        startVolume=audioSource.volume;
    }


    float FadeOutSeconds,FadeDeltaTime;

    float startVolume=1f;

    void Start()
    {
     startVolume=audioSource.volume;   
    }
    void Update()
    {
        if(isFadeOut){
            FadeDeltaTime += Time.deltaTime;
            if (FadeDeltaTime >= FadeOutSeconds)
            {
                FadeDeltaTime = FadeOutSeconds;
                isFadeOut = false;
            }
            audioSource.volume = startVolume * (1.0f-(FadeDeltaTime / FadeOutSeconds));
        }
    }
}
