using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DayNightCycle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer dark;
    [SerializeField] private Transform cloud1Parent;
    [SerializeField] private Transform cloud2Parent;
    [SerializeField] private GameObject SkyRoller;

    [Header("Day Colors")]
    private Color dayColor = new Color(0.776f, 0.988f, 1f, 1f); // C6FCFF
    private Color dayDarkAlpha = new Color(0f, 0f, 0f, 0f); // 불투명도 0
    private Color dayCloudColor = Color.white; // FFFFFF

    [Header("Night Colors")]
    private Color nightColor = new Color(0.180f, 0.180f, 0.247f, 1f); // 2E2E3F
    private Color nightDarkAlpha = new Color(0f, 0f, 0f, 150f / 255f); // 불투명도 150
    private Color nightCloudColor = new Color(0.408f, 0.408f, 0.408f, 1f); // 686868

    [Header("Timing")]
    [SerializeField] private float initialWait = 10f;
    [SerializeField] private float toNightDuration = 20f;
    [SerializeField] private float nightWait = 5f;
    [SerializeField] private float toDayDuration = 15f;
    [SerializeField] private float skyRotationDuration = 50f;

    private Sequence dayNightSequence;
    private SpriteRenderer[] cloud1Sprites;
    private SpriteRenderer[] cloud2Sprites;

    void Start()
    {
        // Cloud 오브젝트 내부의 모든 SpriteRenderer 수집
        cloud1Sprites = cloud1Parent.GetComponentsInChildren<SpriteRenderer>();
        cloud2Sprites = cloud2Parent.GetComponentsInChildren<SpriteRenderer>();

        background.color = dayColor;
        dark.color = dayDarkAlpha;
        SetCloudColors(dayCloudColor);

        // 낮밤 사이클 시작
        StartDayNightCycle();

        // 하늘 회전 시작
        StartSkyRotation();
    }

    private void StartDayNightCycle()
    {
        // 기존 Sequence 있으면 제거
        if (dayNightSequence != null && dayNightSequence.IsActive())
        {
            dayNightSequence.Kill();
        }

        dayNightSequence = DOTween.Sequence();

        dayNightSequence
            //1. 10초 대기
            .AppendInterval(initialWait)

            //2. 20초동안 밤으로 전환 (구름도 같이)
            .AppendCallback(() => TweenCloudColors(nightCloudColor, toNightDuration))
            .Append(background.DOColor(nightColor, toNightDuration).SetEase(Ease.InOutSine))
            .Join(dark.DOColor(nightDarkAlpha, toNightDuration).SetEase(Ease.InOutSine))

            // 3. 5초 대기
            .AppendInterval(nightWait)

            // 4. 15초 동안 낮으로 전환 (구름도 같이)
            .AppendCallback(() => TweenCloudColors(dayCloudColor, toDayDuration))
            .Append(background.DOColor(dayColor, toDayDuration).SetEase(Ease.InOutSine))
            .Join(dark.DOColor(dayDarkAlpha, toDayDuration).SetEase(Ease.InOutSine))

            // 5. 사이클 반복
            .SetLoops(-1, LoopType.Restart);
    }

    private void StartSkyRotation()
    {
        SkyRoller.transform
            .DORotate(new Vector3(0, 0, -360), skyRotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void SetCloudColors(Color color)
    {
        foreach (var sprite in cloud1Sprites)
        {
            sprite.color = color;
        }

        foreach (var sprite in cloud2Sprites)
        {
            sprite.color = color;
        }
    }

    private void TweenCloudColors(Color targetColor, float duration)
    {
        foreach (var sprite in cloud1Sprites)
        {
            sprite.DOColor(targetColor, duration).SetEase(Ease.InOutSine);
        }

        foreach (var sprite in cloud2Sprites)
        {
            sprite.DOColor(targetColor, duration).SetEase(Ease.InOutSine);
        }
    }

    private void OnDestroy()
    {
        // 씬 전환 시 Tween 정리
        if (dayNightSequence != null && dayNightSequence.IsActive())
        {
            dayNightSequence.Kill();
        }

        // Cloud Tween도 정리
        DOTween.Kill(cloud1Sprites);
        DOTween.Kill(cloud2Sprites);

        // SkyRoller Tween 정리
        DOTween.Kill(SkyRoller.transform);
    }
}