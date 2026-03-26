using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FileBase : MonoBehaviour, IDamageable
{
    public int fileIndex;
    [SerializeField] private float _MaxHp;
    [SerializeField] private float _hp;

    public float MaxHp { get; protected set; }

    public float Hp
    {
        get { return _hp; }

        protected set
        {
            _hp = value;        // 실제 값 저장
            hpBar?.UpdateHpBar(_hp, MaxHp); // Hp 바뀔 때마다 자동 갱신
        }
    }

    [SerializeField] private HpBar hpBar;
    [SerializeField] private StatFileData statData; // Inspector에서 에셋 연결



    // Start is called before the first frame update
    void Start()
    {
        MaxHp = _MaxHp; // Inspector에서 설정한 값으로 초기화
        Hp = _hp;       // 프로퍼티 통해서 초기화 (hpBar 갱신됨)

        hpBar = GetComponentInChildren<HpBar>(); // 자동 연결

        FileSearch();
        ApplyBuff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Buff

    private void ApplyBuff()
    {
        if (statData == null) return;
        CharacterBase[] players = FindObjectsByType<CharacterBase>(FindObjectsSortMode.None);
        foreach (CharacterBase player in players)
            player.ApplyBuff(statData);
    }



    private void RemoveBuff()
    {
        if (statData == null) return;
        CharacterBase[] players = FindObjectsByType<CharacterBase>(FindObjectsSortMode.None);
        foreach (CharacterBase player in players)
            player.RemoveBuff(statData);
    }

    #endregion

    #region TakeDamage


    public void TakeDamage(float damage)
    {
        Hp -= damage;

        if (Hp <= 0) OnDead();
    }

    protected virtual void OnDead()
    {
        RemoveBuff();
        Destroy(gameObject);
    }

    #endregion

    #region FileSearch

    public void FileSearch()
    {
        if (fileIndex == 0)
        {

        }
        #region Normal_Grade

        //
        else if (fileIndex == 1)
        {

        }

        #endregion

    }

    #endregion


}
