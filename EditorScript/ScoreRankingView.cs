
using UnityEngine;
public class ScoreRankingView : MonoBehaviour
{
    [SerializeField]int[] scoreThresholds;
    [SerializeField]string[] scoreRankThresholds;
    [SerializeField]ScoreRankingSlotView[] scoreRankingSlotViews;
    [SerializeField]int startWidth;
    [SerializeField]int endWidth;
    [SerializeField]float saturation=1f;
    [SerializeField]float brightness=1f;

    public void Align()
    {
        for(int i=0;i<scoreThresholds.Length;i++){
            float size=(float)(endWidth-startWidth)/(scoreThresholds.Length-1)*i+startWidth;
            float hueValue = (float)i/scoreThresholds.Length;
            Color rainbowColor = Color.HSVToRGB(hueValue, saturation, brightness);
            scoreRankingSlotViews[i].SetThreshold(scoreThresholds[i>=1?i-1:0],scoreRankThresholds[i],rainbowColor,endWidth,size);
        }
    }
    

}
