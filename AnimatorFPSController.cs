
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AnimatorFPSController : UdonSharpBehaviour
{
    [SerializeField, Range(1, 30)]
    int _fps = 8;

    Animator _animator;



    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    public void UpdateInterval(){
        _animator.Update(1f/_fps);
        SendCustomEventDelayedSeconds(nameof(UpdateInterval),1f/_fps);
    }

    /// <summary>
    /// しきい値時間の初期化
    /// </summary>
 
    /*private void _Update()
    {
        _skippedTime += Time.deltaTime;

        if (_thresholdTime > _skippedTime)
        {
            return;
        }

        // アニメーションの時間を計算する
        _animator.Update(_skippedTime);
        _skippedTime = 0f;
    }*/

    /// <summary>
    /// Inspectorの値変更時の処理
    /// </summary>

}
