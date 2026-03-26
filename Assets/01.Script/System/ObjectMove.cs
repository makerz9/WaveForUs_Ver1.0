using UnityEngine;
using DG.Tweening;

public class ObjectMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float y1 = 0f;
    [SerializeField] private float y2 = 5f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float startDelay = 0f;

    private Sequence moveSequence;

    void Start()
    {

        Invoke("StartVerticalMovement", startDelay);
    }

    private void StartVerticalMovement()
    {
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill();
        }

        // 로컬 좌표로 시작 위치 설정
        transform.localPosition = new Vector3(transform.localPosition.x, y1, transform.localPosition.z);

        moveSequence = DOTween.Sequence();

        moveSequence
            .Append(transform.DOLocalMoveY(y2, duration).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveY(y1, duration).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart);
    }

    private void OnDestroy()
    {
        // 씬 전환 시 Tween 정리
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill();
        }
    }
}


