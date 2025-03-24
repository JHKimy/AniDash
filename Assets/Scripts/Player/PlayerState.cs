using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public interface IState
{
    void Enter();    // 상태 진입 시 호출
    void Update();   // 매 프레임 호출
    void Exit();     // 상태 종료 시 호출
}




public class PlayerState : MonoBehaviour
{
    public Transform target;
    private Rigidbody _rigidbody;
    private Animator _animator;

    private Vector3 previousPosition;
    public float speed;

    // private float groundCheckDistance = 1.1f;  // 바닥 감지 거리
    public bool isGrounded = true;  // 현재 바닥에 있는지 여부





    public float health = 100;
    public float maxHealth = 100;
    public float stamina = 100;
    public float maxStamina = 100;

    public int key = 99;


    public Transform[] respawnPoints; // 총 4개의 리스폰 포인트
    private bool isDead = false;



    // ==============================================================================
    // 옵저버의 주체 
    // ==============================================================================
    public interface PlayerObserver
    {
        void OnPlayerStateChanged(PlayerState playerState);
    }

    private List<PlayerObserver> observers = new List<PlayerObserver>();

    public void AttachObserver(PlayerObserver observer)
    {
        observers.Add(observer);
    }

    public void DetachObserver(PlayerObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.OnPlayerStateChanged(this);
        }
    }

    public void SetHealth(float value)
    {
        health = Mathf.Clamp(value, 0, maxHealth);
        NotifyObservers();  // 상태 변경 알림
    }

    public void SetStamina(float value)
    {
        stamina = Mathf.Clamp(value, 0, maxStamina);
        NotifyObservers();
    }

    public void SetKey(int value)
    {
        key = value;
        NotifyObservers();
    }
    // ==============================================================================







    // ============================================
    // 키
    // ============================================
    public float vAxis /* W S */           { get; private set; }
    public float hAxis /* A D */           { get; private set; }
    public bool keyJump /* Space Bar */    { get; private set; }
    public bool keySlide /* C */            { get; set; }
    public bool keyAltCamera /* Alt */      { get; private set; }
    public bool keyGrab /* Mouse Left */    { get; private set; }

    // ============================================
    // 상태
    // ============================================

    public enum State   // 메인 상태
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Sliding,
        Falling,
        AccelFalling,
        FallingImpact,
        Parkouring,
        Climbing,
        Die
    }
    public enum SecondaryState // 보조 상태
    {
        None,
        HoldingObject
    }

    public State currentState { get; private set; } = State.Idle;
    public SecondaryState secondaryState { get; private set; } = SecondaryState.None;

    public void SetState(State newState)
    {
        // if (currentState == State.Parkouring && newState == State.Falling) return;
        // if (currentState == State.Climbing && newState == State.Jumping) return;

        currentState = newState;
    }
    public void SetSecondaryState(SecondaryState newState)
    {
        secondaryState = newState;
    }



    void Start()
    {
        _rigidbody = target.GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (currentState == State.Die) return; // Die 상태이면 더 이상 변경하지 않음

        // 속도 계산
        speed = (transform.position - previousPosition).magnitude / Time.fixedDeltaTime;
        previousPosition = transform.position;



        //== 흔들림 버그 강제 수정
        if (speed <= 0.5f && isGrounded)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        //==






        // 기본 State를 Idle로 하기 떄문에 까다롭게 설정
        // 대부분 State에서 돌아오는 것을 Idle로 설정
        if (speed <= 0.5f && vAxis <= 0.1 && hAxis <= 0.1 &&
           currentState != State.Jumping && isGrounded
           && currentState != State.Climbing
           && currentState != State.AccelFalling
           && currentState != State.FallingImpact
           && currentState != State.Die)
        {
            speed = 0;
            SetState(State.Idle);
            _rigidbody.linearVelocity = Vector3.zero;
        }

        // Debug.Log($"[FixedUpdate] 현재 속도: {speed:F2} m/s");
        // Debug.Log(currentState);
        // Debug.Log(secondaryState);
    }

    void Update()
    {
        SetInput();
        SetAnimation();

        // 스태미나 감소 처리
        if (currentState == State.Running)
        {
            SetStamina(stamina - 10f * Time.deltaTime);
        }
        else if ( currentState == State.Idle || currentState == State.Walking
            /*&& isGrounded*/)
        {
            SetStamina(stamina + 5f * Time.deltaTime);
        }

        Debug.Log("sdfffffffffffffffffffffffssssssssssssssssssss" + currentState);

    }


    public bool CanMove() =>
        currentState != State.Parkouring
        && currentState != State.Climbing
        && currentState != State.Falling
        && currentState != State.AccelFalling
        &&currentState !=State.Die;

    public bool CanJump()
        => currentState != State.Jumping &&
        currentState != State.Parkouring &&
        currentState != State.Climbing &&
        currentState != State.Falling
        && currentState != State.Die;

    public bool CanSlide()
        => currentState != State.Parkouring
        && currentState != State.Climbing
        && currentState != State.Idle
        && (vAxis != 0 || hAxis != 0)
        && currentState != State.Die;

    public bool CanFall()
    => currentState != State.Parkouring
    && currentState != State.Climbing
        && currentState != State.Sliding
        && currentState != State.Die;

    public bool CanGrab()
    => currentState != State.Parkouring
    && currentState != State.Climbing
    && currentState != State.Sliding
        && currentState != State.Die;

    public bool IsHoldingObject()
        => secondaryState == SecondaryState.HoldingObject;


    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;

        //if (collision.gameObject.tag == "Key") // 수정된 부분
        //{
        //    key++;
        //}
    }

    void SetInput()
    {
        // 키입력 받기
        vAxis = Input.GetAxis("Vertical");
        hAxis = Input.GetAxis("Horizontal");
        keyJump = Input.GetButtonDown("Jump");
        keySlide = Input.GetButton("Slide");
        keyAltCamera = Input.GetKey(KeyCode.LeftAlt);

        if (Input.GetButton("Run") && isGrounded &&
            currentState == State.Walking
            && secondaryState != SecondaryState.HoldingObject &&
            stamina > 0) 
        {   
            SetState(State.Running);
        }
        else if ((!Input.GetButton("Run") || stamina <= 0) && currentState == State.Running)
        {
            SetState(State.Walking);
        }
    }

    void SetAnimation()
    {
        _animator.SetFloat("vInput", vAxis);
        _animator.SetFloat("hInput", hAxis);
        _animator.SetBool("isJumping", currentState == PlayerState.State.Jumping);
        _animator.SetBool("isSliding", currentState == PlayerState.State.Sliding);
        _animator.SetBool("isRunning", currentState == PlayerState.State.Running);
        _animator.SetBool("isClimbing", currentState == PlayerState.State.Climbing);
        if (currentState == State.Climbing){ _animator.SetFloat("climbInput", vAxis);}
        _animator.SetBool("isFalling", currentState == State.Falling);
        _animator.SetBool("isAccelFalling", currentState == State.AccelFalling);
        _animator.SetBool("isFallingImpact", currentState == State.FallingImpact);
        _animator.SetBool("Die", currentState == State.Die);

        if (secondaryState == SecondaryState.HoldingObject)
        {
            _animator.SetLayerWeight(1, 1); // 박스 홀딩 레이어 가중치 1로 설정
        }
        if (secondaryState == SecondaryState.None)
        {
            _animator.SetLayerWeight(1, 0); // 박스 홀딩 레이어 레이어 가중치 0으로 설정
        }
    }


    public void TakeDamage(int damage)
    {
        health -= damage;
        SetHealth(health);

        if (health <= 0 && !isDead)
        {
            SetState(State.Die);
            Die();
        }
    }









    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("플레이어 사망. 리스폰 준비 중...");
        
        currentState = State.Die;


        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(10f); // 죽은 후 잠깐 딜레이

        // 가장 가까운 리스폰 위치 계산
        Transform nearestPoint = GetNearestRespawnPoint();

        // 위치 이동
        _rigidbody.position = nearestPoint.position;
        _rigidbody.position = nearestPoint.position;

        // 상태 초기화
        health = maxHealth;
        stamina = maxStamina;
        SetHealth(health);
        SetStamina(stamina);
        SetState(State.Idle);
        SetSecondaryState(SecondaryState.None);
        _animator.Rebind(); // 애니메이션 리셋
        _animator.Update(0f);

        // 물리 초기화
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        isDead = false;

        Debug.Log("리스폰 완료.");
    }

    Transform GetNearestRespawnPoint()
    {
        Transform nearest = respawnPoints[0];
        float minDist = Vector3.Distance(_rigidbody.position, respawnPoints[0].position);

        foreach (Transform point in respawnPoints)
        {
            float dist = Vector3.Distance(_rigidbody.position, point.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = point;
            }
        }

        return nearest;
    }
}
