using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // CharacterBase 대신 둘 다 가진 공통 인터페이스로
    [SerializeField] private bool isPWeapon;

    // 무기 생성할 때 주인이 스탯 주입
    private float attackPower;
    private float criticalChance;
    private float criticalDamage;
    private int level;

    // 플레이어/적 둘 다 이 함수로 초기화
    public void Init(float atk, float critChance, float critDmg, int lv)
    {
        attackPower = atk;
        criticalChance = critChance;
        criticalDamage = critDmg;
        level = lv;
    }

    private float CalculateDamage(float targetDefense)
    {
        bool isCritical = Random.Range(0f, 100f) < criticalChance;
        float baseDamage = isCritical ? attackPower * criticalDamage : attackPower;
        float finalDamage = baseDamage - targetDefense;
        return Mathf.Max(finalDamage, level);
    }

    private void Awake()
    {
        if (isPWeapon)
        {
            CharacterBase owner = GetComponentInParent<CharacterBase>();
            // CharacterBase에서 스탯 가져오기
        }
        else
        {
            EnemyBase owner = GetComponentInParent<EnemyBase>();
            // EnemyBase에서 스탯 가져오기
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPWeapon)
        {
            // 플레이어 무기 -> 적만 타격
            if (collision.CompareTag("Enemy"))
            {
                IDamageable target = collision.GetComponent<IDamageable>();
                float defense = collision.GetComponent<EnemyBase>()?.Defense ?? 0;
                if (target != null)
                    target.TakeDamage(CalculateDamage(defense));
            }
        }
        else
        {
            // 적 무기 -> 플레이어랑 파일 둘 다 타격
            if (collision.CompareTag("Player") || collision.CompareTag("File"))
            {
                IDamageable target = collision.GetComponent<IDamageable>();
                float defense = collision.GetComponent<CharacterBase>()?.Defense ?? 0;
                if (target != null)
                    target.TakeDamage(CalculateDamage(defense));
            }
        }
    }
}
