using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    public Transform _target;
    public PlayerState _playerState;

    public GameObject missilePrefab;
    public GameObject keyPrefab;
    public MeteorStorm meteorStorm;
    public Transform firePoint;

    private BossHealth _bossHealth;

    private float attackCooldown = 3f;
    private float punchAttackRange = 5f;
    private float missileRange = 15f;
    private float chaseRange = 30f;

    private float maxHealth = 100f;
    private float health = 30f;

    private float distanceToPlayer;
    private Vector3 lookVec;

    private bool canAttack = true;
    private bool canUseMeteor = true;
    private bool isDead = false;
    private bool isThinking = false;
    private bool isLook = true;

    public enum State
    {
        Idle,
        Chase,
        Attack_Punch,
        Attack_Missile,
        Attack_Meteor,
        Hit,
        Die
    }

    public State currentState = State.Idle;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _bossHealth = GetComponent<BossHealth>();
    }

    void Start()
    {
        StartCoroutine(AutoMeteorStorm());
    }

    void Update()
    {
        if (isDead) return;

        distanceToPlayer = Vector3.Distance(transform.position, _playerState.transform.position);

        if (isLook)
        {
            lookVec = new Vector3(_playerState.hAxis, 0, _playerState.vAxis);
            transform.LookAt(_target.position + lookVec);
        }

        if (!isThinking)
        {
            StartCoroutine(Think());
        }

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
        }

        SetAnimation();
    }

    IEnumerator Think()
    {
        isThinking = true;

        while (!isDead)
        {
            yield return new WaitForSeconds(0.5f);

            if (currentState == State.Idle || currentState == State.Chase)
            {
                DecideNextAction();
            }
        }

        isThinking = false;
    }

    void DecideNextAction()
    {
        if (isDead || !canAttack) return;

        if (distanceToPlayer <= punchAttackRange)
        {
            StartCoroutine(PunchAttack());
        }
        else if (distanceToPlayer <= missileRange)
        {
            StartCoroutine(LaunchMissile());
        }
        else if (distanceToPlayer <= chaseRange)
        {
            currentState = State.Chase;
        }
    }

    void Idle()
    {
        _navMeshAgent.isStopped = true;
        _rigidbody.linearVelocity = Vector3.zero;

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
    }

    IEnumerator PunchAttack()
    {
        if (isDead) yield break;

        canAttack = false;
        currentState = State.Attack_Punch;
        _navMeshAgent.isStopped = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _animator.SetTrigger("doAttack");

        yield return new WaitForSeconds(1f);

        if (Vector3.Distance(transform.position, _playerState.transform.position) <= punchAttackRange)
        {
            _playerState.TakeDamage(15);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        currentState = State.Idle;
    }

    IEnumerator LaunchMissile()
    {
        if (isDead) yield break;

        canAttack = false;
        currentState = State.Attack_Missile;
        _navMeshAgent.isStopped = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _animator.SetTrigger("doMissile");

        yield return new WaitForSeconds(1f);

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

    IEnumerator AutoMeteorStorm()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(10f);

            if (health <= maxHealth * 0.5f && canUseMeteor)
            {
                StartCoroutine(MeteorStormAttack());
            }
        }
    }

    IEnumerator MeteorStormAttack()
    {
        if (isDead) yield break;

        _rigidbody.linearVelocity = Vector3.zero;
        canUseMeteor = false;
        currentState = State.Attack_Meteor;
        _navMeshAgent.isStopped = true;
        _animator.SetTrigger("doMeteorStorm");

        yield return new WaitForSeconds(1f);

        meteorStorm?.StartStorm();

        yield return new WaitForSeconds(3f);
        meteorStorm?.StopStorm();

        yield return new WaitForSeconds(5f);
        canUseMeteor = true;
    }

    public IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;
        currentState = State.Die;

        _animator.SetTrigger("doDie");
        _navMeshAgent.isStopped = true;
        _rigidbody.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(5f);

        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 2f;
            Vector3 realSpawnPos = spawnPos + Random.insideUnitSphere * 2f;

            GameObject key = Instantiate(keyPrefab, realSpawnPos, Quaternion.identity);
            Rigidbody rb = key.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDir = Random.onUnitSphere;
                rb.AddForce(randomDir * 10f, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if (isDead) return;

        if (other.gameObject.CompareTag("Box"))
        {
            Box thrownBox = other.gameObject.GetComponent<Box>();
            if (thrownBox != null && thrownBox.isThrown)
            {
                currentState = State.Hit;
                _bossHealth.TakeDamage(50f);
                thrownBox.isThrown = false;
            }
        }
    }

    void SetAnimation()
    {
        if (isDead) return;
        _animator.SetBool("isWalk", currentState == State.Chase);
    }
}