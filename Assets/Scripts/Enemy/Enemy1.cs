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
    public Transform _target; // ������ ���

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



    private bool isHitAnimating = false; // Hit �ִϸ��̼� ���� ����
    private bool canMove = true;

    public float attackRange = 2.0f; // ���� ����
    public float chaseRange = 10.0f; // ���� ����
    private float knockBackForce = 5f;// �з����� ��
    private float hitDuration = 5f; // �˹� �� �ٽ� �����ϴ� �ð�

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
        // �Ÿ��� ���� �Ǵ�
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
        if (distance <= attackRange) // ���� ���� �ȿ� ������
        {
            currentState = State.Attack;
        }

    }
    void Attack()
    {
        _navMeshAgent.ResetPath(); // ���� �� �̵� ����
        isAttack = true;

        if (distance > attackRange)
        {
            currentState = State.Chase;
        }

    }
    void Hit()
    {
        if (isHitAnimating) return; // �̹� �ִϸ��̼� �������̸� ��� 

        isHitAnimating = true;
        _navMeshAgent.ResetPath(); // �̵� ����
        // _navMeshAgent.isStopped = true; // �̵� ����
        _animator.SetTrigger("Hit");


        StartCoroutine(HitRoutine());
    }
    IEnumerator HitRoutine()
    {
        yield return new WaitForSeconds(hitDuration);
        
        isHitAnimating = false;
        currentState = State.Idle; // 5�� �� Idle ���·� ����
    }

    // �ڽ��� �¾��� �� �ڷ� �з����� ��� �߰�
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
