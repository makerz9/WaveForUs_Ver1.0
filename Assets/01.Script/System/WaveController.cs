using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveController : MonoBehaviour
{
    private float originalLocalY;
    private Coroutine waveRoutine;
    private bool isWaveEffectActive = false;
    [SerializeField] private bool isWaving = false;
    [SerializeField] private float maxOffset = 1.0f; // Inspector에서 조절 가능하게
    [SerializeField] private float moveRange = 0.3f; // 이동 범위

    void Start()
    {
        originalLocalY = transform.localPosition.y;

        // Inspector 값 사용 (자동 계산 제거)
        //Debug.Log($"Original Y: {originalLocalY}, MaxOffset: {maxOffset}");
        waveRoutine = StartCoroutine(WaveMoveRoutine());
    }

    IEnumerator WaveMoveRoutine()
    {
        isWaving = false;
        while (true)
        {
            WaveMove();
            float randomInterval = Random.Range(0.5f, 1.0f);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    private void WaveMove()
    {
        float currentY = transform.localPosition.y;
        float randomY = currentY + Random.Range(-moveRange, moveRange);

        if (isWaveEffectActive == false)
        {
            float distanceFromOriginal = Mathf.Abs(randomY - originalLocalY);

            //Debug.Log($"Current Y: {currentY}, Random Y: {randomY}, Distance: {distanceFromOriginal}, Max: {maxOffset}");

            if (distanceFromOriginal > maxOffset)
            {
                //Debug.Log("범위 벗어남! ReturnToOriginal 실행");
                if (waveRoutine != null)
                {
                    StopCoroutine(waveRoutine);
                    waveRoutine = null;
                }
                StartCoroutine(ReturnToOriginal());
            }
            else
            {
                //Debug.Log($"정상 이동: {randomY}");
                transform.DOLocalMoveY(randomY, 1f).SetEase(Ease.OutQuad);
            }
        }
    }

    IEnumerator ReturnToOriginal()
    {
        //Debug.Log($"원위치 복귀 시작: {originalLocalY}");
        transform.DOLocalMoveY(originalLocalY, 1.5f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(2.0f);
        //Debug.Log("WaveMoveRoutine 재시작");
        waveRoutine = StartCoroutine(WaveMoveRoutine());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WaveUp") && !isWaveEffectActive)
        {
            WaveMakerMove waveMaker = other.GetComponent<WaveMakerMove>();
            if (waveMaker != null)
            {
                //Debug.Log($"WaveUp 충돌! Power: {waveMaker.posPower}");
                if (waveRoutine != null)
                {
                    StopCoroutine(waveRoutine);
                    waveRoutine = null;
                }
                StartCoroutine(WaveUpEffect(waveMaker.posPower));
            }
        }
    }

    IEnumerator WaveUpEffect(float power)
    {
        isWaving = true;
        isWaveEffectActive = true;
        float beforeEffectY = transform.localPosition.y;
        float targetY = beforeEffectY + Mathf.Abs(power) * 0.6f;

        //Debug.Log($"WaveUpEffect: {beforeEffectY} -> {targetY}");

        Sequence waveSequence = DOTween.Sequence();
        waveSequence.Append(transform.DOLocalMoveY(targetY, 0.5f).SetEase(Ease.OutQuad));
        waveSequence.Append(transform.DOLocalMoveY(beforeEffectY, 0.5f).SetEase(Ease.InQuad));

        yield return waveSequence.WaitForCompletion();

        isWaveEffectActive = false;
        //Debug.Log("WaveUpEffect 완료, WaveMoveRoutine 재시작");
        waveRoutine = StartCoroutine(WaveMoveRoutine());
    }
}