using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    public Transform _target; // �÷��̾� Ÿ��
    public PlayerState _playerState;

    public GameObject missilePrefab;  // �̻��� ������
    public MeteorStorm meteorStorm;   // ���׿� ���� �Ŵ���
    public Transform firePoint;       // �̻��� �߻� ��ġ

    private float attackCooldown = 3f;   // ���� ��Ÿ��
    private float punchAttackRange = 5f; // ��ġ ���� ����
    private float missileRange = 15f;    // �̻��� ���� ����
    private float chaseRange = 30f;      // ���� ����

    private float health = 30f;
    private float maxHealth = 100f;
    private bool canAttack = true;
    private bool canUseMeteor = true;  // ���׿� ��� ���� ����

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
        StartCoroutine(Think()); // ���� �ð����� �ൿ ����
        StartCoroutine(AutoMeteorStorm()); // 10�ʸ��� ���׿� ����
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
                break; // �ڷ�ƾ���� ó��
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
        if (distanceToPlayer > punchAttackRange && distanceToPlayer <= missileRange && canAttack) // �̻��� ����
        {
            StartCoroutine(LaunchMissile());
        }
        else if (distanceToPlayer <= punchAttackRange && canAttack) // ��ġ ����
        {
            StartCoroutine(PunchAttack());
        }
        else if (distanceToPlayer <= chaseRange) // ����
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

        if (distanceToPlayer <= punchAttackRange) // ���� �����ϸ� ����
        {
            StartCoroutine(PunchAttack());
        }
    }

    // ��ġ ����
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

    // �̻��� ����
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

    // ���׿� ���� ���� (�ٸ� ���ݰ� ���������� �����)
    IEnumerator AutoMeteorStorm()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10�ʸ��� ����

            if (health <= maxHealth * 0.5f && canUseMeteor)
            {
                StartCoroutine(MeteorStormAttack());
            }
        }
    }

    IEnumerator MeteorStormAttack()
    {
        canUseMeteor = false;  // ���� ��� ����
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

        yield return new WaitForSeconds(5f); // 5�� �� �ٽ� ��� ����
        canUseMeteor = true;
        // currentState = State.Idle;
    }

    void SetAnimation()
    {
        _animator.SetBool("isWalk", currentState == State.Chase);
    }
}
