
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MissionManager : UdonSharpBehaviour
{
    [SerializeField]Mission[] missions;
    [SerializeField]MissionSelector[] missionSelectors;
    int startIndex=0;
    void Start()
    {
        // script order
        SendCustomEventDelayedFrames(nameof(DelayStart),2);
    }
    public void UpdateMissions(){
        for(int i=0;i<missionSelectors.Length;i++){
            Mission mission;
            if(i+startIndex>=missions.Length){
                mission=null;
            }else{
                mission=missions[i+startIndex];
            }
            missionSelectors[i].SetMission(mission);
            //missionSelectors[i].SetMissionIndex(missions,i+startIndex);
        }
    }
    public void DelayStart(){
        for(int i=0;i<missions.Length-1;i++){
            missions[i].SetNextMission(missions[i+1]);
        }
        UpdateMissions();
        SendCustomEventDelayedFrames(nameof(InitialSelect),2);
    }
    public void InitialSelect(){
        missionSelectors[0].Interact();
    }
    [SerializeField]AudioSource audioSource;

    public void UpMissions(){
        if(startIndex>0){
            startIndex--;
            UpdateMissions();
            audioSource.Play();
        }
    }

    public void DownMissions(){
        if(startIndex<missions.Length-1){
            startIndex++;;
            UpdateMissions();
            audioSource.Play();
        }
    }


    public string ALlMissionScore(){
        string result="";
        for(int i=0;i<missions.Length-1;i++){
            var mission=missions[i];
            result+=mission.PlanetName+": "+mission.HighScore+"\n";
        }
        return result;
    }
 
}
