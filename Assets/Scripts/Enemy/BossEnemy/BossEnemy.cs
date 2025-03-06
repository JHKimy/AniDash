using Gamekit3D;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
    Rigidbody       _rigidbody;
    BoxCollider     _boxCollider;
    MeshRenderer    _meshRenderer;
    NavMeshAgent    _navMeshAgent;
    Animator        _animator;

    public Transform        _target; // 추적할 대상
    public PlayerState     _playerState;


    public GameObject missilePrefab;  // 미사일 프리팹
    public MeteorStorm meteorStorm; // 메테오 스톰 매니저

    public Transform firePoint;  // 미사일 발사 위치
    public float attackRange = 5f;  // 펀치 공격 범위
    public float missileRange = 15f;  // 미사일 공격 범위
    public float attackCooldown = 3f; // 공격 쿨타임
    public float chaseRange = 30f;  // 추적 범위

    private bool canAttack = true;

    public float health = 100f; // 보스 체력
    private float maxHealth = 100f;

    private bool isChasing = false;
    private bool isAttacking = false;

    public enum State
    {
        Idle,
        Chase,
        Attack_Punch,
        Attack_Missile,
        Attack_Meteor,
        Hit,
        Falling
    }

    public State currentState = State.Idle;

    private Vector3 lookVec;

    private float distance;

    public bool isChase;
    public bool isAttack;
    public bool isFallling;



    private bool isLook;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        isLook = true;
    }

    void Update()
    {
        Debug.Log("Boss : " + currentState);
        if(isLook)
        {
            float h = _playerState.hAxis;
            float v = _playerState.vAxis;
            lookVec = new Vector3(h, 0, v) * 1f;
            transform.LookAt(_target.position + lookVec);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, _playerState.transform.position);

        if (canAttack && !isAttacking)
        {
            if (health <= maxHealth * 0.5f)  // 체력이 50% 이하일 때 메테오 스톰 사용
            {
                StartCoroutine(MeteorStormAttack());
            }
            else if (distanceToPlayer > attackRange && distanceToPlayer <= missileRange) // 미사일 공격
            {
                StartCoroutine(LaunchMissile());
            }
            else if (distanceToPlayer <= attackRange) // 펀치 공격
            {
                StartCoroutine(PunchAttack());
            }
        }
        if (!isAttacking)
        {
            if (distanceToPlayer <= chaseRange)
            {
                currentState = State.Chase;
                Chase();
            }
            else
            {
                currentState = State.Idle;
                StopMoving();
            }
        }
    }

    void Chase()
    {
        if (_navMeshAgent.isActiveAndEnabled && !isAttacking)
        {
            currentState = State.Chase;
            _navMeshAgent.isStopped = false; // 이동 가능
            _navMeshAgent.SetDestination(_playerState.transform.position);
            isChasing = true;
            _animator.SetBool("isWalk", true); // 이동 애니메이션 실행
        }

    }

   
    void StopMoving()
    {
        currentState = State.Idle;
        _navMeshAgent.isStopped = true;
        isChasing = false;
        _animator.SetBool("isWalk", false);
    }

    // 미사일 공격
    IEnumerator LaunchMissile()
    {
        isAttacking = true;

        canAttack = false;
        _navMeshAgent.isStopped = true;
        currentState = State.Attack_Missile;
        _animator.SetTrigger("doMissile");

        // 미사일 생성
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody missileRb = missile.GetComponent<Rigidbody>();

        if (missileRb != null)
        {
            Vector3 direction = (_playerState.transform.position - firePoint.position).normalized;
            missileRb.linearVelocity = direction * 10f;  // 미사일 속도 설정
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;

    }

    // 펀치 공격
    IEnumerator PunchAttack()
    {
        isAttacking = true;

        canAttack = false;
        currentState = State.Attack_Punch;
        _animator.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.5f);  // 공격 모션 시간 대기

        float distanceToPlayer = Vector3.Distance(transform.position, _playerState.transform.position);
        if (distanceToPlayer <= attackRange)  // 다시 거리 확인 후 맞으면 데미지 주기
        {
            _playerState.TakeDamage(15); // 펀치 데미지
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;

    }

    // 메테오 스톰 공격
    IEnumerator MeteorStormAttack()
    {
        isAttacking = true;

        canAttack = false;
        _navMeshAgent.isStopped = true;
        currentState = State.Attack_Meteor;
        _animator.SetTrigger("doMeteorStorm");

        yield return new WaitForSeconds(1f);  // 메테오 시전 대기

        if (meteorStorm != null)
        {
            meteorStorm.StartStorm(); // 메테오 스톰 시작
        }

        yield return new WaitForSeconds(5f); // 메테오 스톰 지속 시간
        meteorStorm.StopStorm(); // 메테오 스톰 종료

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true; 
        isAttacking = false;

    }
    //IEnumerator Think()
    //{
    //    yield return new WaitForSeconds(0.1f);

    //    int ranAction = Random.Range(0, 5);
    //    switch (ranAction)
    //    {


    //    }
    //}
}