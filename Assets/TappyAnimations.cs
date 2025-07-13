using DG.Tweening;
using System;
using UnityEngine;

public class TappyAnimations : Singleton<TappyAnimations>
{
    [Header("Tappy")]
    [SerializeField] private Transform _tappyCharacter;
    [SerializeField] private Transform _tappyWhole;
    [SerializeField] private Transform _tappyBody;
    [SerializeField] private Transform _tappyHead;
    [SerializeField] private Transform _tappyFaceParent;
    [SerializeField] private Transform _tappyFaceNormal;
    [SerializeField] private Transform _tappyFaceExcited;
    [SerializeField] private Transform _pogo;

    private string _currentAnimation;
    private Tween _currentTween;

    private Vector3 _tappyBodyLocalScale;

    private void Awake()
    {
        _tappyBodyLocalScale = _tappyWhole.localScale;
    }
    public void ChangeToAnimation(string animationName)
    {
        if (animationName == _currentAnimation) return;

        Debug.Log("Animation: " + animationName);
        _currentTween?.Kill();

        switch (animationName)
        {
            case "IDLE":
                Animate_IDLE();
                break;
            case "POG":
                Animate_POG();
                break;
        }

        _currentAnimation = animationName;
    }

    private void Animate_POG()
    {
        _tappyFaceNormal.gameObject.SetActive(false);
        _tappyFaceExcited.gameObject.SetActive(true);

        _currentTween?.Kill();
        _currentTween = DOVirtual.DelayedCall(999f, () => { }) // dummy tween to keep reference alive
            .OnKill(() => {
                _tappyFaceNormal.gameObject.SetActive(true);
                _tappyFaceExcited.gameObject.SetActive(false);
            });
    }

    private void Animate_IDLE()
    {
        Sequence seq = DOTween.Sequence();

        Tween scaleTween = _tappyWhole.DOScaleY(1.05f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        Tween rotateTween = _tappyHead.DOLocalRotate(new Vector3(0, 0, 15f), 0.5f, RotateMode.Fast)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Combine both
        seq.Join(scaleTween).Join(rotateTween);

        // OnKill: reset scale + rotation
        seq.OnKill(() => {
            _tappyWhole.localScale = _tappyBodyLocalScale;
            _tappyHead.localRotation = Quaternion.identity;
        });

        _currentTween = seq;

    }

}
