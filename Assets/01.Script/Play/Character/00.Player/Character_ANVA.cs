using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Character_ANVA : CharacterBase
{


    protected override void Start()
    {
        base.Start();
        //MaxHp = 10;
        //Hp = 10;
    }





    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            StartCoroutine(ActivateWeapon());

        }

    }


    private IEnumerator ActivateWeapon()
    {
        weaponEffect.SetActive(false);
        weapon.Init(AttackPower, CriticalChance, CriticalDamage, Level);    
        weaponObject.GetComponent<CircleCollider2D>().enabled = true;
        weaponEffect.SetActive(true);

        //누적회전
        //weaponEffect.transform.Rotate(new Vector3(0, 0, Random.Range(0,360)));

        //절대 회전
        weaponEffect.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        weaponEffect.transform.DORotate(new Vector3(0, 0, 720), 0.5f, RotateMode.FastBeyond360);
        Debug.Log(weaponEffect.transform.rotation.eulerAngles);

        yield return new WaitForSeconds(0.1f);
        weaponObject.GetComponent<CircleCollider2D>().enabled = false;

    }
}
