using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VirusType { Destroy, Worm, Trojan, Ransom }

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    #region Stats

    [Header("캐릭터 설정")]
    [SerializeField] protected string characterName;

    protected CharacterData data; // 로드된 스탯 데이터

    [SerializeField] private float showHp; // 필드로 따로 선언
    private float _hp; // Hp의 실제 값 저장용

    [SerializeField] private float _expGive;

    // get set 연습용
    public string CharacterName { get; protected set; }
    public int Level { get; protected set; }
    public float MaxHp { get; protected set; }
    public float Hp
    {
        get { return _hp; }

        protected set
        {
            _hp = value;        // 실제 값 저장
            showHp = value;    // 동시에 ShowHp도 갱신
            hpBar?.UpdateHpBar(_hp, MaxHp); // Hp 바뀔 때마다 자동 갱신
            //?. 는 hpBar가 null이면 실행 안 하는 거야. HpBar 없는 오브젝트도 오류 없이 쓸 수 있어.
        }
    }

    public float ExpGive
    {
        get { return _expGive; }

        protected set
        {
            _expGive = value;        // 실제 값 저장

        }
    }


    public float HpRegen { get; protected set; }
    public float AttackPower { get; protected set; }
    public float MoveSpeed { get; protected set; }
    public float Defense { get; protected set; }
    public float CriticalChance { get; protected set; }
    public float CriticalDamage { get; protected set; }




    #endregion


    #region EnemySetting

    [Header("적 설정")]
    [SerializeField] protected bool isFileTarget = true;
    [SerializeField] protected bool isBoss;
    [SerializeField] protected bool isMeele;
    [SerializeField] protected VirusType virusType;
    private Transform target;

    private float findTargetInterval = 0.5f; // 0.5초마다 타겟 갱신
    private float findTargetTimer = 0f;

    [SerializeField] private HpBar hpBar;

    //public bool IsFileTarget => isFileTarget;

    public bool IsFileTarget
    {
        get { return isFileTarget; }

        set { isFileTarget = value; }
    }

    public VirusType EnemyVirusType => virusType;



    #endregion

    protected virtual void Awake()
    {
        LoadData();

    }


    private void LoadData()
    {
        data = CharacterDataLoader.LoadByName(characterName); //?
        if (data == null) return;

        // CSV 데이터를 프로퍼티에 적용

        CharacterName = data.characterName;
        Level = data.level;
        MaxHp = data.maxHp;
        Hp = data.hp;
        HpRegen = data.hpRegen;
        AttackPower = data.attackPower;
        MoveSpeed = data.moveSpeed;
        Defense = data.defense;
        CriticalChance = data.criticalChance;
        CriticalDamage = data.criticalDamage;
    }


    protected virtual void Start()
    {

        showHp = Hp;

        int go = Random.Range(0, 2);
        if (go == 0)
        {
            IsFileTarget = false;
        }
        else if (go == 1)
        {
            IsFileTarget = true;
        }
    }

    void Update()
    {
        findTargetTimer += Time.deltaTime; //매 프레임 경과 시간을 누적
        if (findTargetTimer >= findTargetInterval)//누적값이 0.5 넘으면 실행
        {
            findTargetTimer = 0f;//다시 0으로 리셋해서 또 0.5초 기다림
            FindTarget(); // 0.5초마다만 실행
        }

        MoveToTarget(); // 이동은 매 프레임


    }

    #region TakeDamage


    public void TakeDamage(float damage)
    {
        Hp -= damage;

        if (Hp <= 0) OnDead();
    }

    protected virtual void OnDead()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Movement

    private void FindTarget()
    {
        string targetTag = isFileTarget ? "File" : "Player";

        // 해당 태그 오브젝트 전부 찾기
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        //배열로 한 이유? 같은 태그 오브젝트가 여러 개일 수 있어서

        if (targets.Length == 0) return; //타겟 없으면 탈출

        float closestDist = float.MaxValue; // "아직 아무것도 비교 안함" 초기값
        foreach (GameObject t in targets)
        // targets 배열을 하나씩 꺼내서 t에 담아 순회
        // for문이랑 같은데 배열 전체를 순서대로 돌 때 더 편함
        {
            float dist = Vector3.Distance(transform.position, t.transform.position);
            // 나(적)와 각 타겟 사이 거리 계산
            if (dist < closestDist)
            {
                closestDist = dist; // 현재까지 가장 짧은 거리 갱신
                target = t.transform; // 현재까지 가장 가까운 타겟 갱신
            }

        }

    }

    private void MoveToTarget()
    {
        if (target == null) return;
        // 타겟 없으면 이동 안함 (타겟 파괴됐을 때 오류 방지)

        transform.position = Vector3.MoveTowards
            (transform.position, target.position, MoveSpeed * Time.deltaTime);
        // 현재위치에서 타겟위치로 MoveSpeed 속도로 이동
        // Time.deltaTime 곱하는 이유: 프레임 상관없이 일정 속도 유지
    }


    #endregion

    #region Attack


    protected abstract void Attack();


    #endregion


}
