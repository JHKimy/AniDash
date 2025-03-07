using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    public Transform _target; // 플레이어 타겟
    public PlayerState _playerState;

    public GameObject missilePrefab;  // 미사일 프리팹
    public MeteorStorm meteorStorm;   // 메테오 스톰 매니저
    public Transform firePoint;       // 미사일 발사 위치

    private float attackCooldown = 3f;   // 공격 쿨타임
    private float punchAttackRange = 5f; // 펀치 공격 범위
    private float missileRange = 15f;    // 미사일 공격 범위
    private float chaseRange = 30f;      // 추적 범위

    private float health = 30f;
    private float maxHealth = 100f;
    private bool canAttack = true;
    private bool canUseMeteor = true;  // 메테오 사용 가능 여부

    private float distanceToPlayer;
    private Vector3 lookVec;

    private bool isLook = true;

    public enum State
    {
        Idle,
        Chase,
        Attack_Punch,
        Attack_Missile,
        Attack_Meteor,
        Hit,
        KnockDown
    }

    public State currentState = State.Idle;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(Think()); // 일정 시간마다 행동 결정
        StartCoroutine(AutoMeteorStorm()); // 10초마다 메테오 실행
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, _playerState.transform.position);

        if (isLook)
        {
            lookVec = new Vector3(_playerState.hAxis, 0, _playerState.vAxis) * 1f;
            transform.LookAt(_target.position + lookVec);
        }

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack_Punch:
            case State.Attack_Missile:
            case State.Attack_Meteor:
                break; // 코루틴에서 처리
            case State.Hit:
                break;
            case State.KnockDown:
                break;
        }

        SetAnimation();
    }

    IEnumerator Think()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (currentState == State.Idle || currentState == State.Chase)
            {
                DecideNextAction();
            }
        }
    }

    void DecideNextAction()
    {
        if (distanceToPlayer > punchAttackRange && distanceToPlayer <= missileRange && canAttack) // 미사일 공격
        {
            StartCoroutine(LaunchMissile());
        }
        else if (distanceToPlayer <= punchAttackRange && canAttack) // 펀치 공격
        {
            StartCoroutine(PunchAttack());
        }
        else if (distanceToPlayer <= chaseRange) // 추적
        {
            currentState = State.Chase;
        }
    }

    void Idle()
    {
        _navMeshAgent.isStopped = true;

        if (distanceToPlayer <= chaseRange)
        {
            currentState = State.Chase;
        }
    }

    void Chase()
    {
        if (!_navMeshAgent.isActiveAndEnabled)
        {
            currentState = State.Idle;
            return;
        }

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(_playerState.transform.position);

        if (distanceToPlayer <= punchAttackRange) // 공격 가능하면 공격
        {
            StartCoroutine(PunchAttack());
        }
    }

    // 펀치 공격
    IEnumerator PunchAttack()
    {
        canAttack = false;
        currentState = State.Attack_Punch;
        _navMeshAgent.isStopped = true;
        _animator.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.5f);

        if (Vector3.Distance(transform.position, _playerState.transform.position) <= punchAttackRange)
        {
            _playerState.TakeDamage(15);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        currentState = State.Idle;
    }

    // 미사일 공격
    IEnumerator LaunchMissile()
    {
        canAttack = false;
        currentState = State.Attack_Missile;
        _navMeshAgent.isStopped = true;
        _animator.SetTrigger("doMissile");

        yield return new WaitForSeconds(2f);

        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody missileRb = missile.GetComponent<Rigidbody>();

        if (missileRb != null)
        {
            Vector3 direction = (_playerState.transform.position - firePoint.position).normalized;
            missileRb.linearVelocity = direction * 10f;
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        currentState = State.Idle;
    }

    // 메테오 스톰 공격 (다른 공격과 독립적으로 실행됨)
    IEnumerator AutoMeteorStorm()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10초마다 실행

            if (health <= maxHealth * 0.5f && canUseMeteor)
            {
                StartCoroutine(MeteorStormAttack());
            }
        }
    }

    IEnumerator MeteorStormAttack()
    {
        canUseMeteor = false;  // 연속 사용 방지
        currentState = State.Attack_Meteor;
        _navMeshAgent.isStopped = true;
        _animator.SetTrigger("doMeteorStorm");

        yield return new WaitForSeconds(1f);

        if (meteorStorm != null)
        {
            meteorStorm.StartStorm();
        }

        yield return new WaitForSeconds(3f);
        meteorStorm.StopStorm();

        yield return new WaitForSeconds(5f); // 5초 후 다시 사용 가능
        canUseMeteor = true;
        // currentState = State.Idle;
    }

    void SetAnimation()
    {
        _animator.SetBool("isWalk", currentState == State.Chase);
    }
}
