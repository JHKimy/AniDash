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

    public Transform        _target; // ������ ���
    public PlayerState     _playerState;


    public GameObject missilePrefab;  // �̻��� ������
    public MeteorStorm meteorStorm; // ���׿� ���� �Ŵ���

    public Transform firePoint;  // �̻��� �߻� ��ġ
    public float attackRange = 5f;  // ��ġ ���� ����
    public float missileRange = 15f;  // �̻��� ���� ����
    public float attackCooldown = 3f; // ���� ��Ÿ��
    public float chaseRange = 30f;  // ���� ����

    private bool canAttack = true;

    public float health = 100f; // ���� ü��
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
            if (health <= maxHealth * 0.5f)  // ü���� 50% ������ �� ���׿� ���� ���
            {
                StartCoroutine(MeteorStormAttack());
            }
            else if (distanceToPlayer > attackRange && distanceToPlayer <= missileRange) // �̻��� ����
            {
                StartCoroutine(LaunchMissile());
            }
            else if (distanceToPlayer <= attackRange) // ��ġ ����
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
            _navMeshAgent.isStopped = false; // �̵� ����
            _navMeshAgent.SetDestination(_playerState.transform.position);
            isChasing = true;
            _animator.SetBool("isWalk", true); // �̵� �ִϸ��̼� ����
        }

    }

   
    void StopMoving()
    {
        currentState = State.Idle;
        _navMeshAgent.isStopped = true;
        isChasing = false;
        _animator.SetBool("isWalk", false);
    }

    // �̻��� ����
    IEnumerator LaunchMissile()
    {
        isAttacking = true;

        canAttack = false;
        _navMeshAgent.isStopped = true;
        currentState = State.Attack_Missile;
        _animator.SetTrigger("doMissile");

        // �̻��� ����
        GameObject missile = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody missileRb = missile.GetComponent<Rigidbody>();

        if (missileRb != null)
        {
            Vector3 direction = (_playerState.transform.position - firePoint.position).normalized;
            missileRb.linearVelocity = direction * 10f;  // �̻��� �ӵ� ����
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;

    }

    // ��ġ ����
    IEnumerator PunchAttack()
    {
        isAttacking = true;

        canAttack = false;
        currentState = State.Attack_Punch;
        _animator.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.5f);  // ���� ��� �ð� ���

        float distanceToPlayer = Vector3.Distance(transform.position, _playerState.transform.position);
        if (distanceToPlayer <= attackRange)  // �ٽ� �Ÿ� Ȯ�� �� ������ ������ �ֱ�
        {
            _playerState.TakeDamage(15); // ��ġ ������
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;

    }

    // ���׿� ���� ����
    IEnumerator MeteorStormAttack()
    {
        isAttacking = true;

        canAttack = false;
        _navMeshAgent.isStopped = true;
        currentState = State.Attack_Meteor;
        _animator.SetTrigger("doMeteorStorm");

        yield return new WaitForSeconds(1f);  // ���׿� ���� ���

        if (meteorStorm != null)
        {
            meteorStorm.StartStorm(); // ���׿� ���� ����
        }

        yield return new WaitForSeconds(5f); // ���׿� ���� ���� �ð�
        meteorStorm.StopStorm(); // ���׿� ���� ����

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