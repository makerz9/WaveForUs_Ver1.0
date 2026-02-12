using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;


public class WaveController : MonoBehaviour
{
    // Start is called before the first frame update

    private float originalLocalY;
    private Coroutine waveRoutine; // 코루틴 참조 저장
    private bool isWaveEffectActive = false;

    [SerializeField] private bool isWaving=false;

    //private SpriteRenderer waveRenderer;


    void Start()
    {
        //InvokeRepeating("WaveMove", 1, 3f);
        waveRoutine = StartCoroutine(WaveMoveRoutine());

        originalLocalY = transform.localPosition.y;

        //waveRenderer = GetComponent<SpriteRenderer>();
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
        float randomY = transform.localPosition.y + Random.Range(-0.15f, 0.15f);

        if(isWaveEffectActive == false)
        {
            if (randomY < -0.5f || randomY > 0.5f)
            {

                StopCoroutine(waveRoutine);
                StartCoroutine(ReturnToOriginal());
            }
            else
            {
                transform.DOLocalMoveY(randomY, 1f).SetEase(Ease.OutQuad);
            }
        }




    }



    IEnumerator ReturnToOriginal()
    {
        transform.DOLocalMoveY(originalLocalY, 1.5f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(2.0f);

        waveRoutine = StartCoroutine(WaveMoveRoutine());
    }

    //void OnDisable()
    //{
    //    CancelInvoke("SpawnObstacle"); // 중지
    //}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("WaveUp") && !isWaveEffectActive)
        {
            WaveMakerMove waveMaker = other.GetComponent<WaveMakerMove>(); // 해당 코드 참조
            if (waveMaker != null)
            {
                StopCoroutine(waveRoutine);
                StartCoroutine(WaveUpEffect(waveMaker.posPower)); //매개변수로 해당 코드의 힘 전달
            }

        }
    }

    IEnumerator WaveUpEffect(float power)
    {
        isWaving = true;

        isWaveEffectActive = true;

        float beforeEffectY = transform.localPosition.y; // 효과 시작 전 위치 저장!
        float targetY = beforeEffectY + Mathf.Abs(power) * 0.6f;

        // DOTween Sequence 생성
        Sequence waveSequence = DOTween.Sequence();

        //값만큼 이동
        waveSequence.Append(transform.DOLocalMoveY(targetY, 0.5f).SetEase(Ease.OutQuad));

        //복귀
        waveSequence.Append(transform.DOLocalMoveY(beforeEffectY, 0.5f).SetEase(Ease.InQuad));



        //Sequence 완료 대기
        yield return waveSequence.WaitForCompletion();

        //기존 코루틴 재시작
        isWaveEffectActive = false;
        waveRoutine = StartCoroutine(WaveMoveRoutine());
    }


}
