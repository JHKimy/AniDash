using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEditor.XR;
using TMPro;

public class Enemy1 : MonoBehaviour
{
    Rigidbody        _rigidbody;
    BoxCollider      _boxCollider;
    Material         _material;
    NavMeshAgent     _navMeshAgent;
    Animator         _animator;
    public Transform _target; // 추적할 대상

    // =========================================
    public enum State 
    { 
        Idle, 
        Chase, 
        Attack,
        Hit,
        Falling
    }
    public State currentState = State.Idle;
    // =========================================

    private float distance;

    public bool isChase; 
    public bool isAttack;  
    public bool isFallling;



    private bool isHitAnimating = false; // Hit 애니메이션 실행 여부
    private bool canMove = true;

    public float attackRange = 2.0f; // 공격 범위
    public float chaseRange = 10.0f; // 추적 범위
    private float knockBackForce = 5f;// 밀려나는 힘
    private float hitDuration = 5f; // 넉백 후 다시 추적하는 시간

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _material = GetComponent<Material>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }


    void Update()
    {
        // 거리로 상태 판단
        distance = Vector3.Distance(transform.position, _target.position);


        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Hit:
                Hit();
                break;
        }

        SetAnimation();
        
    }


    void SetAnimation()
    {
        _animator.SetBool("isWalk", isChase);
        _animator.SetBool("isAttack", isAttack);
    }

    void Idle()
    {
        _navMeshAgent.ResetPath();
        isChase = false;
        isAttack = false;

        if(distance <= chaseRange)
        {
            currentState = State.Chase;
        }
    }
    void Chase()
    {
        _navMeshAgent.SetDestination(_target.position);
        isChase = true;
        isAttack = false;

        if (distance > chaseRange)
        {
            currentState = State.Idle;
        }
        if (distance <= attackRange) // 공격 범위 안에 들어오면
        {
            currentState = State.Attack;
        }

    }
    void Attack()
    {
        _navMeshAgent.ResetPath(); // 공격 시 이동 중지
        isAttack = true;

        if (distance > attackRange)
        {
            currentState = State.Chase;
        }

    }
    void Hit()
    {
        if (isHitAnimating) return; // 이미 애니메이션 실행중이면 취소 

        isHitAnimating = true;
        _navMeshAgent.ResetPath(); // 이동 중지
        // _navMeshAgent.isStopped = true; // 이동 정지
        _animator.SetTrigger("Hit");


        StartCoroutine(HitRoutine());
    }
    IEnumerator HitRoutine()
    {
        yield return new WaitForSeconds(hitDuration);
        
        isHitAnimating = false;
        currentState = State.Idle; // 5초 후 Idle 상태로 변경
    }

    // 박스에 맞았을 때 뒤로 밀려나는 기능 추가
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Box")
        {
            currentState = State.Hit;
            //Vector3 knockbackDir = (transform.position - other.transform.position).normalized;
            //_rigidbody.AddForce(knockbackDir * knockBackForce, ForceMode.Impulse);
        }
    }
}
