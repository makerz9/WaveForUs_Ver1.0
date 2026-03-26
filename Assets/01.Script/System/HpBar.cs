using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image fillImageDelay;

    private Coroutine delayCoroutine;

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        //Debug.Log($"HP: {currentHp} / {maxHp} = {currentHp / maxHp}");
        fillImage.fillAmount = currentHp / maxHp;
        // 0~1 사이 값으로 이미지 채움

        // 기존 코루틴 취소하고 새로 시작
        if (delayCoroutine != null)
            StopCoroutine(delayCoroutine);
        delayCoroutine = StartCoroutine(HpDelay(currentHp / maxHp));
    }

    private IEnumerator HpDelay(float targetFill)
    {
        yield return new WaitForSeconds(0.5f);
        //fillImageDelay 가 0.5초만에 fillImage 값으로
        fillImageDelay.DOFillAmount(targetFill, 0.3f); //DOFillAmount(목표값, 시간)

    }
}
