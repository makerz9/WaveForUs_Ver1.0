using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IDamageable
{
    #region Stats
    [Header("캐릭터 설정")]
    [SerializeField] protected string characterName;// Inspector에서 "ANVA" 등 입력

    protected CharacterData data; // 로드된 스탯 데이터


    private float _hp; // Hp의 실제 값 저장용

    private float _exp;

    // get set 연습용
    public string CharacterName { get; protected set; }
    public int Level { get; protected set; }

    //데이터 없이 
    public float Exp
    {
        get { return _exp; }

        protected set
        {
            _exp = value;        // 실제 값 저장
            //showHp = value;    // 동시에 ShowHp도 갱신
            //hpBar?.UpdateHpBar(_hp, MaxHp); // Hp 바뀔 때마다 자동 갱신
        }
    }
    public float MaxHp { get; protected set; }
    public float Hp
    {
        get { return _hp; }

        protected set
        {
            _hp = value;        // 실제 값 저장
            showHp = value;    // 동시에 ShowHp도 갱신
            hpBar?.UpdateHpBar(_hp, MaxHp); // Hp 바뀔 때마다 자동 갱신
        }
    }
    public float HpRegen { get; protected set; }
    public float AttackPower { get; protected set; }
    public float MoveSpeed { get; protected set; }
    public float Defense { get; protected set; }
    public float CriticalChance { get; protected set; }
    public float CriticalDamage { get; protected set; }




    #endregion

    #region BasicSetting

    [Header("참조")]
    [SerializeField] private CirclePath circlePath;

    [Header("이동 설정")]
    private float pathProgress = 0f; // 0~1, 선 위 위치 비율
    [SerializeField] private float waypointReachDistance = 0.1f;

    private float moveTimer = 0f;
    [SerializeField] private float moveInterval = 0.05f;

    private float segmentProgress = 0f; // 현재 세그먼트 내 0~1 진행도

    private int currentPointIndex = 0;
    private bool isMoving = false;

    [Space(10)]
    [SerializeField] protected GameObject weaponObject;
    [SerializeField] protected GameObject weaponEffect;
    protected Weapon weapon; // 추가

    [SerializeField] private HpBar hpBar;
    ///[SerializeField] private ExpBar expBar;

    #endregion

    #region Buff

    //버프 획득
    public void ApplyBuff(StatFileData data)
    {
        AttackPower += data.attackPowerBonus;
        MoveSpeed += data.moveSpeedBonus;
        Defense += data.defenseBonus;
        CriticalChance += data.criticalChanceBonus;
        CriticalDamage += data.criticalDamageBonus;
        MaxHp += data.maxHpBonus;
        Hp += data.hpBonus;
        HpRegen += data.hpRegenBonus;

    }

    //버프 소실
    public void RemoveBuff(StatFileData data)
    {
        AttackPower -= data.attackPowerBonus;
        MoveSpeed -= data.moveSpeedBonus;
        Defense -= data.defenseBonus;
        CriticalChance -= data.criticalChanceBonus;
        CriticalDamage -= data.criticalDamageBonus;
        MaxHp -= data.maxHpBonus;
        Hp -= data.hpBonus;
        HpRegen -= data.hpRegenBonus;
    }

    #endregion

    #region ShowStats

    [SerializeField] private float showHp; // 필드로 따로 선언
    [SerializeField] private float showAttackPower; // 필드로 따로 선언

    #endregion


    void Awake()
    {
        LoadData();
        weapon = weaponObject.GetComponent<Weapon>(); // 추가
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
        Invoke("StartMoving", 2f);

        showHp = Hp;

    }

    void Update()
    {
        showAttackPower = AttackPower;

        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    #region Moving

    public void StartMoving()
    {
        currentPointIndex = GetClosestPointIndex();
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    private void MoveAlongPath()
    {
        Vector3[] points = circlePath.PathPoints;
        int nextIndex = (currentPointIndex - 1 + points.Length) % points.Length;

        // 현재 세그먼트 길이
        float segmentLength = Vector3.Distance(points[currentPointIndex], points[nextIndex]);

        // 세그먼트 길이에 상관없이 일정 속도
        segmentProgress += (MoveSpeed / segmentLength) * Time.deltaTime;

        // 현재 세그먼트 안에서 보간한 위치 = 항상 선 위
        transform.position = Vector3.Lerp(
            points[currentPointIndex],
            points[nextIndex],
            segmentProgress
        );

        // 세그먼트 끝에 도달하면 다음 인덱스로
        if (segmentProgress >= 1f)
        {
            segmentProgress = 0f;
            currentPointIndex = nextIndex;
        }
    }

    private Vector3 GetPositionOnPath(float t, Vector3[] points, float totalLength)
    {
        // t(0~1)가 전체 길이의 몇 % 지점인지 실제 좌표로 변환
        float targetDist = t * totalLength;
        float accumulated = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            int next = (i + 1) % points.Length;
            float segLen = Vector3.Distance(points[i], points[next]);

            if (accumulated + segLen >= targetDist)
            {
                // 이 세그먼트 안에 목표 지점이 있음
                float localT = (targetDist - accumulated) / segLen;
                return Vector3.Lerp(points[i], points[next], localT);
            }

            accumulated += segLen;
        }

        return points[0];
    }

    public void OnPathChanged() { } // 비워둠

    public void OnDragStarted() { }
    public void OnDragEnded() { }

    private int GetClosestPointIndex()
    {
        Vector3[] points = circlePath.PathPoints;
        int closestIdx = 0;
        float closestDist = float.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, points[i]);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIdx = i;
            }
        }

        return closestIdx;
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
        Destroy(gameObject);
    }

    #endregion

    #region Damage Calculating

    public float CalculateDamage(float targetDefense, int targetLevel)
    {
        bool isCritical = Random.Range(0f, 100f) < CriticalChance;
        //그러니까 0~100 사이 랜덤값 뽑는데 이게 20보다 작으면 치명타
        //즉 CriticalChance이 증가하면 걸릴확률 증가
        // 0~100 랜덤 숫자 뽑아서 치명타확률보다 작으면 치명타
        // CriticalChance = 20이면 20% 확률로 true

        float baseDamage = isCritical ? AttackPower * CriticalDamage : AttackPower;
        // 치명타면 공격력 * 배율, 아니면 그냥 공격력
        // isCritical ? A : B -> 삼항연산자, true면 A 실행 false면 B 실행

        float finalDamage = baseDamage - targetDefense;
        // 기본 데미지에서 방어력 차감

        finalDamage = Mathf.Max(finalDamage, Level);
        // 최소 데미지 보장
        // Mathf.Max(a, b) = 둘 중 큰 값 반환
        // finalDamage가 Level보다 작으면 Level로 올려줌
        return finalDamage;

    }

    #endregion


    #region Attack

    protected abstract void OnTriggerEnter2D(Collider2D collision);




    #endregion


}
