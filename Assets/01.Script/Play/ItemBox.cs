using DG.Tweening.Core.Easing;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private string boxItemName="";

    private Rigidbody2D rb;
    private GameManager gameManager;

    [SerializeField] private GameObject[] WaterEffects;


    [SerializeField] private float buoyancyForce = 5f;
    [SerializeField] private float waterGravity = 1f;
    [SerializeField] private float airGravity = 3;
    [SerializeField] private float waterDrag = 2f;
    [SerializeField] private float airDrag = 5f; // 공중에서 더 큰 저항

    private Tween rotateTween; // 변수로 저장

    private Coroutine waterEffectCoroutine; // 변수 추가
    [SerializeField] private float moveSpeed = 0.8f;

    private int dropCount = 1;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D가 없습니다!");
        }
    }

    public void Start()
    {
        GameManager.Instance.SoundCall("falling");
    }


    private void Update()
    {

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    #region Collision

    void OnTriggerEnter2D(Collider2D other)
    {
        // rb가 null이면 즉시 가져오기
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (other.CompareTag("Water"))
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();

            rb.gravityScale = waterGravity;
            rb.drag = waterDrag;
            rb.AddForce(Vector2.up * buoyancyForce * 4, ForceMode2D.Force);

            rotateTween = transform.DORotate(Vector3.zero, 1.0f).SetEase(Ease.OutQuad);

            if (waterEffectCoroutine != null)
            {
                StopCoroutine(waterEffectCoroutine);
                waterEffectCoroutine = null;
            }

            if (dropCount >= 1 && gameManager != null)
            {
                gameManager.SoundCall("waterMiniDrop");
                dropCount--;
            }

            // null 체크 추가
            if (WaterEffects != null && WaterEffects.Length >= 2)
            {
                if (WaterEffects[0] != null && WaterEffects[1] != null)
                {
                    WaterEffects[0].SetActive(true);
                    WaterEffects[1].SetActive(true);

                    ParticleSystem ps0 = WaterEffects[0].GetComponent<ParticleSystem>();
                    ParticleSystem ps1 = WaterEffects[1].GetComponent<ParticleSystem>();

                    if (ps0 != null) ps0.Play();
                    if (ps1 != null) ps1.Play();
                }
            }
        }






    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // 누가 나랑 부딪혔는지 이름과 태그를 콘솔에 찍어줍니다.
        Debug.Log("부딪힌 물체 이름: " + other.gameObject.name);
        Debug.Log("부딪힌 물체 태그: " + other.gameObject.tag);

        if (other.gameObject.CompareTag("Ship"))
        {
            Debug.Log("플레이어 확인됨! 로직 실행!");

            GameManager.Instance.SoundCall("itemGet");
            GameManager.Instance.ItemBoxGet(boxItemName);

            gameObject.SetActive(false);
        }
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (rb == null) return;

        if (other.CompareTag("Water") || other.CompareTag("SemiWater"))
        {
            rb.AddForce(Vector2.up * buoyancyForce, ForceMode2D.Force);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water") || other.CompareTag("SemiWater"))
        {
            rb.gravityScale = airGravity;
            rb.drag = airDrag;

            // 기존 코루틴이 있으면 먼저 중지
            if (waterEffectCoroutine != null)
            {
                StopCoroutine(waterEffectCoroutine);
            }

            // 새로 시작
            waterEffectCoroutine = StartCoroutine(DisableWaterEffectDelay());

            if (rotateTween != null)
            {
                rotateTween.Kill();
            }
            //Debug.Log("물 벗어남");


        }
    }



    IEnumerator DisableWaterEffectDelay()
    {
        yield return new WaitForSeconds(0.3f);

        // 새 파티클 생성 중지, 기존 파티클은 자연스럽게 소멸
        WaterEffects[0].GetComponent<ParticleSystem>().Stop();
        WaterEffects[1].GetComponent<ParticleSystem>().Stop();

        // 파티클이 완전히 사라질 때까지 대기 (파티클 수명만큼)
        yield return new WaitForSeconds(1.0f);

        // 완전히 사라진 후 오브젝트 비활성화
        WaterEffects[0].SetActive(false);
        WaterEffects[1].SetActive(false);
        Debug.Log("이펙트 꺼짐");
        dropCount = 1;
    }

    #endregion
}
