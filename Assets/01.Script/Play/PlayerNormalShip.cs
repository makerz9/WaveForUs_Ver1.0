using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalShip : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sp;
    [SerializeField] private float buoyancyForce = 5f;
    [SerializeField] private float waterGravity = 1f;
    [SerializeField] private float airGravity = 3;
    [SerializeField] private float waterDrag = 2f;
    [SerializeField] private float airDrag = 5f; // 공중에서 더 큰 저항

    [SerializeField] private GameObject[] DeadEffect;

    [SerializeField] private GameObject[] WaterEffects;

    [SerializeField] private float moveSpeed=0.4f;
    [SerializeField] private GameObject shipBody;

    private GameManager gameManager;

    private Tween rotateTween; // 변수로 저장

    private Coroutine waterEffectCoroutine; // 변수 추가

    private int dropCount = 1;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //sp = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance;

    }

    private void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    #region Collision

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            rb.gravityScale = waterGravity;
            rb.drag = waterDrag; // 물속 저항
            rb.AddForce(Vector2.up * buoyancyForce * 4, ForceMode2D.Force);

            rotateTween = transform.DORotate(Vector3.zero, 1.0f).SetEase(Ease.OutQuad);

            // 진행 중인 코루틴 중지! (중요!)
            if (waterEffectCoroutine != null)
            {
                StopCoroutine(waterEffectCoroutine);
                waterEffectCoroutine = null;
            }

            if (dropCount >= 1)
            {
                gameManager.SoundCall("waterMiniDrop");
                dropCount--;
            }



            WaterEffects[0].SetActive(true);
            WaterEffects[1].SetActive(true);
            WaterEffects[0].GetComponent<ParticleSystem>().Play();
            WaterEffects[1].GetComponent<ParticleSystem>().Play();


        }


        if (other.CompareTag("Dead"))
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<Rigidbody2D>().simulated = false;
            sp.enabled = false; // ← 이렇게!
            shipBody.SetActive(false);

            foreach (GameObject effects in DeadEffect)
            {
                effects.SetActive(true);
            }

            int deadSoundIndex = Random.Range(0, 4);

            if (deadSoundIndex == 0)
            {
                GameManager.Instance.SoundCall("boom1");
            }
            else if (deadSoundIndex == 1)
            {
                GameManager.Instance.SoundCall("boom2");
            }
            else if (deadSoundIndex == 2)
            {
                GameManager.Instance.SoundCall("boom3");
            }
            else if (deadSoundIndex == 3)
            {
                GameManager.Instance.SoundCall("boom4");
            }


            gameManager.GameOver();
        }

        if(other.CompareTag("ScoreUp"))
        {
            gameManager.ScoreUp();
            //other.GetComponent<BoxCollider2D>().enabled = false;
        }



    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            rb.AddForce(Vector2.up * buoyancyForce, ForceMode2D.Force);

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameManager.SoundCall("stone");
            collision.gameObject.tag = "Wall";
        }
        else
        {
            return;
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