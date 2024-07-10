
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ScoreRankingSlotView : UdonSharpBehaviour
{
    [SerializeField]Image scoreImage;
    [SerializeField]TextMeshProUGUI scoreText;
    [SerializeField]TextMeshProUGUI rankText;
    [SerializeField]float sizeY;
    //int scoreThreshold;
    void Start()
    {
        //scoreThreshold=int.Parse(scoreText.text);
        SetVisible(false);
    }
    public void SetThreshold(int score,string rank,Color color,float width,float size){
        scoreImage.color=color;
        scoreText.text=score.ToString();
        rankText.text=rank;
        GetComponent<RectTransform>().sizeDelta=new Vector2(width,sizeY);
        scoreImage.GetComponent<RectTransform>().sizeDelta=new Vector2(size,sizeY);
    }

    public void SetVisible(bool visible){
        rankText.gameObject.SetActive(visible);
        scoreImage.gameObject.SetActive(visible);
    }

    public int ScoreThreshold=>int.Parse(scoreText.text);
    public string RankText=>rankText.text;

}
