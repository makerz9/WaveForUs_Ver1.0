using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dest : EnemyBase
{
    [Header("set")]
    [Space(10)]
    [SerializeField] protected GameObject weaponObject;
    [SerializeField] protected GameObject weaponEffect;
    protected Weapon weapon; // 추가

    [SerializeField] private float attackSpeed = 2.0f;

    [SerializeField] private bool isAttacking;
    private int contactCount = 0; // 현재 닿아있는 타겟 수

    protected override void Awake()
    {
        base.Awake();
        weapon = weaponObject.GetComponent<Weapon>(); // 추가
    }
    protected override void Start()
    {
        base.Start();
        isAttacking = false;

        MaxHp = 30;
        Hp = 30;
    }

    protected override void Attack()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("File"))
        {
            contactCount++;
            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(ActivateWeapon());
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("File"))
        {
            contactCount--;
            // isAttacking 건드리지 않음!
        }
    }

    private IEnumerator ActivateWeapon()
    {
        yield return new WaitForSeconds(attackSpeed);

        // 공격 실행
        weaponEffect.SetActive(false);
        weapon.Init(AttackPower, CriticalChance, CriticalDamage, Level);
        weaponObject.GetComponent<CircleCollider2D>().enabled = true;
        weaponEffect.SetActive(true);
        weaponEffect.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        weaponEffect.transform.DORotate(new Vector3(0, 0, 720), 0.5f, RotateMode.FastBeyond360);
        yield return new WaitForSeconds(0.1f);
        weaponObject.GetComponent<CircleCollider2D>().enabled = false;

        // 공격 끝나고 false로 초기화
        isAttacking = false;

        // 아직 충돌 중이면 다시 공격 시작
        if (contactCount > 0)
        {
            isAttacking = true;
            StartCoroutine(ActivateWeapon());
        }
    }


}
