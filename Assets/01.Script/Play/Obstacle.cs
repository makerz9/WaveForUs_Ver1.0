using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Obstacle : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private int MoveSpeed;

    void Start()
    {
        //약간 Y랜덤
        float yPos = Random.Range(-2, 4);

        transform.position += new Vector3(0, 0 + yPos, 0);

        Invoke(nameof(DeactivateObject), 30f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime);
    }

    void DeactivateObject()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boom"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            // Y축 Freeze 해제
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            // 아래로 떨어지는 힘 추가
            rb.velocity = new Vector2(0, -5f); // 속도 조절 가능

            // 또는 중력 켜기
            rb.gravityScale = 1f;

            transform.DOLocalRotate(new Vector3(0,0,-90), 8f).SetEase(Ease.OutQuad);
        }
    }

}
